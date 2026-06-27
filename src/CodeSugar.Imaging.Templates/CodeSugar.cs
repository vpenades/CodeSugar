using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable

#if !NETSTANDARD
using __UNSAFE = System.Runtime.CompilerServices.Unsafe;
#endif

using __METHODOPTIONS = System.Runtime.CompilerServices.MethodImplOptions;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        #if NETSTANDARD1_6_OR_GREATER
        private const __METHODOPTIONS AGRESSIVE = __METHODOPTIONS.AggressiveInlining;
        #else
        private const __METHODOPTIONS AGRESSIVE = __METHODOPTIONS.AggressiveInlining | __METHODOPTIONS.AggressiveOptimization;
        #endif

        /// <summary>
        /// NetStandard2 equivalent to <see cref="__UNSAFE.As{TFrom, TTo}(ref TFrom)"/>
        /// </summary>
        /// <typeparam name="TSrc"></typeparam>
        /// <typeparam name="TDst"></typeparam>
        /// <param name="valIn"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        private static TDst _UnsafeAs<TSrc,TDst>(ref TSrc valIn)
            where TSrc: unmanaged
            where TDst: unmanaged
        {
            #if !NETSTANDARD
            // notice that we can still use UNSAFE in NetStandard2.1
            // by referencing System.Runtime.CompilerServices.Unsafe package 
            // but it is not guaranteed we have that dependency.
            return __UNSAFE.As<TSrc, TDst>(ref valIn);
            #else

            // netstandard 2.1 fallback
            var span = System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref valIn, 1);
            var cast = System.Runtime.InteropServices.MemoryMarshal.Cast<TSrc, TDst>(span);
            if (cast.Length != 1) throw new InvalidOperationException("Size mismatch");
            return cast[0];

            #endif
        }
        
    }
}
