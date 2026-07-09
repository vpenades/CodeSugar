using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using InteropTypes.TensorBitmaps;

#if __REFERENCES_SIXLABORSIMAGESHARP
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
#endif

using __KPIXFMTS = InteropTypes.Numerics.KnownPixelFormats;

#nullable disable

#if __REFERENCES_SIXLABORSIMAGESHARP

namespace __CODESUGAR_ROOTNAMESPACE__
{    
    internal static partial class CodeSugarImagingExtensions
    {
        public static Image ToImageSharpUntyped<TElement, TPixel>(this ReadOnlyTensorSpanBitmap<TElement, TPixel> srcBitmap)
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged
        {
            if (srcBitmap.Format == __KPIXFMTS.Alpha8) return srcBitmap.Cast<A8>().ToImageSharp();            
            if (srcBitmap.Format == __KPIXFMTS.Luminance8) return srcBitmap.Cast<L8>().ToImageSharp();
            if (srcBitmap.Format == __KPIXFMTS.Rgb8) return srcBitmap.Cast<Rgb24>().ToImageSharp();
            if (srcBitmap.Format == __KPIXFMTS.Bgr8) return srcBitmap.Cast<Bgr24>().ToImageSharp();
            if (srcBitmap.Format == __KPIXFMTS.Rgba8) return srcBitmap.Cast<Rgba32>().ToImageSharp();
            if (srcBitmap.Format == __KPIXFMTS.Bgra8) return srcBitmap.Cast<Bgra32>().ToImageSharp();
            if (srcBitmap.Format == __KPIXFMTS.Argb8) return srcBitmap.Cast<Argb32>().ToImageSharp();
            if (srcBitmap.Format == __KPIXFMTS.Abgr8) return srcBitmap.Cast<Argb32>().ToImageSharp();
            if (srcBitmap.Format == __KPIXFMTS.RgbaF32) return srcBitmap.Cast<RgbaVector>().ToImageSharp();
            throw new NotImplementedException(srcBitmap.Format.ToString());
        }

        public static Image<TPixel> ToImageSharp<TElement, TPixel>(this TensorBitmap<TElement, TPixel> srcBitmap)
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged, SixLabors.ImageSharp.PixelFormats.IPixel<TPixel>
        {
            var dstImage = new Image<TPixel>(srcBitmap.Width, srcBitmap.Height);

            void _processPixels(PixelAccessor<TPixel> pixelAccessor)
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

        public static Image<TPixel> ToImageSharp<TElement, TPixel>(this ReadOnlyTensorSpanBitmap<TElement, TPixel> srcBitmap)
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged, SixLabors.ImageSharp.PixelFormats.IPixel<TPixel>
        {
            var dstImage = new Image<TPixel>(srcBitmap.Width, srcBitmap.Height);

            for(int y=0; y < dstImage.Height; ++y)
            {
                var srcRow = srcBitmap.GetRowPixelsSpan(y);
                var dstRow = dstImage.Frames[0].PixelBuffer.DangerousGetRowSpan(y);
                srcRow.CopyTo(dstRow);
            }            

            return dstImage;
        }        

        public static TensorSpanBitmap<TDstElement,TDstPixel> FitPixelsTo<TSrcPixel,TDstElement,TDstPixel>(this Image<TSrcPixel> srcImage, __TensorSpanBitmapRequestDelegate<TDstElement, TDstPixel> targetRequest, float overflowAmount = 0)
            where TSrcPixel: unmanaged, IPixel<TSrcPixel>
            where TDstElement: unmanaged, INumber<TDstElement>
            where TDstPixel: unmanaged
        {            
            var target = targetRequest(srcImage.Width, srcImage.Height);            

            var newSize = new System.Drawing.SizeF(srcImage.Width, srcImage.Height)
                .CalculateFittingSize(new System.Drawing.Size(target.Width, target.Height), overflowAmount)
                .ToSize();

            var source = srcImage.ToResizedTensorBitmap<TSrcPixel, TDstElement, TDstPixel>(newSize.Width, newSize.Height, target.Format);
            source.AsReadOnlyTensorSpanBitmap().CopyPixelsToCenter(target);

            return target;
        }

        public static TensorSpanPlanes3<TDstElement> FitPixelsTo<TSrcPixel, TDstElement>(this Image<TSrcPixel> srcImage, __TensorSpanPlanes3RequestDelegate<TDstElement> targetRequest, float overflowAmount = 0)
            where TSrcPixel : unmanaged, IPixel<TSrcPixel>
            where TDstElement : unmanaged, INumber<TDstElement>            
        {
            var target = targetRequest(srcImage.Width, srcImage.Height);

            var newSize = new System.Drawing.SizeF(srcImage.Width, srcImage.Height)
                .CalculateFittingSize(new System.Drawing.Size(target.Width, target.Height), overflowAmount)
                .ToSize();

            var source = srcImage.ToResizedTensorBitmap<TSrcPixel, TDstElement, __XYZPixel<TDstElement>>(newSize.Width, newSize.Height, target.Format);
            source.AsReadOnlyTensorSpanBitmap().CopyPixelsToCenter(target);

            return target;
        }

        public static TensorBitmap<TElement, TDstPixel> ToResizedTensorBitmap<TSrcPixel, TElement, TDstPixel>(this Image<TSrcPixel> srcImage, int newWidth, int newHeight, InteropTypes.Numerics.PixelFormat? dstFmt = default)
            where TSrcPixel : unmanaged, SixLabors.ImageSharp.PixelFormats.IPixel<TSrcPixel>
            where TElement : unmanaged, INumber<TElement>
            where TDstPixel : unmanaged
        {
            using(var resized = srcImage.Clone(dc => dc.Resize(newWidth, newHeight)))
            {
                return ToTensorBitmap<TSrcPixel, TElement, TDstPixel>(resized, dstFmt);
            }            
        }

        public static TensorBitmap<TElement, TDstPixel> ToTensorBitmap<TSrcPixel,TElement,TDstPixel>(this Image<TSrcPixel> srcImage, InteropTypes.Numerics.PixelFormat? dstFmt = default)
            where TSrcPixel : unmanaged, SixLabors.ImageSharp.PixelFormats.IPixel<TSrcPixel>
            where TElement: unmanaged, INumber<TElement>
            where TDstPixel : unmanaged
        {
            if (!TryInferTensorBitmapPixelFormat((typeof(TSrcPixel),Unsafe.SizeOf<TSrcPixel>()), out var srcFmt))
            {
                throw new InvalidOperationException("unable to infer src pixel format");
            }

            if (dstFmt == null)
            {
                if (!TryInferTensorBitmapPixelFormat((typeof(TDstPixel), Unsafe.SizeOf<TDstPixel>()), out var dstFmtx))
                {
                    throw new InvalidOperationException("unable to infer dst pixel format");
                }
                dstFmt = dstFmtx;
            }

            var cvt = InteropTypes.Numerics.IPixelConverter<TSrcPixel, TDstPixel>.Create(srcFmt, dstFmt, true);

            var dstBitmap = TensorBitmap<TElement, TDstPixel>.Create(srcImage.Width, srcImage.Height, dstFmt);

            void _processPixels(PixelAccessor<TSrcPixel> pixelAccessor)
            {
                for (int y = 0; y < dstBitmap.Height; ++y)
                {
                    var dstRow = dstBitmap.GetRowPixelsSpan(y);
                    var srcRow = pixelAccessor.GetRowSpan(y);
                    cvt.ConvertPixels(srcRow, dstRow);                    
                }
            }

            srcImage.ProcessPixelRows(_processPixels);

            return dstBitmap;
        }

        /*
        public static InteropTypes.TensorBitmaps.TensorBitmap<TElement, TDstPixel> ToTensorBitmap<TSrcPixel, TElement, TDstPixel>(this SixLabors.ImageSharp.Image<TSrcPixel> srcImage)
            where TElement : unmanaged, INumber<TElement>
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
