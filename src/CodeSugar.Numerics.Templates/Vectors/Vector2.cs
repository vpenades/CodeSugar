using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Linq;

#nullable disable

using __VECTOR2 = System.Numerics.Vector2;
using __VECTOR3 = System.Numerics.Vector3;
using __VECTOR4 = System.Numerics.Vector4;

using __VECTOR2ENUMERATION = System.Collections.Generic.IEnumerable<System.Numerics.Vector2>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarNumericsExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float Angle(this in __VECTOR2 v)
        {
            return MathF.Atan2(v.Y, v.X);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float Angle(this __VECTOR2 a, __VECTOR2 b)
        {
            _AssertFinite(a);
            _AssertFinite(b);

            if (a == __VECTOR2.Zero || b == __VECTOR2.Zero) return 0;

            a = __VECTOR2.Normalize(a);
            b = __VECTOR2.Normalize(b);

            float dot = __VECTOR2.Dot(a, b);
            dot = Math.Clamp(dot,-1,1);
            return MathF.Acos(dot);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float Cross(this __VECTOR2 a, __VECTOR2 b)
        {
            _AssertFinite(a);
            _AssertFinite(b);

            return a.X * b.Y - a.Y * b.X;
        }

        [DebuggerStepThrough]        
        public static __VECTOR2 Centroid(this __VECTOR2ENUMERATION points)
        {
            if (points == null) return __VECTOR2.Zero;

            double c = 0;
            double x = 0;
            double y = 0;            

            foreach (var p in points)
            {
                System.Diagnostics.Debug.Assert(IsFinite(p), $"{(int)c} is not finite");

                x += p.X;
                y += p.Y;                

                c += 1;
            }

            if (c == 0) return __VECTOR2.Zero;

            x /= c;
            y /= c;            

            return new __VECTOR2((float)x, (float)y);
        }

        [DebuggerStepThrough]
        public static __VECTOR2 Min(this __VECTOR2ENUMERATION points)
        {            
            return points.Aggregate(new __VECTOR2(float.MaxValue), (seed, value) => __VECTOR2.Min(seed, value));
        }

        [DebuggerStepThrough]
        public static __VECTOR2 Max(this __VECTOR2ENUMERATION points)
        {
            return points.Aggregate(new __VECTOR2(float.MinValue), (seed, value) => __VECTOR2.Max(seed, value));
        }

        [DebuggerStepThrough]
        public static (__VECTOR2 Min, __VECTOR2 Max) MinMax(this __VECTOR2ENUMERATION points)
        {
            return points.Aggregate((new __VECTOR2(float.MaxValue), new __VECTOR2(float.MinValue)), (seed, value) => (__VECTOR2.Min(seed.Item1,value), __VECTOR2.Max(seed.Item2, value)) );
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformBy(this Span<__VECTOR2> collection, System.Numerics.Matrix3x2 matrix)            
        {
            if (collection.IsEmpty) return;

            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = __VECTOR2.Transform(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformNormalBy(this Span<__VECTOR2> collection, System.Numerics.Matrix3x2 matrix)            
        {
            if (collection.IsEmpty) return;

            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = __VECTOR2.TransformNormal(collection[i], matrix);
            }
        }


        [DebuggerStepThrough]
        public static void InPlaceTransformBy<TCollection>(this TCollection collection, System.Numerics.Matrix3x2 matrix)
            where TCollection: IList<__VECTOR2>
        {
            if (collection == null) return;

            for(int i=0; i<collection.Count; i++)
            {
                collection[i] = __VECTOR2.Transform(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformNormalBy<TCollection>(this TCollection collection, System.Numerics.Matrix3x2 matrix)
            where TCollection : IList<__VECTOR2>
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Count; i++)
            {
                collection[i] = __VECTOR2.TransformNormal(collection[i], matrix);
            }
        }

        #region fallbacks to existing APIs


        #if !NET10_0_OR_GREATER

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static __VECTOR3 AsVector3(this __VECTOR2 v)
        {
            return new __VECTOR3(v.X, v.Y, 0);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static __VECTOR3 AsVector3Unsafe(this __VECTOR2 v)
        {
            return new __VECTOR3(v.X, v.Y, 0);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static __VECTOR4 AsVector4(this __VECTOR2 v)
        {
            return new __VECTOR4(v, 0, 0);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static __VECTOR4 AsVector4Unsafe(this __VECTOR2 v)
        {
            return new __VECTOR4(v, 0, 0);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float GetElement(this __VECTOR2 v, int idx)
        {
            switch (idx)
            {
                case 0: return v.X;
                case 1: return v.Y;                
                default: throw new ArgumentOutOfRangeException(nameof(idx));
            }
        }
        #endif

        #endregion
    }
}