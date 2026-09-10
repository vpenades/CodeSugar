using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Buffers.Text;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarNumericsExtensions
    {
        [DebuggerStepThrough]
        public static string ToBase64String(this Vector2 v)
        {
            Span<Byte> buff = stackalloc byte[4 * 2];
            return __ToBase64String(v.Enumerate(), buff);
        }

        [DebuggerStepThrough]
        public static string ToBase64String(this Vector3 v)
        {
            Span<Byte> buff = stackalloc byte[4 * 3];
            return __ToBase64String(v.Enumerate(), buff);
        }

        [DebuggerStepThrough]
        public static string ToBase64String(this Vector4 v)
        {
            Span<Byte> buff = stackalloc byte[4 * 4];            
            return __ToBase64String(v.Enumerate(), buff);
        }

        [DebuggerStepThrough]
        public static string ToBase64String(this Quaternion v)
        {
            Span<Byte> buff = stackalloc byte[4 * 4];
            return __ToBase64String(new float[] {v.X,v.Y,v.Z,v.W}, buff);
        }

        [DebuggerStepThrough]
        public static string ToBase64String(this Matrix3x2 m)
        {
            Span<Byte> buff = stackalloc byte[4 * 6];
            return __ToBase64String(m.Enumerate(), buff);
        }

        [DebuggerStepThrough]
        public static string ToBase64String(this in Matrix4x4 m)
        {
            Span<Byte> buff = stackalloc byte[4 * 16];
            return __ToBase64String(m.Enumerate(), buff);
        }
    }
}
