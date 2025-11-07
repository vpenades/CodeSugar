// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable

using __SCALAR = System.Single;
using __VECTOR2 = System.Numerics.Vector2;
using __VECTOR3 = System.Numerics.Vector3;
using __VECTOR4 = System.Numerics.Vector4;

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
        public static int DominantAxis(this __VECTOR2 v)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");

            v = __VECTOR2.Abs(v);
            return v.X >= v.Y ? 0 : 1;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static int DominantAxis(this __VECTOR3 v)
        {
            _AssertFinite(v);

            v = __VECTOR3.Abs(v);
            return v.X >= v.Y ? v.X >= v.Z ? 0 : 2 : v.Y >= v.Z ? 1 : 2;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static int DominantAxis(this __VECTOR4 v)
        {
            _AssertFinite(v);

            v = __VECTOR4.Abs(v);
            if (v.X > v.Y && v.X > v.Z && v.X > v.W) return 0;
            if (v.Y > v.Z && v.Y > v.W) return 1;
            if (v.Z > v.W) return 2;
            return 3;
        }
    }
}
