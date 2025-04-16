// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Linq;

#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.Numerics
#else
namespace $rootnamespace$
#endif
{
    using _VECTOR3 = System.Numerics.Vector3;
    using _MATRIX4X4 = System.Numerics.Matrix4x4;
    using _VECTOR3ENUMERATION = System.Collections.Generic.IEnumerable<System.Numerics.Vector3>;

    internal static partial class CodeSugarForNumerics
    {
        [Conditional("DEBUG")]
        private static void _AssertFinite(in _VECTOR3 v)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");
        }        

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float Angle(this _VECTOR3 a, _VECTOR3 b) // AngleWith ?
        {
            _AssertFinite(a);
            _AssertFinite(b);

            if (a == _VECTOR3.Zero || b == _VECTOR3.Zero) return 0;

            a = _VECTOR3.Normalize(a);
            b = _VECTOR3.Normalize(b);

            float dot = _VECTOR3.Dot(a, b);
            dot = Math.Clamp(dot,-1,1);
            return MathF.Acos(dot);
        }

        [DebuggerStepThrough]        
        public static _VECTOR3 Centroid(this _VECTOR3ENUMERATION points)
        {
            if (points == null) return _VECTOR3.Zero;

            double c = 0;
            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var p in points)
            {
                System.Diagnostics.Debug.Assert(IsFinite(p), $"points[{(int)c}] is not finite");

                x += p.X;
                y += p.Y;
                z += p.Z;

                c += 1;
            }

            if (c == 0) return _VECTOR3.Zero;

            x /= c;
            y /= c;
            z /= c;

            return new _VECTOR3((float)x, (float)y, (float)z);
        }

        [DebuggerStepThrough]
        public static _VECTOR3 Min(this _VECTOR3ENUMERATION points)
        {
            return points.Aggregate(new _VECTOR3(float.MaxValue), (seed, value) => _VECTOR3.Min(seed, value));
        }

        [DebuggerStepThrough]
        public static _VECTOR3 Max(this _VECTOR3ENUMERATION points)
        {
            return points.Aggregate(new _VECTOR3(float.MinValue), (seed, value) => _VECTOR3.Max(seed, value));
        }

        [DebuggerStepThrough]
        public static (_VECTOR3 Min, _VECTOR3 Max) MinMax(this _VECTOR3ENUMERATION points)
        {
            return points.Aggregate((new _VECTOR3(float.MaxValue), new _VECTOR3(float.MinValue)), _MinMax);
        }

        [DebuggerStepThrough]
        private static (_VECTOR3 Min, _VECTOR3 Max) _MinMax((_VECTOR3 Min, _VECTOR3 Max) seed, _VECTOR3 value)
        {
            return (_VECTOR3.Min(seed.Min, value), _VECTOR3.Max(seed.Max, value));
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformBy(this Span<_VECTOR3> collection, _MATRIX4X4 matrix)
        {
            if (collection == null) return;            

            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = _VECTOR3.Transform(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformNormalBy(this Span<_VECTOR3> collection, _MATRIX4X4 matrix)
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = _VECTOR3.TransformNormal(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformBy<TCollection>(this TCollection collection, _MATRIX4X4 matrix)
            where TCollection : IList<_VECTOR3>
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Count; i++)
            {
                collection[i] = _VECTOR3.Transform(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformNormalBy<TCollection>(this TCollection collection, _MATRIX4X4 matrix)
            where TCollection : IList<_VECTOR3>
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Count; i++)
            {
                collection[i] = _VECTOR3.TransformNormal(collection[i], matrix);
            }
        }
    }
}