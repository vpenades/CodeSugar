using System;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;

using InteropTypes.TensorBitmaps;

#if __REFERENCES_SKIASHARP
using SkiaSharp;
#endif

#nullable disable

#if __REFERENCES_SKIASHARP

using __TBKFORMATS = InteropTypes.TensorBitmaps.KnownPixelFormats;

using __SKIACOLOR = SkiaSharp.SKColorType;
using __SKIAALPHA = SkiaSharp.SKAlphaType;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        public static TensorBitmap<TElement, TPixel> ToResizedTensorBitmap<TElement, TPixel>(this SkiaSharp.SKBitmap srcImage, int newWidth, int newHeight, TensorPixelFormat? dstFmt = null, SKSamplingOptions? options = null)
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged
        {
            options ??= SkiaSharp.SKSamplingOptions.Default;

            var s = new SkiaSharp.SKSizeI(newWidth, newHeight);            

            using (var resized = srcImage.Resize(s, options.Value))
            {
                return resized.ToTensorBitmap<TElement, TPixel>(dstFmt);
            }
        }

        public static TensorBitmap<TElement, TPixel> ToTensorBitmap<TElement, TPixel>(this SkiaSharp.SKBitmap srcImage, TensorPixelFormat? dstFmt = null)
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged
        {
            if (dstFmt == null && TryInferTensorBitmapPixelFormat((typeof(TPixel), Unsafe.SizeOf<TPixel>()), out var fmt))
            {
                dstFmt = fmt;
            }

            if (dstFmt == null) throw new ArgumentNullException($"Could not infer pixel format from {nameof(TPixel)}:{typeof(TPixel).Name}", nameof(dstFmt));

            if (TryCastToTensorBitmap<byte>(srcImage, out var srcTensor1))
            {
                var dstBitmap = InteropTypes.TensorBitmaps.TensorBitmap<TElement, TPixel>.Create(srcImage.Width, srcImage.Height, dstFmt);
                srcTensor1.CopyPixelsTo(dstBitmap.AsTensorSpanBitmap());
                return dstBitmap;
            }

            if (TryCastToTensorBitmap<int>(srcImage, out var srcTensor4))
            {
                var dstBitmap = InteropTypes.TensorBitmaps.TensorBitmap<TElement, TPixel>.Create(srcImage.Width, srcImage.Height, dstFmt);
                srcTensor4.CopyPixelsTo(dstBitmap.AsTensorSpanBitmap());
                return dstBitmap;
            }

            throw new NotImplementedException();
        }


        public static SKBitmap ToSkiaSharp<TElement, TPixel>(this TensorBitmap<TElement, TPixel> srcBitmap)
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged
        {
            return ToSkiaSharp(srcBitmap.AsReadOnlyTensorSpanBitmap());
        }

        public static SKBitmap ToSkiaSharp<TElement, TPixel>(this TensorSpanBitmap<TElement, TPixel> srcBitmap)
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged
        {
            return ToSkiaSharp(srcBitmap.AsReadOnlyTensorSpanBitmap());
        }

        public static SKBitmap ToSkiaSharp<TElement, TPixel>(this ReadOnlyTensorSpanBitmap<TElement,TPixel> srcBitmap)
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged
        {
            var dstImage = new SKBitmap(srcBitmap.Width, srcBitmap.Height, true); // todo: check srcBitmap.Format for alpha

            if (TryCastToTensorBitmap<byte>(dstImage, out var dstTensor1))
            {
                srcBitmap.CopyPixelsTo(dstTensor1);
                return dstImage;
            }

            if (TryCastToTensorBitmap<int>(dstImage, out var dstTensor2))
            {
                srcBitmap.CopyPixelsTo(dstTensor2);
                return dstImage;
            }

            throw new NotImplementedException();
        }

        public static bool TryCastToTensorBitmap<TPixel>(this SKBitmap srcImage, out TensorSpanBitmap<byte, TPixel> tensorBitmap)
            where TPixel : unmanaged
        {
            if (Unsafe.SizeOf<TPixel>() == 1 && srcImage.BytesPerPixel == 1)
            {
                var srcTensor = DangerousGetPixelsAsTensorSpan(srcImage);
                var srcFmt = _SkiaSharpToTensorPixelFormat(srcImage.ColorType, srcImage.AlphaType);
                tensorBitmap = new TensorSpanBitmap<byte, byte>(srcTensor, srcFmt).Cast<TPixel>();
                return true;
            }

            if (Unsafe.SizeOf<TPixel>() == 4 && srcImage.BytesPerPixel == 4)
            {
                var srcTensor = DangerousGetPixelsAsTensorSpan(srcImage);
                var srcFmt = _SkiaSharpToTensorPixelFormat(srcImage.ColorType, srcImage.AlphaType);
                tensorBitmap = new TensorSpanBitmap<byte, int>(srcTensor, srcFmt).Cast<TPixel>();
                return true;
            }

            tensorBitmap = default;
            return false;
        }

        private static TensorPixelFormat _SkiaSharpToTensorPixelFormat(__SKIACOLOR ct, __SKIAALPHA at)
        {
            switch(ct)
            {
                case __SKIACOLOR.Alpha8: return __TBKFORMATS.Alpha8;
                case __SKIACOLOR.Gray8: return __TBKFORMATS.Luminance8;
                case __SKIACOLOR.Rgb888x: return __TBKFORMATS.Rgbx8888;
                
                case __SKIACOLOR.Rgba8888 when at == __SKIAALPHA.Premul: return __TBKFORMATS.Rgbp8888;
                case __SKIACOLOR.Rgba8888: return __TBKFORMATS.Rgba8888;
                
                case __SKIACOLOR.Bgra8888 when at == __SKIAALPHA.Premul: return __TBKFORMATS.Bgrp8888;
                case __SKIACOLOR.Bgra8888: return __TBKFORMATS.Bgra8888;                

                default: throw new NotImplementedException($"{ct} {at}");
            }
        }
    }
}

#endif