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
        public static Single GetVolumeScale(this in Matrix4x4 matrix)
        {
            System.Diagnostics.Debug.Assert(IsFinite(matrix), "matrix is not finite.");

            // the determinant also represents the signed volume defined by the three axes
            var det = matrix.GetDeterminant();
            var volume = Math.Abs(det);            
            return MathF.Pow(volume, 1f / 3f);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static Matrix4x4 InvertedFast(this Matrix4x4 matrix)
        {
            System.Diagnostics.Debug.Assert(IsFinite(matrix), "matrix is not finite.");            
            System.Diagnostics.Debug.Assert(Math.Abs(Math.Abs(matrix.GetDeterminant())-1) < 0.01f, "matrix is not normalized.");

            // http://content.gpwiki.org/index.php/MathGem:Fast_Matrix_Inversion
            // R' = transpose(R)
            // M = R' * (- (R' * T))

            var t = matrix.Translation;
            matrix.Translation = Vector3.Zero;
            matrix = Matrix4x4.Transpose(matrix);

            matrix.Translation = -Vector3.Transform(t, matrix);

            return matrix;
        }
    }
}