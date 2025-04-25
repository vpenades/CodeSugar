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
        private static float _GetLerpAmount(float distance, float deadZone, float smoothDistance)
        {
            System.Diagnostics.Debug.Assert(distance >= 0, "distance must not be negative");
            System.Diagnostics.Debug.Assert(deadZone >= 0, "deadZone must not be negative");
            System.Diagnostics.Debug.Assert(smoothDistance > deadZone, "smoothDistance must larger than deadZone");

            distance = Math.Max(0, distance - deadZone);

            smoothDistance -= deadZone;
            distance = Math.Min(smoothDistance, distance) / smoothDistance;
            return distance;
        }


        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static _SCALAR Lerp(this (_SCALAR A, _SCALAR B) pair, float bamount)
        {
            var result = pair.A * (1.0f - bamount);
            result += pair.B * bamount;

            return result;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static _SCALAR Lerp(this (_SCALAR? A, _SCALAR B) pair, float deadZone, float smoothDistance)
        {
            if (!pair.A.HasValue || smoothDistance <= deadZone) return pair.B;

            var amount = _GetLerpAmount(Math.Abs(pair.A.Value - pair.B), deadZone, smoothDistance);
            return Lerp(pair, amount);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static _SCALAR Lerp(this (_SCALAR? A, _SCALAR B) pair, float bamount)
        {
            if (!pair.A.HasValue) return pair.B;

            var result = pair.A.Value * (1.0f - bamount);
            result += pair.B * bamount;

            return result;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static _VECTOR2 Lerp(this (_VECTOR2? A, _VECTOR2 B) pair, float deadZone, float smoothDistance)
        {
            if (!pair.A.HasValue || smoothDistance <= deadZone) return pair.B;

            var amount = _GetLerpAmount(_VECTOR2.Distance(pair.A.Value, pair.B), deadZone, smoothDistance);
            return Lerp(pair, amount);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static _VECTOR2 Lerp(this (_VECTOR2? A, _VECTOR2 B) pair, float bamount)
        {
            return !pair.A.HasValue
                ? pair.B
                : _VECTOR2.Lerp(pair.A.Value, pair.B, bamount);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static _VECTOR3 Lerp(this (_VECTOR3? A, _VECTOR3 B) pair, float deadZone, float smoothDistance)
        {
            if (!pair.A.HasValue || smoothDistance <= deadZone) return pair.B;

            var amount = _GetLerpAmount(_VECTOR3.Distance(pair.A.Value, pair.B), deadZone, smoothDistance);
            return Lerp(pair, amount);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static _VECTOR3 Lerp(this (_VECTOR3? A, _VECTOR3 B) pair, float bamount)
        {
            return !pair.A.HasValue
                ? pair.B
                : _VECTOR3.Lerp(pair.A.Value, pair.B, bamount);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static _VECTOR4 Lerp(this (_VECTOR4? A, _VECTOR4 B) pair, float deadZone, float smoothDistance)
        {
            if (!pair.A.HasValue || smoothDistance <= deadZone) return pair.B;
            
            var amount = _GetLerpAmount(_VECTOR4.Distance(pair.A.Value, pair.B), deadZone, smoothDistance);
            return Lerp(pair, amount);
        }        

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static _VECTOR4 Lerp(this (_VECTOR4? A, _VECTOR4 B) pair, float bamount)
        {
            return !pair.A.HasValue
                ? pair.B
                : _VECTOR4.Lerp(pair.A.Value, pair.B, bamount);
        }
    }
}
