using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics.Tensors;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Buffers;

using SkiaSharp;

#nullable disable


using __RWTENSORSPANB = System.Numerics.Tensors.TensorSpan<byte>;
using __RWTENSORSPANF = System.Numerics.Tensors.TensorSpan<float>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static Tensor<byte> ToRgbTensor(this SKBitmap srcBitmap)
        {
            var span = srcBitmap.DangerousGetPixelsAsTensorSpan().AsReadOnlyTensorSpan();

            if (!srcBitmap.ColorType.IsBGRFormat() && srcBitmap.BytesPerPixel == 3)
            {
                return ToTensor(span);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the pixels of the bitmap as a byte tensor
        /// </summary>        
        /// <remarks>
        /// Notice that:<br/>
        /// - The returned TensorSpan is valid only as long as the <see cref="SKBitmap"/> is not disposed.<br/>
        /// - If pixels are modified, you must call <see cref="SKBitmap.NotifyPixelsChanged"/>.<br/>
        /// - The pixels contained within the tensor may be premultiplied. Check <see cref="SKBitmap.AlphaType"/>.
        /// </remarks>
        public static __RWTENSORSPANB DangerousGetPixelsAsTensorSpan(this SKBitmap srcBitmap)
        {
            if (srcBitmap == null) throw new ArgumentNullException(nameof(srcBitmap));            

            var srcBuffer = srcBitmap.GetPixelSpan();            

            var lengths = new nint[3];
            lengths[0] = srcBitmap.Height;
            lengths[1] = srcBitmap.Width;
            lengths[2] = srcBitmap.BytesPerPixel; // pixel elements count; could be 4 or 3 actually (but isDense would fail)

            var strides = new nint[3];
            strides[0] = srcBitmap.RowBytes;
            strides[1] = srcBitmap.BytesPerPixel; // pixel stride; here needs to be 4 
            strides[2] = strides[1] > 1 ? 1 : 0;            

            return new __RWTENSORSPANB(srcBuffer, lengths, strides);
        }        

        public static void CopyPixelsToTensor(this SKBitmap srcBitmap, __RWTENSORSPANF dstTensor, bool dstTensorIsBGR)
        {
            var normalized = Vector3.One / 255f;

            CopyPixelsToTensor(srcBitmap, dstTensor, dstTensorIsBGR, normalized, Vector3.Zero);
        }

        public static void CopyPixelsToTensor(this SKBitmap srcBitmap, __RWTENSORSPANF dstTensor, bool dstTensorIsBGR, Vector3 mul, Vector3 add)
        {
            if (srcBitmap == null) throw new ArgumentNullException(nameof(srcBitmap));            

            dstTensor = dstTensor.SqueezeIfRequired();

            if (!TryInferBitmapSize(dstTensor, out _, out _, out var channels, out var isCHW) || isCHW) GuardIsBitmap(dstTensor);

            if (channels != 3) throw new ArgumentException("must be RGB or BGR");

            var dstBitmap = _TensorSpanBitmap<float, Vector3>.Create(dstTensor);

            var needsReverseRGB = IsBGRFormat(srcBitmap.ColorType) ^ dstTensorIsBGR;

            switch (srcBitmap.ColorType)
            {
                case SKColorType.Rgb888x:
                case SKColorType.Rgba8888:

                    if (srcBitmap.AlphaType == SKAlphaType.Premul)
                    {
                        var srcBmp = new _SkiaSharpInternalBitmap<_SkiaSharpPremulPixel>(srcBitmap);
                        dstBitmap.CopyFrom(srcBmp, p=> _SkiaSharpPremulPixel.ToScaledVector3(p));
                    }
                    else
                    {
                        var srcBmp = new _SkiaSharpInternalBitmap<_RGBA>(srcBitmap);
                        dstBitmap.CopyFrom(srcBmp, p => p.ToScaledVector3());
                        
                    }
                    break;
            }

            /*
            
            
            int channels = GetByteStride(srcBitmap.ColorType);
            if (channels != 4) throw new NotImplementedException($"Color type {srcBitmap.ColorType} is not supported");
            
            
            
            var srcBuffer = srcBitmap.GetPixelSpan();
            var srcRowSize = channels * srcBitmap.Width;
            var srcRowStride = srcBitmap.RowBytes;

            var needsReverseRGB = IsBGRFormat(srcBitmap.ColorType) ^ dstTensorIsBGR;            

            if (srcBitmap.AlphaType == SKAlphaType.Premul)
            {
                var pixelUnpremul = _SkiaSharpPremulPixel.ToVector3Forward;
                if (needsReverseRGB) pixelUnpremul = _SkiaSharpPremulPixel.ToVector3Reverse;

                for (int y = 0; y < dstBitmap.Height; y++)
                {
                    var dstRow = dstBitmap.GetRowSpan(y);
                    var srcRow = srcBuffer.Slice(y * srcRowStride, srcRowSize);
                    var srcPixels = System.Runtime.InteropServices.MemoryMarshal.Cast<byte, _SkiaSharpPremulPixel>(srcRow);

                    for (int x = 0; x < dstRow.Length; x++)
                    {
                        dstRow[x] = pixelUnpremul(srcPixels[x]) * mul + add;
                    }
                }
            }
            else
            {
                for (int y = 0; y < dstBitmap.Height; y++)
                {
                    var dstRow = dstBitmap.GetRowSpan(y);
                    var srcRow = srcBuffer.Slice(y * srcRowStride, srcRowSize);                    

                    if (needsReverseRGB) ConvertBGRAtoRGB(srcRow, dstRow, mul, add);
                    else ConvertRGBAtoRGB(srcRow, dstRow, mul, add);
                }
            }

            return;
            */
        }        
    }
}
