using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics.Tensors;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Buffers;

using SkiaSharp;

#nullable disable

using __ITENSOR = System.Numerics.Tensors.ITensor;
using __TENSORSPAN = System.Numerics.Tensors.TensorSpan<float>;
using __READONLYTENSORSPAN = System.Numerics.Tensors.ReadOnlyTensorSpan<float>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarNumericsExtensions
    {
        /// <summary>
        /// Gets the pixels of the bitmap as a byte tensor
        /// </summary>        
        /// <remarks>
        /// If pixels are modified, you must call <see cref="SKBitmap.NotifyPixelsChanged"/>
        /// </remarks>
        public static System.Numerics.Tensors.TensorSpan<Byte> GetPixelsAsTensor(this SKBitmap srcBitmap)
        {
            if (srcBitmap == null) throw new ArgumentNullException(nameof(srcBitmap));
            if (srcBitmap.AlphaType == SKAlphaType.Premul) throw new NotImplementedException("Premul alpha type is not supported");

            int channels = GetByteStride(srcBitmap.ColorType);
            if (channels != 4) throw new NotImplementedException($"Color type {srcBitmap.ColorType} is not supported");

            var srcBuffer = srcBitmap.GetPixelSpan();            
            var srcRowStride = srcBitmap.RowBytes;

            var lengths = new nint[3];
            lengths[0] = srcBitmap.Height;
            lengths[1] = srcBitmap.Width;
            lengths[2] = 4; // could be 3 or 4 actually

            var strides = new nint[3];
            strides[2] = 4;
            strides[1] = strides[2] * srcBitmap.Width;
            strides[0] = strides[1] * srcBitmap.Height;            

            return new System.Numerics.Tensors.TensorSpan<Byte>(srcBuffer, lengths, strides);
        }        

        public static void CopyPixelsToTensor(this SKBitmap srcBitmap, __TENSORSPAN dstTensor, bool dstTensorIsBGR)
        {
            var normalized = Vector3.One / 255f;

            CopyPixelsToTensor(srcBitmap, dstTensor, dstTensorIsBGR, normalized, Vector3.Zero);
        }

        public static void CopyPixelsToTensor(this SKBitmap srcBitmap, __TENSORSPAN dstTensor, bool dstTensorIsBGR, Vector3 mul, Vector3 add)
        {
            if (srcBitmap == null) throw new ArgumentNullException(nameof(srcBitmap));
            if (dstTensor.IsEmpty) throw new ArgumentNullException(nameof(dstTensor));

            
            int channels = GetByteStride(srcBitmap.ColorType);
            if (channels != 4) throw new NotImplementedException($"Color type {srcBitmap.ColorType} is not supported");

            var dstBitmap = new _TensorSpanBitmapHWC<float, Vector3>(dstTensor);
            
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
        }        
    }
}
