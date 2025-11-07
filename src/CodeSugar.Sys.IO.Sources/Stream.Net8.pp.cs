// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;

#nullable disable

using __STREAM = System.IO.Stream;


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

        [DebuggerStepThrough]
        public static void ReadExactly(this __STREAM stream, Span<byte> buffer)
        {
            _ = ReadAtLeastCore(stream, buffer, buffer.Length, throwOnEndOfStream: true);
        }

        [DebuggerStepThrough]
        public static Task ReadExactlyAsync(this __STREAM stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return ReadAtLeastAsyncCore(stream, buffer, buffer.Length, throwOnEndOfStream: true, cancellationToken);
        }

        [DebuggerStepThrough]
        public static int ReadAtLeast(this __STREAM stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true)
        {
            ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

            return ReadAtLeastCore(stream, buffer, minimumBytes, throwOnEndOfStream);
        }        

        [DebuggerStepThrough]
        public static Task<int> ReadAtLeastAsync(this __STREAM stream, Memory<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true, CancellationToken cancellationToken = default)
        {
            ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

            return ReadAtLeastAsyncCore(stream, buffer, minimumBytes, throwOnEndOfStream, cancellationToken);
        }


        // No argument checking is done here. It is up to the caller.
        private static int ReadAtLeastCore(__STREAM stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream)
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

        private static async Task<int> ReadAtLeastAsyncCore(__STREAM stream, Memory<byte> buffer, int minimumBytes, bool throwOnEndOfStream, CancellationToken cancellationToken)
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
