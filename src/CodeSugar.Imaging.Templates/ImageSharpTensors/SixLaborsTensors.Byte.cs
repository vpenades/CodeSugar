using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics.Tensors;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

#nullable disable

using __SIXLABORS = SixLabors.ImageSharp;
using __SIXLABORSPIXFMT = SixLabors.ImageSharp.PixelFormats;

using __RWTENSORSPANB = System.Numerics.Tensors.TensorSpan<byte>;
using __ROTENSORSPANB = System.Numerics.Tensors.ReadOnlyTensorSpan<byte>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static void CopyToTensor(this Image src, __RWTENSORSPANB dst, bool dstIsBGR = false)
        {
            switch (src)
            {
                case null: throw new ArgumentNullException(nameof(src));
                case Image<__SIXLABORSPIXFMT.A8> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Abgr32> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Argb32> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr24> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr565> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra32> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra4444> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra5551> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Byte4> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfSingle> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector2> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector4> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L16> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L8> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La16> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La32> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte2> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte4> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort2> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort4> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rg32> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb24> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb48> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba1010102> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba32> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba64> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.RgbaVector> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short2> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short4> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;

                default: throw new NotImplementedException(src.GetType().Name);
            }
        }

        public static void CopyToTensor(this Image src, System.Numerics.Matrix3x2 srcXform, __RWTENSORSPANB dst, bool dstIsBGR = false)
        {
            switch (src)
            {
                case null: throw new ArgumentNullException(nameof(src));
                case Image<__SIXLABORSPIXFMT.A8> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Abgr32> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Argb32> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr24> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr565> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra32> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra4444> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra5551> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Byte4> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfSingle> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector2> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector4> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L16> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L8> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La16> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La32> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte2> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte4> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort2> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort4> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rg32> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb24> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb48> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba1010102> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba32> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba64> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.RgbaVector> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short2> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short4> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;

                default: throw new NotImplementedException(src.GetType().Name);
            }
        }

        public static void CopyToImageSharp(this __ROTENSORSPANB src, Image dst, bool srcIsBGR = false)
        {
            switch (dst)
            {
                case null: throw new ArgumentNullException(nameof(src));
                case Image<__SIXLABORSPIXFMT.A8> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Abgr32> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Argb32> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr24> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr565> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra32> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra4444> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra5551> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Byte4> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfSingle> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector2> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector4> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L16> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L8> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La16> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La32> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte2> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte4> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort2> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort4> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rg32> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb24> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb48> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba1010102> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba32> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba64> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.RgbaVector> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short2> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short4> dstFmt: CopyToImageSharp(src, dstFmt, srcIsBGR); break;

                default: throw new NotImplementedException(dst.GetType().Name);
            }
        }


        public static void CopyToTensor<TPixel>(this Image<TPixel> src, __RWTENSORSPANB dst, bool dstIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            throw new NotImplementedException();
        }

        public static void CopyToTensor<TPixel>(this Image<TPixel> src, System.Numerics.Matrix3x2 srcXform, __RWTENSORSPANB dst, bool dstIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            throw new NotImplementedException();

            switch (src)
            {
                case Image<Rgb24> srcTyped:
                    if (DangerousTryGetDenseBytesTensor(src, out var srcTensor))
                    {
                        // srcTensor.DrawRgbPixelsOverRgbaBilinear
                    }
                    break;
            }

            
        }

        public static void CopyToImageSharp<TPixel>(this __ROTENSORSPANB src, Image<TPixel> dst, bool srcIsBGR = false)
           where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            throw new NotImplementedException();
        }
    }

}
