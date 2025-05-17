// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

#nullable disable

using _STREAM = System.IO.Stream;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForSerialization
    {
        // these methods are copied from CodeSugar.Sys.IO.Sources/Stream.Net8.pp.cs  for internal use only in this library.

        #if !NET8_0_OR_GREATER        

        [DebuggerStepThrough]
        private static void ReadExactly(_STREAM stream, Span<byte> buffer)
        {
            _ = _ReadAtLeastCore(stream, buffer, buffer.Length, throwOnEndOfStream: true);
        }

        [DebuggerStepThrough]
        private static Task ReadExactlyAsync(_STREAM stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _ReadAtLeastAsyncCore(stream, buffer, buffer.Length, throwOnEndOfStream: true, cancellationToken);
        }

        [DebuggerStepThrough]
        private static int ReadAtLeast(_STREAM stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true)
        {
            _ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

            return _ReadAtLeastCore(stream, buffer, minimumBytes, throwOnEndOfStream);
        }
        
        [DebuggerStepThrough]
        private static Task<int> ReadAtLeastAsync(_STREAM stream, Memory<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true, CancellationToken cancellationToken = default)
        {
            _ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

            return _ReadAtLeastAsyncCore(stream, buffer, minimumBytes, throwOnEndOfStream, cancellationToken);
        }


        // No argument checking is done here. It is up to the caller.
        private static int _ReadAtLeastCore(_STREAM stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream)
        {
            Debug.Assert(minimumBytes <= buffer.Length);

            int totalRead = 0;
            while (totalRead < minimumBytes)
            {
                int read = stream.Read(buffer.Slice(totalRead));
                if (read == 0)
                {
                    if (throwOnEndOfStream)
                    {
                        throw new System.IO.EndOfStreamException();
                    }

                    return totalRead;
                }

                totalRead += read;
            }

            return totalRead;
        }

        private static async Task<int> _ReadAtLeastAsyncCore(_STREAM stream, Memory<byte> buffer, int minimumBytes, bool throwOnEndOfStream, CancellationToken cancellationToken)
        {
            Debug.Assert(minimumBytes <= buffer.Length);

            int totalRead = 0;
            while (totalRead < minimumBytes)
            {
                int read = await stream.ReadAsync(buffer.Slice(totalRead), cancellationToken).ConfigureAwait(true);
                if (read == 0)
                {
                    if (throwOnEndOfStream)
                    {
                        throw new System.IO.EndOfStreamException();
                    }

                    return totalRead;
                }

                totalRead += read;
            }

            return totalRead;
        }

        private static void _ValidateReadAtLeastArguments(int bufferLength, int minimumBytes)
        {
            if (minimumBytes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumBytes), "must be non negative");
            }

            if (bufferLength < minimumBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumBytes), "not greater than buffer len");
            }
        }

        #endif
    }
}