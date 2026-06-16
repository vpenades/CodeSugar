// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

#nullable disable

using __STREAM = System.IO.Stream;
using __MEMSTREAM = System.IO.MemoryStream;
using __BYTESSEGMENT = System.ArraySegment<byte>;


#if __REFERENCES_MICROSOFTIORECYCLABLEMEMORYSTREAM
using __BIGMEMSTREAM = Microsoft.IO.RecyclableMemoryStream;
#endif



#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarForSystemIO
    {
        public static __MEMSTREAM ToMemoryStream([DisallowNull] this __STREAM stream)
        {
            switch (stream)
            {
                case null: throw new ArgumentNullException(nameof(stream));
                case __MEMSTREAM ms: return ms;
                default:
                    {
                        var ms = new __MEMSTREAM();
                        stream.CopyTo(ms);
                        ms.Position = 0;
                        return ms;
                    }
            }
        }

        public static async Task<__MEMSTREAM> ToMemoryStreamAsync([DisallowNull] this __STREAM stream, CancellationToken token)
        {
            switch (stream)
            {
                case null: throw new ArgumentNullException(nameof(stream));
                case __MEMSTREAM ms: return ms;
                default:
                    {
                        var ms = new __MEMSTREAM();
                        await stream.CopyToAsync(ms, token);
                        ms.Position = 0;
                        return ms;
                    }
            }
        }

        #if __REFERENCES_MICROSOFTIORECYCLABLEMEMORYSTREAM

        public static __MEMSTREAM ToMemoryStream([DisallowNull] this __STREAM stream, Microsoft.IO.RecyclableMemoryStreamManager manager)
        {
            switch (stream)
            {
                case null: throw new ArgumentNullException(nameof(stream));
                case __MEMSTREAM ms: return ms;
                default:
                    {
                        var ms = new __BIGMEMSTREAM(manager);
                        stream.CopyTo(ms);
                        ms.Position = 0;
                        return ms;
                    }
            }
        }

        public static async Task<__MEMSTREAM> ToMemoryStreamAsync([DisallowNull] this __STREAM stream, Microsoft.IO.RecyclableMemoryStreamManager manager, CancellationToken token)
        {
            switch (stream)
            {
                case null: throw new ArgumentNullException(nameof(stream));
                case __MEMSTREAM ms: return ms;
                default:
                    {
                        var ms = new __BIGMEMSTREAM(manager);
                        await stream.CopyToAsync(ms, token);
                        ms.Position = 0;
                        return ms;
                    }
            }
        }

        #endif
    }
}