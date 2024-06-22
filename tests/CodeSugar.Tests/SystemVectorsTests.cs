using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace CodeSugar
{
    internal class SystemVectorsTests
    {

        private static void _TestEquality(Vector3 a, Vector3 b, float tolerance)
        {
            // Assert.That(a, Has.Length.EqualTo(1).Within(0.0001f)); // should work because Vector3 HAS length
            // Assert.That(a, Is.EqualTo(b).WithinSquare(tolerance));
            // Assert.That(a, Is.EqualTo(b).WithinDistance(tolerance));
            // Assert.That(a, Is.EqualTo(b).Within(0,0.0001f,0));


            // Assert.That(b-a, Has.Length.LessThan(0.1f));
            // Assert.That(Vector3.Distance(a, b), Is.LessThanOrEqualTo(tolerance));

            Assert.That(a.X, Is.EqualTo(b.X).Within(tolerance));
            Assert.That(a.Y, Is.EqualTo(b.Y).Within(tolerance));
            Assert.That(a.Z, Is.EqualTo(b.Z).Within(tolerance));
        }
        

        [Test]
        public void TestConvert()
        {
            var v2 = new Vector2(1, 2);
            var p2 = v2.ConvertTo<System.Drawing.PointF>();
            var v2v = p2.ConvertToVector2();
            Assert.That(v2, Is.EqualTo(v2v));

            var v3 = new Vector3(1, 2, 3);
            var p3 = v3.ConvertTo<TestVector3Fields>();
            var v3v = p3.ConvertToVector3();
            Assert.That(v3, Is.EqualTo(v3v));

            v3 = new Vector3(1, 2, 3);
            var p3v = v3.ConvertTo<TestVector3Properties>();
            v3v = p3v.ConvertToVector3();
            Assert.That(v3, Is.EqualTo(v3v));

            /* no longer supported
            var v4 = new Vector4(1, 2, 3, 4);
            var p4v = v4.ConvertTo<(float,float,float,float)>();
            var v4v = p4v.ConvertToVector4();
            Assert.That(v4, Is.EqualTo(v4v));
            */

            Assert.Throws(typeof(TypeInitializationException),()=> v3.ConvertTo<Double>());
        }

        [Test]
        public void TestDeconstruct()
        {
            var v2 = new Vector2(1, 2);

            var (v2x, v2y) = v2;
            Assert.That(v2x, Is.EqualTo(1));
            Assert.That(v2y, Is.EqualTo(2));

            var p = new Plane(0, 1, 0, 2);
            var (n, d) = p;
            Assert.That(n, Is.EqualTo(Vector3.UnitY));
            Assert.That(d, Is.EqualTo(2));

            var (px, py, pz, pd) = p;
            Assert.That(px, Is.EqualTo(0));
            Assert.That(py, Is.EqualTo(1));
            Assert.That(pz, Is.EqualTo(0));
            Assert.That(pd, Is.EqualTo(2));

            var m = Matrix3x2.Identity;

            var (mr1, mr2, mr3) = m;
            Assert.That(mr1, Is.EqualTo(Vector2.UnitX));
            Assert.That(mr2, Is.EqualTo(Vector2.UnitY));
            Assert.That(mr3, Is.EqualTo(Vector2.Zero));

            var (mc1, mc2) = m;
            Assert.That(mc1, Is.EqualTo(Vector3.UnitX));
            Assert.That(mc2, Is.EqualTo(Vector3.UnitY));
        }

        [Test]
        public void TestDominantAxis()
        {
            Assert.That(new Vector3(3, 2, 1).DominantAxis(), Is.EqualTo(0));
            Assert.That(new Vector3(-3, 2, 1).DominantAxis(), Is.EqualTo(0));

            Assert.That(new Vector3(2, 3, 1).DominantAxis(), Is.EqualTo(1));
            Assert.That(new Vector3(-2, -3, 1).DominantAxis(), Is.EqualTo(1));

            Assert.That(new Vector3(2, 1, 3).DominantAxis(), Is.EqualTo(2));
            Assert.That(new Vector3(-2, -1, -3).DominantAxis(), Is.EqualTo(2));
        }

        [Test]
        public void TestDecomposeScale()
        {
            Assert.That(Matrix3x2.Identity.GetAreaScale(), Is.EqualTo(1));
            Assert.That(Matrix3x2.CreateScale(7).GetAreaScale(), Is.EqualTo(7));
            Assert.That(Matrix3x2.CreateScale(0.5f).GetAreaScale(), Is.EqualTo(0.5f));
            Assert.That(Matrix3x2.CreateScale(0.1f).GetAreaScale(), Is.EqualTo(0.1f).Within(0.0000001f));

            Assert.That(Matrix4x4.Identity.GetVolumeScale(), Is.EqualTo(1));
            Assert.That(Matrix4x4.CreateScale(7).GetVolumeScale(), Is.EqualTo(7).Within(0.000001f));
            Assert.That(Matrix4x4.CreateScale(0.5f).GetVolumeScale(), Is.EqualTo(0.5f));
            Assert.That(Matrix4x4.CreateScale(0.1f).GetVolumeScale(), Is.EqualTo(0.1f).Within(0.0000001f));            
        }

        [Test]
        public void TestInvert()
        {
            var m = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, 1);
            m.Translation = new Vector3(1, 2, 3);

            Assert.That(Matrix4x4.Invert(m, out var im));           

            var imm = m.InvertedFast();

            _TestEquality(im.Translation, imm.Translation, 0.0001f);
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        struct TestVector3Fields
        {
            public float X;
            public float Y;
            public float Z;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        struct TestVector3Properties
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
        }
    }
}
