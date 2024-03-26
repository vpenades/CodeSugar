// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

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

        [MethodImpl(AGRESSIVE)]
        public static T UnsafeConvertTo<T>(this Vector2 src) where T:unmanaged { return _UnsafeAs<Vector2, T>(ref src); }

        [MethodImpl(AGRESSIVE)]
        public static T UnsafeConvertTo<T>(this Vector3 src) where T : unmanaged { return _UnsafeAs<Vector3, T>(ref src); }

        [MethodImpl(AGRESSIVE)]
        public static T UnsafeConvertToDouble<T>(this Vector2 src) where T : unmanaged
        {
            var vv = new _Vector2Double(src);
            return _UnsafeAs<_Vector2Double, T>(ref vv);
        }


        [MethodImpl(AGRESSIVE)]
        public static T ConvertTo<T>(this Vector2 src) where T:unmanaged                    
        {
            if (typeof(T) == typeof(Vector2)) return _UnsafeAs<Vector2, T>(ref src);
            if (typeof(T) == typeof(System.Drawing.PointF)) return _UnsafeAs<Vector2, T>(ref src);
            if (typeof(T) == typeof(System.Drawing.SizeF)) return _UnsafeAs<Vector2, T>(ref src);

            switch (typeof(T).FullName) // reasonably safe (and slow) conversions
            {
                case _V2_XNA_VECTOR2: return _UnsafeAs<Vector2, T>(ref src);
                case _V2_SIXLABORS_POINTF: return _UnsafeAs<Vector2, T>(ref src);
                case _V2_SIXLABORS_SIZEF: return _UnsafeAs<Vector2, T>(ref src);
                case "System.Windows.Point": var vv = new _Vector2Double(src); return _UnsafeAs<_Vector2Double, T>(ref vv);
                case "System.Windows.Vector": vv = new _Vector2Double(src); return _UnsafeAs<_Vector2Double, T>(ref vv);
                case "System.Windows.Size": vv = new _Vector2Double(src); return _UnsafeAs<_Vector2Double, T>(ref vv);
            }

            throw new NotImplementedException();
        }

        [MethodImpl(AGRESSIVE)]
        public static T ConvertTo<T>(this Vector3 src) where T:unmanaged            
        {
            if (typeof(T) == typeof(Vector3)) return _UnsafeAs<Vector3, T>(ref src);

            switch (typeof(T).FullName) // reasonably safe (and slow) conversions
            {
                case _V3_XNA_VECTOR2: return _UnsafeAs<Vector3, T>(ref src);
            }

            throw new NotImplementedException();
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private readonly struct _Vector2Double
        {
            public _Vector2Double(Vector2 v) { X=v.X; Y=v.Y; }

            public readonly Double X;
            public readonly Double Y;
        }
    }
}
