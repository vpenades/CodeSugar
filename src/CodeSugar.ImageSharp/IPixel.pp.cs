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

        /// <summary>
        /// Converts a scalar value (which can be either alpha or luminance) to a sixlabors pixel format
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="l"></param>
        /// <returns></returns>
        private static TPixel _ScalarToSixLaborsPixel<TPixel>(float l)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            TPixel pix = default;            

            if (typeof(TPixel) == typeof(A8))
            {
                __SIXLABORSPIXFMT.Rgba32 tmpA = new Rgba32(255, 255, 255);
                tmpA.A = (byte)(Math.Clamp(l, 0, 1) * byte.MaxValue);
                pix.FromRgba32(tmpA);
            }
            else
            {
                __SIXLABORSPIXFMT.L16 tmpL = default;
                tmpL.PackedValue = (ushort)(Math.Clamp(l, 0, 1) * ushort.MaxValue);
                pix.FromL16(tmpL);
            }

            return pix;
        }

        private static float LuminanceOrAlphaToScalar<TPixel>(this TPixel pixel)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return (float)pixel.ToL16().PackedValue / ushort.MaxValue;
        }

        private static L16 ToL16<TPixel>(this TPixel pixel)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            __SIXLABORSPIXFMT.L16 pix = default;

            switch (pixel)
            {
                case A8 alpha: int a = alpha.PackedValue; return new L16((ushort)((a << 8) + a));
                case Rgb24 color: pix.FromRgb24(color); break;
                case Bgr24 color: pix.FromBgr24(color); break;
                case Rgba32 color: pix.FromRgba32(color); break;
                case Argb32 color: pix.FromArgb32(color); break;
                case Bgra32 color: pix.FromBgra32(color); break;
                default: pix.FromScaledVector4(pixel.ToScaledVector4()); break;
            }

            return pix;
        }

        
    }
}
