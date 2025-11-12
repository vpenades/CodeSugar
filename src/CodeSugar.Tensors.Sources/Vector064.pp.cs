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
using __XY = System.Numerics.Vector2;
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
        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct __Vector3x64
        {
            public const int RepeatXYZCount = 4;

            #if NET6_0_OR_GREATER

            public static __Vector3x64 Repeat(__XYZ v)
            {
                return new __Vector3x64
                {
                    X = Vector64.Create(v.X, v.Y),
                    Y = Vector64.Create(v.Z, v.X),
                    Z = Vector64.Create(v.Y, v.Z)
                };
            }

            public Vector64<float> X;
            public Vector64<float> Y;
            public Vector64<float> Z;

            #else

            public static __Vector3x64 Repeat(__XYZ v)
            {
                return new __Vector3x64
                {
                    X = new __XY(v.X, v.Y),
                    Y = new __XY(v.Z, v.X),
                    Z = new __XY(v.Y, v.Z)
                };
            }      

            public __XY X;
            public __XY Y;
            public __XY Z;

            #endif
        }

        #if NET6_0_OR_GREATER

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Vector64<float> RepeatVector64(this __XY v)
        {
            return Vector64.Create(v.X, v.Y);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Vector64<float> ConvertToSingle(this Vector64<int> value)
        {
            #if NET8_0_OR_GREATER
            return Vector64.ConvertToSingle(value);
            #else
            var span = __MMARSHALL.Cast<Vector64<int>,int>(__MMARSHALL.CreateSpan(ref value, 1));            

            return Vector64.Create((float)span[0], (float)span[1]);
            #endif
        }

        #endif
    }
}

