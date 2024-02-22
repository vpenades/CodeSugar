// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using UNSAFE = System.Runtime.CompilerServices.Unsafe;

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
        public static float DecomposeScale(this in Matrix3x2 xform)
        {
            var det = xform.GetDeterminant();
            var area = Math.Abs(det);
            return (float)Math.Sqrt(area);
        }

        public static Vector2 GetScale(this in Matrix3x2 matrix)
        {
            var sx = matrix.M12 == 0 ? Math.Abs(matrix.M11) : new Vector2(matrix.M11, matrix.M12).Length();
            var sy = matrix.M21 == 0 ? Math.Abs(matrix.M22) : new Vector2(matrix.M21, matrix.M22).Length();
            if (matrix.GetDeterminant() < 0) sy = -sy;
            return new Vector2(sx, sy);
        }

        public static float GetRotation(this in Matrix3x2 matrix)
        {
            return (float)Math.Atan2(matrix.M12, matrix.M11);
        }

        public static void Decompose(this Matrix3x2 matrix, out Vector2 scale, out float rotation, out Vector2 translation)
        {
            scale = matrix.GetScale();
            rotation = matrix.GetRotation();
            translation = matrix.Translation;
        }

        public static Matrix3x2 WithScale(this Matrix3x2 matrix, Vector2 scale)
        {
            return (scale, matrix.GetRotation(), matrix.Translation).CreateMatrix3x2();
        }

        public static Matrix3x2 WithRotation(this Matrix3x2 matrix, float radians)
        {
            return (matrix.GetScale(), radians, matrix.Translation).CreateMatrix3x2();
        }

        public static Matrix3x2 CreateMatrix3x2(this (Vector2 Scale, float Radians, Vector2 Translation) terms)
        {
            var m = Matrix3x2.CreateRotation(terms.Radians);
            m.M11 *= terms.Scale.X;
            m.M12 *= terms.Scale.X;
            m.M21 *= terms.Scale.Y;
            m.M22 *= terms.Scale.Y;
            m.M31 = terms.Translation.X;
            m.M32 = terms.Translation.Y;
            return m;
        }
    }
}