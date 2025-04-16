// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;

#nullable disable

using _STREAM = System.IO.Stream;


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
        // these mathods have been backported from net8 so they can be called from netstandard/net6

        #if !NET8_0_OR_GREATER

        public static int ReadAtLeast(this System.IO.Stream stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true)
        {
            ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

            return ReadAtLeastCore(stream, buffer, minimumBytes, throwOnEndOfStream);
        }

        // No argument checking is done here. It is up to the caller.
        private static int ReadAtLeastCore(System.IO.Stream stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream)
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

        public static Task<int> ReadAtLeastAsync(this System.IO.Stream stream, Memory<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true, CancellationToken cancellationToken = default)
        {
            ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

            return ReadAtLeastAsyncCore(stream, buffer, minimumBytes, throwOnEndOfStream, cancellationToken);
        }

        
        private static async Task<int> ReadAtLeastAsyncCore(System.IO.Stream stream, Memory<byte> buffer, int minimumBytes, bool throwOnEndOfStream, CancellationToken cancellationToken)
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
