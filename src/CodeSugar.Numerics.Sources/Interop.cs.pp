// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using UNSAFE = System.Runtime.CompilerServices.Unsafe;

#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.Numerics
#else
namespace $rootnamespace$
#endif
{    
    internal static partial class CodeSugarForNumerics
    { 

        private const string _V2_XNA_VECTOR2 = "Microsoft.Xna.Framework.Vector2";
        private const string _V2_SIXLABORS_POINTF = "SixLabors.ImageSharp.PointF";
        private const string _V2_SIXLABORS_SIZEF = "SixLabors.ImageSharp.SizeF";

        private const string _V3_XNA_VECTOR2 = "Microsoft.Xna.Framework.Vector3";


        public static T ConvertTo<T>(this Vector2 src) where T:unmanaged                    
        {
            if (UNSAFE.SizeOf<T>() == 8) // reinterpret cast conversion
            {
                if (typeof(T) == typeof(Vector2)) return UNSAFE.As<Vector2, T>(ref src);
                if (typeof(T) == typeof(System.Drawing.PointF)) return UNSAFE.As<Vector2, T>(ref src);
                if (typeof(T) == typeof(System.Drawing.SizeF)) return UNSAFE.As<Vector2, T>(ref src);

                switch(typeof(T).FullName) // reasonably safe conversions
                {
                    case "_V2_XNA_VECTOR2": return UNSAFE.As<Vector2, T>(ref src);
                    case "_V2_SIXLABORS_POINTF": return UNSAFE.As<Vector2, T>(ref src);
                    case "_V2_SIXLABORS_SIZEF": return UNSAFE.As<Vector2, T>(ref src);
                }
            }

            if (UNSAFE.SizeOf<T>() == 16) // reinterpret cast conversion for double vector2            
            {
                var vv = new _Vector2Double(src);

                switch(typeof(T).FullName) // reasonably safe conversions
                {
                    case "System.Windows.Point": return UNSAFE.As<_Vector2Double, T>(ref vv);
                    case "System.Windows.Vector": return UNSAFE.As<_Vector2Double, T>(ref vv);
                    case "System.Windows.Size": return UNSAFE.As<_Vector2Double, T>(ref vv);
                }
            }

            throw new NotImplementedException();
        }

        public static T ConvertTo<T>(this Vector3 src) where T:unmanaged            
        {
            if (UNSAFE.SizeOf<T>() == 12) // reinterpret cast conversion
            {
                if (typeof(T) == typeof(Vector3)) return UNSAFE.As<Vector3, T>(ref src);

                switch(typeof(T).FullName) // reasonably safe conversions
                {
                    case "_V3_XNA_VECTOR2": return UNSAFE.As<Vector3, T>(ref src);
                }
            }

            throw new NotImplementedException();
        }
        

        private readonly struct _Vector2Double        
        {
            public _Vector2Double(Vector2 v) { X=v.X; Y=v.Y; }

            public readonly Double X;
            public readonly Double Y;
        }
    }
}
