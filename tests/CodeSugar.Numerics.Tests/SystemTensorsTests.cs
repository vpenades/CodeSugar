using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Numerics.Tensors;
using System.Runtime.Intrinsics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;



using SixLabors.ImageSharp.PixelFormats;

using TUnit;

using XYZ = System.Numerics.Vector3;
using XYZW = System.Numerics.Vector4;

namespace CodeSugar
{
    internal class SystemTensorsTests
    {
        [Before(HookType.Class)]
        public static void Initialize()
        {
            #if NET8_0_OR_GREATER
            Console.Out.WriteLine($"Vector512 HwdSupport:{Vector512.IsHardwareAccelerated}");
            Console.Out.WriteLine($"Vector256 HwdSupport:{Vector256.IsHardwareAccelerated}");
            Console.Out.WriteLine($"Vector128 HwdSupport:{Vector128.IsHardwareAccelerated}");
            #endif
        }
            

        [Test]
        public async Task TestVector4MultiplyAdd()
        {
            var src = new XYZW[5];
            src.AsSpan().Fill(XYZW.One);

            src.AsSpan().InPlaceMultiplyAdd(new XYZW(2), new XYZW(3));

            await Assert.That(src.All(item => item == new XYZW(5))).IsTrue();
        }

        [Test]
        public async Task TestScaledCast()
        {
            var bytes = Enumerable.Range(0,256).Select(item => (byte)item).ToArray();

            var floats = new float[256];

            bytes.AsReadOnlySpan().ScaledCastTo(floats);            

            for(int i=0; i < floats.Length; i++)
            {
                var v = floats[i];
                await Assert.That(v).IsBetween(0, 1);
                await Assert.That(floats[i]).IsEqualTo(i/255f).Within(0.0000001f);
            }            
        }


        [Test]
        [Arguments(1, 1, 1, 0, 0, 0)]
        [Arguments(2, 2, 2, 0, 0, 0)]
        [Arguments(2, 2, 2, 3, 3, 3)]
        [Arguments(1, 2, 3, 4, 5, 6)]
        public async Task TestScaledMultiplyAddXYZ(float mx, float my, float mz, float ax, float ay, float az)
        {
            var mul = new XYZ(mx, my, mz);
            var add = new XYZ(ax, ay, az);

            for (int len=47; len > 0; len--)
            {
                //-------------------------- ScaledMultiplyAddTo

                // src
                var src = new Byte[len * 3];
                new Random().NextBytes(src);

                // reference dst
                var dstRef = new XYZ[len];
                for (int i = 0; i < dstRef.Length; ++i)
                {
                    dstRef[i] = new XYZ(src[i * 3 + 0], src[i * 3 + 1], src[i * 3 + 2]) * (mul / 255) + add;
                }

                // dst
                var dst = new XYZ[len];
                src.AsReadOnlySpan().ScaledMultiplyAddTo(mul, add, dst);

                await Assert.That(dst).IsSequenceEqualTo(dstRef);

                //--------------------------

                // reference
                dstRef.AsSpan().Fill(XYZ.One);
                for (int i = 0; i < dstRef.Length; ++i)
                {
                    dstRef[i] = dstRef[i] * mul + add;
                }

                dst.AsSpan().Fill(XYZ.One);
                dst.AsSpan().InPlaceMultiplyAdd(mul, add);

                await Assert.That(dst).IsSequenceEqualTo(dstRef);
            }           
        }




        [Test]
        [Arguments("RGB", "RGB")]
        [Arguments("RGB", "BGR")]
        [Arguments("RGB", "RGBA")]
        [Arguments("BGR", "RGBA")]
        [Arguments("RGB", "BGRA")]
        [Arguments("RGB", "ARGB")]
        [Arguments("RGBA", "RGBA")]
        [Arguments("RGBA", "RGB")]
        [Arguments("BGRA", "RGB")]
        [Arguments("ARGB", "RGB")]        
        [Arguments("RGBA", "RGBA")]
        [Arguments("RGBA", "BGRA")]
        [Arguments("ARGB", "BGRA")]        
        public async Task TestColorConversion(string srcFmt, string dstFmt)
        {
            foreach (var itemsLen in new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 50, 100, 128, 256, 1000, 1001, 1002, 5000, 20000 })
            {
                var mul = new XYZW(1, 2, 3, 4);
                var add = new XYZW(4, 3, 2, 1);

                mul = XYZW.One;
                add = XYZW.Zero;

                var srcBytes = new Byte[itemsLen * srcFmt.Length]; new Random().NextBytes(srcBytes);

                var dstImplPixels = new float[itemsLen * dstFmt.Length];
                var dstRefPixels = new float[itemsLen * dstFmt.Length];

                _ConvertPixelsImpl(srcBytes, srcFmt, dstImplPixels, dstFmt, mul, add);
                _ConvertPixelsRef(srcBytes, srcFmt, dstRefPixels, dstFmt, mul, add);

                await Assert.That(dstImplPixels).IsSequenceEqualTo(dstRefPixels);
            }
        }

        private static void _ConvertPixelsImpl(ReadOnlySpan<byte> src, string srcFmt, Span<float> dst, string dstFmt, XYZW mul, XYZW add)
        {
            var dstXYZ = System.Runtime.InteropServices.MemoryMarshal.Cast<float, XYZ>(dst);
            var dstXYZW = System.Runtime.InteropServices.MemoryMarshal.Cast<float, XYZW>(dst);

            var mul3 = new XYZ(mul.X,mul.Y, mul.Z);
            var add3 = new XYZ(add.X, add.Y, add.Z);

            switch (srcFmt)
            {
                case "RGB":
                    switch (dstFmt)
                    {
                        case "RGB": src.ConvertRGBtoRGB(dstXYZ, mul3, add3); break;
                        case "BGR": src.ConvertRGBtoBGR(dstXYZ, mul3, add3); break;
                        case "RGBA": src.ConvertRGBtoRGBA(dstXYZW, mul, add); break;
                        case "BGRA": src.ConvertRGBtoBGRA(dstXYZW, mul, add); break;
                        case "ARGB": src.ConvertRGBtoARGB(dstXYZW, mul, add); break;
                    } break;
                case "BGR":
                    switch (dstFmt)
                    {
                        case "RGB": src.ConvertBGRtoRGB(dstXYZ, mul3, add3); break;
                        case "BGR": src.ConvertBGRtoBGR(dstXYZ, mul3, add3); break;
                        case "RGBA": src.ConvertBGRtoRGBA(dstXYZW, mul, add); break;
                        case "BGRA": src.ConvertBGRtoBGRA(dstXYZW, mul, add); break;
                        case "ARGB": src.ConvertBGRtoARGB(dstXYZW, mul, add); break;
                    }
                    break;
                case "RGBA":
                    switch (dstFmt)
                    {
                        case "RGB": src.ConvertRGBAtoRGB(dstXYZ, mul3, add3); break;
                        case "BGR": src.ConvertRGBAtoBGR(dstXYZ, mul3, add3); break;
                        case "RGBA": src.ConvertRGBAtoRGBA(dstXYZW, mul, add); break;
                        case "BGRA": src.ConvertRGBAtoBGRA(dstXYZW, mul, add); break;
                        case "ARGB": src.ConvertRGBAtoARGB(dstXYZW, mul, add); break;
                    }
                    break;
                case "BGRA":
                    switch (dstFmt)
                    {
                        case "RGB": src.ConvertBGRAtoRGB(dstXYZ, mul3, add3); break;
                        case "BGR": src.ConvertBGRAtoBGR(dstXYZ, mul3, add3); break;
                        case "RGBA": src.ConvertBGRAtoRGBA(dstXYZW, mul, add); break;
                        case "BGRA": src.ConvertBGRAtoBGRA(dstXYZW, mul, add); break;
                        case "ARGB": src.ConvertBGRAtoARGB(dstXYZW, mul, add); break;
                    }
                    break;
                case "ARGB":
                    switch (dstFmt)
                    {
                        case "RGB": src.ConvertARGBtoRGB(dstXYZ, mul3, add3); break;
                        case "BGR": src.ConvertARGBtoBGR(dstXYZ, mul3, add3); break;
                        case "RGBA": src.ConvertARGBtoRGBA(dstXYZW, mul, add); break;
                        case "BGRA": src.ConvertARGBtoBGRA(dstXYZW, mul, add); break;
                        case "ARGB": src.ConvertARGBtoARGB(dstXYZW, mul, add); break;
                    }
                    break;
            }
        }

        private static void _ConvertPixelsRef(ReadOnlySpan<byte> src, string srcFmt, Span<float> dst, string dstFmt, XYZW mul, XYZW add)
        {
            IReadOnlyList<int> _ComponentIndices(string fmt)
            {
                switch(fmt)
                {
                    case "RGB": return new int[] { 0, 1, 2 };
                    case "BGR": return new int[] { 2, 1, 0 };
                    case "RGBA": return new int[] { 0, 1, 2, 3 };
                    case "BGRA": return new int[] { 2, 1, 0, 3 };
                    case "ARGB": return new int[] { 1, 2, 3, 0 };
                    default: throw new NotImplementedException();
                }
            }

            var srcIdx = _ComponentIndices(srcFmt);
            var dstIdx = _ComponentIndices(dstFmt);

            var len = Math.Min(src.Length / srcIdx.Count, dst.Length / dstIdx.Count);            

            dst.Fill(255); // missing alphas

            // transfer bytes

            for(int i=0; i < len; ++i)
            {
                for(int j =0; j < dstIdx.Count; ++j)
                {
                    // get from src
                    var val = j < srcIdx.Count
                        ? src[i * srcIdx.Count + srcIdx[j]]
                        : 255;

                    // set to dst
                    dst[i * dstIdx.Count + dstIdx[j]] = val;
                }                
            }

            // convert to float, and apply MAD
            switch(dstIdx.Count)
            {
                case 4:
                    var dstXYZW = System.Runtime.InteropServices.MemoryMarshal.Cast<float, XYZW>(dst);
                    for(int i=0; i < dstXYZW.Length; ++i)
                    {
                        dstXYZW[i] *= mul;
                        dstXYZW[i] += add;
                    }
                    break;
                case 3:
                    var dstXYZ = System.Runtime.InteropServices.MemoryMarshal.Cast<float, XYZ>(dst);
                    var mul3 = new XYZ(mul.X, mul.Y, mul.Z);
                    var add3 = new XYZ(add.X, add.Y, add.Z);
                    for (int i = 0; i < dstXYZ.Length; ++i)
                    {
                        dstXYZ[i] *= mul3;
                        dstXYZ[i] += add3;
                    }
                    break;
            }            
        }

        [Explicit]
        [Test]
        public async Task BenchmarkScaledCast()
        {
            var bytes = new Byte[65536 * 30];
            new Random().NextBytes(bytes);

            var floats = new float[bytes.Length];

            for (int r = 0; r < 2; ++r)
            {
                var sw = Stopwatch.StartNew();

                for (int i = 0; i < 5000; ++i)
                {
                    bytes.AsReadOnlySpan().ScaledCastTo(floats);
                }

                Console.Out.WriteLine($"Run {r}: {sw.Elapsed}  {sw.ElapsedMilliseconds}ms");
            }
        }

        [Explicit]
        [Test]
        public async Task BenchmarkColorConversion()
        {
            var bytes = new Byte[65536 * 20 * 3];
            new Random().NextBytes(bytes);

            var floats = new float[bytes.Length];
            var vects = System.Runtime.InteropServices.MemoryMarshal.Cast<float, XYZ>(floats.AsSpan());

            for (int r = 0; r < 2; ++r)
            {
                var sw = Stopwatch.StartNew();

                for(int i=0; i < 100; ++i)
                {
                    bytes.AsReadOnlySpan().ConvertRGBtoRGB(vects, XYZ.One * 3, XYZ.One * 0.5f);
                }                

                Console.Out.WriteLine($"Run {r}: {sw.Elapsed}  {sw.ElapsedMilliseconds}ms");
            }
        }

        [Explicit]
        [Test]
        public async Task BenchmarkScaledMultiplyAddToXYZ()
        {
            var xyzs = new XYZ[65536 * 10];

            var bytes = new Byte[xyzs.Length*3];
            new Random().NextBytes(bytes);            

            for (int r = 0; r < 2; ++r)
            {
                var sw = Stopwatch.StartNew();

                for (int i = 0; i < 5000; ++i)
                {
                    bytes.AsReadOnlySpan().ScaledMultiplyAddTo(new XYZ(1,2,3), new XYZ(4,5,6), xyzs);
                }

                Console.Out.WriteLine($"Run {r}: {sw.Elapsed}  {sw.ElapsedMilliseconds}ms");
            }
        }


        #if NET8_0_OR_GREATER

        [Test]
        public async Task TestTensorSpanCasting()
        {
            var tensorVector3 = System.Numerics.Tensors.Tensor.Create<XYZ>(new XYZ[256]).Reshape([16,16]).AsTensorSpan();
            var tensorVector3Info = TensorInfo.From(tensorVector3);
            await Assert.That(tensorVector3Info.FlattenedLength).IsEqualTo(256);
            await Assert.That(tensorVector3Info.Rank).IsEqualTo(2);
            await Assert.That(tensorVector3Info.Lengths[0]).IsEqualTo(16);
            await Assert.That(tensorVector3Info.Lengths[1]).IsEqualTo(16);

            tensorVector3 = System.Numerics.Tensors.Tensor.Create<XYZ>(new XYZ[256]).Reshape([16, 16]).AsTensorSpan();
            tensorVector3.CastTo(out System.Numerics.Tensors.TensorSpan<float> tensorFloats);
            tensorVector3Info = TensorInfo.From(tensorFloats);
            await Assert.That(tensorVector3Info.FlattenedLength).IsEqualTo((nint)256 * 3);
            await Assert.That(tensorVector3Info.Rank).IsEqualTo(3);
            await Assert.That(tensorVector3Info.Lengths[0]).IsEqualTo(16);
            await Assert.That(tensorVector3Info.Lengths[1]).IsEqualTo(16);
            await Assert.That(tensorVector3Info.Lengths[2]).IsEqualTo(3);

            /*
            tensorFloats[new nint[] { 0, 0, 1 }] = 5;            

            tensorFloats.CastTo(out tensorVector3);
            await Assert.That(tensorVector3.FlattenedLength).IsEqualTo((nint)256);
            await Assert.That(tensorVector3.Rank).IsEqualTo(2);
            await Assert.That(tensorVector3.Lengths[0]).IsEqualTo((nint)16);
            await Assert.That(tensorVector3.Lengths[1]).IsEqualTo((nint)16);
            await Assert.That(tensorVector3[new nint[] { 0, 0, }]).IsEqualTo(new XYZ(0,5,0));
            */
        }

        [Test]
        public async Task TestTensorDrawing()
        {
            var icon = ResourceInfo.From("CodeSugar.png");
            using var img = SixLabors.ImageSharp.Image.Load<Rgba32>(icon.FilePath);            

            var srcBmp = img.ToTensor<float>(3, false);
            
            var dstBmp = System.Numerics.Tensors.Tensor.Create(new float[512 * 512 * 3], new nint[] { 512, 512, 3 });

            dstBmp.AsTensorSpan().DrawRgbPixelsOverRgb(System.Numerics.Matrix3x2.CreateScale(2, 2) * System.Numerics.Matrix3x2.CreateRotation(0.1f), srcBmp, true);            

            AttachmentInfo.From("dstBmp.png").WriteObjectEx(f => dstBmp.SaveToSixLaborsImage(f));

            var h = dstBmp.AsReadOnlyTensorSpan().GetContentHashCode();

            var expected = -770854322;
            #if NET10_0_OR_GREATER
            expected = -987842336;
            #endif

            await Assert.That(h).IsEqualTo(expected);
        }

        #if NET10_0_OR_GREATER

        [Test]
        public async Task TestSubTensorSpan()
        {
            // we need to create it each time to cross the boundary
            static TensorSpan<int> _create()
            {
                return System.Numerics.Tensors.Tensor.Create<int>(Enumerable.Range(0, 16 * 16).ToArray()).Reshape([16, 16]).AsTensorSpan();
            }

            // rank 2 tests
            var tensor2 = _create();
            var tensor2Info = TensorInfo.From(tensor2);
            await Assert.That(tensor2Info.IsStrided).IsFalse();

            tensor2 = _create();
            var row5 = tensor2.SliceSubTensor(5);
            var row5Info = TensorInfo.From(row5);
            var row5span = row5.GetFullSpan().ToArray();            
            await Assert.That(row5Info.Rank).IsEqualTo(tensor2.Rank-1);            
            await Assert.That(row5span.Length).IsEqualTo(16);
            await Assert.That(row5span[0]).IsEqualTo(80);

            tensor2 = _create();
            var xrow5 = tensor2.AsReadOnlyTensorSpan().SliceSubTensor(5);
            var xrow5span = xrow5.GetFullSpan().ToArray();
            
            await Assert.That(xrow5.Rank).IsEqualTo(tensor2.Rank - 1);            
            await Assert.That(xrow5span.Length).IsEqualTo(16);
            await Assert.That(xrow5span[0]).IsEqualTo(80);

            // rank 3 tests
            var tensor3 = System.Numerics.Tensors.Tensor.Create<int>(Enumerable.Range(0, 16 * 16 * 16).ToArray()).Reshape([16, 16, 16]).AsTensorSpan();
            var tensor3info = TensorInfo.From(tensor3);
            var bmp5 = tensor3.SliceSubTensor(5);
            var bmp5info = TensorInfo.From(bmp5);
            await Assert.That(tensor3.IsStrided()).IsFalse();            
            await Assert.That(bmp5info.Rank).IsEqualTo(tensor3info.Rank - 1);
        }

        #endif

        #endif

        /// <summary>
        /// this is used to query tensor's info without crossing the async await boundary
        /// </summary>
        struct TensorInfo
        {
            public static TensorInfo From<T>(TensorSpan<T> tensor)
            {
                return new TensorInfo(tensor.Rank, tensor.Lengths, tensor.Strides, tensor.IsStrided());
            }

            public static TensorInfo From<T>(ReadOnlyTensorSpan<T> tensor)
            {
                return new TensorInfo(tensor.Rank, tensor.Lengths, tensor.Strides, tensor.IsStrided());
            }

            public static TensorInfo From(ITensor tensor)
            {
                return new TensorInfo(tensor.Rank, tensor.Lengths, tensor.Strides, tensor.IsStrided());
            }

            

            private TensorInfo(int rank, ReadOnlySpan<nint> lengths, ReadOnlySpan<nint> strides, bool isStrided)
            {
                Rank = rank;
                Lengths = lengths.ToArray();
                Strides = strides.ToArray();
            }

            public int Rank { get; }
            public nint[] Lengths { get; }
            public nint[] Strides { get; }

            public bool IsStrided { get; }

            public nint FlattenedLength => Lengths.Aggregate((a, b) => a * b);
        }


    }
}
