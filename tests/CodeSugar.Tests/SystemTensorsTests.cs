using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using XYZ = System.Numerics.Vector3;
using XYZW = System.Numerics.Vector4;

namespace CodeSugar
{
    internal class SystemTensorsTests
    {
        [SetUp]
        public void Initialize()
        {
            #if NET8_0_OR_GREATER
            TestContext.Out.WriteLine($"Vector512 HwdSupport:{Vector512.IsHardwareAccelerated}");
            TestContext.Out.WriteLine($"Vector256 HwdSupport:{Vector256.IsHardwareAccelerated}");
            TestContext.Out.WriteLine($"Vector128 HwdSupport:{Vector128.IsHardwareAccelerated}");
            #endif
        }
            

        [Test]
        public void TestVector4MultiplyAdd()
        {
            var src = new System.Numerics.Vector4[5];
            src.AsSpan().Fill(System.Numerics.Vector4.One);

            src.AsSpan().InPlaceMultiplyAdd(new System.Numerics.Vector4(2), new System.Numerics.Vector4(3));

            Assert.That(src.All(item => item == new System.Numerics.Vector4(5)));
        }

        [Test]
        public void TestScaledCast()
        {
            var bytes = Enumerable.Range(0,256).Select(item => (byte)item).ToArray();

            var floats = new float[256];

            bytes.AsReadOnlySpan().ScaledCastTo(floats);            

            for(int i=0; i < floats.Length; i++)
            {
                var v = floats[i];
                Assert.That(v, Is.GreaterThanOrEqualTo(0));
                Assert.That(v, Is.LessThanOrEqualTo(1));

                Assert.That(floats[i], Is.EqualTo(i/255f).Within(0.0000001f));
            }            
        }



        [TestCase(1, 1, 1, 0, 0, 0)]
        [TestCase(2, 2, 2, 0, 0, 0)]
        [TestCase(2, 2, 2, 3, 3, 3)]
        [TestCase(1, 2, 3, 4, 5, 6)]
        public void TestScaledMultiplyAddXYZ(float mx, float my, float mz, float ax, float ay, float az)
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

                Assert.That(dst, Is.EqualTo(dstRef));

                //--------------------------

                // reference
                dstRef.AsSpan().Fill(XYZ.One);
                for (int i = 0; i < dstRef.Length; ++i)
                {
                    dstRef[i] = dstRef[i] * mul + add;
                }

                dst.AsSpan().Fill(XYZ.One);
                dst.AsSpan().InPlaceMultiplyAdd(mul, add);

                Assert.That(dst, Is.EqualTo(dstRef));

            }           
        }





        [TestCase("RGB", "RGB")]
        [TestCase("RGB", "BGR")]
        [TestCase("RGB", "RGBA")]
        [TestCase("BGR", "RGBA")]
        [TestCase("RGB", "BGRA")]
        [TestCase("RGB", "ARGB")]
        [TestCase("RGBA", "RGBA")]
        [TestCase("RGBA", "RGB")]
        [TestCase("BGRA", "RGB")]
        [TestCase("ARGB", "RGB")]        
        [TestCase("RGBA", "RGBA")]
        [TestCase("RGBA", "BGRA")]
        [TestCase("ARGB", "BGRA")]        
        public void TestColorConversion(string srcFmt, string dstFmt)
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

                Assert.That(dstImplPixels, Is.EqualTo(dstRefPixels));
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
        public void BenchmarkScaledCast()
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

                TestContext.Out.WriteLine($"Run {r}: {sw.Elapsed}  {sw.ElapsedMilliseconds}ms");
            }
        }

        [Explicit]
        [Test]
        public void BenchmarkColorConversion()
        {
            var bytes = new Byte[65536 * 20 * 3];
            new Random().NextBytes(bytes);

            var floats = new float[bytes.Length];
            var vects = System.Runtime.InteropServices.MemoryMarshal.Cast<float, XYZ>(floats);

            for (int r = 0; r < 2; ++r)
            {
                var sw = Stopwatch.StartNew();

                for(int i=0; i < 100; ++i)
                {
                    bytes.AsReadOnlySpan().ConvertRGBtoRGB(vects, XYZ.One * 3, XYZ.One * 0.5f);
                }                

                TestContext.Out.WriteLine($"Run {r}: {sw.Elapsed}  {sw.ElapsedMilliseconds}ms");
            }
        }

        [Explicit]
        [Test]
        public void BenchmarkScaledMultiplyAddToXYZ()
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

                TestContext.Out.WriteLine($"Run {r}: {sw.Elapsed}  {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}
