﻿using System;
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
        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct __Vector3x128
        {
            public const int RepeatXYZCount = 4;

            #if NET6_0_OR_GREATER

            public static __Vector3x128 Repeat(XYZ v)
            {
                return new __Vector3x128
                {
                    X = Vector128.Create(v.X, v.Y, v.Z, v.X),
                    Y = Vector128.Create(v.Y, v.Z, v.X, v.Y),
                    Z = Vector128.Create(v.Z, v.X, v.Y, v.Z)
                };
            }

            public Vector128<float> X;
            public Vector128<float> Y;
            public Vector128<float> Z;

            #else

            public static __Vector3x128 Repeat(XYZ v)
            {
                return new __Vector3x128
                {
                    X = new XYZW(v.X, v.Y, v.Z, v.X),
                    Y = new XYZW(v.Y, v.Z, v.X, v.Y),
                    Z = new XYZW(v.Z, v.X, v.Y, v.Z)
                };
            }      

            public XYZW X;
            public XYZW Y;
            public XYZW Z;

            #endif
        }

        #if NET6_0_OR_GREATER

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> RepeatVector128(this XYZW v)
        {
            return Vector128.AsVector128(v);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> ConvertToSingle(this Vector128<int> value)
        {
            #if NET8_0_OR_GREATER
            return Vector128.ConvertToSingle(value);
            #else
            var span = MMARSHALL.Cast<Vector128<int>,int>(MMARSHALL.CreateSpan(ref value, 1));
            return Vector128.Create((float)span[0], (float)span[1], (float)span[2], (float)span[3]);
            #endif
        }

        #endif
    }
}

