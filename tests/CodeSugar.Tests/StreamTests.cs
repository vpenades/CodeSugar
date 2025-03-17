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

        [Test]
        public async Task MemoryStreamTests()
        {
            using (var m = new System.IO.MemoryStream())
            {
                m.WriteU8(1);
                m.WriteU8(2);
                m.WriteU8(3);

                m.Position = 1;
                var fromPosition1 = m.ReadAllBytes();
                Assert.That(fromPosition1, Is.EqualTo(new Byte[] { 2, 3 }));

                m.Position = 1;
                fromPosition1 = await m.ReadAllBytesAsync(System.Threading.CancellationToken.None);
                Assert.That(fromPosition1, Is.EqualTo(new Byte[] { 2, 3 }));
            }
        }
    }
}
