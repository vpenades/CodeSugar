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
        public static float ManhattanLength(this __VECTOR2 v)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");

            v = __VECTOR2.Abs(v);
            return v.X + v.Y;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static __VECTOR2 WithLength(this __VECTOR2 v, float newLen)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");

            return (v == __VECTOR2.Zero ? __VECTOR2.UnitX : __VECTOR2.Normalize(v)) * newLen;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float ManhattanLength(this __VECTOR3 v)
        {
            _AssertFinite(v);

            v = __VECTOR3.Abs(v);
            return v.X + v.Y + v.Z;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static __VECTOR3 WithLength(this __VECTOR3 v, float newLen)
        {
            _AssertFinite(v);

            return (v == __VECTOR3.Zero ? __VECTOR3.UnitX : __VECTOR3.Normalize(v)) * newLen;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float ManhattanLength(this __VECTOR4 v)
        {
            _AssertFinite(v);

            v = __VECTOR4.Abs(v);
            return v.X + v.Y + v.Z;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static __VECTOR4 WithLength(this __VECTOR4 v, float newLen)
        {
            _AssertFinite(v);

            return (v == __VECTOR4.Zero ? __VECTOR4.UnitX : __VECTOR4.Normalize(v)) * newLen;
        }
    }
}
