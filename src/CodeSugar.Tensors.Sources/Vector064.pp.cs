using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.InteropServices;
using System.Text;

using System.Runtime.Intrinsics;

#nullable disable

using __XY = System.Numerics.Vector2;
using __XYZ = System.Numerics.Vector3;

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
        struct __Vector3x64
        {
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
        }
        
        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        public static Vector64<float> RepeatVector64(this __XY v)
        {
            return Vector64.Create(v.X, v.Y);
        }

        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        public static Vector64<float> ConvertToSingle(this Vector64<int> value)
        {
            return Vector64.ConvertToSingle(value);            
        }
    }
}

