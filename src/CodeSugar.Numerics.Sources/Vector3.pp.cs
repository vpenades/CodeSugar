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
    using VECTOR3 = System.Numerics.Vector3;
    using MATRIX4X4 = System.Numerics.Matrix4x4;
    using VECTOR3ENUMERATION = System.Collections.Generic.IEnumerable<System.Numerics.Vector3>;

    internal static partial class CodeSugarForNumerics
    {
        [Conditional("DEBUG")]
        private static void _AssertFinite(in VECTOR3 v)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static int DominantAxis(this VECTOR3 v)
        {
            _AssertFinite(v);

            v = VECTOR3.Abs(v);
            return v.X >= v.Y ? v.X >= v.Z ? 0 : 2 : v.Y >= v.Z ? 1 : 2;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float ManhattanLength(this VECTOR3 v)
        {
            _AssertFinite(v);

            v = VECTOR3.Abs(v);
            return v.X + v.Y + v.Z;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static VECTOR3 WithLength(this VECTOR3 v, float newLen)
        {
            _AssertFinite(v);

            return (v == VECTOR3.Zero ? VECTOR3.UnitX : VECTOR3.Normalize(v)) * newLen;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float Angle(this VECTOR3 a, VECTOR3 b) // AngleWith ?
        {
            _AssertFinite(a);
            _AssertFinite(b);

            if (a == VECTOR3.Zero || b == VECTOR3.Zero) return 0;

            a = VECTOR3.Normalize(a);
            b = VECTOR3.Normalize(b);

            float dot = VECTOR3.Dot(a, b);
            dot = Math.Clamp(dot,-1,1);
            return MathF.Acos(dot);
        }

        [DebuggerStepThrough]        
        public static VECTOR3 Centroid(this VECTOR3ENUMERATION points)
        {
            if (points == null) return VECTOR3.Zero;

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

            if (c == 0) return VECTOR3.Zero;

            x /= c;
            y /= c;
            z /= c;

            return new VECTOR3((float)x, (float)y, (float)z);
        }

        [DebuggerStepThrough]
        public static VECTOR3 Min(this VECTOR3ENUMERATION points)
        {
            return points.Aggregate(new VECTOR3(float.MaxValue), (seed, value) => VECTOR3.Min(seed, value));
        }

        [DebuggerStepThrough]
        public static VECTOR3 Max(this VECTOR3ENUMERATION points)
        {
            return points.Aggregate(new VECTOR3(float.MinValue), (seed, value) => VECTOR3.Max(seed, value));
        }

        [DebuggerStepThrough]
        public static (VECTOR3 Min, VECTOR3 Max) MinMax(this VECTOR3ENUMERATION points)
        {
            return points.Aggregate((new VECTOR3(float.MaxValue), new VECTOR3(float.MinValue)), _MinMax);
        }

        [DebuggerStepThrough]
        private static (VECTOR3 Min, VECTOR3 Max) _MinMax((VECTOR3 Min, VECTOR3 Max) seed, VECTOR3 value)
        {
            return (VECTOR3.Min(seed.Min, value), VECTOR3.Max(seed.Max, value));
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformBy(this Span<VECTOR3> collection, MATRIX4X4 matrix)
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = VECTOR3.Transform(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformNormalBy(this Span<VECTOR3> collection, MATRIX4X4 matrix)
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = VECTOR3.TransformNormal(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformBy<TCollection>(this TCollection collection, MATRIX4X4 matrix)
            where TCollection : IList<VECTOR3>
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Count; i++)
            {
                collection[i] = VECTOR3.Transform(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformNormalBy<TCollection>(this TCollection collection, MATRIX4X4 matrix)
            where TCollection : IList<VECTOR3>
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Count; i++)
            {
                collection[i] = VECTOR3.TransformNormal(collection[i], matrix);
            }
        }
    }
}