using System;
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

using CodeSugar;

namespace CodeSugar.Tests
{
    public class Tests
    {
        [Test]
        public void TestFileInfo()
        {
            var readme_txt = ResourceInfo.From("readme.txt");

            

            var text = readme_txt.File.ReadAllText();
            Assert.That(text, Is.EqualTo("hello world"));

            var file2 = readme_txt.File.Directory.GetFile("readme.txt");

            Assert.That(file2.Exists);
            Assert.That(readme_txt.File.FullNameEquals(file2));

            var rfinfo = AttachmentInfo.From("readme.txt").WriteObjectEx(f => f.WriteAllText("hello world 2"));

            Assert.That(rfinfo.ReadAllText(),
                Is.EqualTo("hello world 2"));

            Assert.That(System.Convert.ToHexString(readme_txt.File.ComputeSha256()),
                Is.EqualTo("B94D27B9934D3E08A52E52D7DA7DABFAC484EFE37A5380EE9088F7ACE2EFCDE9"));            

            Assert.That(System.Convert.ToHexString(readme_txt.File.ComputeSha512()),
                Is.EqualTo("309ECC489C12D6EB4CC40F50C902F2B4D0ED77EE511A7C7A9BCD3CA86D4CD86F989DD35BC5FF499670DA34255B45B0CFD830E81F605DCF7DC5542E93AE9CD76F"));

            Assert.That(readme_txt.File.Directory.ContainsFileOrDirectory(readme_txt.File));

            Assert.That(readme_txt.File.GetRelativePath(readme_txt.File.Directory),
                Is.EqualTo("readme.txt"));

            Assert.That(readme_txt.File.GetRelativePath(readme_txt.File.Directory.Parent),
                Is.EqualTo("Resources\\readme.txt"));

            var dcomparer = Environment.OSVersion.GetFullNameComparer<DirectoryInfo>();

            var tmp0 = new System.IO.DirectoryInfo("temp\\");
            var tmp1 = tmp0.GetDirectory("a", "..", ".", "b", "..");
            Assert.That(dcomparer.Equals(tmp0, tmp1));

            Assert.That(() => readme_txt.File.Directory.GetFile(".."), Throws.ArgumentException);
            Assert.That(() => readme_txt.File.Directory.GetFile(":*?"), Throws.ArgumentException);
            Assert.That(() => readme_txt.File.Directory.GetFile("\\"), Throws.ArgumentException);
            
        }

        
        [TestCase("\\\\192.168.0.200\\temp\\xyz\\", "\\\\192.168.0.200")]
        [TestCase("\\\\192.168.0.200\\temp\\", "\\\\192.168.0.200")]
        [TestCase("\\\\X\\temp\\xyz\\", "\\\\X")]
        [TestCase("c:\\xyz\\abc\\", "C:")]
        [TestCase("c:\\xyz\\abc", "C:")]
        [TestCase("c:", "C:")]
        public void TestDrives(string path, string expected)
        {
            var root = System.IO.Path.GetPathRoot(path);            

            var networkDir = new System.IO.DirectoryInfo(path);

            var driveOrNetwork = networkDir.GetDriveOrNetworkName();

            var driveName = "-";            

            if (networkDir.TryGetDriveInfo(out var drive))
            {
                driveName = drive.Name;                
            }

            TestContext.WriteLine($"{root} {driveOrNetwork} {driveName}");

            Assert.That(driveOrNetwork, Is.EqualTo(expected));            
        }


        [Test]
        public void TestFileInfoEquality()
        {
            var fcomparer = Environment.OSVersion.GetFullNameComparer<FileInfo>();
            var dcomparer = Environment.OSVersion.GetFullNameComparer<DirectoryInfo>();

            var readme_txt_0 = ResourceInfo.From("readme.txt");
            var readme_txt_1 = ResourceInfo.From("README.TXT");
            Assert.That(fcomparer.Equals(readme_txt_0.File, readme_txt_1.File));

            var tmp0 = new System.IO.DirectoryInfo("temp");
            var tmp1 = new System.IO.DirectoryInfo("temp/");
            var tmp2 = new System.IO.DirectoryInfo("temp\\");

            Assert.That(dcomparer.Equals(tmp0, tmp1));
            Assert.That(dcomparer.Equals(tmp0, tmp2));
            Assert.That(dcomparer.Equals(tmp1, tmp2));
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

            Assert.Throws<ArgumentException>( ()=>m.WriteValue((0, 1)));

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
    }
}