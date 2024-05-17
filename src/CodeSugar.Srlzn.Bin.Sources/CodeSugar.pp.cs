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

using METHODOPTIONS = System.Runtime.CompilerServices.MethodImplOptions;


#nullable disable

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
        private const METHODOPTIONS AGRESSIVE = METHODOPTIONS.AggressiveInlining;
        #else
        private const METHODOPTIONS AGRESSIVE = METHODOPTIONS.AggressiveInlining | METHODOPTIONS.AggressiveOptimization;
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
    }
}
