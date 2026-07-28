// GENERATOR_REQUIRES: SixLabors.ImageSharp

using System;
using System.Runtime.CompilerServices;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

#nullable disable

using __SIXLABORS = SixLabors.ImageSharp;

using __SIZE = System.Drawing.Size;
using __STREAMFUNC = System.Func<System.IO.Stream>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static void ImageSharpReadRawBitmap<TPixel>(this __STREAMFUNC streamFunc, out __SIZE bitmapSize, out byte[] bitmapPixels)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (var s = streamFunc.Invoke())
            {
                var image = SixLabors.ImageSharp.Image.Load<TPixel>(s);
                ConvertToRawBitmap(image.Frames.RootFrame.PixelBuffer, out bitmapSize, out bitmapPixels);
            }
        }

        public static (__SIZE size, TPixel[] pixels) ImageSharpReadRawBitmap<TPixel>(this __STREAMFUNC streamFunc)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (var s = streamFunc.Invoke())
            {
                var image = SixLabors.ImageSharp.Image.Load<TPixel>(s);
                return ConvertToRawBitmap(image.Frames.RootFrame.PixelBuffer);
            }
        }

        public static void ConvertToRawBitmap<TPixel>(this __SIXLABORS.Memory.Buffer2D<TPixel> buffer, out __SIZE bitmapSize, out byte[] bitmapPixels)
            where TPixel: unmanaged, IPixel<TPixel>
        {
            var rowLen = buffer.Width * Unsafe.SizeOf<TPixel>();

            bitmapPixels = new byte[buffer.Height * rowLen];
            bitmapSize = new __SIZE(buffer.Width, buffer.Height);

            for (int y = 0; y < buffer.Height; ++y)
            {
                var srcRow = System.Runtime.InteropServices.MemoryMarshal.Cast<TPixel, byte>(buffer.DangerousGetRowSpan(y));
                var dstRow = bitmapPixels.AsSpan(y * rowLen, rowLen);
                srcRow.CopyTo(dstRow);
            }
        }

        public static (__SIZE size, TPixel[] pixels) ConvertToRawBitmap<TPixel>(this __SIXLABORS.Memory.Buffer2D<TPixel> buffer)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var bitmapPixels = new TPixel[buffer.Height * buffer.Width];
            var bitmapSize = new __SIZE(buffer.Width, buffer.Height);

            for (int y = 0; y < buffer.Height; ++y)
            {
                var srcRow = buffer.DangerousGetRowSpan(y);
                var dstRow = bitmapPixels.AsSpan(y * buffer.Width);
                srcRow.CopyTo(dstRow);
            }

            return (bitmapSize, bitmapPixels);
        }        
    }
}
