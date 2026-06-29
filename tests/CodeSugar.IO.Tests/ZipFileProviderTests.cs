using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.FileProviders;

using TUnit;

namespace CodeSugar
{
    internal class ZipFileProviderTests
    {
        [Test]
        public async Task TestAccessingZip()
        {
            Func<ZipArchive> zipFactory = _CreateTestZip;

            // lazy access

            await _TestZipContents(zipFactory.ToIFileProvider());

            // access while open

            using (var zip = zipFactory())
            {
                await Assert.That(zip).IsNotNull();

                foreach (var entry in zip.Entries)
                {
                    Console.Out.WriteLine($"{entry.FullName} => {entry.Name}");
                }

                await _TestZipContents(zip.ToIFileProvider());                
            }            
        }

        

        [Test]
        public async Task TestCreatedZip()
        {
            using var zip = ZipFile.OpenRead(ResourceInfo.From("7zipCreated.zip"));

            var provider = zip.ToIFileProvider();

            await Assert.That(provider.GetFileInfo(string.Empty).IsDirectory).IsTrue();

            var root = provider.GetFileInfo(string.Empty);
            await Assert.That(root.IsDirectory).IsTrue();

            var rootContent = provider.GetDirectoryContents(string.Empty).ToList();
            await Assert.That(rootContent.Count).IsEqualTo(1);
            await Assert.That(rootContent[0].IsDirectory).IsTrue();
        }



        public static ZipArchive _CreateTestZip()
        {
            var content = new Dictionary<string, ArraySegment<Byte>>();

            content["a.bin"] = new byte[5];
            content["a/b.bin"] = new byte[] { 1, 2, 3, 4, 5 };
            content["a\\b.bin"] = new byte[5]; // this is VERY malformed because it would extract to the same path
            content["c.bin"] = new byte[5];
            content["b/c\\d\\c.bin"] = new byte[5]; // In Linux and Mac, Entry.Name reports "c\d\c.bin" instead of "c.bin"

            var m = new System.IO.MemoryStream();

            using (var writeZip = new ZipArchive(m, ZipArchiveMode.Create, true))
            {
                foreach(var item in content)
                {                    
                    writeZip.CreateEntry(item.Key).GetWriteStreamFunction().WriteAllBytes(item.Value);
                }
            }

            m.Position = 0;

            return new ZipArchive(m);
        }

        /// <summary>
        /// Tests the contents created by <see cref="_CreateTestZip"/>
        /// </summary>
        private static async Task _TestZipContents(IFileProvider provider)
        {
            await Assert.That(provider.GetFileInfo(string.Empty).IsDirectory).IsTrue();

            var rootContent = provider.GetDirectoryContents(string.Empty).ToList();
            await Assert.That(rootContent.Count).IsEqualTo(4);
            await Assert.That(rootContent[0].Name).IsEqualTo("a.bin");
            await Assert.That(rootContent[1].Name).IsEqualTo("a");
            await Assert.That(rootContent[2].Name).IsEqualTo("c.bin");
            await Assert.That(rootContent[3].Name).IsEqualTo("b");

            rootContent = provider.GetDirectoryContents("a").ToList();

            await Assert.That(rootContent[0].Name).IsEqualTo("b.bin"); // on windows it gets 2 files, in mac it gets one
            if (rootContent.Count == 2)
            {
                await Assert.That(rootContent[1].Name).IsEqualTo("b.bin");
            }

            rootContent = provider.GetDirectoryContents("b/").ToList();
            await Assert.That(rootContent.Count).IsEqualTo(1);
            await Assert.That(rootContent[0].Name).EqualTo("c");

            rootContent = provider.GetDirectoryContents("b\\c").ToList();
            await Assert.That(rootContent.Count).IsEqualTo(1);
            await Assert.That(rootContent[0].Name).IsEqualTo("d");

            rootContent = provider.GetDirectoryContents("b\\c/d").ToList();
            await Assert.That(rootContent.Count).IsEqualTo(1);
            await Assert.That(rootContent[0].Name).IsEqualTo("c.bin");

            var bytes = provider.GetFileInfo("a/b.bin").GetReadStreamFunction().ReadAllBytes();
            await Assert.That(bytes).IsSequenceEqualTo(new byte[] { 1, 2, 3, 4, 5 });
        }
    }
}
