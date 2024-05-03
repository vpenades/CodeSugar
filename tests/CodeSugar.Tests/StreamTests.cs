using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InteropTypes.IO;

using NUnit.Framework;

namespace CodeSugar
{
    internal class StreamTests
    {
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
            Assert.That(m.ReadValues<int, int, int, int>(), Is.EqualTo((1, 2, 3, 4)));
            Assert.That(m.ReadValue<TypeCode>(), Is.EqualTo(TypeCode.Int32));

            using (var br = m.CreateBinaryReader(true))
            {
                Assert.That(br.ReadString(), Is.EqualTo("Direct"));
            }
            Assert.That(m.ReadString(), Is.EqualTo("BinaryWriter"));

            Assert.Throws<ArgumentException>(() => m.ReadValue<(int, int)>());
        }

        [Test]
        public async Task TestStreamsAsync()
        {
            await _TestReadWriteBytesAsync(() => new System.IO.MemoryStream());

            AttachmentInfo.From("tmp.bin").WriteObjectEx(async f => await _TestReadWriteBytesAsync(() => f.Open(FileMode.Create, FileAccess.ReadWrite)));
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



        [TestCase(65536 * 10,10000, false)]
        [TestCase(65536 * 10, 65536 * 20, false)]
        [TestCase(65536 * 10, 1, true)]        
        [TestCase(65536L * 65536 * 2, 10000, true, Explicit = true)]
        [TestCase(65536L * 65536 * 2, 65536 * 20, true, Explicit = true)]
        [TestCase(65536L * 65536 * 2, 1, true, Explicit = true)]        
        public void TestStreamEquality(long streamsLen, int buffLen, bool useFactory)
        {
            var rnd1 = new RandomStream(streamsLen, 1);
            var rnd2 = new RandomStream(streamsLen, 2);
            var rnd3 = new RandomStream(streamsLen, 2);

            System.IO.MemoryStream memStreamFactory(long len)
            {
                if (len < int.MaxValue) return new MemoryStream((int)len);

                var mgr = new Microsoft.IO.RecyclableMemoryStreamManager();
                return new Microsoft.IO.RecyclableMemoryStream(mgr, null, len);
            }

            Func<long, System.IO.MemoryStream> factory = memStreamFactory;

            if (!useFactory) factory = null;

            Assert.That(rnd1.StreamEquals(rnd1));

            Assert.That(rnd1.StreamEquals(rnd2, factory, buffLen), Is.Not.True);
            Assert.That(rnd1.Position, Is.Zero);
            Assert.That(rnd2.Position, Is.Zero);

            Assert.That(rnd2.StreamEquals(rnd3, factory, buffLen), Is.True);
            Assert.That(rnd1.Position, Is.Zero);
            Assert.That(rnd2.Position, Is.Zero);
            
            // same length, same content, different streams
            var f1 = ResourceInfo.From("readme.txt").File;
            var f2 = ResourceInfo.From("readme.txt").File;
            Assert.That(f1.StreamEquals(f2, factory, buffLen), Is.True);

            // same length but not same content
            using var m1 = new System.IO.MemoryStream(); m1.WriteAllText("ABC"); m1.Position = 0;
            using var m2 = new System.IO.MemoryStream(); m2.WriteAllText("CBA"); m2.Position = 0;
            Assert.That(m1.StreamEquals(m2, factory, buffLen), Is.Not.True);
            Assert.That(m1.Position, Is.Zero);
            Assert.That(m2.Position, Is.Zero);
        }
    }
}
