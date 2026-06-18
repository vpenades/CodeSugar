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

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarNumericsExtensions
    {
        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct __Vector3x128
        {
            public static __Vector3x128 Repeat(__XYZ v)
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
        }        

        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        public static Vector128<float> RepeatVector128(this __XYZW v)
        {
            return Vector128.AsVector128(v);
        }

        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        public static Vector128<float> ConvertToSingle(this Vector128<int> value)
        {            
            return Vector128.ConvertToSingle(value);            
        }
    }
}

