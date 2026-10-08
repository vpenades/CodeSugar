using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

#nullable disable

using __STREAM = System.IO.Stream;
using __MEMSTREAM = System.IO.MemoryStream;
using __BYTESSEGMENT = System.ArraySegment<byte>;

using __STREAMFUNC = System.Func<System.IO.FileMode, System.IO.Stream>;
using __STREAMTASK = System.Func<System.IO.FileMode, System.Threading.CancellationToken, System.Threading.Tasks.Task<System.IO.Stream>>;

#if __REFERENCES_MICROSOFTIORECYCLABLEMEMORYSTREAM
using __BIGMEMSTREAM = Microsoft.IO.RecyclableMemoryStream;
#endif

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        [Obsolete("Use TryGetArraySegment", true)]
        public static bool TryGetMemoryBuffer(this __STREAM stream, out __BYTESSEGMENT segment)
        {
            return TryGetArraySegment(stream, out segment);
        }

        /// <summary>
        /// returns the internal memory buffer if the stream is a <see cref="__MEMSTREAM"/>
        /// </summary>
        /// <param name="stream">The stream to probe</param>
        /// <param name="segment">the buffer</param>
        /// <returns>true on success</returns>
        public static bool TryGetArraySegment(this __STREAM stream, out __BYTESSEGMENT segment)
        {
            if (stream is __MEMSTREAM mem && mem.TryGetBuffer(out segment)) return true;
            segment = default;
            return false;
        }

        public static __MEMSTREAM ToMemoryStream([DisallowNull] this __STREAMFUNC readerFunc)
        {
            // wrapping this way prevents a double memory copy
            // if readerFunc already returns a MemoryStream
            var s = readerFunc.OpenRead();
            return GetOrReadAsMemoryStream(s, true); 
        }

        public static async Task<__MEMSTREAM> ToMemoryStreamAsync([DisallowNull] this __STREAMTASK readerTask, CancellationToken token)
        {
            // wrapping this way prevents a double memory copy
            // if readerTask already returns a MemoryStream

            var s = await readerTask.OpenWriteAsync(token).ConfigureAwait(false);
            return await GetOrReadAsMemoryStreamAsync(s, token, true).ConfigureAwait(false);
        }


        [return: NotNull]
        private static __MEMSTREAM _ToMemoryStream(__BYTESSEGMENT segment)
        {
            return new __MEMSTREAM(segment.Array ?? Array.Empty<byte>(), segment.Offset, segment.Count, false);
        }

        [return: NotNull]
        public static __MEMSTREAM GetOrReadAsMemoryStream([DisallowNull] this __STREAM stream, bool disposeStream = true)
        {
            switch (stream)
            {
                case null: throw new ArgumentNullException(nameof(stream));
                case __MEMSTREAM ms:
                    // do not set ms.Position = 0
                    return ms;
                default:
                    {
                        var ms = new __MEMSTREAM();
                        stream.CopyTo(ms);
                        ms.Position = 0;
                        if (disposeStream) stream.Dispose();
                        return ms;
                    }
            }
        }

        [return: NotNull]
        public static async Task<__MEMSTREAM> GetOrReadAsMemoryStreamAsync([DisallowNull] this __STREAM stream, CancellationToken token, bool disposeStream = true)
        {
            switch (stream)
            {
                case null: throw new ArgumentNullException(nameof(stream));
                case __MEMSTREAM ms:
                    // do not set ms.Position = 0
                    return ms;
                default:
                    {
                        var ms = new __MEMSTREAM();
                        await stream.CopyToAsync(ms, token).ConfigureAwait(false);
                        ms.Position = 0;
                        if (disposeStream) stream.Dispose();
                        return ms;
                    }
            }
        }

        #if __REFERENCES_MICROSOFTIORECYCLABLEMEMORYSTREAM

        [return: NotNull]
        public static __MEMSTREAM GetOrReadAsMemoryStream([DisallowNull] this __STREAM stream, Microsoft.IO.RecyclableMemoryStreamManager manager, bool disposeStream = true)
        {
            switch (stream)
            {
                case null: throw new ArgumentNullException(nameof(stream));
                case __MEMSTREAM ms:
                    // do not set ms.Position = 0
                    return ms;
                default:
                    {
                        var ms = new __BIGMEMSTREAM(manager);
                        stream.CopyTo(ms);
                        ms.Position = 0;
                        if (disposeStream) stream.Dispose();
                        return ms;
                    }
            }
        }

        [return: NotNull]
        public static async Task<__MEMSTREAM> GetOrReadAsMemoryStreamAsync([DisallowNull] this __STREAM stream, Microsoft.IO.RecyclableMemoryStreamManager manager, CancellationToken token, bool disposeStream = true)
        {
            switch (stream)
            {
                case null: throw new ArgumentNullException(nameof(stream));
                case __MEMSTREAM ms:
                    // do not set ms.Position = 0
                    return ms;
                default:
                    {
                        var ms = new __BIGMEMSTREAM(manager);
                        await stream.CopyToAsync(ms, token).ConfigureAwait(false);
                        ms.Position = 0;
                        if (disposeStream) stream.Dispose();
                        return ms;
                    }
            }
        }

        #endif
    }
}