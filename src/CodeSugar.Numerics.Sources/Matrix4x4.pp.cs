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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 CreateViewportMatrix4x4(this System.Drawing.RectangleF rect, float minDepth = 0, float maxDepth = 1, bool isLeftHanded = false)
        {
            // https://github.com/dotnet/runtime/blob/5fd3f22b98cb0d126e4f6328f5a0bf922a532f69/src/libraries/System.Private.CoreLib/src/System/Numerics/Matrix4x4.Impl.cs#L889

            #if NET8_0_OR_GREATER

            return isLeftHanded
                ? Matrix4x4.CreateViewportLeftHanded(rect.X, rect.Y, rect.Width, rect.Height, minDepth, maxDepth)
                : Matrix4x4.CreateViewport(rect.X, rect.Y, rect.Width, rect.Height, minDepth, maxDepth);

            #else

            var depthDist = isLeftHanded ? maxDepth - minDepth : minDepth - maxDepth;
            
            var w = new Vector4(rect.Width, rect.Height, 0f, 0f);
            w *= new Vector4(0.5f, 0.5f, 0f, 0f);

            var x = new Vector4(w.X, 0f, 0f, 0f);
            var y = new Vector4(0f, -w.Y, 0f, 0f);
            var z = new Vector4(0f, 0f, depthDist, 0f);
            w += new Vector4(rect.X, rect.Y, minDepth, 1f);

            return (x,y,z,w).CreateMatrix4x4FromRows();

            #endif
        }

        public static Matrix4x4 CreateMatrix4x4FromRows(this (Vector4 x, Vector4 y, Vector4 z, Vector4 w) rows)
        {
            return new Matrix4x4(
                rows.x.X, rows.x.Y, rows.x.Z, rows.x.W,
                rows.y.X, rows.y.Y, rows.y.Z, rows.y.W,
                rows.z.X, rows.z.Y, rows.z.Z, rows.z.W,
                rows.w.X, rows.w.Y, rows.w.Z, rows.w.W
                );
        }
    }
}