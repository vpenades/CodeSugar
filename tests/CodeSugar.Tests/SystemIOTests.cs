using System;
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;

namespace CodeSugar.Tests
{
    public class Tests
    {
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

            TestContext.WriteLine($"{Environment.OSVersion.Platform}");
            TestContext.WriteLine($" comparison: {CodeSugarIO.FileSystemStringComparison}");

            TestContext.WriteLine($"Separators {System.IO.Path.DirectorySeparatorChar} {System.IO.Path.AltDirectorySeparatorChar}");
            TestContext.WriteLine($"invalid name chars: " + string.Join(" ",System.IO.Path.GetInvalidFileNameChars()));
            TestContext.WriteLine($"invalid path chars: " + string.Join(" ", System.IO.Path.GetInvalidPathChars()));

            var finfo = AttachmentInfo.From("test.txt").WriteAllText("hello");

            if (finfo.Directory.TryGetDriveInfo(out var drive))
            {
                TestContext.WriteLine($"{drive.Name} {drive.DriveFormat} {drive.DriveType}");
            }
            else
            {
                TestContext.WriteLine("drive can't be retrieved.");
            }

            TestContext.WriteLine("fixed drives:");
            foreach (var drive2 in DriveInfo.GetDrives())
            {
                if (!drive2.IsReady) continue;
                TestContext.WriteLine($"{drive2.DriveFormat} {drive2.DriveType}  Name:\"{drive2.Name}\"");
            }
        }


        [Test]
        public void TestPaths()
        {
            Assert.That(CodeSugarIO.SplitPath("/abc/d/e"), Is.EqualTo(new string[] { "abc", "d", "e" }));

            Assert.That(CodeSugarIO.SplitPath("//network/abc/d/e"), Is.EqualTo(new string[] { "//network", "abc", "d", "e" }));

            Assert.That(CodeSugarIO.SplitDirectoryAndName("/abc/d/e"), Is.EqualTo(("abc/d", "e")));
            Assert.That(CodeSugarIO.SplitDirectoryAndName("//network/abc/d/e"), Is.EqualTo(( "//network/abc/d", "e" )));

            if (CodeSugarIO.FileSystemIsCaseSensitive)
            {
                Assert.That(CodeSugarIO.ArePathsEqual("c:/abc", "c:/abc/"));
                Assert.That(CodeSugarIO.ArePathsEqual("c:/abc", "c:/abC/"), Is.Not.True);                
            }
            else
            {
                Assert.That(CodeSugarIO.ArePathsEqual("c:/abc", "c:/abc/"));
                Assert.That(CodeSugarIO.ArePathsEqual("C:/abc", "c:/ABC/"));
                Assert.That(CodeSugarIO.ArePathsEqual("c:/abc", "c:/abX/"), Is.Not.True);                
            }

            if (IsWindowsPlatform)
            {
                Assert.That(CodeSugarIO.SplitPath("\\abc/d\\e"), Is.EqualTo(new string[] { "abc", "d", "e" }));
                Assert.That(CodeSugarIO.ArePathsEqual("C:\\AbC/", "c:/aBc\\"));
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
                .GetFile("readme.txt")
                .TryGetAlternateDataStream("data.bin", out var someFile);
            */

            var someFile = someDir.GetFile("readme.txt");

            // var xroot = System.IO.Path.GetPathRoot(path);
            // var xdrive = new System.IO.DriveInfo(xroot);            

            var driveOrNetwork = someDir.GetDriveOrNetworkName();
            if (driveOrNetwork == null)
            {
                TestContext.WriteLine($"Skipping: {path}");
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

            TestContext.WriteLine($"{root} {driveOrNetwork} {driveName}");

            // Assert.That(string.Equals(driveOrNetwork,expected, CodeSugarIO.FileSystemStringComparison));            
        }
        

        [Test]
        public void TestFileInfo()
        {
            // var readme_txt = ResourceInfo.From("readme.txt").File;

            // readme_txt = new System.IO.FileInfo(readme_txt.GetNormalizedFullName().Replace("\\","/"));

            var readme_txt = AttachmentInfo.From("readme.txt").WriteAllText("hello world");

            TestContext.WriteLine(readme_txt.FullName);


            var text = readme_txt.ReadAllText();
            Assert.That(text, Is.EqualTo("hello world"));

            var file2 = readme_txt.Directory.GetFile("readme.txt");

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

            var dcomparer = CodeSugarIO.GetFullNameComparer<DirectoryInfo>();

            var tmp0 = new System.IO.DirectoryInfo("temp\\");
            var tmp1 = tmp0.GetDirectory("a", "..", ".", "b", "..");
            Assert.That(dcomparer.Equals(tmp0, tmp1));
            
            Assert.That(() => readme_txt.Directory.GetFile(".."), Throws.Exception);
            Assert.That(() => readme_txt.Directory.GetFile("."), Throws.Exception);            
            Assert.That(() => readme_txt.Directory.GetFile("/"), Throws.Exception);

            if (IsWindowsPlatform)
            {
                Assert.That(() => readme_txt.Directory.GetFile(":"), Throws.Exception);
                Assert.That(() => readme_txt.Directory.GetFile("*"), Throws.Exception);
                Assert.That(() => readme_txt.Directory.GetFile("?"), Throws.Exception);
                Assert.That(() => readme_txt.Directory.GetFile("\\"), Throws.Exception);
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

            var readme_txt_0 = testDir.GetFile("readme.txt");
            var readme_txt_1 = testDir.GetFile("README.txt");

            readme_txt_0.WriteAllText("lowercase");
            readme_txt_1.WriteAllText("uppercase");

            foreach(var readme_txt in testDir.GetFiles("*.txt"))
            {
                TestContext.WriteLine(readme_txt.FullName);
            }

            bool isCaseSensitiveOS = readme_txt_0.ReadAllText() == "lowercase";

            TestContext.WriteLine($"OS file system is case sensitive: {isCaseSensitiveOS}");

            var fcomparer = CodeSugarIO.GetFullNameComparer<FileInfo>();
            var dcomparer = CodeSugarIO.GetFullNameComparer<DirectoryInfo>();
            
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
        public void TestSerialization()
        {
            using var m = new System.IO.MemoryStream();

            var dt = new DateTime(2000, 3, 4).ToLocalTime();
            var d = new DateOnly(dt.Year, dt.Month, dt.Day);
            var v3 = new System.Numerics.Vector3(1, 2, 3);
            
            m.WriteValue<int>(10);
            m.WriteValue<long>(-100, true);
            m.WriteString("hello world");
            m.WriteValue(dt);
            m.WriteValue(v3);
            m.WriteSigned64Packed(-345);
            m.WriteSigned64Packed(long.MinValue);
            m.WriteSigned64Packed(long.MaxValue);
            m.WriteUnsigned64Packed(345);
            m.WriteUnsigned64Packed(ulong.MinValue);
            m.WriteUnsigned64Packed(ulong.MaxValue);
            m.WriteValue(d);
            m.WriteValues(1, 2, 3, 4);
            m.WriteValue(TypeCode.Int32);

            m.WriteString("Direct");
            using (var bw = m.CreateBinaryWriter(true))
            {
                bw.Write("BinaryWriter");
            }

            Assert.Throws<ArgumentException>(() => m.WriteValue((0, 1)));

            m.Position = 0;            

            Assert.That(m.ReadValue<int>(), Is.EqualTo(10));
            Assert.That(m.ReadValue<long>(true), Is.EqualTo(-100));
            Assert.That(m.ReadString(), Is.EqualTo("hello world"));
            Assert.That(m.ReadValue<DateTime>(), Is.EqualTo(dt));
            Assert.That(m.ReadValue<System.Numerics.Vector3>(), Is.EqualTo(v3));
            Assert.That(m.ReadSigned64Packed(), Is.EqualTo(-345));
            Assert.That(m.ReadSigned64Packed(), Is.EqualTo(long.MinValue));
            Assert.That(m.ReadSigned64Packed(), Is.EqualTo(long.MaxValue));
            Assert.That(m.ReadUnsigned64Packed(), Is.EqualTo(345));
            Assert.That(m.ReadUnsigned64Packed(), Is.EqualTo(ulong.MinValue));
            Assert.That(m.ReadUnsigned64Packed(), Is.EqualTo(ulong.MaxValue));
            Assert.That(m.ReadValue<DateOnly>(), Is.EqualTo(d));
            Assert.That(m.ReadValues<int,int,int,int>(), Is.EqualTo((1,2,3,4)));
            Assert.That(m.ReadValue<TypeCode>(), Is.EqualTo(TypeCode.Int32));

            using(var br = m.CreateBinaryReader(true))
            {
                Assert.That(br.ReadString(), Is.EqualTo("Direct"));
            }
            Assert.That(m.ReadString(), Is.EqualTo("BinaryWriter"));

            Assert.Throws<ArgumentException>(() => m.ReadValue<(int,int)>());
        }

        [Test]
        public async Task TestStreamsAsync()
        {
            await _TestReadWriteBytesAsync(()=> new System.IO.MemoryStream());

            AttachmentInfo.From("tmp.bin").WriteObjectEx(async f => await _TestReadWriteBytesAsync(() => f.Open(FileMode.Create,FileAccess.ReadWrite)));
        }

        private static async Task _TestReadWriteBytesAsync(Func<System.IO.Stream> streamFactory)
        {
            var rnd = new byte[1000000];
            new Random().NextBytes(rnd);

            using (var m = streamFactory())
            {
                m.WriteAllBytes(rnd);
                Assert.That(m.Length == rnd.Length);
                m.Position = 0;
                Assert.That(m.ReadAllBytes(), Is.EqualTo(rnd));
            }

            using (var m = streamFactory())
            {
                await m.WriteAllBytesAsync(rnd, System.Threading.CancellationToken.None);
                Assert.That(m.Length == rnd.Length);
                m.Position = 0;
                var r = await m.ReadAllBytesAsync(System.Threading.CancellationToken.None);
                Assert.That(r, Is.EqualTo(rnd));
            }
        }


        [Test]
        public void TestShortcuts()
        {
            var uri = new Uri("http://www.google.com");

            var finfo = AttachmentInfo.From("test.url").WriteObjectEx(f => f.WriteShortcutUri(uri));

            Assert.That(finfo.ReadShortcutUri(), Is.EqualTo(uri));

            var finfo2 = AttachmentInfo.From("test2.url").WriteObjectEx(f => f.WriteShortcutUri(new Uri(finfo.FullName)));            

            if (finfo2.TryReadShortcutFile(out var targetFile))
            {
                finfo.FullNameEquals(targetFile);
            }            
        }

        [Test]
        public void TestZip()
        {
            using (var context = new AttachmentDirectory("tests"))
            {
                using(var zip = context.Directory.GetFile("test.zip").CreateZipArchive())
                {
                    zip.CreateEntry("readme.txt").WriteAllText("hello world");
                }

                using (var zip = context.Directory.GetFile("test.zip").OpenReadZipArchive())
                {
                    var txt = zip.GetEntry("readme.txt").ReadAllText();
                    Assert.That(txt, Is.EqualTo("hello world"));
                }
            }

        }

    }
}