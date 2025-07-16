using System;
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;

using CODESUGARIO = CodeSugar.CodeSugarForSystemIO;

namespace CodeSugar
{
    public class SystemIOTests : IProgress<int>
    {
        public void Report(int value)
        {
            TestContext.Progress.WriteLine($"{value}%");
        }

        public void Report(string value)
        {
            TestContext.Progress.WriteLine(value);
        }

        public static bool IsWindowsPlatform => Environment.OSVersion.Platform == PlatformID.Win32NT;

        // these are drive names returned by various DriveInfo.GetDrives()
        private static readonly IEnumerable<string> _WindowsDrives = new string[] { "C:\\", "D:\\" };

        private static readonly IEnumerable<string> _UbuntuDrives = new string[] { "/", "/boot/efi", "/mnt", "/sys/fs/pstore", "/sys/fs/bpf", "/sys/kernel/tracing", "/snap/core20/2015" };

        private static readonly IEnumerable<string> _MacOsDrives = new string[] { "/", "/System/Volumes/VM", "/System/Volumes/Preboot", "/System/Volumes/Data" };

        [Test]
        public void TestEnvironment()
        {
            // https://stackoverflow.com/questions/430256/how-do-i-determine-whether-the-filesystem-is-case-sensitive-in-net            

            // Ubuntu & mac:
            //      Separators / /
            //      invalid name chars: /

            // Windows:
            //      Separators \ /
            //      invalid name chars: " < > |                             : * ? \ /

            TestContext.Out.WriteLine($"{Environment.OSVersion.Platform}");
            TestContext.Out.WriteLine($" comparison: {CODESUGARIO.FileSystemStringComparison}");

            TestContext.Out.WriteLine($"Separators {System.IO.Path.DirectorySeparatorChar} {System.IO.Path.AltDirectorySeparatorChar}");
            TestContext.Out.WriteLine($"invalid name chars: " + string.Join(" ",System.IO.Path.GetInvalidFileNameChars()));
            TestContext.Out.WriteLine($"invalid path chars: " + string.Join(" ", System.IO.Path.GetInvalidPathChars()));

            var finfo = AttachmentInfo.From("test.txt").WriteAllText("hello");

            if (finfo.Directory.TryGetDriveInfo(out var drive))
            {
                TestContext.Out.WriteLine($"{drive.Name} {drive.DriveFormat} {drive.DriveType}");
            }
            else
            {
                TestContext.Out.WriteLine("drive can't be retrieved.");
            }

            TestContext.Out.WriteLine("fixed drives:");
            foreach (var drive2 in DriveInfo.GetDrives())
            {
                if (!drive2.IsReady) continue;
                TestContext.Out.WriteLine($"{drive2.DriveFormat} {drive2.DriveType}  Name:\"{drive2.Name}\"");
            }
        }

        [Test]
        public void TestPathLengthLimits()
        {
            var tooBigPath = "c:\\" + string.Join("\\", Enumerable.Repeat("PathTooLong", 24));

            var finfo = new System.IO.FileInfo(tooBigPath);
        }


        [Test]
        public void TestPaths()
        {
            Assert.That(System.IO.MatchCasing.PlatformDefault.IsTempPath(System.IO.Path.GetTempPath()));

            Assert.That(CODESUGARIO.SplitPath("/abc/d/e"),
                Is.EqualTo(new string[] { "abc", "d", "e" }));

            Assert.That(CODESUGARIO.SplitPath("//network/abc/d/e"),
                Is.EqualTo(new string[] { "//network", "abc", "d", "e" }));            

            Assert.That(CODESUGARIO.SplitDirectoryAndName("/abc/d/e"),
                Is.EqualTo(("abc/d", "e")));

            Assert.That(CODESUGARIO.SplitDirectoryAndName("//network/abc/d/e"),
                Is.EqualTo(( "//network/abc/d", "e" )));

            if (CODESUGARIO.FileSystemIsCaseSensitive)
            {
                Assert.That(MatchCasing.PlatformDefault.ArePathsEqual("c:/abc", "c:/abc/"));
                Assert.That(MatchCasing.PlatformDefault.ArePathsEqual("c:/abc", "c:/abC/"), Is.Not.True);                
            }
            else
            {
                Assert.That(MatchCasing.PlatformDefault.ArePathsEqual("c:/abc", "c:/abc/"));
                Assert.That(MatchCasing.PlatformDefault.ArePathsEqual("C:/abc", "c:/ABC/"));
                Assert.That(MatchCasing.PlatformDefault.ArePathsEqual("c:/abc", "c:/abX/"), Is.Not.True);                
            }

            if (IsWindowsPlatform)
            {
                Assert.That(CODESUGARIO.SplitPath("\\abc/d\\e"), Is.EqualTo(new string[] { "abc", "d", "e" }));
                Assert.That(MatchCasing.PlatformDefault.ArePathsEqual( "C:\\AbC/", "c:/aBc\\"));
            }
        }


        [TestCase("\\\\192.168.0.200\\temp\\xyz\\", "\\\\192.168.0.200")]
        [TestCase("\\\\192.168.0.200\\temp\\", "\\\\192.168.0.200")]
        [TestCase("\\\\X\\temp\\xyz\\", "\\\\X")]
        [TestCase("c:\\xyz\\abc\\", "C:")]
        [TestCase("c:\\xyz\\abc", "C:")]
        [TestCase("c:", "C:")]
        public void TestDrives(string path, string expected)
        {
            // https://github.com/dotnet/runtime/tree/main/src/libraries/System.IO.FileSystem.DriveInfo/src/System/IO

            var root = System.IO.Path.GetPathRoot(path);

            var someDir = new System.IO.DirectoryInfo(path);

            /*
            someDir // create a FileInfo with an ADS which also contains a ":" in the path.
                .GetFileInfo("readme.txt")
                .TryGetAlternateDataStream("data.bin", out var someFile);
            */            

            var someFile = new System.IO.FileInfo(CODESUGARIO.ConcatenatePaths(someDir.FullName, "readme.txt"));

            // var xroot = System.IO.Path.GetPathRoot(path);
            // var xdrive = new System.IO.DriveInfo(xroot);            

            var driveOrNetwork = someDir.GetDriveOrNetworkName();
            if (driveOrNetwork == null)
            {
                TestContext.Out.WriteLine($"Skipping: {path}");
                return;
            }

            var driveName = "-";

            if (someDir.TryGetDriveInfo(out var drive))
            {
                driveName = drive.Name;
            }

            if (someFile.Directory.TryGetDriveInfo(out var drive2))
            {
                Assert.That(drive2.Name, Is.EqualTo(driveName));
            }
            else
            {
                Assert.That(driveName, Is.EqualTo("-"));
            }

            TestContext.Out.WriteLine($"{root} {driveOrNetwork} {driveName}");

            // Assert.That(string.Equals(driveOrNetwork,expected, CodeSugarIO.FileSystemStringComparison));            
        }
        

        [Test]
        public void TestFileInfo()
        {
            // var readme_txt = ResourceInfo.From("readme.txt").File;

            // readme_txt = new System.IO.FileInfo(readme_txt.GetNormalizedFullName().Replace("\\","/"));

            var readme_txt = AttachmentInfo.From("readme.txt").WriteAllText("hello world");

            TestContext.Out.WriteLine(readme_txt.FullName);


            var text = readme_txt.ReadAllText();
            Assert.That(text, Is.EqualTo("hello world"));

            var file2 = readme_txt.Directory.GetFileInfo("readme.txt");

            Assert.That(file2.Exists);
            // Assert.That(readme_txt.FullNameEquals(file2)); // must fix equality handling

            var rfinfo = AttachmentInfo.From("readme_2.txt").WriteObjectEx(f => f.WriteAllText("hello world 2"));

            Assert.That(rfinfo.ReadAllText(),
                Is.EqualTo("hello world 2"));

            Assert.That(System.Convert.ToHexString(readme_txt.ComputeSha256()),
                Is.EqualTo("B94D27B9934D3E08A52E52D7DA7DABFAC484EFE37A5380EE9088F7ACE2EFCDE9"));            

            Assert.That(System.Convert.ToHexString(readme_txt.ComputeSha512()),
                Is.EqualTo("309ECC489C12D6EB4CC40F50C902F2B4D0ED77EE511A7C7A9BCD3CA86D4CD86F989DD35BC5FF499670DA34255B45B0CFD830E81F605DCF7DC5542E93AE9CD76F"));

            Assert.That(readme_txt.Directory.IsParentOf(readme_txt));

            Assert.That(readme_txt.GetPathRelativeTo(readme_txt.Directory),
                Is.EqualTo("readme.txt"));

            // Assert.That(readme_txt.GetRelativePath(readme_txt.Directory.Parent), Is.EqualTo("Resources\\readme.txt")); // equality

            var dcomparer = MatchCasing.PlatformDefault.GetFullNameComparer<DirectoryInfo>();

            var tmp0 = new System.IO.DirectoryInfo("temp\\");
            var tmp1 = tmp0.DefineDirectoryInfo("a", "..", ".", "b", "..");
            Assert.That(dcomparer.Equals(tmp0, tmp1));

            Assert.That(() => readme_txt.Directory.DefineFileInfo(" abc"), Throws.Exception);
            Assert.That(() => readme_txt.Directory.DefineFileInfo("abc "), Throws.Exception);
            Assert.That(() => readme_txt.Directory.DefineFileInfo("abc ", "readme.txt"), Throws.Exception);

            Assert.That(() => readme_txt.Directory.DefineFileInfo(".."), Throws.Exception);
            Assert.That(() => readme_txt.Directory.DefineFileInfo("."), Throws.Exception);            
            Assert.That(() => readme_txt.Directory.DefineFileInfo("/"), Throws.Exception);
            

            if (IsWindowsPlatform)
            {
                Assert.That(() => readme_txt.Directory.DefineFileInfo(":"), Throws.Exception);
                Assert.That(() => readme_txt.Directory.DefineFileInfo("*"), Throws.Exception);
                Assert.That(() => readme_txt.Directory.DefineFileInfo("?"), Throws.Exception);
                Assert.That(() => readme_txt.Directory.DefineFileInfo("\\"), Throws.Exception);
            }            
        }


        [Test]
        public void TestAlternateDataStreams()
        {
            if (!IsWindowsPlatform) return;

            // https://arstechnica.com/civis/threads/is-winfs-vulnerable-to-alternate-data-streams.468008/
            // https://blog.j2i.net/2021/12/11/working-with-alternative-data-streamsthe-hidden-part-of-your-windows-file-system-on-windows/

            // apparently, writing the main content erases all additional data streams.

            var workFile = AttachmentInfo.From("readme_with_ADS.txt").WriteAllText("hello world");
            Assert.That(workFile.TryGetAlternateDataStream("ads.bin", out var adsInfo));
            
            Assert.That(adsInfo.Exists, Is.False);

            var data = new Byte[] { 1, 2, 3, 4 };

            adsInfo.WriteAllBytes(data);
            Assert.That(adsInfo.Exists, Is.True);

            Assert.That(workFile.ReadAllText(), Is.EqualTo("hello world"));
            Assert.That(adsInfo.ReadAllBytes(), Is.EqualTo(data));
        }


        [Test]
        public void TestFileInfoEquality()
        {
            // https://stackoverflow.com/questions/430256/how-do-i-determine-whether-the-filesystem-is-case-sensitive-in-net
            // https://stackoverflow.com/questions/7344978/verifying-path-equality-with-net

            // linux is case insensitive, but external drives on windows can also be.

            // so the procedure would be:
            // 1- check whether both files are located in the SAME DRIVE, if not, return false.
            // 2- check the drive type to identify whether to compare as case sensitive or not.

            // test file case sensitivity

            var testDir = new System.IO.DirectoryInfo(TestContext.CurrentContext.WorkDirectory);

            var readme_txt_0 = testDir.UseFileInfo("readme.txt");
            var readme_txt_1 = testDir.UseFileInfo("README.txt");

            readme_txt_0.WriteAllText("lowercase");
            readme_txt_1.WriteAllText("uppercase");

            foreach(var readme_txt in testDir.GetFiles("*.txt"))
            {
                TestContext.Out.WriteLine(readme_txt.FullName);
            }

            bool isCaseSensitiveOS = readme_txt_0.ReadAllText() == "lowercase";

            TestContext.Out.WriteLine($"OS file system is case sensitive: {isCaseSensitiveOS}");

            var fcomparer = MatchCasing.PlatformDefault.GetFullNameComparer<FileInfo>();
            var dcomparer = MatchCasing.PlatformDefault.GetFullNameComparer<DirectoryInfo>();
            
            Assert.That(fcomparer.Equals(readme_txt_0, readme_txt_1), Is.Not.EqualTo(isCaseSensitiveOS));

            // test directory equality with various tails.

            var tmp0 = new System.IO.DirectoryInfo("temp");
            var tmp1 = new System.IO.DirectoryInfo("temp/");           

            Assert.That(dcomparer.Equals(tmp0, tmp1));

            if (System.IO.Path.DirectorySeparatorChar == '\\')
            {
                var tmp2 = new System.IO.DirectoryInfo("temp\\"); // ubuntu & mac consider \\ as a valid character
                Assert.That(dcomparer.Equals(tmp0, tmp2));
                Assert.That(dcomparer.Equals(tmp1, tmp2));
            }
        }

        


        [Test]
        public void TestShortcuts()
        {
            var uri = new Uri("http://www.google.com");

            var finfo = AttachmentInfo.From("test.url").WriteObjectEx(f => f.WriteShortcut(uri));

            Assert.That(finfo.ReadShortcutUri(), Is.EqualTo(uri));

            var finfo2 = AttachmentInfo.From("test2.url").WriteObjectEx(f => f.WriteShortcut(new Uri(finfo.FullName)));            

            if (finfo2.TryReadShortcutFile(out var targetFile))
            {
                finfo.FullNameEquals(targetFile);
            }

            // test resolve file:

            var textFile = AttachmentInfo
                .From("readme.txt")
                .WriteAllText("Hello world");

            var url1 = AttachmentInfo.From("readme.1.url").WriteShortcut(textFile.FullName);
            var url2 = AttachmentInfo.From("readme.2.url").WriteShortcut(url1.FullName);

            Assert.That(url2.TryResolveShortcutFile(out var resolvedFile));            

            Assert.That(resolvedFile.FullNameEquals(textFile));

            // test resolve file:            

            url1 = AttachmentInfo.From("dir.1.url").WriteShortcut(textFile.Directory.FullName);
            url2 = AttachmentInfo.From("dir.2.url").WriteShortcut(url1.FullName);

            Assert.That(url2.TryResolveShortcutDir(out var resolvedDir));

            Assert.That(resolvedDir.FullNameEquals(textFile.Directory));

        }

        [Test]
        public void TestZip()
        {
            using (var context = new AttachmentDirectory("tests"))
            {
                var zpath = context.Directory.UseFileInfo("test.zip");

                zpath.Delete();

                using (var zip = zpath.CreateZipArchive())
                {
                    zip.CreateEntry("readme.txt").WriteAllText("hello world");
                }                

                using (var zip = zpath.OpenReadZipArchive())
                {
                    var txt = zip.GetEntry("readme.txt").ReadAllText();
                    Assert.That(txt, Is.EqualTo("hello world"));

                    var dict = zip.ToDictionary();
                    Assert.That(dict.ContainsKey("readme.txt"));
                }
            }            


        }


        [Test]
        public async Task TestEnumeration()
        {
            var rinfo = ResourceInfo.From("readme.txt");            

            var file = await rinfo.File.Directory.Parent.FindFirstFileAsync(f => f.Name == "readme.txt", SearchOption.AllDirectories, System.Threading.CancellationToken.None, this);
            Assert.That(file, Is.Not.Null);
            
            var files = await rinfo.File.Directory.Parent.FindAllFilesAsync(f => f.Name == "readme.txt", SearchOption.AllDirectories, System.Threading.CancellationToken.None, this);
            Assert.That(files, Is.Not.Null);
            Assert.That(files.Count, Is.AtLeast(1));

            var dir = await rinfo.File.Directory.Parent.FindFirstDirectoryAsync(d => d.Name == "Resources", SearchOption.AllDirectories, System.Threading.CancellationToken.None, this);
            Assert.That(dir, Is.Not.Null);

            var dirs = await rinfo.File.Directory.Parent.FindAllDirectoriesAsync(d => d.Name == "Resources", SearchOption.AllDirectories, System.Threading.CancellationToken.None, this);
            Assert.That(dirs, Is.Not.Null);
            Assert.That(dirs.Count, Is.AtLeast(1));
        }

        [Test]
        public async Task TestNet8StreamFunctions()
        {
            var data = Enumerable.Range(0, 256).Select(item => (byte)item).ToArray();

            using var m = new System.IO.MemoryStream();            

            m.WriteAllBytes(data);
            m.Position = 0;

            var dst = new byte[128];
            m.ReadExactly(dst);
            Assert.That(dst.AsSpan().SequenceEqual(data.AsSpan().Slice(0, 128)));

            await m.ReadExactlyAsync(dst);
            Assert.That(dst.AsSpan().SequenceEqual(data.AsSpan().Slice(128, 128)));
        }
    }
}