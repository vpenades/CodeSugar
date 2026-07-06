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
#if NET

        [DebuggerStepThrough]
        public static string ToBase64String(this Vector2 v)
        {
            Span<Byte> buff = stackalloc byte[8];
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(0), v.X);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(4), v.Y);
            return System.Convert.ToBase64String(buff);
        }

        [DebuggerStepThrough]
        public static string ToBase64String(this Vector3 v)
        {
            Span<Byte> buff = stackalloc byte[12];
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(0), v.X);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(4), v.Y);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(8), v.Z);
            return System.Convert.ToBase64String(buff);
        }

        [DebuggerStepThrough]
        public static string ToBase64String(this Vector4 v)
        {
            Span<Byte> buff = stackalloc byte[16];
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(0), v.X);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(4), v.Y);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(8), v.Z);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(12), v.W);
            return System.Convert.ToBase64String(buff);
        }

        [DebuggerStepThrough]
        public static string ToBase64String(this Quaternion v)
        {
            Span<Byte> buff = stackalloc byte[16];
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(0), v.X);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(4), v.Y);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(8), v.Z);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(12), v.W);
            return System.Convert.ToBase64String(buff);
        }

        [DebuggerStepThrough]
        public static string ToBase64String(this Matrix3x2 m)
        {
            Span<Byte> buff = stackalloc byte[4 * 6];
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(0), m.M11);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(4), m.M12);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(8), m.M21);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(12), m.M22);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(16), m.M31);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(20), m.M32);
            return System.Convert.ToBase64String(buff);
        }

        [DebuggerStepThrough]
        public static string ToBase64(this in Matrix4x4 m)
        {
            Span<Byte> buff = stackalloc byte[4 * 16];
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(0), m.M11);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(4), m.M12);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(8), m.M13);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(12), m.M14);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(16), m.M21);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(20), m.M22);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(24), m.M23);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(28), m.M24);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(32), m.M31);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(36), m.M32);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(40), m.M33);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(44), m.M34);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(48), m.M41);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(52), m.M42);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(56), m.M43);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff.Slice(60), m.M44);
            return System.Convert.ToBase64String(buff);
        }        

        #endif

    }
}
