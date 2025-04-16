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
    using _VECTOR2ENUMERATION = System.Collections.Generic.IEnumerable<System.Numerics.Vector2>;

    internal static partial class CodeSugarForNumerics    
    {
        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float Angle(this in Vector2 v)
        {
            return MathF.Atan2(v.Y, v.X);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float Angle(this Vector2 a, Vector2 b)
        {
            _AssertFinite(a);
            _AssertFinite(b);

            if (a == Vector2.Zero || b == Vector2.Zero) return 0;

            a = Vector2.Normalize(a);
            b = Vector2.Normalize(b);

            float dot = Vector2.Dot(a, b);
            dot = Math.Clamp(dot,-1,1);
            return MathF.Acos(dot);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float Cross(this Vector2 a, Vector2 b)
        {
            _AssertFinite(a);
            _AssertFinite(b);

            return a.X * b.Y - a.Y * b.X;
        }

        [DebuggerStepThrough]        
        public static Vector2 Centroid(this _VECTOR2ENUMERATION points)
        {
            if (points == null) return Vector2.Zero;

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

            if (c == 0) return Vector2.Zero;

            x /= c;
            y /= c;            

            return new Vector2((float)x, (float)y);
        }

        [DebuggerStepThrough]
        public static Vector2 Min(this _VECTOR2ENUMERATION points)
        {            
            return points.Aggregate(new Vector2(float.MaxValue), (seed, value) => Vector2.Min(seed, value));
        }

        [DebuggerStepThrough]
        public static Vector2 Max(this _VECTOR2ENUMERATION points)
        {
            return points.Aggregate(new Vector2(float.MinValue), (seed, value) => Vector2.Max(seed, value));
        }

        [DebuggerStepThrough]
        public static (Vector2 Min, Vector2 Max) MinMax(this _VECTOR2ENUMERATION points)
        {
            return points.Aggregate((new Vector2(float.MaxValue), new Vector2(float.MinValue)), (seed, value) => (Vector2.Min(seed.Item1,value), Vector2.Max(seed.Item2, value)) );
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformBy(this Span<Vector2> collection, System.Numerics.Matrix3x2 matrix)            
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = Vector2.Transform(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformNormalBy(this Span<Vector2> collection, System.Numerics.Matrix3x2 matrix)            
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = Vector2.TransformNormal(collection[i], matrix);
            }
        }


        [DebuggerStepThrough]
        public static void InPlaceTransformBy<TCollection>(this TCollection collection, System.Numerics.Matrix3x2 matrix)
            where TCollection: IList<Vector2>
        {
            if (collection == null) return;

            for(int i=0; i<collection.Count; i++)
            {
                collection[i] = Vector2.Transform(collection[i], matrix);
            }
        }

        [DebuggerStepThrough]
        public static void InPlaceTransformNormalBy<TCollection>(this TCollection collection, System.Numerics.Matrix3x2 matrix)
            where TCollection : IList<Vector2>
        {
            if (collection == null) return;

            for (int i = 0; i < collection.Count; i++)
            {
                collection[i] = Vector2.TransformNormal(collection[i], matrix);
            }
        }
    }
}