// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable

using _SCALAR = System.Single;
using _VECTOR2 = System.Numerics.Vector2;
using _VECTOR3 = System.Numerics.Vector3;
using _VECTOR4 = System.Numerics.Vector4;

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
        [MethodImpl(AGRESSIVE)]
        public static int DominantAxis(this _VECTOR2 v)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");

            v = _VECTOR2.Abs(v);
            return v.X >= v.Y ? 0 : 1;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static int DominantAxis(this _VECTOR3 v)
        {
            _AssertFinite(v);

            v = _VECTOR3.Abs(v);
            return v.X >= v.Y ? v.X >= v.Z ? 0 : 2 : v.Y >= v.Z ? 1 : 2;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static int DominantAxis(this _VECTOR4 v)
        {
            _AssertFinite(v);

            v = _VECTOR4.Abs(v);
            if (v.X > v.Y && v.X > v.Z && v.X > v.W) return 0;
            if (v.Y > v.Z && v.Y > v.W) return 1;
            if (v.Z > v.W) return 2;
            return 3;
        }
    }
}
