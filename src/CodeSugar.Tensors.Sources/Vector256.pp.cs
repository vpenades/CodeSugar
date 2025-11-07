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

using __MMARSHALL = System.Runtime.InteropServices.MemoryMarshal;
using __UNSAFE = System.Runtime.CompilerServices.Unsafe;
using __TENSORPRIMS = System.Numerics.Tensors.TensorPrimitives;


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
        #if NET6_0_OR_GREATER

        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct __Vector3x256
        {
            public const int RepeatXYZCount = 8;

            public static __Vector3x256 Repeat(System.Numerics.Vector3 v)
            {
                return new __Vector3x256
                {
                    X = Vector256.Create(v.X, v.Y, v.Z, v.X, v.Y, v.Z, v.X, v.Y),
                    Y = Vector256.Create(v.Z, v.X, v.Y, v.Z, v.X, v.Y, v.Z, v.X),
                    Z = Vector256.Create(v.Y, v.Z, v.X, v.Y, v.Z, v.X, v.Y, v.Z)
                };
            }

            public Vector256<float> X;
            public Vector256<float> Y;
            public Vector256<float> Z;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> RepeatVector256(this Vector4 v)
        {
            var vvvv = Vector128.AsVector128(v);
            return Vector256.Create(vvvv, vvvv);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> ConvertToSingle(this Vector256<int> value)
        {
            #if NET8_0_OR_GREATER
            return Vector256.ConvertToSingle(value);
            #else
            var span = __MMARSHALL.Cast<Vector256<int>, int>(__MMARSHALL.CreateSpan(ref value, 1));
            return Vector256.Create((float)span[0], (float)span[1], (float)span[2], (float)span[3], (float)span[4], (float)span[5], (float)span[6], (float)span[7]);
            #endif
        }

        #endif
    }
}

