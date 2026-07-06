using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable

using __VECTOR2 = System.Numerics.Vector2;
using __VECTOR3 = System.Numerics.Vector3;
using __VECTOR4 = System.Numerics.Vector4;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarNumericsExtensions
    {
        #region fallbacks to existing APIs

        #if !NET10_0_OR_GREATER

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static __VECTOR2 AsVector2(this __VECTOR4 v)
        {
            return new __VECTOR2(v.X, v.Y);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static __VECTOR3 AsVector3(this __VECTOR4 v)
        {
            return new __VECTOR3(v.X, v.Y, v.Z);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static float GetElement(this __VECTOR4 v, int idx)
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

        #endregion
    }
}