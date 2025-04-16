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


                    
    }
}
