using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.InteropServices;
using System.Text;
using System.Buffers;
using System.Reflection;
using System.Transactions;
using System.Threading;

#nullable disable

using __RGB = System.Numerics.Vector3;
using __RGBA = System.Numerics.Vector4;
using __RGBP = System.Numerics.Vector4;

using __RWTENSOR = System.Numerics.Tensors.ITensor;
using __ROTENSOR = System.Numerics.Tensors.IReadOnlyTensor;
using __RWTENSORSPANB = System.Numerics.Tensors.TensorSpan<byte>;
using __ROTENSORSPANB = System.Numerics.Tensors.ReadOnlyTensorSpan<byte>;
using __RWTENSORSPANF = System.Numerics.Tensors.TensorSpan<float>;
using __ROTENSORSPANF = System.Numerics.Tensors.ReadOnlyTensorSpan<float>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        /// <summary>
        /// Draws the bitmap represented by srcBitmap into dstBitmap using the given transform
        /// </summary>
        /// <param name="dstBitmap">The render target bitmap</param>
        /// <param name="srcTransform">The transform of the source bitmap</param>
        /// <param name="srcBitmap">The source bitmap</param>
        static void DrawPixelsOverStep<TSrcPixel,TVector, TDstPixel>(this __RWTENSORSPANB dstBitmap, System.Numerics.Matrix3x2 srcTransform, __ROTENSORSPANB srcBitmap)
            where TSrcPixel: unmanaged, _IPixelSample
            where TDstPixel : unmanaged, _IPixelSample
        {
            var srcSampler = new _StepSampler<byte, TSrcPixel>(srcBitmap);
            var srcXform = new _PixelSamplerTransform(srcTransform);
            var dstRendTgt = _TensorSpanBitmap<byte, TDstPixel>.Create(dstBitmap);

            if (default(TSrcPixel).HasAlpha)
            {
                for (int y = 0; y < dstRendTgt.Height; ++y)
                {
                    var dstRow = dstRendTgt.GetRowSpan(y);
                    for (int x = 0; x < dstRow.Length; ++x)
                    {
                        if (srcSampler.TryGetSample(srcXform.Transform(x, y), out __RGBP sample))
                        {
                            dstRow[x].Assign(sample);
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < dstRendTgt.Height; ++y)
                {
                    var dstRow = dstRendTgt.GetRowSpan(y);
                    for (int x = 0; x < dstRow.Length; ++x)
                    {
                        if (srcSampler.TryGetSample(srcXform.Transform(x, y), out __RGB sample))
                        {
                            dstRow[x].Assign(sample);
                        }
                    }
                }
            }            
        }

        /// <summary>
        /// Draws the bitmap represented by srcBitmap into dstBitmap using the given transform
        /// </summary>
        /// <param name="dstBitmap">The render target bitmap</param>
        /// <param name="srcTransform">The transform of the source bitmap</param>
        /// <param name="srcBitmap">The source bitmap</param>
        static void DrawPixelsOverBilinear<TSrcPixel, TVector, TDstPixel>(this __RWTENSORSPANB dstBitmap, System.Numerics.Matrix3x2 srcTransform, __ROTENSORSPANB srcBitmap)
            where TSrcPixel : unmanaged, _IPixelSample
            where TDstPixel : unmanaged, _IPixelSample
        {
            var srcSampler = new _BilinearSampler<byte, TSrcPixel>(srcBitmap);
            var srcXform = new _PixelSamplerTransform(srcTransform);
            var dstRendTgt = _TensorSpanBitmap<byte, TDstPixel>.Create(dstBitmap);

            if (default(TSrcPixel).HasAlpha)
            {
                for (int y = 0; y < dstRendTgt.Height; ++y)
                {
                    var dstRow = dstRendTgt.GetRowSpan(y);
                    for (int x = 0; x < dstRow.Length; ++x)
                    {
                        if (srcSampler.TryGetSample(srcXform.Transform(x, y), out __RGBP sample))
                        {
                            dstRow[x].Assign(sample);
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < dstRendTgt.Height; ++y)
                {
                    var dstRow = dstRendTgt.GetRowSpan(y);
                    for (int x = 0; x < dstRow.Length; ++x)
                    {
                        if (srcSampler.TryGetSample(srcXform.Transform(x, y), out __RGB sample))
                        {
                            dstRow[x].Assign(sample);
                        }
                    }
                }
            }
        }
    }
}
