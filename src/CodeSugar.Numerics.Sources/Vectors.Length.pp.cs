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
        public static float ManhattanLength(this _VECTOR2 v)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");

            v = _VECTOR2.Abs(v);
            return v.X + v.Y;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static _VECTOR2 WithLength(this _VECTOR2 v, float newLen)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");

            return (v == _VECTOR2.Zero ? _VECTOR2.UnitX : _VECTOR2.Normalize(v)) * newLen;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float ManhattanLength(this _VECTOR3 v)
        {
            _AssertFinite(v);

            v = _VECTOR3.Abs(v);
            return v.X + v.Y + v.Z;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static _VECTOR3 WithLength(this _VECTOR3 v, float newLen)
        {
            _AssertFinite(v);

            return (v == _VECTOR3.Zero ? _VECTOR3.UnitX : _VECTOR3.Normalize(v)) * newLen;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float ManhattanLength(this _VECTOR4 v)
        {
            _AssertFinite(v);

            v = _VECTOR4.Abs(v);
            return v.X + v.Y + v.Z;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static _VECTOR4 WithLength(this _VECTOR4 v, float newLen)
        {
            _AssertFinite(v);

            return (v == _VECTOR4.Zero ? _VECTOR4.UnitX : _VECTOR4.Normalize(v)) * newLen;
        }
    }
}
