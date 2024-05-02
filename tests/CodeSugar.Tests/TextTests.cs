using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace CodeSugar
{
    internal class TextTests
    {
        [Test]
        public void TestTextSplit()
        {
            "abcd".TrySplitAfterFirst(out var p1, "b", out var p2);
            Assert.That(p1, Is.EqualTo("a"));
            Assert.That(p2, Is.EqualTo("cd"));

            "abcdabc".TrySplitAfterLast(out p1, "b", out p2);
            Assert.That(p1, Is.EqualTo("abcda"));
            Assert.That(p2, Is.EqualTo("c"));

            "abcdabc".TrySplitAfterFirst(out p1, "b", out p2, "a", out var p3);
            Assert.That(p1, Is.EqualTo("a"));
            Assert.That(p2, Is.EqualTo("cd"));
            Assert.That(p3, Is.EqualTo("bc"));

            "a cd  bc".TrySplitAfterFirst(out p1, null, out p2, null, out p3);
            Assert.That(p1, Is.EqualTo("a"));
            Assert.That(p2, Is.EqualTo("cd"));
            Assert.That(p3, Is.EqualTo("bc"));

            "a b\r\n c".TrySplitAfterLast(out p1, null, out p2);
            Assert.That(p1, Is.EqualTo("a b"));
            Assert.That(p2, Is.EqualTo("c"));


        }

    }
}
