using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace CodeSugar
{
    internal class ZipFileProviderTests
    {
        [Test]
        public void TestAccessingZip()
        {
            using var zip = _CreateTestZip();

            foreach(var entry in zip.Entries)
            {
                TestContext.WriteLine(entry.FullName);
            }

            var provider = zip.ToFileIFileProvider();
            
            Assert.That(provider.GetFileInfo(string.Empty).IsDirectory);

            var rootContent = provider.GetDirectoryContents(string.Empty).ToList();
            Assert.That(rootContent, Has.Count.EqualTo(4));
            Assert.That(rootContent[0].Name, Is.EqualTo("a.bin"));
            Assert.That(rootContent[1].Name, Is.EqualTo("a"));
            Assert.That(rootContent[2].Name, Is.EqualTo("c.bin"));
            Assert.That(rootContent[3].Name, Is.EqualTo("b"));

            rootContent = provider.GetDirectoryContents("a").ToList();

            Assert.That(rootContent[0].Name, Is.EqualTo("b.bin")); // on windows it gets 2 files, in mac it gets one
            if (rootContent.Count == 2)
            {
                Assert.That(rootContent[1].Name, Is.EqualTo("b.bin"));
            }            

            rootContent = provider.GetDirectoryContents("b/").ToList();
            Assert.That(rootContent, Has.Count.EqualTo(1));
            Assert.That(rootContent[0].Name, Is.EqualTo("c"));

            rootContent = provider.GetDirectoryContents("b\\c").ToList();
            Assert.That(rootContent, Has.Count.EqualTo(1));
            Assert.That(rootContent[0].Name, Is.EqualTo("d"));

            rootContent = provider.GetDirectoryContents("b\\c/d").ToList();
            Assert.That(rootContent, Has.Count.EqualTo(1));
            Assert.That(rootContent[0].Name, Is.EqualTo("c.bin"));
        }

        [Test]
        public void TestCreatedZip()
        {
            using var zip = ZipFile.OpenRead(ResourceInfo.From("7zipCreated.zip"));

            var provider = zip.ToFileIFileProvider();

            Assert.That(provider.GetFileInfo(string.Empty).IsDirectory);

            var root = provider.GetFileInfo(string.Empty);
            Assert.That(root.IsDirectory);

            var rootContent = provider.GetDirectoryContents(string.Empty).ToList();
            Assert.That(rootContent, Has.Count.EqualTo(1));
            Assert.That(rootContent[0].IsDirectory);
        }



        public static ZipArchive _CreateTestZip()
        {
            var content = new Dictionary<string, ArraySegment<Byte>>();

            content["a.bin"] = new byte[5];
            content["a/b.bin"] = new byte[5];
            content["a\\b.bin"] = new byte[5]; // this is VERY malformed because it would extract to the same path
            content["c.bin"] = new byte[5];
            content["b/c\\d\\c.bin"] = new byte[5];

            var m = new System.IO.MemoryStream();

            using (var writeZip = new ZipArchive(m, ZipArchiveMode.Create, true))
            {
                foreach(var item in content)
                {                    
                    writeZip.CreateEntry(item.Key).WriteAllBytes(item.Value);
                }
            }

            m.Position = 0;

            return new ZipArchive(m);
        }
    }
}
