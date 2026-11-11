// GENERATOR_REQUIRES: SkiaSharp

using System;

using SkiaSharp;

#nullable disable

using __STREAMFUNC = System.Func<System.IO.FileMode, System.IO.Stream>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static SkiaSharp.SKBitmap ReadSkiaSharpBitmap(this System.IO.FileInfo finfo)
        {
            return ReadSkiaSharpBitmap(finfo.Open);
        }

        public static SkiaSharp.SKBitmap ReadSkiaSharpBitmap(this __STREAMFUNC stream)
        {
            using (var s = stream.Invoke(System.IO.FileMode.Open))
            {
                return ReadSkiaSharpBitmap(s);
            }
        }

        public static SkiaSharp.SKBitmap ReadSkiaSharpBitmap(this System.IO.Stream stream)
        {
            return SkiaSharp.SKBitmap.Decode(stream);
        }

        public static void WriteSkiaSharpBitmap(this System.IO.FileInfo finfo, SKBitmap bitmap, SKEncodedImageFormat fmt, int quality = 0)
        {
            WriteSkiaSharpBitmap(finfo.Open, bitmap, fmt, quality);
        }

        public static void WriteSkiaSharpBitmap(this __STREAMFUNC stream, SKBitmap bitmap, SKEncodedImageFormat fmt, int quality = 0)
        {
            if (quality == 0)
            {
                switch(fmt)
                {
                    case SKEncodedImageFormat.Jpeg: quality = 75; break;                        
                    default: quality = 100; break;
                }
            }

            using (var s = stream.Invoke(System.IO.FileMode.Create))
            {
                bitmap.Encode(s, fmt, quality);
            }
        }

    }
}
