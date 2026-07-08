using System;
using System.Runtime.CompilerServices;

using InteropTypes.TensorBitmaps;

#if __REFERENCES_SIXLABORSIMAGESHARP
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
#endif

#nullable disable

#if __REFERENCES_SIXLABORSIMAGESHARP

namespace __CODESUGAR_ROOTNAMESPACE__
{    
    internal static partial class CodeSugarImagingExtensions
    {
        public static SixLabors.ImageSharp.Image<TPixel> ToImageSharp<TElement, TPixel>(this TensorBitmap<TElement, TPixel> srcBitmap)
            where TElement : unmanaged
            where TPixel : unmanaged, SixLabors.ImageSharp.PixelFormats.IPixel<TPixel>
        {
            var dstImage = new SixLabors.ImageSharp.Image<TPixel>(srcBitmap.Width, srcBitmap.Height);

            void _processPixels(SixLabors.ImageSharp.PixelAccessor<TPixel> pixelAccessor)
            {
                for (int y = 0; y < dstImage.Height; ++y)
                {
                    var srcRow = srcBitmap.GetRowPixelsSpan(y);
                    var dstRow = pixelAccessor.GetRowSpan(y);
                    srcRow.CopyTo(dstRow);
                }
            }

            dstImage.ProcessPixelRows(_processPixels);

            return dstImage;
        }

        public static SixLabors.ImageSharp.Image<TPixel> ToImageSharp<TElement, TPixel>(this ReadOnlyTensorSpanBitmap<TElement, TPixel> srcBitmap)
            where TElement : unmanaged
            where TPixel : unmanaged, SixLabors.ImageSharp.PixelFormats.IPixel<TPixel>
        {
            var dstImage = new SixLabors.ImageSharp.Image<TPixel>(srcBitmap.Width, srcBitmap.Height);

            for(int y=0; y < dstImage.Height; ++y)
            {
                var srcRow = srcBitmap.GetRowPixelsSpan(y);
                var dstRow = dstImage.Frames[0].PixelBuffer.DangerousGetRowSpan(y);
                srcRow.CopyTo(dstRow);
            }            

            return dstImage;
        }

        public static InteropTypes.TensorBitmaps.TensorBitmap<TElement, TPixel> ToResizedTensorBitmap<TElement, TPixel>(this SixLabors.ImageSharp.Image<TPixel> srcImage, int newWidth, int newHeight)
            where TElement : unmanaged
            where TPixel : unmanaged, SixLabors.ImageSharp.PixelFormats.IPixel<TPixel>
        {
            using(var resized = srcImage.Clone(dc => dc.Resize(newWidth, newHeight)))
            {
                return resized.ToTensorBitmap<TElement, TPixel>();
            }            
        }

        public static InteropTypes.TensorBitmaps.TensorBitmap<TElement, TPixel> ToTensorBitmap<TElement,TPixel>(this SixLabors.ImageSharp.Image<TPixel> srcImage)
            where TElement: unmanaged
            where TPixel:unmanaged, SixLabors.ImageSharp.PixelFormats.IPixel<TPixel>
        {
            if (!TryInferTensorBitmapPixelFormat((typeof(TPixel), Unsafe.SizeOf<TPixel>()), out var dstFmt)) throw new InvalidOperationException("Incompatible pixel type");
            var dstBitmap = InteropTypes.TensorBitmaps.TensorBitmap<TElement, TPixel>.Create(srcImage.Width, srcImage.Height, dstFmt);

            void _processPixels(SixLabors.ImageSharp.PixelAccessor<TPixel> pixelAccessor)
            {
                for (int y = 0; y < dstBitmap.Height; ++y)
                {
                    var dstRow = dstBitmap.GetRowPixelsSpan(y);
                    var srcRow = pixelAccessor.GetRowSpan(y);
                    srcRow.CopyTo(dstRow);
                }
            }

            srcImage.ProcessPixelRows(_processPixels);

            return dstBitmap;
        }

        /*
        public static InteropTypes.TensorBitmaps.TensorBitmap<TElement, TDstPixel> ToTensorBitmap<TSrcPixel, TElement, TDstPixel>(this SixLabors.ImageSharp.Image<TSrcPixel> srcImage)
            where TElement : unmanaged
            where TDstPixel : unmanaged
            where TSrcPixel : unmanaged, SixLabors.ImageSharp.PixelFormats.IPixel<TSrcPixel>
        {
            if (!TryInferTensorBitmapPixelFormat((typeof(TDstPixel), Unsafe.SizeOf<TDstPixel>()), out var dstFmt)) throw new InvalidOperationException("Incompatible pixel type");
            var dstBitmap = InteropTypes.TensorBitmaps.TensorBitmap<TElement, TDstPixel>.Create(srcImage.Width, srcImage.Height, dstFmt);

            void _processPixels(SixLabors.ImageSharp.PixelAccessor<TSrcPixel> pixelAccessor)
            {
                for (int y = 0; y < dstBitmap.Height; ++y)
                {
                    // we need a row converter
                    var dstRow = dstBitmap.GetRowPixelsSpan(y);
                    var srcRow = pixelAccessor.GetRowSpan(y);
                    srcRow.CopyTo(dstRow);
                }
            }

            srcImage.ProcessPixelRows(_processPixels);

            return dstBitmap;
        }*/
    }
}

#endif
