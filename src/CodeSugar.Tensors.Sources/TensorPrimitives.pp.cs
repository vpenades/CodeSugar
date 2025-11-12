using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.InteropServices;
using System.Text;

#if NET
using System.Runtime.Intrinsics;
using __UNSAFE = System.Runtime.CompilerServices.Unsafe;
#endif

#nullable disable

using __MMARSHALL = System.Runtime.InteropServices.MemoryMarshal;
using __TENSORPRIMS = System.Numerics.Tensors.TensorPrimitives;

using __SRCBYTES = System.ReadOnlySpan<byte>;
using __DSTBYTES = System.Span<byte>;

using __SRCX = System.ReadOnlySpan<float>;
using __DSTX = System.Span<float>;

using __SRCXYZ = System.ReadOnlySpan<System.Numerics.Vector3>;
using __DSTXYZ = System.Span<System.Numerics.Vector3>;

using __RCXYZW = System.ReadOnlySpan<System.Numerics.Vector4>;
using __DSTXYZW = System.Span<System.Numerics.Vector4>;

using __XYZ = System.Numerics.Vector3;
using __XYZW = System.Numerics.Vector4;


#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.Numerics
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForTensors
    {
        public static void InPlaceAdd(this __DSTX span, float addend)
        {
            __TENSORPRIMS.Add(span, addend, span);
        }
        public static void InPlaceMultiply(this __DSTX span, float scalar)
        {
            __TENSORPRIMS.Multiply(span, scalar, span);
        }
        public static void InPlaceMultiplyAdd(this __DSTX span, float mul, float add)
        {            
            __TENSORPRIMS.Multiply(span, mul, span);
            __TENSORPRIMS.Add(span, add, span);
        }
        public static void InPlaceMultiplyAdd(this __DSTXYZ src, __XYZ mul, __XYZ add)
        {
            MultiplyAddTo(src, mul, add, src);
        }
        public static void InPlaceMultiplyAdd(this __DSTXYZW src, __XYZW mul, __XYZW add)
        {
            MultiplyAddTo(src, mul, add, src);
        }        
        

        public static void ScaledCastTo(this __SRCBYTES src, __DSTX dst)
        {
            ScaledMultiplyTo(src, 1, dst);

            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248
            // TruncatedCastTo(src, dst);
            // InPlaceMultiply(dst, 1f / 255f);
        }
        public static void ScaledCastTo(this __SRCX src, __DSTBYTES dst)
        {
            for (int i = 0; i < dst.Length; i++)
            {
                dst[i] = (byte)Math.Clamp(src[i] * 255f, 0f, 255f);
            }            
        }
        public static void TruncatedCastTo(this __SRCBYTES src, __DSTX dst)
        {
            #if NET8_0_OR_GREATER
            for (int i = 0; i < dst.Length; ++i) { dst[i] = Byte.CreateTruncating(src[i]); }            
            #else
            for (int i = 0; i < dst.Length; ++i) { dst[i] = (byte)src[i]; }
            #endif
        }        
        public static void SaturatedCastTo(this __SRCX src, __DSTBYTES dst)
        {
            #if NET8_0_OR_GREATER
            for (int i = 0; i < dst.Length; ++i) { dst[i] = Byte.CreateSaturating(src[i]); }            
            #else
            for (int i = 0; i < dst.Length; ++i) { dst[i] = (byte)Math.Clamp(src[i],0,255); }
            #endif
        }


        public static void MultiplyTo(this __SRCX src, float mul, __DSTX dst)
        {
            __TENSORPRIMS.Multiply(src, mul, dst);
        }
        public static void MultiplyAddTo(this __SRCX src, float mul, float add, __DSTX dst)
        {
            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248            

            if (src.Length != dst.Length) throw new ArgumentException("length mismatch", nameof(dst));

            if (add == 0)
            {
                __TENSORPRIMS.Multiply(src, mul, dst);
                return;
            }

            #if NET8_0_OR_GREATER

            if (Vector512.IsHardwareAccelerated && Vector512<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<float, Vector512<float>>(src);
                var dstXXXX = __MMARSHALL.Cast<float, Vector512<float>>(dst);
                var mulXXXX = Vector512.Create(mul);
                var addXXXX = Vector512.Create(add);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    dstPtr = srcPtr * mulXXXX + addXXXX;

                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 16);
                dst = dst.Slice(dstXXXX.Length * 16);

            }
            else

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<float, Vector256<float>>(src);
                var dstXXXX = __MMARSHALL.Cast<float, Vector256<float>>(dst);
                var mulXXXX = Vector256.Create(mul);
                var addXXXX = Vector256.Create(add);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    dstPtr = srcPtr * mulXXXX + addXXXX;

                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 8);
                dst = dst.Slice(dstXXXX.Length * 8);

            }
            else

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<float, Vector128<float>>(src);
                var dstXXXX = __MMARSHALL.Cast<float, Vector128<float>>(dst);
                var mulXXXX = Vector128.Create(mul);
                var addXXXX = Vector128.Create(add);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    dstPtr = srcPtr * mulXXXX + addXXXX;

                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 4);
                dst = dst.Slice(dstXXXX.Length * 4);
            }

            #endif

            // fallback

            for (int i = 0; i < dst.Length; ++i)
            {
                dst[i] = src[i] * mul + add;
            }
        }
        public static void MultiplyAddTo(this __SRCXYZ src, __XYZ mul, __XYZ add, __DSTXYZ dst)
        {
            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248

            if (src.Length != dst.Length) throw new ArgumentException("length mismatch", nameof(dst));

            if (mul.X == mul.Y && mul.X == mul.Z && add.X == add.Y && add.X == add.Z)
            {
                var src1 = __MMARSHALL.Cast<__XYZ, float>(src);
                var dst1 = __MMARSHALL.Cast<__XYZ, float>(dst);
                MultiplyAddTo(src1, mul.X, add.X, dst1);
                return;
            }            

            #if NET8_0_OR_GREATER

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<__XYZ, __Vector3x256>(src);
                var dstXXXX = __MMARSHALL.Cast<__XYZ, __Vector3x256>(dst);
                var mulXXXX = __Vector3x256.Repeat(mul);
                var addXXXX = __Vector3x256.Repeat(add);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);                

                for (int i = 0; i < dstXXXX.Length; ++i)
                {                    
                    dstPtr.X = srcPtr.X * mulXXXX.X + addXXXX.X;                    
                    dstPtr.Y = srcPtr.Y * mulXXXX.Y + addXXXX.Y;                    
                    dstPtr.Z = srcPtr.Z * mulXXXX.Z + addXXXX.Z;

                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);                    
                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 8);
                dst = dst.Slice(dstXXXX.Length * 8);
            }
            else

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<__XYZ, __Vector3x128>(src);
                var dstXXXX = __MMARSHALL.Cast<__XYZ, __Vector3x128>(dst);
                var mulXXXX = __Vector3x128.Repeat(mul);
                var addXXXX = __Vector3x128.Repeat(add);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    dstPtr.X = srcPtr.X * mulXXXX.X + addXXXX.X;
                    dstPtr.Y = srcPtr.Y * mulXXXX.Y + addXXXX.Y;
                    dstPtr.Z = srcPtr.Z * mulXXXX.Z + addXXXX.Z;                    

                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 4);
                dst = dst.Slice(dstXXXX.Length * 4);
            }

            #endif

            // fallback

            for (int i = 0; i < dst.Length; ++i)
            {
                dst[i] = src[i] * mul + add;
            }
        }
        public static void MultiplyAddTo(this __RCXYZW src, __XYZW mul, __XYZW add, __DSTXYZW dst)
        {
            if (src.Length != dst.Length) throw new ArgumentException("length mismatch", nameof(dst));

            #if NET8_0_OR_GREATER

            if (Vector512.IsHardwareAccelerated && Vector512<float>.IsSupported)
            {
                var src512 = __MMARSHALL.Cast<__XYZW, Vector512<float>>(src);
                var dst512 = __MMARSHALL.Cast<__XYZW, Vector512<float>>(dst);

                var mul128 = Vector128.AsVector128(mul);
                var mul256 = Vector256.Create(mul128, mul128);
                var mul512 = Vector512.Create(mul256, mul256);

                var add128 = Vector128.AsVector128(add);
                var add256 = Vector256.Create(add128, add128);
                var add512 = Vector512.Create(add256, add256);

                for (int i = 0; i < dst512.Length; i++)
                {
                    var r = src512[i];
                    dst512[i] = r * mul512 + add512;
                }

                src = src.Slice(src512.Length * 4);
                dst = dst.Slice(dst512.Length * 4);
            }
            else

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var src256 = __MMARSHALL.Cast<__XYZW, Vector256<float>>(src);
                var dst256 = __MMARSHALL.Cast<__XYZW, Vector256<float>>(dst);

                var mul128 = Vector128.AsVector128(mul);
                var mul256 = Vector256.Create(mul128, mul128);

                var add128 = Vector128.AsVector128(add);
                var add256 = Vector256.Create(add128, add128);

                for (int i = 0; i < dst256.Length; i++)
                {
                    var r = src256[i];
                    dst256[i] = r * mul256 + add256;
                }

                src = src.Slice(src256.Length * 2);
                dst = dst.Slice(dst256.Length * 2);
            }
            else

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var src128 = __MMARSHALL.Cast<__XYZW, Vector128<float>>(src);
                var dst128 = __MMARSHALL.Cast<__XYZW, Vector128<float>>(dst);

                var mul128 = Vector128.AsVector128(mul);
                var add128 = Vector128.AsVector128(add);

                for (int i = 0; i < dst128.Length; i++)
                {
                    var r = src128[i];
                    dst128[i] = r * mul128 + add128;
                }

                src = src.Slice(src128.Length);
                dst = dst.Slice(dst128.Length);
            }

            #endif

            // fallback

            for (int i = 0; i < src.Length; i++)
            {
                var r = src[i];
                dst[i] = r * mul + add;
            }
        }


        public static void ScaledMultiplyTo(this __SRCBYTES src, float mul, __DSTX dst)
        {
            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248

            if (src.Length != dst.Length) throw new ArgumentException("length mismatch", nameof(dst));

            mul /= 255f;

            #if NET8_0_OR_GREATER

            if (Vector512.IsHardwareAccelerated && Vector512<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<byte, __PackedBytes16>(src);
                var dstXXXX = __MMARSHALL.Cast<float, Vector512<float>>(dst);
                var mulXXXX = Vector512.Create(mul);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    var uuuu = Vector512.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H, srcPtr.I, srcPtr.J, srcPtr.K, srcPtr.L, srcPtr.M, srcPtr.N, srcPtr.O, srcPtr.P);
                    dstPtr = Vector512.ConvertToSingle(uuuu) * mulXXXX;

                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 16);
                dst = dst.Slice(dstXXXX.Length * 16);

            }
            else

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<byte, __PackedBytes8>(src);
                var dstXXXX = __MMARSHALL.Cast<float, Vector256<float>>(dst);
                var mulXXXX = Vector256.Create(mul);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                Vector256<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector256.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H);
                    dstPtr = Vector256.ConvertToSingle(uuuu) * mulXXXX;

                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 8);
                dst = dst.Slice(dstXXXX.Length * 8);

            }
            else

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<byte, __PackedBytes4>(src);
                var dstXXXX = __MMARSHALL.Cast<float, Vector128<float>>(dst);
                var mulXXXX = Vector128.Create(mul);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                Vector128<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector128.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D);
                    dstPtr = Vector128.ConvertToSingle(uuuu) * mulXXXX;

                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 4);
                dst = dst.Slice(dstXXXX.Length * 4);
            }

            #endif

            // fallback

            for (int i = 0; i < dst.Length; ++i)
            {
                dst[i] = src[i] * mul;
            }
        }
        public static void ScaledMultiplyAddTo(this __SRCBYTES src, float mul, float add, __DSTX dst)
        {
            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248            

            if (src.Length != dst.Length) throw new ArgumentException("length mismatch", nameof(dst));

            if (add == 0)
            {
                ScaledMultiplyTo(src, mul, dst);
                return;
            }

            mul /= 255f;

            #if NET8_0_OR_GREATER

            if (Vector512.IsHardwareAccelerated && Vector512<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<byte, __PackedBytes16>(src);
                var dstXXXX = __MMARSHALL.Cast<float, Vector512<float>>(dst);
                var mulXXXX = Vector512.Create(mul);
                var addXXXX = Vector512.Create(add);

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX.Slice(i));
                    ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX.Slice(i));

                    var uuuu = Vector512.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H, srcPtr.I, srcPtr.J, srcPtr.K, srcPtr.L, srcPtr.M, srcPtr.N, srcPtr.O, srcPtr.P);
                    dstPtr = Vector512.ConvertToSingle(uuuu) * mulXXXX + addXXXX;
                }

                src = src.Slice(srcXXXX.Length * 16);
                dst = dst.Slice(dstXXXX.Length * 16);

            }
            else

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<byte, __PackedBytes8>(src);
                var dstXXXX = __MMARSHALL.Cast<float, Vector256<float>>(dst);
                var mulXXXX = Vector256.Create(mul);
                var addXXXX = Vector256.Create(add);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                Vector256<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector256.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H);
                    dstPtr = Vector256.ConvertToSingle(uuuu) * mulXXXX + addXXXX;

                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 8);
                dst = dst.Slice(dstXXXX.Length * 8);

            }
            else

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<byte, __PackedBytes4>(src);
                var dstXXXX = __MMARSHALL.Cast<float, Vector128<float>>(dst);
                var mulXXXX = Vector128.Create(mul);
                var addXXXX = Vector128.Create(add);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                Vector128<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector128.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D);
                    dstPtr = Vector128.ConvertToSingle(uuuu) * mulXXXX + addXXXX;

                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 4);
                dst = dst.Slice(dstXXXX.Length * 4);
            }

            #endif

            // fallback

            for (int i = 0; i < dst.Length; ++i)
            {
                dst[i] = src[i] * mul + add;
            }
        }
        public static void ScaledMultiplyAddTo(this __SRCBYTES src, __XYZ mul, __XYZ add, __DSTXYZ dst)
        {
            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248

            if (src.Length != dst.Length * 3) throw new ArgumentException("length mismatch", nameof(dst));

            if (mul.X == mul.Y && mul.X == mul.Z && add.X == add.Y && add.X == add.Z)
            {
                ScaledMultiplyAddTo(src, mul.X, add.X, __MMARSHALL.Cast<__XYZ, float>(dst));
                return;
            }

            mul /= 255f;

            #if NET8_0_OR_GREATER

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<byte, __PackedBytes8>(src);
                var dstXXXX = __MMARSHALL.Cast<__XYZ, __Vector3x256>(dst);
                var mulXXXX = __Vector3x256.Repeat(mul);
                var addXXXX = __Vector3x256.Repeat(add);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                Vector256<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector256.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H);
                    dstPtr.X = Vector256.ConvertToSingle(uuuu) * mulXXXX.X + addXXXX.X;
                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);

                    uuuu = Vector256.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H);
                    dstPtr.Y = Vector256.ConvertToSingle(uuuu) * mulXXXX.Y + addXXXX.Y;
                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);

                    uuuu = Vector256.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H);
                    dstPtr.Z = Vector256.ConvertToSingle(uuuu) * mulXXXX.Z + addXXXX.Z;
                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);

                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(dstXXXX.Length * 8 * 3);
                dst = dst.Slice(dstXXXX.Length * 8);

            }
            else
            
            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<byte, __PackedBytes4>(src);
                var dstXXXX = __MMARSHALL.Cast<__XYZ, __Vector3x128>(dst);
                var mulXXXX = __Vector3x128.Repeat(mul);
                var addXXXX = __Vector3x128.Repeat(add);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                Vector128<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector128.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D);
                    dstPtr.X = Vector128.ConvertToSingle(uuuu) * mulXXXX.X + addXXXX.X;
                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);

                    uuuu = Vector128.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D);
                    dstPtr.Y = Vector128.ConvertToSingle(uuuu) * mulXXXX.Y + addXXXX.Y;
                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);

                    uuuu = Vector128.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D);
                    dstPtr.Z = Vector128.ConvertToSingle(uuuu) * mulXXXX.Z + addXXXX.Z;
                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);

                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(dstXXXX.Length * 4 * 3);
                dst = dst.Slice(dstXXXX.Length * 4);
            }
            
            #endif

            // fallback

            for(int i=0; i < dst.Length; ++i)
            {
                dst[i] = new __XYZ(src[i * 3 + 0], src[i * 3 + 1], src[i * 3 + 2]) * mul + add;
            }            
        }
        public static void ScaledMultiplyAddTo(this __SRCBYTES src, __XYZW mul, __XYZW add, __DSTXYZW dst)
        {
            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248

            if (src.Length != dst.Length * 4) throw new ArgumentException("length mismatch", nameof(dst));

            if (mul.X == mul.Y && mul.X == mul.Z && mul.X == mul.W && add.X == add.Y && add.X == add.Z && add.X == add.W)
            {
                ScaledMultiplyAddTo(src, mul.X, add.X, __MMARSHALL.Cast<__XYZW, float>(dst));
                return;
            }

            mul /= 255f;

            #if NET8_0_OR_GREATER

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<byte, __PackedBytes8>(src);
                var dstXXXX = __MMARSHALL.Cast<__XYZW, Vector256<float>>(dst);
                var mulXXXX = Vector256.Create(Vector128.AsVector128(mul), Vector128.AsVector128(mul));
                var addXXXX = Vector256.Create(Vector128.AsVector128(add), Vector128.AsVector128(add));

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                Vector256<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector256.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H);
                    dstPtr = Vector256.ConvertToSingle(uuuu) * mulXXXX + addXXXX;

                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 8);
                dst = dst.Slice(dstXXXX.Length * 8);

            }
            else

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var srcXXXX = __MMARSHALL.Cast<byte, __PackedBytes4>(src);
                var dstXXXX = __MMARSHALL.Cast<__XYZW, Vector128<float>>(dst);
                var mulXXXX = Vector128.AsVector128(mul);
                var addXXXX = Vector128.AsVector128(add);

                ref var srcPtr = ref __MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref __MMARSHALL.GetReference(dstXXXX);

                Vector128<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector128.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D);
                    dstPtr = Vector128.ConvertToSingle(uuuu) * mulXXXX + addXXXX;

                    srcPtr = ref __UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref __UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 4);
                dst = dst.Slice(dstXXXX.Length * 4);
            }

            #endif

            // fallback

            for (int i = 0; i < dst.Length; ++i)
            {
                dst[i] = new __XYZW(src[i * 4 + 0], src[i * 4 + 1], src[i * 4 + 2], src[i * 4 + 3]) * mul + add;
            }
        }        
        

        public static void ShuffleTo<T>(this ReadOnlySpan<T> src, Span<T> dst, params int[] shuffleIndices)
            where T : unmanaged
        {
            var step = shuffleIndices.Length;
            bool isSequential = true;

            for (int i = 0; i < shuffleIndices.Length; ++i)
            {
                var idx = shuffleIndices[i];
                if (idx < 0 || idx >= step) throw new ArgumentOutOfRangeException(nameof(shuffleIndices));
                if (idx != i) isSequential = false;
            }

            if (isSequential)
            {
                src.CopyTo(dst);
                return;
            }

            for (int i = 0; i < dst.Length; i += step)
            {
                for (int j = 0; j < shuffleIndices.Length; ++j)
                {
                    dst[i + j] = src[i + shuffleIndices[j]];
                }
            }
        }
    }
}
