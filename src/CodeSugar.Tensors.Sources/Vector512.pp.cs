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
        struct __Vector3x512
        {
            public static __Vector3x512 Repeat(__XYZ v)
            {
                return new __Vector3x512
                {
                    X = Vector512.Create(v.X, v.Y, v.Z, v.X, v.Y, v.Z, v.X, v.Y, v.Z, v.X, v.Y, v.Z, v.X, v.Y, v.Z, v.X),
                    Y = Vector512.Create(v.Y, v.Z, v.X, v.Y, v.Z, v.X, v.Y, v.Z, v.X, v.Y, v.Z, v.X, v.Y, v.Z, v.X, v.Y),
                    Z = Vector512.Create(v.Z, v.X, v.Y, v.Z, v.X, v.Y, v.Z, v.X, v.Y, v.Z, v.X, v.Y, v.Z, v.X, v.Y, v.Z)
                };
            }

            public Vector512<float> X;
            public Vector512<float> Y;
            public Vector512<float> Z;
        }

        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        public static Vector512<float> RepeatVector512(this __XYZW v)
        {
            var vvvv = Vector128.AsVector128(v);
            var vvvvvvvv = Vector256.Create(vvvv, vvvv);
            return Vector512.Create(vvvvvvvv, vvvvvvvv);
        }

        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        public static Vector512<float> ConvertToSingle(this Vector512<int> value)
        {            
            return Vector512.ConvertToSingle(value);            
        }        
    }
}

