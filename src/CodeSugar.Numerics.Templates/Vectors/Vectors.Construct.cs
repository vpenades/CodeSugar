using System;
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
        public static Vector2 ToVector2(this (float x, float y) v) => new Vector2(v.x, v.y);

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static Vector3 ToVector3(this (float x, float y, float z) v) => new Vector3(v.x, v.y, v.z);

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static Vector4 ToVector4(this (float x, float y, float z, float w) v) => new Vector4(v.x, v.y, v.z, v.w);
    }
}
