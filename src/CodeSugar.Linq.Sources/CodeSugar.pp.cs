// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

#nullable disable

#if !NETSTANDARD
using ___UNSAFE = System.Runtime.CompilerServices.Unsafe;
#endif

using ___METHODOPTIONS = System.Runtime.CompilerServices.MethodImplOptions;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.Collections.Generic
#else
namespace $rootnamespace$
#endif
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("CodeSugar.CodeGen", "1.0.0.0")]
    internal static partial class CodeSugarForLinq
    {
        #if NETSTANDARD1_6_OR_GREATER
        private const ___METHODOPTIONS AGRESSIVE = ___METHODOPTIONS.AggressiveInlining;
        #else
        private const ___METHODOPTIONS AGRESSIVE = ___METHODOPTIONS.AggressiveInlining | ___METHODOPTIONS.AggressiveOptimization;
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
            return ___UNSAFE.As<TSrc, TDst>(ref valIn);
            #else

            // netstandard 2.1 fallback
            var span = System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref valIn, 1);
            var cast = System.Runtime.InteropServices.MemoryMarshal.Cast<TSrc, TDst>(span);
            if (cast.Length != 1) throw new InvalidOperationException("Size mismatch");
            return cast[0];

            #endif
        }

        /// <summary>
        /// sometimes you just need to force an enumeration to happen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void Yield<T>(this IEnumerable<T> collection)
        {
            using (var ptr = collection.GetEnumerator())
            {
                while (ptr.MoveNext()) { }
            }
        }

        /// <summary>
        /// sometimes you just need to force an enumeration to happen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            using (var ptr = collection.GetEnumerator())
            {
                while (ptr.MoveNext())
                {
                    action(ptr.Current);
                }
            }
        }

    }
}
