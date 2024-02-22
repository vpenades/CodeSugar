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
        private const float SCALEDECOMPOSITIONEPSILON = 0.00001f;

        public static Single DecomposeScale(this Matrix4x4 matrix)
        {
            var det = matrix.GetDeterminant();
            var volume = Math.Abs(det);

            if (Math.Abs(volume - 1) < SCALEDECOMPOSITIONEPSILON) return 1;

            // scale is the cubic root of volume:            
            return MathF.Pow(volume, 1f / 3f);
        }        
    }
}