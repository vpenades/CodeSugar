// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
        public static IEnumerable<float> Enumerate(this Vector2 v)
        {
            yield return v.X;
            yield return v.Y;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<float> Enumerate(this Vector3 v)
        {
            yield return v.X;
            yield return v.Y;
            yield return v.Z;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<float> Enumerate(this Vector4 v)
        {
            yield return v.X;
            yield return v.Y;
            yield return v.Z;
            yield return v.W;
        }
    }
}
