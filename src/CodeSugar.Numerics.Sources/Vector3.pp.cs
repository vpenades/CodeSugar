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
    using __VECTOR3 = System.Numerics.Vector3;
    using __MATRIX4X4 = System.Numerics.Matrix4x4;
    using __VECTOR3ENUMERATION = System.Collections.Generic.IEnumerable<System.Numerics.Vector3>;

    internal static partial class CodeSugarForNumerics
    {
        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float Angle(this __VECTOR3 a, __VECTOR3 b) // AngleWith ?
        {
            _AssertFinite(a);
            _AssertFinite(b);

            if (a == __VECTOR3.Zero || b == __VECTOR3.Zero) return 0;

            a = __VECTOR3.Normalize(a);
            b = __VECTOR3.Normalize(b);

            float dot = __VECTOR3.Dot(a, b);
            dot = Math.Clamp(dot,-1,1);
            return MathF.Acos(dot);
        }

        [DebuggerStepThrough]        
        public static __VECTOR3 Centroid(this __VECTOR3ENUMERATION points)
        {
            if (points == null) return __VECTOR3.Zero;

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

            if (c == 0) return __VECTOR3.Zero;

            x /= c;
            y /= c;
            z /= c;

            return new __VECTOR3((float)x, (float)y, (float)z);
        }

        [DebuggerStepThrough]
        public static __VECTOR3 Min(this __VECTOR3ENUMERATION points)
        {
            return points.Aggregate(new __VECTOR3(float.MaxValue), (seed, value) => __VECTOR3.Min(seed, value));
        }

        [DebuggerStepThrough]
        public static __VECTOR3 Max(this __VECTOR3ENUMERATION points)
        {
            return points.Aggregate(new __VECTOR3(float.MinValue), (seed, value) => __VECTOR3.Max(seed, value));
        }

        [DebuggerStepThrough]
        public static (__VECTOR3 Min, __VECTOR3 Max) MinMax(this __VECTOR3ENUMERATION points)
        {
            return points.Aggregate((new __VECTOR3(float.MaxValue), new __VECTOR3(float.MinValue)), _MinMax);
        }

        [DebuggerStepThrough]
        private static (__VECTOR3 Min, __VECTOR3 Max) _MinMax((__VECTOR3 Min, __VECTOR3 Max) seed, __VECTOR3 value)
        {
            return (__VECTOR3.Min(seed.Min, value), __VECTOR3.Max(seed.Max, value));
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformBy(this Span<__VECTOR3> collection, __MATRIX4X4 matrix)
        {
            if (collection.IsEmpty) return;            

            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = __VECTOR3.Transform(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformNormalBy(this Span<__VECTOR3> collection, __MATRIX4X4 matrix)
        {
            if (collection.IsEmpty) return;

            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = __VECTOR3.TransformNormal(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformBy<TCollection>(this TCollection collection, __MATRIX4X4 matrix)
            where TCollection : IList<__VECTOR3>
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Count; i++)
            {
                collection[i] = __VECTOR3.Transform(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformNormalBy<TCollection>(this TCollection collection, __MATRIX4X4 matrix)
            where TCollection : IList<__VECTOR3>
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Count; i++)
            {
                collection[i] = __VECTOR3.TransformNormal(collection[i], matrix);
            }
        }
    }
}