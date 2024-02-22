// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using UNSAFE = System.Runtime.CompilerServices.Unsafe;

#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.Numerics
#else
namespace $rootnamespace$
#endif
{    
    internal static partial class CodeSugarForNumerics
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DominantAxis(this Vector3 v)
        {
            v = Vector3.Abs(v);
            return v.X >= v.Y ? v.X >= v.Z ? 0 : 2 : v.Y >= v.Z ? 1 : 2;
        }
    }
}