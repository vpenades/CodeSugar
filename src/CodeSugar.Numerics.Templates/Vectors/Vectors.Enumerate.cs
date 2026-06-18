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
        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static IEnumerable<float> Enumerate(this Vector2 v)
        {
            yield return v.X;
            yield return v.Y;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static IEnumerable<float> Enumerate(this Vector3 v)
        {
            yield return v.X;
            yield return v.Y;
            yield return v.Z;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static IEnumerable<float> Enumerate(this Vector4 v)
        {
            yield return v.X;
            yield return v.Y;
            yield return v.Z;
            yield return v.W;
        }
    }
}
