using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SixLabors.ImageSharp.PixelFormats;

#nullable disable

using __SIXLABORS = SixLabors.ImageSharp;
using __SIXLABORSPIXFMT = SixLabors.ImageSharp.PixelFormats;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarNumericsExtensions
    {
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

        private static float _LuminanceOrAlphaToScalar<TPixel>(TPixel pixel)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return (float)(_ToL16(pixel).PackedValue / ushort.MaxValue);
        }



        private static L16 _ToL16<TPixel>(TPixel pixel)
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
