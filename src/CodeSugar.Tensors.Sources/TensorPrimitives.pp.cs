using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.InteropServices;
using System.Text;

#if NET
using System.Runtime.Intrinsics;
#endif

#nullable disable

using MMARSHALL = System.Runtime.InteropServices.MemoryMarshal;
using UNSAFE = System.Runtime.CompilerServices.Unsafe;
using TENSORPRIMS = System.Numerics.Tensors.TensorPrimitives;

using SRCBYTES = System.ReadOnlySpan<byte>;
using DSTBYTES = System.Span<byte>;

using SRCX = System.ReadOnlySpan<float>;
using DSTX = System.Span<float>;

using SRCXYZ = System.ReadOnlySpan<System.Numerics.Vector3>;
using DSTXYZ = System.Span<System.Numerics.Vector3>;

using SRCXYZW = System.ReadOnlySpan<System.Numerics.Vector4>;
using DSTXYZW = System.Span<System.Numerics.Vector4>;

using XYZ = System.Numerics.Vector3;
using XYZW = System.Numerics.Vector4;


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
        public static void InPlaceAdd(this DSTX span, float addend)
        {
            TENSORPRIMS.Add(span, addend, span);
        }
        public static void InPlaceMultiply(this DSTX span, float scalar)
        {
            TENSORPRIMS.Multiply(span, scalar, span);
        }
        public static void InPlaceMultiplyAdd(this DSTX span, float mul, float add)
        {            
            TENSORPRIMS.Multiply(span, mul, span);
            TENSORPRIMS.Add(span, add, span);
        }
        public static void InPlaceMultiplyAdd(this DSTXYZ src, XYZ mul, XYZ add)
        {
            MultiplyAddTo(src, mul, add, src);
        }
        public static void InPlaceMultiplyAdd(this DSTXYZW src, XYZW mul, XYZW add)
        {
            MultiplyAddTo(src, mul, add, src);
        }        
        

        public static void ScaledCastTo(this SRCBYTES src, DSTX dst)
        {
            ScaledMultiplyTo(src, 1, dst);

            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248
            // TruncatedCastTo(src, dst);
            // InPlaceMultiply(dst, 1f / 255f);
        }
        public static void ScaledCastTo(this SRCX src, DSTBYTES dst)
        {
            for (int i = 0; i < dst.Length; i++)
            {
                dst[i] = (byte)Math.Clamp(src[i] * 255f, 0f, 255f);
            }            
        }
        public static void TruncatedCastTo(this SRCBYTES src, DSTX dst)
        {
            #if NET8_0_OR_GREATER
            for (int i = 0; i < dst.Length; ++i) { dst[i] = Byte.CreateTruncating(src[i]); }            
            #else
            for (int i = 0; i < dst.Length; ++i) { dst[i] = (byte)src[i]; }
            #endif
        }        
        public static void SaturatedCastTo(this SRCX src, DSTBYTES dst)
        {
            #if NET8_0_OR_GREATER
            for (int i = 0; i < dst.Length; ++i) { dst[i] = Byte.CreateSaturating(src[i]); }            
            #else
            for (int i = 0; i < dst.Length; ++i) { dst[i] = (byte)Math.Clamp(src[i],0,255); }
            #endif
        }


        public static void MultiplyTo(this SRCX src, float mul, DSTX dst)
        {
            TENSORPRIMS.Multiply(src, mul, dst);
        }
        public static void MultiplyAddTo(this SRCX src, float mul, float add, DSTX dst)
        {
            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248            

            if (src.Length != dst.Length) throw new ArgumentException("length mismatch", nameof(dst));

            if (add == 0)
            {
                TENSORPRIMS.Multiply(src, mul, dst);
                return;
            }

            #if NET8_0_OR_GREATER

            if (Vector512.IsHardwareAccelerated && Vector512<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<float, Vector512<float>>(src);
                var dstXXXX = MMARSHALL.Cast<float, Vector512<float>>(dst);
                var mulXXXX = Vector512.Create(mul);
                var addXXXX = Vector512.Create(add);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    dstPtr = srcPtr * mulXXXX + addXXXX;

                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 16);
                dst = dst.Slice(dstXXXX.Length * 16);

            }
            else

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<float, Vector256<float>>(src);
                var dstXXXX = MMARSHALL.Cast<float, Vector256<float>>(dst);
                var mulXXXX = Vector256.Create(mul);
                var addXXXX = Vector256.Create(add);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    dstPtr = srcPtr * mulXXXX + addXXXX;

                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 8);
                dst = dst.Slice(dstXXXX.Length * 8);

            }
            else

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<float, Vector128<float>>(src);
                var dstXXXX = MMARSHALL.Cast<float, Vector128<float>>(dst);
                var mulXXXX = Vector128.Create(mul);
                var addXXXX = Vector128.Create(add);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    dstPtr = srcPtr * mulXXXX + addXXXX;

                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
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
        public static void MultiplyAddTo(this SRCXYZ src, XYZ mul, XYZ add, DSTXYZ dst)
        {
            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248

            if (src.Length != dst.Length) throw new ArgumentException("length mismatch", nameof(dst));

            if (mul.X == mul.Y && mul.X == mul.Z && add.X == add.Y && add.X == add.Z)
            {
                var src1 = MMARSHALL.Cast<XYZ, float>(src);
                var dst1 = MMARSHALL.Cast<XYZ, float>(dst);
                MultiplyAddTo(src1, mul.X, add.X, dst1);
                return;
            }            

            #if NET8_0_OR_GREATER

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<XYZ, __Vector3x256>(src);
                var dstXXXX = MMARSHALL.Cast<XYZ, __Vector3x256>(dst);
                var mulXXXX = __Vector3x256.Repeat(mul);
                var addXXXX = __Vector3x256.Repeat(add);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);                

                for (int i = 0; i < dstXXXX.Length; ++i)
                {                    
                    dstPtr.X = srcPtr.X * mulXXXX.X + addXXXX.X;                    
                    dstPtr.Y = srcPtr.Y * mulXXXX.Y + addXXXX.Y;                    
                    dstPtr.Z = srcPtr.Z * mulXXXX.Z + addXXXX.Z;

                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);                    
                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 8);
                dst = dst.Slice(dstXXXX.Length * 8);
            }
            else

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<XYZ, __Vector3x128>(src);
                var dstXXXX = MMARSHALL.Cast<XYZ, __Vector3x128>(dst);
                var mulXXXX = __Vector3x128.Repeat(mul);
                var addXXXX = __Vector3x128.Repeat(add);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    dstPtr.X = srcPtr.X * mulXXXX.X + addXXXX.X;
                    dstPtr.Y = srcPtr.Y * mulXXXX.Y + addXXXX.Y;
                    dstPtr.Z = srcPtr.Z * mulXXXX.Z + addXXXX.Z;                    

                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
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
        public static void MultiplyAddTo(this SRCXYZW src, XYZW mul, XYZW add, DSTXYZW dst)
        {
            if (src.Length != dst.Length) throw new ArgumentException("length mismatch", nameof(dst));

            #if NET8_0_OR_GREATER

            if (Vector512.IsHardwareAccelerated && Vector512<float>.IsSupported)
            {
                var src512 = MMARSHALL.Cast<XYZW, Vector512<float>>(src);
                var dst512 = MMARSHALL.Cast<XYZW, Vector512<float>>(dst);

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
                var src256 = MMARSHALL.Cast<XYZW, Vector256<float>>(src);
                var dst256 = MMARSHALL.Cast<XYZW, Vector256<float>>(dst);

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
                var src128 = MMARSHALL.Cast<XYZW, Vector128<float>>(src);
                var dst128 = MMARSHALL.Cast<XYZW, Vector128<float>>(dst);

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


        public static void ScaledMultiplyTo(this SRCBYTES src, float mul, DSTX dst)
        {
            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248

            if (src.Length != dst.Length) throw new ArgumentException("length mismatch", nameof(dst));

            mul /= 255f;

            #if NET8_0_OR_GREATER

            if (Vector512.IsHardwareAccelerated && Vector512<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<byte, __PackedBytes16>(src);
                var dstXXXX = MMARSHALL.Cast<float, Vector512<float>>(dst);
                var mulXXXX = Vector512.Create(mul);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    var uuuu = Vector512.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H, srcPtr.I, srcPtr.J, srcPtr.K, srcPtr.L, srcPtr.M, srcPtr.N, srcPtr.O, srcPtr.P);
                    dstPtr = Vector512.ConvertToSingle(uuuu) * mulXXXX;

                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 16);
                dst = dst.Slice(dstXXXX.Length * 16);

            }
            else

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<byte, __PackedBytes8>(src);
                var dstXXXX = MMARSHALL.Cast<float, Vector256<float>>(dst);
                var mulXXXX = Vector256.Create(mul);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                Vector256<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector256.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H);
                    dstPtr = Vector256.ConvertToSingle(uuuu) * mulXXXX;

                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 8);
                dst = dst.Slice(dstXXXX.Length * 8);

            }
            else

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<byte, __PackedBytes4>(src);
                var dstXXXX = MMARSHALL.Cast<float, Vector128<float>>(dst);
                var mulXXXX = Vector128.Create(mul);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                Vector128<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector128.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D);
                    dstPtr = Vector128.ConvertToSingle(uuuu) * mulXXXX;

                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
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
        public static void ScaledMultiplyAddTo(this SRCBYTES src, float mul, float add, DSTX dst)
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
                var srcXXXX = MMARSHALL.Cast<byte, __PackedBytes16>(src);
                var dstXXXX = MMARSHALL.Cast<float, Vector512<float>>(dst);
                var mulXXXX = Vector512.Create(mul);
                var addXXXX = Vector512.Create(add);

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX.Slice(i));
                    ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX.Slice(i));

                    var uuuu = Vector512.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H, srcPtr.I, srcPtr.J, srcPtr.K, srcPtr.L, srcPtr.M, srcPtr.N, srcPtr.O, srcPtr.P);
                    dstPtr = Vector512.ConvertToSingle(uuuu) * mulXXXX + addXXXX;
                }

                src = src.Slice(srcXXXX.Length * 16);
                dst = dst.Slice(dstXXXX.Length * 16);

            }
            else

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<byte, __PackedBytes8>(src);
                var dstXXXX = MMARSHALL.Cast<float, Vector256<float>>(dst);
                var mulXXXX = Vector256.Create(mul);
                var addXXXX = Vector256.Create(add);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                Vector256<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector256.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H);
                    dstPtr = Vector256.ConvertToSingle(uuuu) * mulXXXX + addXXXX;

                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 8);
                dst = dst.Slice(dstXXXX.Length * 8);

            }
            else

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<byte, __PackedBytes4>(src);
                var dstXXXX = MMARSHALL.Cast<float, Vector128<float>>(dst);
                var mulXXXX = Vector128.Create(mul);
                var addXXXX = Vector128.Create(add);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                Vector128<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector128.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D);
                    dstPtr = Vector128.ConvertToSingle(uuuu) * mulXXXX + addXXXX;

                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
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
        public static void ScaledMultiplyAddTo(this SRCBYTES src, XYZ mul, XYZ add, DSTXYZ dst)
        {
            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248

            if (src.Length != dst.Length * 3) throw new ArgumentException("length mismatch", nameof(dst));

            if (mul.X == mul.Y && mul.X == mul.Z && add.X == add.Y && add.X == add.Z)
            {
                ScaledMultiplyAddTo(src, mul.X, add.X, MMARSHALL.Cast<XYZ, float>(dst));
                return;
            }

            mul /= 255f;

            #if NET8_0_OR_GREATER

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<byte, __PackedBytes8>(src);
                var dstXXXX = MMARSHALL.Cast<XYZ, __Vector3x256>(dst);
                var mulXXXX = __Vector3x256.Repeat(mul);
                var addXXXX = __Vector3x256.Repeat(add);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                Vector256<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector256.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H);
                    dstPtr.X = Vector256.ConvertToSingle(uuuu) * mulXXXX.X + addXXXX.X;
                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);

                    uuuu = Vector256.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H);
                    dstPtr.Y = Vector256.ConvertToSingle(uuuu) * mulXXXX.Y + addXXXX.Y;
                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);

                    uuuu = Vector256.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H);
                    dstPtr.Z = Vector256.ConvertToSingle(uuuu) * mulXXXX.Z + addXXXX.Z;
                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);

                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(dstXXXX.Length * 8 * 3);
                dst = dst.Slice(dstXXXX.Length * 8);

            }
            else
            
            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<byte, __PackedBytes4>(src);
                var dstXXXX = MMARSHALL.Cast<XYZ, __Vector3x128>(dst);
                var mulXXXX = __Vector3x128.Repeat(mul);
                var addXXXX = __Vector3x128.Repeat(add);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                Vector128<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector128.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D);
                    dstPtr.X = Vector128.ConvertToSingle(uuuu) * mulXXXX.X + addXXXX.X;
                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);

                    uuuu = Vector128.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D);
                    dstPtr.Y = Vector128.ConvertToSingle(uuuu) * mulXXXX.Y + addXXXX.Y;
                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);

                    uuuu = Vector128.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D);
                    dstPtr.Z = Vector128.ConvertToSingle(uuuu) * mulXXXX.Z + addXXXX.Z;
                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);

                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(dstXXXX.Length * 4 * 3);
                dst = dst.Slice(dstXXXX.Length * 4);
            }
            
            #endif

            // fallback

            for(int i=0; i < dst.Length; ++i)
            {
                dst[i] = new XYZ(src[i * 3 + 0], src[i * 3 + 1], src[i * 3 + 2]) * mul + add;
            }            
        }
        public static void ScaledMultiplyAddTo(this SRCBYTES src, XYZW mul, XYZW add, DSTXYZW dst)
        {
            // https://github.com/dotnet/runtime/issues/103756#issuecomment-2180747248

            if (src.Length != dst.Length * 4) throw new ArgumentException("length mismatch", nameof(dst));

            if (mul.X == mul.Y && mul.X == mul.Z && mul.X == mul.W && add.X == add.Y && add.X == add.Z && add.X == add.W)
            {
                ScaledMultiplyAddTo(src, mul.X, add.X, MMARSHALL.Cast<XYZW, float>(dst));
                return;
            }

            mul /= 255f;

            #if NET8_0_OR_GREATER

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<byte, __PackedBytes8>(src);
                var dstXXXX = MMARSHALL.Cast<XYZW, Vector256<float>>(dst);
                var mulXXXX = Vector256.Create(Vector128.AsVector128(mul), Vector128.AsVector128(mul));
                var addXXXX = Vector256.Create(Vector128.AsVector128(add), Vector128.AsVector128(add));

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                Vector256<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector256.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D, srcPtr.E, srcPtr.F, srcPtr.G, srcPtr.H);
                    dstPtr = Vector256.ConvertToSingle(uuuu) * mulXXXX + addXXXX;

                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 8);
                dst = dst.Slice(dstXXXX.Length * 8);

            }
            else

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported)
            {
                var srcXXXX = MMARSHALL.Cast<byte, __PackedBytes4>(src);
                var dstXXXX = MMARSHALL.Cast<XYZW, Vector128<float>>(dst);
                var mulXXXX = Vector128.AsVector128(mul);
                var addXXXX = Vector128.AsVector128(add);

                ref var srcPtr = ref MMARSHALL.GetReference(srcXXXX);
                ref var dstPtr = ref MMARSHALL.GetReference(dstXXXX);

                Vector128<int> uuuu = default;

                for (int i = 0; i < dstXXXX.Length; ++i)
                {
                    uuuu = Vector128.Create(srcPtr.A, srcPtr.B, srcPtr.C, srcPtr.D);
                    dstPtr = Vector128.ConvertToSingle(uuuu) * mulXXXX + addXXXX;

                    srcPtr = ref UNSAFE.Add(ref srcPtr, 1);
                    dstPtr = ref UNSAFE.Add(ref dstPtr, 1);
                }

                src = src.Slice(srcXXXX.Length * 4);
                dst = dst.Slice(dstXXXX.Length * 4);
            }

            #endif

            // fallback

            for (int i = 0; i < dst.Length; ++i)
            {
                dst[i] = new XYZW(src[i * 4 + 0], src[i * 4 + 1], src[i * 4 + 2], src[i * 4 + 3]) * mul + add;
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
