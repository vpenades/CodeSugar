using System;
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CodeSugar
{
    using CODESUGAR = CodeSugarForSystem;

    public class LinqTests
    {
        [Test]
        public void TestIndexOf()
        {
            var list = new List<int> { 1, 3, 4 };
            var array = new int[] { 1, 3, 4 };

            Assert.That(2, Is.EqualTo(list.IndexOf(4)));
            Assert.That(2, Is.EqualTo(array.IndexOf(4)));

            Assert.That(2, Is.EqualTo((list as IReadOnlyList<int>).IndexOf(4)));

            Assert.That(1, Is.EqualTo(array.Slice(1,2).IndexOf(4)));

            Assert.That(2, Is.EqualTo(array.SelectList(item => item.ToString()).IndexOf("4")));
        }
    }
}
