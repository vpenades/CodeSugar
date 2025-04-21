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

using _MMARSHALL = System.Runtime.InteropServices.MemoryMarshal;
using _UNSAFE = System.Runtime.CompilerServices.Unsafe;
using _TENSORPRIMS = System.Numerics.Tensors.TensorPrimitives;
using _XY = System.Numerics.Vector2;
using _XYZ = System.Numerics.Vector3;
using _XYZW = System.Numerics.Vector4;


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

            public static __Vector3x64 Repeat(_XYZ v)
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

            public static __Vector3x64 Repeat(_XYZ v)
            {
                return new __Vector3x64
                {
                    X = new _XY(v.X, v.Y),
                    Y = new _XY(v.Z, v.X),
                    Z = new _XY(v.Y, v.Z)
                };
            }      

            public _XY X;
            public _XY Y;
            public _XY Z;

            #endif
        }

        #if NET6_0_OR_GREATER

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Vector64<float> RepeatVector64(this _XY v)
        {
            return Vector64.Create(v.X, v.Y);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Vector64<float> ConvertToSingle(this Vector64<int> value)
        {
            #if NET8_0_OR_GREATER
            return Vector64.ConvertToSingle(value);
            #else
            var span = _MMARSHALL.Cast<Vector64<int>,int>(_MMARSHALL.CreateSpan(ref value, 1));            

            return Vector64.Create((float)span[0], (float)span[1]);
            #endif
        }

        #endif
    }
}

