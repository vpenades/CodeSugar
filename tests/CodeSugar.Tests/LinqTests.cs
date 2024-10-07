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
            var array2 = new int[] { 1, 3, 4, 7, 9, 5 };

            Assert.That(list.IndexOf(4), Is.EqualTo(2));
            Assert.That(array.IndexOf(4), Is.EqualTo(2));

            Assert.That((list as IReadOnlyList<int>).IndexOf(4), Is.EqualTo(2));

            

            Assert.That(array.SelectList(item => item.ToString()).IndexOf("4"), Is.EqualTo(2));

            Assert.That(array.AtLoop(3), Is.EqualTo(1));
            Assert.That(array.AtLoop(-1), Is.EqualTo(4));

            Assert.That(list.AtLoop(3), Is.EqualTo(1));
            Assert.That(list.AtLoop(-1), Is.EqualTo(4));


            Assert.That(array.Slice(1, 2).IndexOf(4), Is.EqualTo(1));            
            var segm = array2.Slice(2, 3);
            Assert.That(segm.IndexOf(7), Is.EqualTo(1));
            Assert.That(segm.IndexOf(7, EqualityComparer<int>.Default), Is.EqualTo(1));
        }
    }
}
