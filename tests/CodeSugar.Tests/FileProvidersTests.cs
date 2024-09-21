using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.FileProviders;

using NUnit.Framework;

using CODESUGARIO = CodeSugar.CodeSugarForFileProviders;

namespace CodeSugar
{
    internal class FileProvidersTests
    {
        public static bool IsWindowsPlatform => Environment.OSVersion.Platform == PlatformID.Win32NT;

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

        private static System.IO.DirectoryInfo _CreateMockup1()
        {
            var baseDir = new System.IO.DirectoryInfo(TestContext.CurrentContext.WorkDirectory).UseDirectory("FileProviders");

            baseDir.DefineFile("file1.txt").WriteAllText("hello");
            baseDir.DefineFile("file2.txt").WriteAllText("hello");
            baseDir.UseDirectory("subdir1").DefineFile("file3.txt").WriteAllText("hello");

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
    }
}
