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
        [Conditional("DEBUG")]
        private static void _AssertFinite(in Vector3 v)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static int DominantAxis(this Vector3 v)
        {
            _AssertFinite(v);

            v = Vector3.Abs(v);
            return v.X >= v.Y ? v.X >= v.Z ? 0 : 2 : v.Y >= v.Z ? 1 : 2;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float ManhattanLength(this Vector3 v)
        {
            _AssertFinite(v);

            v = Vector3.Abs(v);
            return v.X + v.Y + v.Z;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static Vector3 WithLength(this Vector3 v, float newLen)
        {
            _AssertFinite(v);

            return (v == Vector3.Zero ? Vector3.UnitX : Vector3.Normalize(v)) * newLen;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float Angle(this Vector3 a, Vector3 b) // AngleWith ?
        {
            _AssertFinite(a);
            _AssertFinite(b);

            if (a == Vector3.Zero || b == Vector3.Zero) return 0;

            a = Vector3.Normalize(a);
            b = Vector3.Normalize(b);

            float dot = Vector3.Dot(a, b);
            dot = Math.Clamp(dot,-1,1);
            return MathF.Acos(dot);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static Vector3 Centroid(this System.Collections.Generic.IEnumerable<Vector3> points)
        {
            if (points == null) return Vector3.Zero;

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

            if (c == 0) return Vector3.Zero;

            x /= c;
            y /= c;
            z /= c;

            return new Vector3((float)x, (float)y, (float)z);
        }    
    }
}