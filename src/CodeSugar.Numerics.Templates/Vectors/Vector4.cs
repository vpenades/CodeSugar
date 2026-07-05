using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarNumericsExtensions
    {
        #if !NET10_0_OR_GREATER
        public static float GetElement(this Vector4 v, int idx)
        {
            switch (idx)
            {
                case 0: return v.X;
                case 1: return v.Y;
                case 2: return v.Z;
                case 3: return v.W;
                default: throw new ArgumentOutOfRangeException(nameof(idx));
            }
        }
        #endif
    }
}