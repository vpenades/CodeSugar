// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

        #if !NETSTANDARD
        static partial class _Unsafe
        {
            public static int GetByteOffset<T>(Span<T> span,Span<T> other)
            {
                if (!span.Overlaps(other)) throw new ArgumentException(nameof(other));

                IntPtr byteOffset = Unsafe.ByteOffset(
                    ref MemoryMarshal.GetReference(span),
                    ref MemoryMarshal.GetReference(other));

                return (int)byteOffset;
            }
        }
        #endif


        /// <summary>
        /// Represents a poor man struct sizeof
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [System.Diagnostics.DebuggerStepThrough]
        private static class __SizeOf<T> where T : unmanaged
        {
            static __SizeOf()
            {
                // this is not available in a number of frameworks, including Unity.
                // System.Runtime.CompilerServices.Unsafe.SizeOf<T>();

                // unity seems to fail when the struct has nested properties too.
                // ByteSize = System.Runtime.InteropServices.Marshal.SizeOf<T>();

                Span<T> span = stackalloc T[1];                
                var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes(span);                
                ByteSize = buff.Length;                
            }

            public static int ByteSize { get; }            
        }
    }
}