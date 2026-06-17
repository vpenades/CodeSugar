using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InteropTypes.IO;

using TUnit;

namespace CodeSugar
{
    internal class StreamTests
    {
        [Test]
        public async Task TestStreamsAsync()
        {
            await _TestReadWriteBytesAsync(() => new System.IO.MemoryStream());

            void _write(System.IO.FileInfo path)
            {
                Task.Run(() => _TestReadWriteBytesAsync(() => path.Open(FileMode.Create, FileAccess.ReadWrite)));
            }

            AttachmentInfo.From("tmp.bin").WriteObjectEx(_write);
        }

        private static async Task _TestReadWriteBytesAsync(Func<System.IO.Stream> streamFactory)
        {
            var rnd = new byte[1000000];
            new Random().NextBytes(rnd);

            using (var m = streamFactory())
            {
                m.WriteAllBytes(rnd);
                await Assert.That(m.Length).IsEqualTo(rnd.Length);
                m.Position = 0;
                await Assert.That(m.ReadAllBytes()).IsSequenceEqualTo(rnd);
            }

            using (var m = streamFactory())
            {
                await m.WriteAllBytesAsync(rnd, System.Threading.CancellationToken.None);
                await Assert.That(m.Length).IsEqualTo(rnd.Length);
                m.Position = 0;
                var r = await m.ReadAllBytesAsync(System.Threading.CancellationToken.None);
                await Assert.That(r).IsSequenceEqualTo(rnd);
            }
        }



        [Test]
        [Explicit]
        [Arguments(65536 * 10, 10000, false)]
        [Arguments(65536 * 10, 65536 * 20, false)]
        [Arguments(65536 * 10, 1, true)]

        // explicits due to being very large
        [Arguments(65536L * 65536 * 2, 10000, true)]        
        [Arguments(65536L * 65536 * 2, 65536 * 20, true)]        
        [Arguments(65536L * 65536 * 2, 1, true)]       
        public async Task TestStreamEquality(long streamsLen, int buffLen, bool useFactory)
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

            await Assert.That(rnd1.StreamEquals(rnd1)).IsTrue();
            // TODO: TUnit migration - Complex NUnit constraint. Manual conversion required.

            await Assert.That(rnd1.StreamEquals(rnd2, factory, buffLen)).IsFalse();
            await Assert.That(rnd1.Position).IsZero();
            await Assert.That(rnd2.Position).IsZero();

            await Assert.That(rnd2.StreamEquals(rnd3, factory, buffLen)).IsTrue();
            await Assert.That(rnd1.Position).IsZero();
            await Assert.That(rnd2.Position).IsZero();

            // same length, same content, different streams
            var f1 = ResourceInfo.From("readme.txt").File;
            var f2 = ResourceInfo.From("readme.txt").File;
            await Assert.That(f1.StreamEquals(f2, factory, buffLen)).IsTrue();

            // same length but not same content
            using var m1 = new System.IO.MemoryStream(); m1.WriteAllText("ABC"); m1.Position = 0;
            using var m2 = new System.IO.MemoryStream(); m2.WriteAllText("CBA"); m2.Position = 0;
            // TODO: TUnit migration - Complex NUnit constraint. Manual conversion required.
            await Assert.That(m1.StreamEquals(m2, factory, buffLen)).IsFalse();
            await Assert.That(m1.Position).IsZero();
            await Assert.That(m2.Position).IsZero();
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
                await Assert.That(fromPosition1).IsSequenceEqualTo(new Byte[] { 2, 3 });

                m.Position = 1;
                fromPosition1 = await m.ReadAllBytesAsync(System.Threading.CancellationToken.None);
                await Assert.That(fromPosition1).IsSequenceEqualTo(new Byte[] { 2, 3 });
            }
        }
    }
}