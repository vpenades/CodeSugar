using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace CodeSugar
{
    internal class SystemNumericsTests
    {
        [Test]
        public void TestConvert()
        {
            var v = new Vector2(1, 2);
            var p = v.ConvertTo<System.Drawing.PointF>();
            Assert.That(p.X, Is.EqualTo(1));
            Assert.That(p.Y, Is.EqualTo(2));
        }
    }
}
