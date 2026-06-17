using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.FileProviders;

using TUnit;

namespace CodeSugar
{
    internal class FileProvidersTests
    {
        public static bool IsWindowsPlatform => Environment.OSVersion.Platform == PlatformID.Win32NT;

        [Test]
        public async Task TestPathSlicer()
        {
            var slicer = new CodeSugarExtensions._PathSlicer("dir\\", StringComparison.OrdinalIgnoreCase);
            await Assert.That(slicer.FullPath).IsEqualTo("dir");

            await Assert.That(slicer.Contains("")).IsFalse();
            await Assert.That(slicer.Contains("dir")).IsFalse();
            await Assert.That(slicer.Contains("other/")).IsFalse();
            await Assert.That(slicer.Contains("other/dir")).IsFalse();
            await Assert.That(slicer.Contains("dir/")).IsFalse();
            await Assert.That(slicer.Contains("dir\\")).IsFalse();
            await Assert.That(slicer.Contains("dir/x")).IsTrue();
            await Assert.That(slicer.Contains("/dir\\x")).IsTrue();
            await Assert.That(slicer.Contains("dir/x/y")).IsFalse();
            await Assert.That(slicer.Contains("dir/x/y/")).IsFalse();

            await Assert.That(slicer.Filter(new[] { "", "dir/", "dir", "\\dir", "/dir\\", "dir/x/y", "other", "other/j/y",  }).Single().name).IsEqualTo("x");

            await Assert.That(slicer.Filter(new[] { "dir/x", "dir/y" })).Count().IsEqualTo(2);
            await Assert.That(slicer.Filter(new[] { "dir/x", "dir/y"}).All(item => item.isDirectory==false)).IsTrue();
            await Assert.That(slicer.Filter(new[] { "dir/x/" }).Single().isDirectory).IsTrue();            
        }
        

        [Test]
        public async Task TestMicrosoftPhysicalFileProvider()
        {
            using (var pp = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(_CreateMockup1().FullName))
            {
                _TestMockup1(pp.GetDirectoryContents(string.Empty));                
            }
        }

        [Test]
        public async Task TestBasicPhysicalFileProvider()
        {
            var root = _CreateMockup1().ToIFileInfo().GetDirectoryContents();

            _TestMockup1(root);
        }

        [Test]
        public async Task TestTreeBuilding()
        {
            // assuming a flat collection of entries, typically taken from an archive, we test if we can build
            // a working, browsable hierarchical tree.

            var rootDir = CreateZipFlatEntries().ToIDirectoryContents(entry => entry.Key, null, MatchCasing.CaseSensitive);

            await Assert.That(rootDir).Count().IsEqualTo(2);
            await Assert.That(rootDir.Count(item => item.IsDirectory)).IsEqualTo(1);
            await Assert.That(rootDir.Select(item => item.Name)).IsEquivalentTo(new[] { "dir","abc.txt" }, StringComparer.Ordinal);

            var childDir = CreateZipFlatEntries().ToIDirectoryContents(entry => entry.Key, "dir\\", MatchCasing.CaseSensitive);
            await Assert.That(childDir).Count().IsEqualTo(1);
            await Assert.That(childDir.First().Name).IsEqualTo("def.txt");

            childDir = rootDir.FirstOrDefault(item => item.IsDirectory).GetDirectoryContents();
            await Assert.That(childDir).Count().IsEqualTo(1);
            await Assert.That(childDir.First().Name).IsEqualTo("def.txt");

            var provider = rootDir.ToIFileProvider(MatchCasing.CaseSensitive);

            var f1 = provider.GetFileInfo("abc.txt");
            await Assert.That(f1.Exists).IsTrue();

            var f2 = provider.GetFileInfo("dir","def.txt");
            await Assert.That(f2.Exists).IsTrue();

            var c1 = provider.GetDirectoryContents(string.Empty);
            await Assert.That(c1).Count().IsEqualTo(2);

            var c2 = provider.GetDirectoryContents("dir");
            await Assert.That(c2.Select(item => item.Name)).IsEquivalentTo(new[] { "def.txt" }, StringComparer.Ordinal);

            var c2x = c1.FindEntry(MatchCasing.CaseSensitive, "dir").GetDirectoryContents();
            await Assert.That(c2x.Select(item => item.Name)).IsEquivalentTo(new[] { "def.txt" }, StringComparer.Ordinal);

            var subPrivider = c2.ToIFileProvider(MatchCasing.CaseSensitive);
            var c3 = subPrivider.GetDirectoryContents(string.Empty);
            await Assert.That(c3.Select(item => item.Name)).IsEquivalentTo(new[] { "def.txt" }, StringComparer.Ordinal);            
        }



        [Test]
        [Arguments("arch.cbz")]
        [Arguments("arch.cb7")]
        public async Task TestSharpCompress(string archiveName)
        {
            var provider = await ResourceInfo.From(archiveName).File.TryLoadSharpCompressArchiveAsync(null, null, null);

            await Assert.That(provider.GetFileInfo("subd\\02.png").Exists).IsTrue();
        }

        private static readonly Object _CreateMockup1Mutex = new Object();

        private static System.IO.DirectoryInfo _CreateMockup1()
        {
            lock (_CreateMockup1Mutex)
            {
                var baseDir = new System.IO.DirectoryInfo(AppContext.BaseDirectory).DefineDirectoryInfo("FileProviders");

                baseDir.DefineFileInfo("file1.txt").WriteAllText("hello");
                baseDir.DefineFileInfo("file2.txt").WriteAllText("hello");
                baseDir.UseDirectoryInfo("subdir1").DefineFileInfo("file3.txt").WriteAllText("hello");

                return baseDir;
            }
        }

        private static async Task _TestMockup1(IDirectoryContents root)
        {
            await Assert.That(root).IsNotNull();
            await Assert.That(root.Exists).IsTrue();

            await Assert.That(root.FindEntry("file2.txt").Exists).IsTrue();
            await Assert.That(root.FindEntry("subdir1", "file3.txt").Exists).IsTrue();
            await Assert.That(!root.FindEntry("subdir1", "missing.txt").Exists).IsTrue();
            await Assert.That(!root.FindEntry("subdir1", "missingSubdir", "file3.txt").Exists).IsTrue();

            if (IsWindowsPlatform) // check windows case INsensitive
            {
                await Assert.That(root.FindEntry("subdir1", "FILE3.txt").Exists).IsTrue();
            }
        }


        private static IEnumerable<_ZipEntry> CreateZipFlatEntries()
        {
            yield return new _ZipEntry("dir/def.txt");
            yield return new _ZipEntry("abc.txt");
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

        private static async Task _TestMockup2(IDirectoryContents root)
        {
            await Assert.That(root).IsNotNull();
            await Assert.That(root.Exists).IsTrue();

            await Assert.That(root.FindEntry("file2.txt").Exists).IsTrue();
            await Assert.That(root.FindEntry("subdir1", "file3.txt").Exists).IsTrue();
            await Assert.That(!root.FindEntry("subdir1", "missing.txt").Exists).IsTrue();
            await Assert.That(!root.FindEntry("subdir1", "missingSubdir", "file3.txt").Exists).IsTrue();

            if (IsWindowsPlatform) // check windows case INsensitive
            {
                await Assert.That(root.FindEntry("subdir1", "FILE3.txt").Exists).IsTrue();
            }
        }

        
    }
}
