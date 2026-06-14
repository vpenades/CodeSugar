using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.FileProviders;

using NUnit.Framework;

namespace CodeSugar
{
    internal class FileProvidersTests
    {
        public static bool IsWindowsPlatform => Environment.OSVersion.Platform == PlatformID.Win32NT;

        [Test]
        public void TestPathSlicer()
        {
            var slicer = new CodeSugarForFileProviders._PathSlicer("dir\\", StringComparison.OrdinalIgnoreCase);
            Assert.That(slicer.FullPath, Is.EqualTo("dir"));

            Assert.That(!slicer.Contains(""));
            Assert.That(!slicer.Contains("dir"));
            Assert.That(!slicer.Contains("other/"));
            Assert.That(!slicer.Contains("other/dir"));
            Assert.That(!slicer.Contains("dir/"));
            Assert.That(!slicer.Contains("dir\\"));
            Assert.That(slicer.Contains("dir/x"));
            Assert.That(slicer.Contains("/dir\\x"));
            Assert.That(!slicer.Contains("dir/x/y"));
            Assert.That(!slicer.Contains("dir/x/y/"));

            Assert.That(slicer.Filter(new[] { "", "dir/", "dir", "\\dir", "/dir\\", "dir/x/y", "other", "other/j/y",  }).Single().name == "x");

            Assert.That(slicer.Filter(new[] { "dir/x", "dir/y" }).Count() == 2);
            Assert.That(slicer.Filter(new[] { "dir/x", "dir/y"}).All(item => item.isDirectory==false));
            Assert.That(slicer.Filter(new[] { "dir/x/" }).Single().isDirectory == true);
        }
        

        [Test]
        public void TestMicrosoftPhysicalFileProvider()
        {
            using (var pp = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(_CreateMockup1().FullName))
            {
                _TestMockup1(pp.GetDirectoryContents(string.Empty));                
            }
        }

        [Test]
        public void TestBasicPhysicalFileProvider()
        {
            var root = _CreateMockup1().ToIFileInfo().GetDirectoryContents();

            _TestMockup1(root);
        }

        [Test]
        public void TestTreeBuilding()
        {
            // assuming a flat collection of entries, typically taken from an archive, we test if we can build
            // a working, browsable hierarchical tree.

            var rootDir = CreateZipFlatEntries().ToIDirectoryContents(entry => entry.Key, null, MatchCasing.CaseSensitive);

            Assert.That(rootDir.Count(), Is.EqualTo(2));
            Assert.That(rootDir.Count(item => item.IsDirectory), Is.EqualTo(1));
            Assert.That(rootDir.Select(item => item.Name), Is.EquivalentTo(new[] { "dir","abc.txt" }));

            var childDir = CreateZipFlatEntries().ToIDirectoryContents(entry => entry.Key, "dir\\", MatchCasing.CaseSensitive);
            Assert.That(childDir.Count(), Is.EqualTo(1));            
            Assert.That(childDir.First().Name, Is.EqualTo("def.txt"));

            childDir = rootDir.FirstOrDefault(item => item.IsDirectory).GetDirectoryContents();            
            Assert.That(childDir.Count(), Is.EqualTo(1));
            Assert.That(childDir.First().Name, Is.EqualTo("def.txt"));

            var provider = rootDir.ToIFileProvider(MatchCasing.CaseSensitive);

            var f1 = provider.GetFileInfo("abc.txt");            
            Assert.That(f1.Exists);

            var f2 = provider.GetFileInfo("dir","def.txt");            
            Assert.That(f2.Exists);

            var c1 = provider.GetDirectoryContents(string.Empty);
            Assert.That(c1.Count(), Is.EqualTo(2));

            var c2 = provider.GetDirectoryContents("dir");
            Assert.That(c2.Select(item => item.Name), Is.EquivalentTo(new[] { "def.txt" }));

            var subPrivider = c2.ToIFileProvider(MatchCasing.CaseSensitive);
            var c3 = subPrivider.GetDirectoryContents(string.Empty);
            Assert.That(c3.Select(item => item.Name), Is.EquivalentTo(new[] { "def.txt" }));
        }

        private static System.IO.DirectoryInfo _CreateMockup1()
        {
            var baseDir = new System.IO.DirectoryInfo(TestContext.CurrentContext.WorkDirectory).UseDirectoryInfo("FileProviders");

            baseDir.DefineFileInfo("file1.txt").WriteAllText("hello");
            baseDir.DefineFileInfo("file2.txt").WriteAllText("hello");
            baseDir.UseDirectoryInfo("subdir1").DefineFileInfo("file3.txt").WriteAllText("hello");

            return baseDir;
        }

        private static void _TestMockup1(IDirectoryContents root)
        {
            Assert.That(root, Is.Not.Null);
            Assert.That(root.Exists);

            Assert.That(root.FindEntry("file2.txt").Exists);
            Assert.That(root.FindEntry("subdir1", "file3.txt").Exists);
            Assert.That(!root.FindEntry("subdir1", "missing.txt").Exists);
            Assert.That(!root.FindEntry("subdir1", "missingSubdir", "file3.txt").Exists);

            if (IsWindowsPlatform) // check windows case INsensitive
            {
                Assert.That(root.FindEntry("subdir1", "FILE3.txt").Exists);
            }
        }


        private static IEnumerable<_ZipEntry> CreateZipFlatEntries()
        {
            yield return new _ZipEntry("abc.txt");            
            yield return new _ZipEntry("dir/def.txt");
            yield return new _ZipEntry("dir/");
        }

        [System.Diagnostics.DebuggerDisplay("{Name}")]
        private readonly struct _ZipEntry : IFileInfo
        {
            public _ZipEntry(string key) { Key = key; }

            public Stream CreateReadStream() { throw new NotImplementedException(); }

            public string Key { get; }
            public bool Exists => true;
            public long Length => IsDirectory ? 10 : 0;

            [MaybeNull]
            public string PhysicalPath => null;
            public string Name => System.IO.Path.GetFileName(Key.TrimEnd('/'));
            public DateTimeOffset LastModified => DateTime.Today;
            public bool IsDirectory => Key.EndsWith('/');
        }

        
        #nullable enable

        private static void _TestMockup2(IDirectoryContents root)
        {
            Assert.That(root, Is.Not.Null);
            Assert.That(root.Exists);

            Assert.That(root.FindEntry("file2.txt").Exists);
            Assert.That(root.FindEntry("subdir1", "file3.txt").Exists);
            Assert.That(!root.FindEntry("subdir1", "missing.txt").Exists);
            Assert.That(!root.FindEntry("subdir1", "missingSubdir", "file3.txt").Exists);

            if (IsWindowsPlatform) // check windows case INsensitive
            {
                Assert.That(root.FindEntry("subdir1", "FILE3.txt").Exists);
            }
        }

        #nullable restore
    }
}
