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
        struct __PackedBytes4
        {
            public Byte A;
            public Byte B;
            public Byte C;
            public Byte D;            
        }

        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct __PackedBytes8
        {
            public Byte A;
            public Byte B;
            public Byte C;
            public Byte D;
            public Byte E;
            public Byte F;
            public Byte G;
            public Byte H;
        }

        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct __PackedBytes16
        {
            public Byte A;
            public Byte B;
            public Byte C;
            public Byte D;
            public Byte E;
            public Byte F;
            public Byte G;
            public Byte H;
            public Byte I;
            public Byte J;
            public Byte K;
            public Byte L;
            public Byte M;
            public Byte N;
            public Byte O;
            public Byte P;
        }


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

        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct __Vector3x256
        {
            public const int RepeatXYZCount = 8;

            public static __Vector3x256 Repeat(XYZ v)
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

        #endif
    }
}
