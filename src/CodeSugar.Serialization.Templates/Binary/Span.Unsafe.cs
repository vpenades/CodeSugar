using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarSerializationExtensions
    {

        #if !NETSTANDARD
        static partial class __UNSAFE
        {
            public static int GetByteOffset<T>(Span<T> span,Span<T> other)
            {
                if (!span.Overlaps(other)) throw new ArgumentException(nameof(other));

                IntPtr byteOffset = System.Runtime.CompilerServices.Unsafe.ByteOffset(
                    ref System.Runtime.InteropServices.MemoryMarshal.GetReference(span),
                    ref System.Runtime.InteropServices.MemoryMarshal.GetReference(other));

                return (int)byteOffset;
            }
        }
        #endif
        
    }
}