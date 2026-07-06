using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using XY = System.Numerics.Vector2;
using XYZ = System.Numerics.Vector3;

namespace CodeSugar
{
    internal class SystemVectorsTests
    {

        private static async Task _TestEquality(Vector3 a, Vector3 b, float tolerance)
        {
            // Assert.That(a, Has.Length.EqualTo(1).Within(0.0001f)); // should work because Vector3 HAS length
            // Assert.That(a, Is.EqualTo(b).WithinSquare(tolerance));
            // Assert.That(a, Is.EqualTo(b).WithinDistance(tolerance));
            // Assert.That(a, Is.EqualTo(b).Within(0,0.0001f,0));


            // Assert.That(b-a, Has.Length.LessThan(0.1f));
            // Assert.That(Vector3.Distance(a, b), Is.LessThanOrEqualTo(tolerance));

            await Assert.That(a.X).IsEqualTo(b.X).Within(tolerance);
            await Assert.That(a.Y).IsEqualTo(b.Y).Within(tolerance);
            await Assert.That(a.Z).IsEqualTo(b.Z).Within(tolerance);
        }
        

        [Test]
        public async Task TestConvert()
        {
            var v2 = new Vector2(1, 2);
            var p2 = v2.ConvertTo<System.Drawing.PointF>();
            var v2v = p2.ConvertToVector2();
            await Assert.That(v2).IsEqualTo(v2v);

            var v3 = new Vector3(1, 2, 3);
            var p3 = v3.ConvertTo<TestVector3Fields>();
            var v3v = p3.ConvertToVector3();
            await Assert.That(v3).IsEqualTo(v3v);

            v3 = new Vector3(1, 2, 3);
            var p3v = v3.ConvertTo<TestVector3Properties>();
            v3v = p3v.ConvertToVector3();
            await Assert.That(v3).IsEqualTo(v3v);

            /* no longer supported
            var v4 = new Vector4(1, 2, 3, 4);
            var p4v = v4.ConvertTo<(float,float,float,float)>();
            var v4v = p4v.ConvertToVector4();
            Assert.That(v4, Is.EqualTo(v4v));
            */

            Assert.Throws<TypeInitializationException>(()=> v3.ConvertTo<Double>());
        }

        [Test]
        public async Task TestDeconstruct()
        {
            var v2 = new Vector2(1, 2);

            var (v2x, v2y) = v2;
            await Assert.That(v2x).IsEqualTo(1);
            await Assert.That(v2y).IsEqualTo(2);

            var p = new Plane(0, 1, 0, 2);
            var (n, d) = p;
            await Assert.That(n).IsEqualTo(Vector3.UnitY);
            await Assert.That(d).IsEqualTo(2);

            var (px, py, pz, pd) = p;
            await Assert.That(px).IsEqualTo(0);
            await Assert.That(py).IsEqualTo(1);
            await Assert.That(pz).IsEqualTo(0);
            await Assert.That(pd).IsEqualTo(2);

            var m = Matrix3x2.Identity;

            var (mr1, mr2, mr3) = m;
            await Assert.That(mr1).IsEqualTo(Vector2.UnitX);
            await Assert.That(mr2).IsEqualTo(Vector2.UnitY);
            await Assert.That(mr3).IsEqualTo(Vector2.Zero);

            var (mc1, mc2) = m;
            await Assert.That(mc1).IsEqualTo(Vector3.UnitX);
            await Assert.That(mc2).IsEqualTo(Vector3.UnitY);
        }

        [Test]
        public async Task TestDominantAxis()
        {
            await Assert.That(new Vector3(3, 2, 1).DominantAxis()).IsEqualTo(0);
            await Assert.That(new Vector3(-3, 2, 1).DominantAxis()).IsEqualTo(0);

            await Assert.That(new Vector3(2, 3, 1).DominantAxis()).IsEqualTo(1);
            await Assert.That(new Vector3(-2, -3, 1).DominantAxis()).IsEqualTo(1);

            await Assert.That(new Vector3(2, 1, 3).DominantAxis()).IsEqualTo(2);
            await Assert.That(new Vector3(-2, -1, -3).DominantAxis()).IsEqualTo(2);
        }

        [Test]
        public async Task TestDecomposeScale()
        {
            await Assert.That(Matrix3x2.Identity.GetAreaScale()).IsEqualTo(1);
            await Assert.That(Matrix3x2.CreateScale(7).GetAreaScale()).IsEqualTo(7);
            await Assert.That(Matrix3x2.CreateScale(0.5f).GetAreaScale()).IsEqualTo(0.5f);
            await Assert.That(Matrix3x2.CreateScale(0.1f).GetAreaScale()).IsEqualTo(0.1f).Within(0.0000001f);

            await Assert.That(Matrix4x4.Identity.GetVolumeScale()).IsEqualTo(1);
            await Assert.That(Matrix4x4.CreateScale(7).GetVolumeScale()).IsEqualTo(7).Within(0.000001f);
            await Assert.That(Matrix4x4.CreateScale(0.5f).GetVolumeScale()).IsEqualTo(0.5f);
            await Assert.That(Matrix4x4.CreateScale(0.1f).GetVolumeScale()).IsEqualTo(0.1f).Within(0.0000001f);
        }

        [Test]
        public async Task TestInvert()
        {
            var m = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, 1);
            m.Translation = new Vector3(1, 2, 3);

            await Assert.That(Matrix4x4.Invert(m, out var im)).IsTrue();           

            var imm = m.InvertedFast();

            _TestEquality(im.Translation, imm.Translation, 0.0001f);
        }

        [Test]
        public async Task TestCollectionTransform()
        {
            var c1 = new XY[] { XY.Zero, XY.One };
            var c2 = c1.ToList();

            c1.InPlaceTransformBy(System.Numerics.Matrix3x2.Identity);
            c2.InPlaceTransformBy(System.Numerics.Matrix3x2.Identity);            
        }

        [Test]
        public async Task TestFallbacks()
        {
            var v3 = new XYZ(1, 2, 3);            
            await Assert.That(v3.GetElement(1)).IsEqualTo(2f);

            var v2 = v3.AsVector2();
            await Assert.That(v2.GetElement(1)).IsEqualTo(2f);

            var v4 = v3.AsVector4();
            await Assert.That(v4.GetElement(1)).IsEqualTo(2f);

            v2.AsVector3();
            v2.AsVector4();

            v4.AsVector2();
            v4.AsVector3();

            await Assert.That(v4.W).IsEqualTo(0f);
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