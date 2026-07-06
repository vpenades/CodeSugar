using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

using PhotoSauce.MagicScaler;

#nullable disable

using __SIZE = System.Drawing.Size;


namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static void MagicScalerReadRawBitmap<TElement>(this Func<System.IO.Stream> streamFunc, ProcessImageSettings settings, out __SIZE bitmapSize, out int channels, out Guid pixelFormat, out TElement[] bitmapPixels)
            where TElement:unmanaged
        {
            using (var s = streamFunc.Invoke())
            {                
                using var pipeline = MagicImageProcessor.BuildPipeline(s, settings);

                var ps = pipeline.PixelSource!;

                pixelFormat = ps.Format;

                ConvertToRawBitmap<TElement>(ps, out bitmapSize, out channels, out bitmapPixels);                
            }
        }

        public static void ConvertToRawBitmap<TElement>(this PhotoSauce.MagicScaler.IPixelSource srcImg, out __SIZE bitmapSize, out int channels, out TElement[] bitmapPixels)
            where TElement:unmanaged
        {
            bitmapSize = new __SIZE(srcImg.Width, srcImg.Height);

            // Determine the number of channels from the format
            channels = GetMagicScalerChannelCount(srcImg.Format);
            if (channels < 1) throw new NotSupportedException($"Pixel format {srcImg.Format} is not supported");

            var rowStride = bitmapSize.Width * channels;

            bitmapPixels = new TElement[bitmapSize.Height * rowStride];            

            switch(bitmapPixels)
            {
                case byte[] rows:
                    {
                        for (int y = 0; y < bitmapSize.Height; y++)
                        {
                            var dstRow = rows.AsSpan(y * rowStride, rowStride);
                            srcImg.CopyRowPixels(y, rowStride, dstRow);
                        }
                        break;
                    }

                case float[] rows:
                    {
                        for (int y = 0; y < bitmapSize.Height; y++)
                        {
                            var dstRow = rows.AsSpan(y * rowStride, rowStride);
                            srcImg.CopyRowPixels(y, rowStride, dstRow);
                        }
                        break;
                    }

                default: throw new NotImplementedException(typeof(TElement).Name);
            }            
        }

        #if __REFERENCES_SYSTEMNUMERICSTENSORS

        public static System.Numerics.Tensors.Tensor<TElement> MagicScalerReadTensor<TElement>(this Func<System.IO.Stream> streamFunc, ProcessImageSettings settings, out Guid pixelFormat)
            where TElement:unmanaged
        {
            using (var s = streamFunc.Invoke())
            {
                return MagicScalerReadTensor<TElement>(s, settings, out pixelFormat);
            }
        }

        public static System.Numerics.Tensors.Tensor<TElement> MagicScalerReadTensor<TElement>(this System.IO.Stream s, ProcessImageSettings settings, out Guid pixelFormat) where TElement : unmanaged
        {
            using var pipeline = MagicImageProcessor.BuildPipeline(s, settings);

            var ps = pipeline.PixelSource!;

            pixelFormat = ps.Format;

            return ConvertToTensor<TElement>(ps);
        }

        public static System.Numerics.Tensors.Tensor<TElement> ConvertToTensor<TElement>(this PhotoSauce.MagicScaler.IPixelSource srcImg)
            where TElement:unmanaged
        {
            ConvertToRawBitmap<TElement>(srcImg, out var bitmapSize, out var channels, out var bitmapPixels);

            var lenghts = new nint[3];
            lenghts[0] = bitmapSize.Width;
            lenghts[1] = bitmapSize.Height;
            lenghts[2] = channels;

            return System.Numerics.Tensors.Tensor.Create<TElement>(bitmapPixels, lenghts);
        }

        #endif

    }
}
