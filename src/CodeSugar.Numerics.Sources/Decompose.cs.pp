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
        public static (float X, float Y) Decompose(this Vector2 v)
        {
            return (v.X, v.Y);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (float X, float Y, float Z) Decompose(this Vector3 v)
        {
            return (v.X, v.Y, v.Z);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (float X, float Y, float Z, float W) Decompose(this Vector4 v)
        {
            return (v.X, v.Y, v.Z, v.W);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Vector3 Normal, float D) Decompose(this Plane p)
        {
            return (p.Normal, p.D);
        }
    }
}
