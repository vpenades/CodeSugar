using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Text;

using SixLabors.ImageSharp.PixelFormats;

using __SIXLABORSPIXFMT = SixLabors.ImageSharp.PixelFormats;

#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESIXLABORSNAMESPACE
namespace SixLabors.ImageSharp
#else
namespace $rootnamespace$
#endif
{
    internal static partial class CodeSugarForImageSharp
    {
        public static Vector4 ToPremultipliedVector4<TPixel>(this TPixel pixel)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var value = pixel.ToScaledVector4();
            value.X *= value.W;
            value.Y *= value.W;
            value.Z *= value.W;
            return value;
        }

        private static Vector4 _Unpremultiply(this Vector4 value)
        {
            if (value.W == 0) return value;            
            value.X /= value.W;
            value.Y /= value.W;
            value.Z /= value.W;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ComposeScaledVectorNormal(this Vector4 backPixel, Vector4 forePixel)
        {
            // premultiply
            forePixel.X *= forePixel.W;
            forePixel.Y *= forePixel.W;
            forePixel.Z *= forePixel.W;

            // premultiply
            backPixel.X *= backPixel.W;
            backPixel.Y *= backPixel.W;
            backPixel.Z *= backPixel.W;

            // alpha composition:            
            var ia = 1f - forePixel.W;
            backPixel.X = forePixel.X + backPixel.X * ia;
            backPixel.Y = forePixel.Y + backPixel.Y * ia;
            backPixel.Z = forePixel.Z + backPixel.Z * ia;
            backPixel.W = forePixel.W + backPixel.W * ia;

            // unpremultiply
            if (backPixel.W > 0)
            {
                backPixel.X /= backPixel.W;
                backPixel.Y /= backPixel.W;
                backPixel.Z /= backPixel.W;
            }

            return backPixel;
        }

        public static bool HasAlphaChannel<T>()
            where T : unmanaged, IPixel<T>
        {
            var t = typeof(T);
            if (t == typeof(__SIXLABORSPIXFMT.A8)) return true;
            if (t == typeof(__SIXLABORSPIXFMT.Abgr32)) return true;
            if (t == typeof(__SIXLABORSPIXFMT.Argb32)) return true;
            if (t == typeof(__SIXLABORSPIXFMT.Bgra32)) return true;
            if (t == typeof(__SIXLABORSPIXFMT.Bgra4444)) return true;
            if (t == typeof(__SIXLABORSPIXFMT.Bgra5551)) return true;
            if (t == typeof(__SIXLABORSPIXFMT.La16)) return true;
            if (t == typeof(__SIXLABORSPIXFMT.La32)) return true;
            if (t == typeof(__SIXLABORSPIXFMT.Rgba1010102)) return true;
            if (t == typeof(__SIXLABORSPIXFMT.Rgba32)) return true;
            if (t == typeof(__SIXLABORSPIXFMT.Rgba64)) return true;
            if (t == typeof(__SIXLABORSPIXFMT.RgbaVector)) return true;

            return false;
        }
    }
}
