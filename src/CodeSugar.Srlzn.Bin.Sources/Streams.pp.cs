// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
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
        #if !NET8_0_OR_GREATER

        private static int ReadAtLeast(this _STREAM stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true)
        {
            ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

            return ReadAtLeastCore(stream, buffer, minimumBytes, throwOnEndOfStream);
        }

        // No argument checking is done here. It is up to the caller.
        private static int ReadAtLeastCore(_STREAM stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream)
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

        private static Task<int> ReadAtLeastAsync(this _STREAM stream, Memory<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true, CancellationToken cancellationToken = default)
        {
            ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

            return ReadAtLeastAsyncCore(stream, buffer, minimumBytes, throwOnEndOfStream, cancellationToken);
        }


        private static async Task<int> ReadAtLeastAsyncCore(_STREAM stream, Memory<byte> buffer, int minimumBytes, bool throwOnEndOfStream, CancellationToken cancellationToken)
        {
            Debug.Assert(minimumBytes <= buffer.Length);

            int totalRead = 0;
            while (totalRead < minimumBytes)
            {
                int read = await stream.ReadAsync(buffer.Slice(totalRead), cancellationToken).ConfigureAwait(false);
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

        private static void ValidateReadAtLeastArguments(int bufferLength, int minimumBytes)
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