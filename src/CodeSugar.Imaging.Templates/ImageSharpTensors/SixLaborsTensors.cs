using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics.Tensors;

using SixLabors.ImageSharp;

#nullable disable

using __SIXLABORS = SixLabors.ImageSharp;
using __SIXLABORSPIXFMT = SixLabors.ImageSharp.PixelFormats;

using __RWTENSOR = System.Numerics.Tensors.ITensor;
using __ROTENSOR = System.Numerics.Tensors.IReadOnlyTensor;
using __ROTENSORSPANF = System.Numerics.Tensors.ReadOnlyTensorSpan<float>;
using __RWTENSORSPANF = System.Numerics.Tensors.TensorSpan<float>;

using __XY = System.Numerics.Vector2;
using __XYZ = System.Numerics.Vector3;
using __XYZW = System.Numerics.Vector4;

using SixLabors.ImageSharp.PixelFormats;
using PhotoSauce.MagicScaler;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static void ImageSharpAction(this __ROTENSOR tensor, Action<Image> imageAction, bool tensorIsBGR = false)
        {
            GuardIsBitmap(tensor, false);

            switch (tensor)
            {
                case System.Numerics.Tensors.Tensor<float> typedTensor: typedTensor.AsTensorSpan().ImageSharpAction(imageAction, tensorIsBGR); break;
                default: throw new ArgumentException($"{tensor.GetType().Name} not implemented", nameof(tensor));
            }
        }

        public static void ImageSharpAction(this __RWTENSORSPANF tensor, Action<Image> imageAction, bool tensorIsBGR = false)
        {
            GuardIsBitmap(tensor, false);

            tensor.AsReadOnlyTensorSpan().ImageSharpAction(imageAction, tensorIsBGR);
        }

        public static void ImageSharpAction(this __ROTENSORSPANF tensor, Action<Image> imageAction, bool tensorIsBGR = false)
        {
            tensor = tensor.SqueezeIfRequired();
            if (!TryInferBitmapSize(tensor, out _, out _, out var channels, out _)) GuardIsBitmap(tensor, false);

            switch (channels)
            {
                case 1: _ImageSharpAction<L16>(tensor, imageAction, tensorIsBGR); break;
                case 3: _ImageSharpAction<Rgb24>(tensor, imageAction, tensorIsBGR); break;
                case 4: _ImageSharpAction<Rgba32>(tensor, imageAction, tensorIsBGR); break;
                default: throw new NotSupportedException("number of channels");
            }
        }

        private static void _ImageSharpAction<TPixel>(__ROTENSORSPANF tensor, Action<Image> imageAction, bool tensorIsBGR)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (var img = tensor.ToImageSharp<TPixel>(tensorIsBGR))
            {
                imageAction(img);
            }
        }

        public static Image<TPixel> ToImageSharp<TPixel>(this __ROTENSOR tensor, bool tensorIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            GuardIsBitmap(tensor, false);

            if (tensor is System.Numerics.Tensors.Tensor<float> ft)
            {
                return ft.AsTensorSpan().ToImageSharp<TPixel>(tensorIsBGR);
            }

            throw new ArgumentException($"{tensor.GetType().Name} not implemented", nameof(tensor));
        }

        public static void CopyToImageSharp(this __RWTENSOR src, Image dst, bool srcIsBGR = false)
        {
            GuardIsBitmap(src);

            if (src is System.Numerics.Tensors.Tensor<float> ft)
            {
                var fts = ft.AsTensorSpan();
                CopyToImageSharp(fts, dst, srcIsBGR);
                return;
            }

            throw new ArgumentException($"{dst.GetType().Name} not implemented", nameof(dst));
        }           

        

        /*
        public static Tensor<TElement> ToTensor<TElement>(this Image img, int numChannels, bool dstIsBgr)
            where TElement : unmanaged
        {
            if (numChannels < 1 || numChannels > 4) throw new ArgumentOutOfRangeException("must be a value between 1 and 4", nameof(numChannels));

            var size = new System.Drawing.Size(img.Width, img.Height);

            var dstBmp = size.CreateTensorBitmapHWC<TElement>(numChannels);
            img.CopyToTensor(dstBmp.AsTensorSpan(), dstIsBgr);
            return dstBmp;
        }
        
        public static void CopyToTensor<TElement>(this Image src, TensorSpan<TElement> dst, bool dstIsBGR = false)
            where TElement : unmanaged
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
        }*/       


        public static Tensor<TElement> ToTensor<TPixel, TElement>(this Image<TPixel> img, int numChannels, bool dstIsBgr)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
            where TElement : unmanaged
        {
            if (numChannels < 1 || numChannels > 4) throw new ArgumentOutOfRangeException("must be a value between 1 and 4", nameof(numChannels));

            var size = new System.Drawing.Size(img.Width, img.Height);

            var dstBmp = size.CreateTensorBitmapHWC<TElement>(numChannels);
            img.CopyToTensor(dstBmp.AsTensorSpan(), dstIsBgr);
            return dstBmp;
        }

        public static void CopyToTensor<TPixel, TElement>(this Image<TPixel> src, TensorSpan<TElement> dst, bool dstIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
            where TElement : unmanaged
        {
            if (typeof(TElement) == typeof(byte)) { _CopyToByteTensor(src, ref dst, dstIsBGR); return; }
            if (typeof(TElement) == typeof(float)) { _CopyToFloatTensor(src, ref dst, dstIsBGR); return; }

            throw new NotImplementedException(typeof(TElement).Name);
        }

        private static void _CopyToFloatTensor<TPixel, TElement>(Image<TPixel> src, ref TensorSpan<TElement> dst, bool dstIsBGR)
            where TPixel : unmanaged, IPixel<TPixel>
            where TElement : unmanaged
        {
            if (typeof(TElement) != typeof(float)) throw new InvalidOperationException();

            GuardNotNullOrEmpty(src);

            dst = dst.SqueezeIfRequired();
            if (!TryInferBitmapSize(dst, out var dstWidth, out var dstHeight, out var dstChannels, out var dstIsCHW))
            {
                GuardIsBitmap(dst, false);
            }

            var srcBmp = new _ImageSharpInteropBitmap<TPixel>(src);
            var swapRGB = dstIsBGR ^ GetIsBGR<TPixel>();

            if (dstIsCHW)
            {
                if (_TensorSpanBitmap<TElement, __XYZ>.TryCreatePlanes(dst, out var dstX, out var dstY, out var dstZ))
                {
                    if (swapRGB) { var tmp = dstX; dstX = dstZ; dstZ = tmp; }

                    var numRows = Math.Min(srcBmp.Height, dstHeight);

                    for (int y = 0; y < numRows; y++)
                    {
                        var srcRow = srcBmp.GetReadOnlyRowSpan(y);
                        var dstRowX = System.Runtime.InteropServices.MemoryMarshal.Cast<TElement, float>(dstX.GetRowSpan(y));
                        var dstRowY = System.Runtime.InteropServices.MemoryMarshal.Cast<TElement, float>(dstY.GetRowSpan(y));
                        var dstRowZ = System.Runtime.InteropServices.MemoryMarshal.Cast<TElement, float>(dstZ.GetRowSpan(y));

                        CopyRowToPlanes(srcRow, dstRowX, dstRowY, dstRowZ);
                    }

                    return;
                }

                throw new NotImplementedException();
            }

            switch (dstChannels)
            {
                case 1: _TensorSpanBitmap<TElement, float>.Create(dst).CopyFrom(srcBmp, _LuminanceOrAlphaToScalar); return;
                case 3: _TensorSpanBitmap<TElement, __XYZ>.Create(dst).CopyFrom(srcBmp, swapRGB ? ToScaledBGR : ToScaledRGB); return;
                case 4: _TensorSpanBitmap<TElement, __XYZW>.Create(dst).CopyFrom(srcBmp, swapRGB ? ToScaledBGRA : ToScaledRGBA); return;
                default: throw new ArgumentException($"invalid channels count {dstChannels}", nameof(dst));
            }
        }

        private static void _CopyToByteTensor<TPixel, TElement>(Image<TPixel> src, ref TensorSpan<TElement> dst, bool dstIsBGR)
            where TPixel : unmanaged, IPixel<TPixel>
            where TElement : unmanaged
        {
            if (typeof(TElement) != typeof(byte)) throw new InvalidOperationException();

            GuardNotNullOrEmpty(src);

            dst = dst.SqueezeIfRequired();
            if (!TryInferBitmapSize(dst, out var dstWidth, out var dstHeight, out var dstChannels, out var dstIsCHW))
            {
                GuardIsBitmap(dst, false);
            }

            var srcBmp = new _ImageSharpInteropBitmap<TPixel>(src);
            var swapRGB = dstIsBGR ^ GetIsBGR<TPixel>();

            if (dstIsCHW)
            {
                if (_TensorSpanBitmap<TElement, Rgb24>.TryCreatePlanes(dst, out var dstX, out var dstY, out var dstZ))
                {
                    if (swapRGB) { var tmp = dstX; dstX = dstZ; dstZ = tmp; }

                    var numRows = Math.Min(srcBmp.Height, dstHeight);

                    for (int y = 0; y < numRows; y++)
                    {
                        var srcRow = srcBmp.GetReadOnlyRowSpan(y);
                        var dstRowX = System.Runtime.InteropServices.MemoryMarshal.Cast<TElement, byte>(dstX.GetRowSpan(y));
                        var dstRowY = System.Runtime.InteropServices.MemoryMarshal.Cast<TElement, byte>(dstY.GetRowSpan(y));
                        var dstRowZ = System.Runtime.InteropServices.MemoryMarshal.Cast<TElement, byte>(dstZ.GetRowSpan(y));

                        CopyRowToPlanes(srcRow, dstRowX, dstRowY, dstRowZ);
                    }

                    return;
                }

                throw new NotImplementedException();                
            }

            switch (dstChannels)
            {
                case 1: _TensorSpanBitmap<TElement, L8>.Create(dst).CopyFrom(srcBmp, ToImageSharpL8); return;
                case 3 when dstIsBGR: _TensorSpanBitmap<TElement, Bgr24>.Create(dst).CopyFrom(srcBmp, ToImageSharpBgr24); return;
                case 3 when !dstIsBGR: _TensorSpanBitmap<TElement, Rgb24>.Create(dst).CopyFrom(srcBmp, ToImageSharpRgb24); return;
                case 4 when dstIsBGR: _TensorSpanBitmap<TElement, Bgra32>.Create(dst).CopyFrom(srcBmp, ToImageSharpBgra32); return;
                case 4 when !dstIsBGR: _TensorSpanBitmap<TElement, Rgba32>.Create(dst).CopyFrom(srcBmp, ToImageSharpRgba32); return;
                default: throw new ArgumentException($"invalid channels count {dstChannels}", nameof(dst));
            }
        }


        public static void CopyToTensor(this Image src, System.Numerics.Matrix3x2 srcXform, __RWTENSOR dst, bool dstIsBGR = false)
        {
            GuardIsBitmap(dst);

            if (dst is System.Numerics.Tensors.Tensor<float> ft)
            {
                var fts = ft.AsTensorSpan();
                CopyToTensor(src, srcXform, fts, dstIsBGR);
                return;
            }

            throw new ArgumentException($"{dst.GetType().Name} not implemented", nameof(dst));
        }
    }

}
