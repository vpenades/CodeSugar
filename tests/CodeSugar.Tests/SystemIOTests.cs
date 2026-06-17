using System;
using System.IO;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Linq;

using CODESUGARIO = CodeSugar.CodeSugarForSystemIO;

using TUnit;

namespace CodeSugar
{
    public class SystemIOTests : IProgress<int>
    {
        public void Report(int value)
        {
            // TestContext.Progress.WriteLine($"{value}%");
        }

        public void Report(string value)
        {
            // TestContext.Progress.WriteLine(value);
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

            Console.Out.WriteLine($"{Environment.OSVersion.Platform}");
            Console.Out.WriteLine($" comparison: {CODESUGARIO.FileSystemStringComparison}");

            Console.Out.WriteLine($"Separators {System.IO.Path.DirectorySeparatorChar} {System.IO.Path.AltDirectorySeparatorChar}");
            Console.Out.WriteLine($"invalid name chars: " + string.Join(" ",System.IO.Path.GetInvalidFileNameChars()));
            Console.Out.WriteLine($"invalid path chars: " + string.Join(" ", System.IO.Path.GetInvalidPathChars()));

            var finfo = AttachmentInfo.From("test.txt").WriteAllText("hello");

            if (finfo.Directory.TryGetDriveInfo(out var drive))
            {
                Console.Out.WriteLine($"{drive.Name} {drive.DriveFormat} {drive.DriveType}");
            }
            else
            {
                Console.Out.WriteLine("drive can't be retrieved.");
            }

            Console.Out.WriteLine("fixed drives:");
            foreach (var drive2 in DriveInfo.GetDrives())
            {
                if (!drive2.IsReady) continue;
                Console.Out.WriteLine($"{drive2.DriveFormat} {drive2.DriveType}  Name:\"{drive2.Name}\"");
            }
        }

        [Test]
        public void TestPathLengthLimits()
        {
            var tooBigPath = "c:\\" + string.Join("\\", Enumerable.Repeat("PathTooLong", 24));

            var finfo = new System.IO.FileInfo(tooBigPath);
        }


        [Test]
        public async Task TestPaths()
        {
            await Assert.That(System.IO.MatchCasing.PlatformDefault.IsTempPath(System.IO.Path.GetTempPath())).IsTrue();

            await Assert.That(CODESUGARIO.SplitPath("/abc/d/e")).IsSequenceEqualTo(new string[] { "abc", "d", "e" });

            await Assert.That(CODESUGARIO.SplitPath("//network/abc/d/e")).IsSequenceEqualTo(new string[] { "//network", "abc", "d", "e" });

            await Assert.That(CODESUGARIO.SplitDirectoryAndName("/abc/d/e")).IsEqualTo(("abc/d", "e"));

            await Assert.That(CODESUGARIO.SplitDirectoryAndName("//network/abc/d/e")).IsEqualTo(( "//network/abc/d", "e" ));

            if (CODESUGARIO.FileSystemIsCaseSensitive)
            {
                await Assert.That(MatchCasing.PlatformDefault.ArePathsEqual("c:/abc", "c:/abc/")).IsTrue();
                // TODO: TUnit migration - Complex NUnit constraint. Manual conversion required.
                await Assert.That(MatchCasing.PlatformDefault.ArePathsEqual("c:/abc", "c:/abC/")).IsFalse();
            }
            else
            {
                await Assert.That(MatchCasing.PlatformDefault.ArePathsEqual("c:/abc", "c:/abc/")).IsTrue();
                await Assert.That(MatchCasing.PlatformDefault.ArePathsEqual("C:/abc", "c:/ABC/")).IsTrue();
                // TODO: TUnit migration - Complex NUnit constraint. Manual conversion required.
                await Assert.That(MatchCasing.PlatformDefault.ArePathsEqual("c:/abc", "c:/abX/")).IsFalse();
            }

            if (IsWindowsPlatform)
            {
                await Assert.That(CODESUGARIO.SplitPath("\\abc/d\\e")).IsSequenceEqualTo(new string[] { "abc", "d", "e" });
                await Assert.That(MatchCasing.PlatformDefault.ArePathsEqual( "C:\\AbC/", "c:/aBc\\")).IsTrue();
            }
        }


        [Test]
        [Arguments("\\\\192.168.0.200\\temp\\xyz\\", "\\\\192.168.0.200")]
        [Arguments("\\\\192.168.0.200\\temp\\", "\\\\192.168.0.200")]
        [Arguments("\\\\X\\temp\\xyz\\", "\\\\X")]
        [Arguments("c:\\xyz\\abc\\", "C:")]
        [Arguments("c:\\xyz\\abc", "C:")]
        [Arguments("c:", "C:")]
        public async Task TestDrives(string path, string expected)
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
                Console.Out.WriteLine($"Skipping: {path}");
                return;
            }

            var driveName = "-";

            if (someDir.TryGetDriveInfo(out var drive))
            {
                driveName = drive.Name;
            }

            if (someFile.Directory.TryGetDriveInfo(out var drive2))
            {
                await Assert.That(drive2.Name).IsEqualTo(driveName);
            }
            else
            {
                await Assert.That(driveName).IsEqualTo("-");
            }

            Console.Out.WriteLine($"{root} {driveOrNetwork} {driveName}");

            // Assert.That(string.Equals(driveOrNetwork,expected, CodeSugarIO.FileSystemStringComparison));            
        }
        

        [Test]
        public async Task TestFileInfo()
        {
            // var readme_txt = ResourceInfo.From("readme.txt").File;

            // readme_txt = new System.IO.FileInfo(readme_txt.GetNormalizedFullName().Replace("\\","/"));

            var readme_txt = AttachmentInfo.From("readme.txt").WriteAllText("hello world");

            Console.Out.WriteLine(readme_txt.FullName);


            var text = readme_txt.ReadAllText();
            await Assert.That(text).IsEqualTo("hello world");

            var file2 = readme_txt.Directory.GetFileInfo("readme.txt");

            await Assert.That(file2.Exists).IsTrue();
            // Assert.That(readme_txt.FullNameEquals(file2)); // must fix equality handling

            var rfinfo = AttachmentInfo.From("readme_2.txt").WriteObjectEx(f => f.WriteAllText("hello world 2"));

            await Assert.That(rfinfo.ReadAllText()).IsEqualTo("hello world 2");

            await Assert.That(System.Convert.ToHexString(readme_txt.ComputeSha256())).IsEqualTo("B94D27B9934D3E08A52E52D7DA7DABFAC484EFE37A5380EE9088F7ACE2EFCDE9");

            await Assert.That(System.Convert.ToHexString(readme_txt.ComputeSha512())).IsEqualTo("309ECC489C12D6EB4CC40F50C902F2B4D0ED77EE511A7C7A9BCD3CA86D4CD86F989DD35BC5FF499670DA34255B45B0CFD830E81F605DCF7DC5542E93AE9CD76F");

            await Assert.That(readme_txt.Directory.IsParentOf(readme_txt)).IsTrue();

            await Assert.That(readme_txt.GetPathRelativeTo(readme_txt.Directory)).IsEqualTo("readme.txt");

            // Assert.That(readme_txt.GetRelativePath(readme_txt.Directory.Parent), Is.EqualTo("Resources\\readme.txt")); // equality

            var dcomparer = MatchCasing.PlatformDefault.GetFullNameEqualityComparer<DirectoryInfo>();

            var tmp0 = new System.IO.DirectoryInfo("temp\\");
            var tmp1 = tmp0.DefineDirectoryInfo("a", "..", ".", "b", "..");
            await Assert.That(dcomparer.Equals(tmp0, tmp1)).IsTrue();

            

            Assert.Throws<ArgumentException>(() => readme_txt.Directory.DefineFileInfo(" abc"));
            Assert.Throws<ArgumentException>(() => readme_txt.Directory.DefineFileInfo("abc "));
            Assert.Throws<ArgumentException>(() => readme_txt.Directory.DefineFileInfo("abc ", "readme.txt"));

            Assert.Throws<ArgumentException>(() => readme_txt.Directory.DefineFileInfo(".."));
            Assert.Throws<ArgumentException>((() => readme_txt.Directory.DefineFileInfo(".")));
            Assert.Throws<ArgumentNullException>(() => readme_txt.Directory.DefineFileInfo("/"));


            if (IsWindowsPlatform)
            {
                // Assert.Throws<ArgumentException>((Action)(() => readme_txt.Directory.DefineFileInfo(":"))); // throws DebugAssetException which is internal
                Assert.Throws<ArgumentException>(() => readme_txt.Directory.DefineFileInfo("*"));
                Assert.Throws<ArgumentException>(() => readme_txt.Directory.DefineFileInfo("?"));
                Assert.Throws<ArgumentNullException>(() => readme_txt.Directory.DefineFileInfo("\\"));
            }            
        }


        [Test]
        public async Task TestAlternateDataStreams()
        {
            if (!IsWindowsPlatform) return;

            // https://arstechnica.com/civis/threads/is-winfs-vulnerable-to-alternate-data-streams.468008/
            // https://blog.j2i.net/2021/12/11/working-with-alternative-data-streamsthe-hidden-part-of-your-windows-file-system-on-windows/

            // apparently, writing the main content erases all additional data streams.

            var workFile = AttachmentInfo.From("readme_with_ADS.txt").WriteAllText("hello world");
            await Assert.That(workFile.TryGetAlternateDataStream("ads.bin", out var adsInfo)).IsTrue();
            
            await Assert.That(adsInfo.Exists).IsFalse();

            var data = new Byte[] { 1, 2, 3, 4 };

            adsInfo.WriteAllBytes(data);
            await Assert.That(adsInfo.Exists).IsTrue();

            await Assert.That(workFile.ReadAllText()).IsEqualTo("hello world");
            await Assert.That(adsInfo.ReadAllBytes()).IsSequenceEqualTo(data);
        }


        [Test]
        public async Task TestFileInfoEquality()
        {
            // https://stackoverflow.com/questions/430256/how-do-i-determine-whether-the-filesystem-is-case-sensitive-in-net
            // https://stackoverflow.com/questions/7344978/verifying-path-equality-with-net

            // linux is case insensitive, but external drives on windows can also be.

            // so the procedure would be:
            // 1- check whether both files are located in the SAME DRIVE, if not, return false.
            // 2- check the drive type to identify whether to compare as case sensitive or not.

            // test file case sensitivity

            var testDir = new System.IO.DirectoryInfo(AppContext.BaseDirectory);

            var readme_txt_0 = testDir.UseFileInfo("readme.txt");
            var readme_txt_1 = testDir.UseFileInfo("README.txt");

            readme_txt_0.WriteAllText("lowercase");
            readme_txt_1.WriteAllText("uppercase");

            foreach(var readme_txt in testDir.GetFiles("*.txt"))
            {
                Console.Out.WriteLine(readme_txt.FullName);
            }

            bool isCaseSensitiveOS = readme_txt_0.ReadAllText() == "lowercase";

            Console.Out.WriteLine($"OS file system is case sensitive: {isCaseSensitiveOS}");

            var fcomparer = MatchCasing.PlatformDefault.GetFullNameEqualityComparer<FileInfo>();
            var dcomparer = MatchCasing.PlatformDefault.GetFullNameEqualityComparer<DirectoryInfo>();
            
            await Assert.That(fcomparer.Equals(readme_txt_0, readme_txt_1)).IsNotEqualTo(isCaseSensitiveOS);

            // test directory equality with various tails.

            var tmp0 = new System.IO.DirectoryInfo("temp");
            var tmp1 = new System.IO.DirectoryInfo("temp/");           

            await Assert.That(dcomparer.Equals(tmp0, tmp1)).IsTrue();

            if (System.IO.Path.DirectorySeparatorChar == '\\')
            {
                var tmp2 = new System.IO.DirectoryInfo("temp\\"); // ubuntu & mac consider \\ as a valid character
                await Assert.That(dcomparer.Equals(tmp0, tmp2)).IsTrue();
                await Assert.That(dcomparer.Equals(tmp1, tmp2)).IsTrue();
            }
        }

        


        [Test]
        public async Task TestShortcuts()
        {
            var uri = new Uri("http://www.google.com");

            var finfo = AttachmentInfo.From("test.url").WriteObjectEx(f => f.WriteShortcut(uri));

            await Assert.That(finfo.ReadShortcutUri()).IsEqualTo(uri);

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

            await Assert.That(url2.TryResolveShortcutFile(out var resolvedFile)).IsTrue();

            await Assert.That(resolvedFile.FullNameEquals(textFile)).IsTrue();

            // test resolve file:            

            url1 = AttachmentInfo.From("dir.1.url").WriteShortcut(textFile.Directory.FullName);
            url2 = AttachmentInfo.From("dir.2.url").WriteShortcut(url1.FullName);

            await Assert.That(url2.TryResolveShortcutDir(out var resolvedDir)).IsTrue();

            await Assert.That(resolvedDir.FullNameEquals(textFile.Directory)).IsTrue();

        }

        [Test]
        public async Task TestZip()
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
                    await Assert.That(txt).IsEqualTo("hello world");

                    var dict = zip.ToDictionary();
                    await Assert.That(dict).ContainsKey("readme.txt");
                }
            }            


        }


        [Test]
        public async Task TestEnumeration()
        {
            var rinfo = ResourceInfo.From("readme.txt");            

            var file = await rinfo.File.Directory.Parent.FindFirstFileAsync(f => f.Name == "readme.txt", SearchOption.AllDirectories, System.Threading.CancellationToken.None, this);
            await Assert.That(file).IsNotNull();

            var files = await rinfo.File.Directory.Parent.FindAllFilesAsync(f => f.Name == "readme.txt", SearchOption.AllDirectories, System.Threading.CancellationToken.None, this);
            await Assert.That(files).IsNotNull();
            // TODO: TUnit migration - Complex NUnit constraint. Manual conversion required.
            await Assert.That(files).Count().IsGreaterThanOrEqualTo(1);

            var dir = await rinfo.File.Directory.Parent.FindFirstDirectoryAsync(d => d.Name == "Resources", SearchOption.AllDirectories, System.Threading.CancellationToken.None, this);
            await Assert.That(dir).IsNotNull();

            var dirs = await rinfo.File.Directory.Parent.FindAllDirectoriesAsync(d => d.Name == "Resources", SearchOption.AllDirectories, System.Threading.CancellationToken.None, this);
            await Assert.That(dirs).IsNotNull();
            // TODO: TUnit migration - Complex NUnit constraint. Manual conversion required.
            await Assert.That(dirs).Count().IsGreaterThanOrEqualTo(1);
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
            await Assert.That(dst.AsSpan().SequenceEqual(data.AsSpan().Slice(0, 128))).IsTrue();

            await m.ReadExactlyAsync(dst);
            await Assert.That(dst.AsSpan().SequenceEqual(data.AsSpan().Slice(128, 128))).IsTrue();
        }
    }
}