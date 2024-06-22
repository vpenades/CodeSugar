using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using XYZ = System.Numerics.Vector3;

namespace CodeSugar
{
    internal class SystemTensorsTests
    {
        [SetUp]
        public void Initialize()
        {
            #if NET8_0_OR_GREATER
            TestContext.WriteLine($"Vector512 HwdSupport:{Vector512.IsHardwareAccelerated}");
            TestContext.WriteLine($"Vector256 HwdSupport:{Vector256.IsHardwareAccelerated}");
            TestContext.WriteLine($"Vector128 HwdSupport:{Vector128.IsHardwareAccelerated}");
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

                TestContext.WriteLine($"Run {r}: {sw.Elapsed}  {sw.ElapsedMilliseconds}ms");
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

                TestContext.WriteLine($"Run {r}: {sw.Elapsed}  {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}
