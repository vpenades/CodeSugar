// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable

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
        public static int DominantAxis(this Vector2 v)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");

            v = Vector2.Abs(v);
            return v.X >= v.Y ? 0 : 1;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float ManhattanLength(this Vector2 v)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");

            v = Vector2.Abs(v);
            return v.X + v.Y;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static Vector2 WithLength(this Vector2 v, float newLen)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");

            return (v == Vector2.Zero ? Vector2.UnitX : Vector2.Normalize(v)) * newLen;
        }

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
            System.Diagnostics.Debug.Assert(a.IsFinite(), "a is not finite");
            System.Diagnostics.Debug.Assert(b.IsFinite(), "b is not finite");

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
            System.Diagnostics.Debug.Assert(a.IsFinite(), "a is not finite");
            System.Diagnostics.Debug.Assert(b.IsFinite(), "b is not finite");

            return a.X * b.Y - a.Y * b.X;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static Vector2 Centroid(this System.Collections.Generic.IEnumerable<Vector2> points)
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
    }
}