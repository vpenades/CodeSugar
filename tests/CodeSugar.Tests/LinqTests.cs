using System;
using System.IO;
using System.Threading.Tasks;

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
        public async Task TestIndexOf()
        {
            var list = new List<int> { 1, 3, 4 };
            var array = new int[] { 1, 3, 4 };
            var array2 = new int[] { 1, 3, 4, 7, 9, 5 };

            await Assert.That(list.IndexOf(4)).IsEqualTo(2);
            await Assert.That(array.IndexOf(4)).IsEqualTo(2);

            await Assert.That((list as IReadOnlyList<int>).IndexOf(4)).IsEqualTo(2);



            await Assert.That(array.SelectList(item => item.ToString()).IndexOf("4")).IsEqualTo(2);

            await Assert.That(array.AtLoop(3)).IsEqualTo(1);
            await Assert.That(array.AtLoop(-1)).IsEqualTo(4);

            await Assert.That(list.AtLoop(3)).IsEqualTo(1);
            await Assert.That(list.AtLoop(-1)).IsEqualTo(4);


            await Assert.That(array.Slice(1, 2).IndexOf(4)).IsEqualTo(1);
            var segm = array2.Slice(2, 3);
            await Assert.That(segm.IndexOf(7)).IsEqualTo(1);
            await Assert.That(segm.IndexOf(7, EqualityComparer<int>.Default)).IsEqualTo(1);
        }

        [Test]
        public async Task TestRangeList()
        {
            await Assert.That((0, 3).RangeList()[1]).IsEqualTo(1);
            await Assert.That((1, 3).RangeList()[1]).IsEqualTo(2);

            await Assert.That((0, 3).RangeList(idx => idx)[1]).IsEqualTo(1);
            await Assert.That((1, 3).RangeList(idx => idx)[1]).IsEqualTo(2);
        }

        [Test]
        public async Task TestCachedEnumerable()
        {
            var collection = new int[] { 1, 2, 3 }.Select(item => item - 1);

            collection = collection.AsCachedEnumerable();

            await Assert.That(collection.TryGetNonEnumeratedCount(out _)).IsTrue();
        }
    }
}