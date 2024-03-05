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
    }
}