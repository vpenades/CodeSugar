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

using __XY = System.Numerics.Vector2;
using __XYZ = System.Numerics.Vector3;
using __XYZW = System.Numerics.Vector4;

using __TENSORSPAN = System.Numerics.Tensors.TensorSpan<float>;
using __READONLYTENSORSPAN = System.Numerics.Tensors.ReadOnlyTensorSpan<float>;

using System.Security.Cryptography;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static void CopyToTensor(this Image src, __TENSORSPAN dst, bool dstIsBGR = false)
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

        public static void CopyToTensor(this Image src, System.Numerics.Matrix3x2 srcXform, __TENSORSPAN dst, bool dstIsBGR = false)
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

        public static void CopyToTensor<TPixel>(this Image<TPixel> src, __TENSORSPAN dst, bool dstIsBGR = false)
            where TPixel: unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            GuardNotNullOrEmpty(src);

            dst = dst.SqueezeIfRequired();
            if (!TryInferBitmapSize(dst, out var dstWidth, out var dstHeight, out var dstChannels, out var dstIsCHW))
            {
                GuardIsBitmap(dst, false);
            }

            var srcBmp = new _ImageSharpInteropBitmap<TPixel>(src);            

            if (dstIsCHW)
            {
                var numRows = Math.Min(srcBmp.Height, dstHeight);                

                for (int y = 0; y < numRows; y++)
                {
                    var srcRow = srcBmp.GetReadOnlyRowSpan(y);

                    GetBitmapPlaneRows(dst, y, dstIsBGR, out var dstRowR, out var dstRowG, out var dstRowB);
                    
                    CopyRowToPlanes<TPixel>(srcRow, dstRowR, dstRowG, dstRowB);
                }

                return;
            }
            
            switch (dstChannels)
            {
                case 1: _TensorSpanBitmap<float, float>.Create(dst).CopyFrom(srcBmp, _LuminanceOrAlphaToScalar); return;
                case 3: _TensorSpanBitmap<float, __XYZ>.Create(dst).CopyFrom(srcBmp, dstIsBGR ? ToScaledBGR : ToScaledRGB); return;
                case 4: _TensorSpanBitmap<float, __XYZW>.Create(dst).CopyFrom(srcBmp, dstIsBGR ? ToScaledBGRA : ToScaledRGBA); return;
                default: throw new ArgumentException("invalid lengths[2] range", nameof(dst));
            }
        }

        public static Image<TPixel> ToImageSharp<TPixel>(this __TENSORSPAN tensor, bool tensorIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            return tensor.AsReadOnlyTensorSpan().ToImageSharp<TPixel>(tensorIsBGR);
        }

        public static Image<TPixel> ToImageSharp<TPixel>(this __READONLYTENSORSPAN tensor, bool tensorIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            tensor = tensor.SqueezeIfRequired();
            if (!TryInferBitmapSize(tensor, out var w, out var h, out var c, out _)) GuardIsBitmap(tensor, false);

            var img = new Image<TPixel>((int)w, (int)h);

            tensor.CopyToImageSharp(img, tensorIsBGR);

            return img;
        }

        public static void CopyToImageSharp(this __TENSORSPAN src, Image dst, bool srcIsBGR = false)
        {
            CopyToImageSharp(src.AsReadOnlyTensorSpan(), dst, srcIsBGR);
        }        

        public static void CopyToImageSharp<TPixel>(this __TENSORSPAN src, Image<TPixel> dst, bool srcIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            CopyToImageSharp(src.AsReadOnlyTensorSpan(), dst, srcIsBGR);
        }

        public static void CopyToImageSharp(this __READONLYTENSORSPAN src, Image dst, bool srcIsBGR = false)
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

        public static void CopyToImageSharp<TPixel>(this __READONLYTENSORSPAN src, Image<TPixel> dst, bool srcIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            GuardNotNullOrEmpty(dst);

            src = src.SqueezeIfRequired();
            if (!TryInferBitmapSize(src, out var srcWidth, out var srcHeight, out var srcChannels, out var srcIsCHW))
            {
                GuardIsBitmap(src, false);
            }

            var dstBmp = new _ImageSharpInteropBitmap<TPixel>(dst);

            if (srcIsCHW)
            {
                TPixel pixConverter(__XYZ rgb)
                {
                    TPixel pix = default;
                    pix.FromScaledVector4(new __XYZW(rgb, 1));
                    return pix;
                }

                _CopyTensorPlanesToBitmap(src, srcIsBGR, dstBmp, pixConverter);

                return;
            }

            static TPixel _fromBGR(__XYZ bgr)
            {
                TPixel pix = default;
                pix.FromScaledVector4(new __XYZW(bgr.Z, bgr.Y, bgr.X, 1));
                return pix;
            }

            static TPixel _fromRGB(__XYZ rgb)
            {
                TPixel pix = default;
                pix.FromScaledVector4(new __XYZW(rgb, 1));
                return pix;
            }

            static TPixel _fromBGRA(__XYZW bgra)
            {
                TPixel pix = default;
                pix.FromScaledVector4(new __XYZW(bgra.Z, bgra.Y, bgra.X, bgra.W));
                return pix;
            }

            static TPixel _fromRGBA(__XYZW rgba)
            {
                TPixel pix = default;
                pix.FromScaledVector4(rgba);
                return pix;
            }

            switch (srcChannels)
            {
                case 1: _ReadOnlyTensorSpanBitmap<float, float>.Create(src).CopyTo(dstBmp, _ScalarToImageSharpPixel<TPixel>); return;
                case 3: _ReadOnlyTensorSpanBitmap<float, __XYZ>.Create(src).CopyTo(dstBmp, srcIsBGR ? _fromBGR : _fromRGB); return;
                case 4: _ReadOnlyTensorSpanBitmap<float, __XYZW>.Create(src).CopyTo(dstBmp, srcIsBGR ? _fromBGRA : _fromRGBA); return;
                default: throw new ArgumentException("invalid lengths[2] range", nameof(dst));
            }
        }        
    }

}
