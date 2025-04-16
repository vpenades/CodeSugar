// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

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
        /// <summary>
        /// Reads bytes from the stream until the end of the stream or until the destination buffer is full.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="bytes">The destination buffer.</param>
        /// <returns>true if the bytes have successfully read, or false if EOF</returns>
        private static bool TryReadBytes(this _STREAM stream, Span<Byte> bytes)
        {
            if (stream == null) return false;

            var bbb = bytes;

            while (bbb.Length > 0)
            {
                var l = stream.Read(bbb);
                if (l <= 0) return false;

                bbb = bbb.Slice(l);
            }

            return true;
        }                
    }
}