using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using NuGet.Frameworks;

using NUnit.Framework;

namespace CodeSugar
{
    internal class SystemNumericsTests
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]

        [Test]
        public void TestConvert()
        {
            var v = new Vector2(1, 2);
            var p = v.ConvertTo<System.Drawing.PointF>();
            Assert.That(p.X, Is.EqualTo(1));
            Assert.That(p.Y, Is.EqualTo(2));
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
    }
}
