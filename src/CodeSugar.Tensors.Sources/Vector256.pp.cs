using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.InteropServices;
using System.Text;

using System.Runtime.Intrinsics;

#nullable disable

using __XYZ = System.Numerics.Vector3;
using __XYZW = System.Numerics.Vector4;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.Numerics.Tensors
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForTensors
    {
        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct __Vector3x256
        {
            public static __Vector3x256 Repeat(__XYZ v)
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

        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        public static Vector256<float> RepeatVector256(this __XYZW v)
        {
            var vvvv = Vector128.AsVector128(v);
            return Vector256.Create(vvvv, vvvv);
        }

        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        public static Vector256<float> ConvertToSingle(this Vector256<int> value)
        {         
            return Vector256.ConvertToSingle(value);
        }        
    }
}

