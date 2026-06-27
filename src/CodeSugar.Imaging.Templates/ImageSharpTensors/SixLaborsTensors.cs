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

using SixLabors.ImageSharp.PixelFormats;


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

        
        public static void CopyToTensor(this Image src, __RWTENSOR dst, bool dstIsBGR = false)
        {
            GuardIsBitmap(dst, false);

            switch (dst)
            {
                // case System.Numerics.Tensors.Tensor<byte> typedTensor: CopyToTensor(src, typedTensor.AsTensorSpan(), dstIsBGR); return;
                case System.Numerics.Tensors.Tensor<float> typedTensor: CopyToTensor(src, typedTensor.AsTensorSpan(), dstIsBGR); return;
                default: throw new ArgumentException($"{dst.GetType().Name} not implemented", nameof(dst));
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

        public static __RWTENSOR ToTensor<TPixel>(this Image img)            
            where TPixel:unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            var pixFactory = default(TPixel);

            var size = new System.Drawing.Size(img.Width, img.Height);

            __RWTENSOR _createByteTensor(int channels, bool dstIsBgr)
            {
                var srcBmp = size.CreateTensorBitmapHWC<byte>(channels);
                img.CopyToTensor(srcBmp.AsTensorSpan(), dstIsBgr);
                return srcBmp;
            }

            __RWTENSOR _createFloatTensor(int channels, bool dstIsBgr)
            {
                var srcBmp = size.CreateTensorBitmapHWC<float>(channels);
                img.CopyToTensor(srcBmp.AsTensorSpan(), dstIsBgr);
                return srcBmp;
            }

            switch (pixFactory)
            {
                case __SIXLABORSPIXFMT.Rgb24: return _createByteTensor(3, false);
                case __SIXLABORSPIXFMT.Bgr24: return _createByteTensor(3, true);
                case __SIXLABORSPIXFMT.Rgba32: return _createByteTensor(4, false);
                case __SIXLABORSPIXFMT.Bgra32: return _createByteTensor(4, true);

                case __SIXLABORSPIXFMT.RgbaVector: return _createFloatTensor(4, false);

                default: throw new NotImplementedException();
            }
        }

        public static __RWTENSOR ToTensor<TElement>(this Image img, int numChannels, bool dstIsBgr)
            where TElement : unmanaged
        {
            if (numChannels < 1 || numChannels > 4) throw new ArgumentOutOfRangeException("must be a value between 1 and 4", nameof(numChannels));

            var size = new System.Drawing.Size(img.Width, img.Height);

            if (typeof(TElement) == typeof(Byte))
            {
                var srcBmp = size.CreateTensorBitmapHWC<byte>(numChannels);
                img.CopyToTensor(srcBmp.AsTensorSpan(), dstIsBgr);
                return srcBmp;
            }

            if (typeof(TElement) == typeof(Single))
            {
                var srcBmp = size.CreateTensorBitmapHWC<float>(numChannels);
                img.CopyToTensor(srcBmp.AsTensorSpan(), dstIsBgr);
                return srcBmp;
            }

            throw new NotSupportedException($"{nameof(TElement)} must be {nameof(Byte)} or {nameof(Single)}");
        }        
    }
}
