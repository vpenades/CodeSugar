// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable disable

using _METHODOPTIONS = System.Runtime.CompilerServices.MethodImplOptions;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System
#else
namespace $rootnamespace$
#endif
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [global::System.CodeDom.Compiler.GeneratedCode("CodeSugar.CodeGen", "1.0.0.0")]
    [System.Diagnostics.DebuggerStepThrough]    
    static partial class CodeSugarForSerialization
    {
        #if NETSTANDARD1_6_OR_GREATER
        private const _METHODOPTIONS AGRESSIVE = _METHODOPTIONS.AggressiveInlining;
        #else
        private const _METHODOPTIONS AGRESSIVE = _METHODOPTIONS.AggressiveInlining | _METHODOPTIONS.AggressiveOptimization;
        #endif

        #if NET6_0_OR_GREATER
        private const DynamicallyAccessedMemberTypes STRUCTMEMBERTYPES = DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields;
        #endif        

        private static void _CheckUnmanagedTypeIsSerializable<T>(bool isBigEndian) where T : unmanaged
        {
            if (BitConverter.IsLittleEndian && !isBigEndian) return;

            // under these circumstances, these types would be wrongly serialized because the element order would also be reversed.

            if (Type.GetTypeCode(typeof(T)) == TypeCode.Empty) throw new NotImplementedException($"Composite values not supported on Big Endian");            
        }

        #if NETSTANDARD

        /// <summary>
        /// NetStandard CUSTOM implementation for missing System.Runtime.CompilerServices.Unsafe
        /// </summary>
        internal static class Unsafe
        {
            public static TTo As<TFrom, TTo>(ref TFrom source)
            {
                return (TTo)(Object)source;
            }

        }

        #endif
    }
}


