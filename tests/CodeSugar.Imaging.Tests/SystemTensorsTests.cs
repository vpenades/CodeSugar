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
