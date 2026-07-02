using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

#if __REFERENCES_SIXLABORSIMAGESHARP
using IMAGESHARPFMTS = SixLabors.ImageSharp.PixelFormats;
#endif

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        /// <summary>
        /// Converts any type representing a pixel color into a packed RGBA value
        /// </summary>
        /// <typeparam name="TPixel">the pixel type</typeparam>
        /// <param name="pixel">the pixel to convert</param>
        /// <returns>the packed rgba value</returns>
        /// <exception cref="NotSupportedException">when the conversion failed</exception>
        public static uint ToPackedRGBA<TPixel>(TPixel pixel)
            where TPixel : unmanaged
        {
            switch(pixel)
            {                
                case uint value: return value;

                #if __REFERENCES_SIXLABORSIMAGESHARP

                case IMAGESHARPFMTS.Rgba32 rgba: return rgba.PackedValue;
                case IMAGESHARPFMTS.Rgba64 src: return src.ToImageSharpPackedValue();
                case IMAGESHARPFMTS.Rgba1010102 src: return src.ToImageSharpPackedValue();
                case IMAGESHARPFMTS.RgbaVector src: return src.ToImageSharpPackedValue();

                case IMAGESHARPFMTS.A8 src: return src.ToImageSharpPackedValue();
                case IMAGESHARPFMTS.L8 src: return src.ToImageSharpPackedValue();
                case IMAGESHARPFMTS.L16 src: return src.ToImageSharpPackedValue();
                case IMAGESHARPFMTS.La16 src: return src.ToImageSharpPackedValue();
                case IMAGESHARPFMTS.La32 src: return src.ToImageSharpPackedValue();

                case IMAGESHARPFMTS.Rgb24 src: return src.ToImageSharpPackedValue();
                case IMAGESHARPFMTS.Rgb48 src: return src.ToImageSharpPackedValue();

                case IMAGESHARPFMTS.Bgr24 src: return src.ToImageSharpPackedValue();
                case IMAGESHARPFMTS.Bgr565 src: return src.ToImageSharpPackedValue();

                case IMAGESHARPFMTS.Bgra32 src: return src.ToImageSharpPackedValue();
                case IMAGESHARPFMTS.Bgra4444 src: return src.ToImageSharpPackedValue();
                case IMAGESHARPFMTS.Bgra5551 src: return src.ToImageSharpPackedValue();

                case IMAGESHARPFMTS.Argb32 src: return src.ToImageSharpPackedValue();
                case IMAGESHARPFMTS.Abgr32 src: return src.ToImageSharpPackedValue();

                #endif

                #if __REFERENCES_SKIASHARP

                case SkiaSharp.SKColor src:
                    // SKColor seems to be BGRA | ARGB
                    uint packed = src.Alpha;
                    packed <<= 8; packed |= src.Blue;
                    packed <<= 8; packed |= src.Green;
                    packed <<= 8; packed |= src.Red;
                    return packed;

                #endif

                default: throw new NotSupportedException(typeof(TPixel).Name);
            }
        }

    }
}
