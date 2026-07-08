using System;
using System.Collections.Generic;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Text;

using SkiaSharp;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static SkiaSharp.SKBitmap ReadSkiaSharpBitmap(this System.IO.FileInfo finfo)
        {
            return ReadSkiaSharpBitmap(finfo.OpenRead);
        }

        public static SkiaSharp.SKBitmap ReadSkiaSharpBitmap(this Func<System.IO.Stream> stream)
        {
            using (var s = stream.Invoke())
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
            WriteSkiaSharpBitmap(finfo.Create, bitmap, fmt, quality);
        }

        public static void WriteSkiaSharpBitmap(this Func<System.IO.Stream> stream, SKBitmap bitmap, SKEncodedImageFormat fmt, int quality = 0)
        {
            if (quality == 0)
            {
                switch(fmt)
                {
                    case SKEncodedImageFormat.Jpeg: quality = 75; break;                        
                    default: quality = 100; break;
                }
            }

            using (var s = stream.Invoke())
            {
                bitmap.Encode(s, fmt, quality);
            }
        }

    }
}
