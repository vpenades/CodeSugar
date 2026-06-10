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


#if NET8_0_OR_GREATER
using __ITENSOR = System.Numerics.Tensors.ITensor;
#endif

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESIXLABORSNAMESPACE
namespace SixLabors.ImageSharp
#else
namespace $rootnamespace$
#endif
{
    internal static partial class CodeSugarForImageSharp
    {
        #if NET8_0_OR_GREATER

        public static bool DangerousTryGetSpanTensor<TPixel>(this Image<TPixel> src, out System.Numerics.Tensors.TensorSpan<TPixel> dst)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            if (!src.DangerousTryGetSinglePixelMemory(out var memory))
            {
                dst = System.Numerics.Tensors.TensorSpan<TPixel>.Empty;                
                return false;
            }

            Span<nint> size = stackalloc nint[2];
            size[0] = src.Height;
            size[1] = src.Width;

            dst = new System.Numerics.Tensors.TensorSpan<TPixel>(memory.Span, size);

            return true;
        }

        public static void CopyToTensor(this Image src, __ITENSOR dst, bool dstIsBGR = false)
        {
            if (dst == null || dst.IsEmpty) throw new ArgumentNullException("null or empty", nameof(dst));
            if (dst.IsReadOnly) throw new ArgumentException("is read only", nameof(dst));
            if (!dst.IsDense) throw new ArgumentException("is not dense", nameof(dst));

            switch(dst)
            {
                // case System.Numerics.Tensors.Tensor<byte> typedTensor: CopyToTensor(src, typedTensor.AsTensorSpan(), dstIsBGR); return;
                case System.Numerics.Tensors.Tensor<float> typedTensor: CopyToTensor(src, typedTensor.AsTensorSpan(), dstIsBGR); return;
                default: throw new ArgumentException($"{dst.GetType().Name} not implemented", nameof(dst));
            }            
        }

        public static void CopyToTensor(this Image src, System.Numerics.Matrix3x2 srcXform, __ITENSOR dst, bool dstIsBGR = false)
        {
            if (dst == null || dst.IsEmpty) throw new ArgumentNullException("null or empty", nameof(dst));
            if (dst.IsReadOnly) throw new ArgumentException("is read only", nameof(dst));
            if (!dst.IsDense) throw new ArgumentException("is not dense", nameof(dst));

            if (dst is System.Numerics.Tensors.Tensor<float> ft)
            {
                var fts = ft.AsTensorSpan();
                CopyToTensor(src, srcXform, fts, dstIsBGR);
                return;
            }

            throw new ArgumentException($"{dst.GetType().Name} not implemented", nameof(dst));
        }

        public static void CopyToSixLaborsImage(this __ITENSOR src, Image dst, bool srcIsBGR = false)
        {
            if (src == null || src.IsEmpty) throw new ArgumentNullException("null or empty", nameof(dst));

            if (!src.IsDense) throw new ArgumentException("is not dense", nameof(dst));

            if (src is System.Numerics.Tensors.Tensor<float> ft)
            {
                var fts = ft.AsTensorSpan();
                CopyToSixLaborsImage(fts, dst, srcIsBGR);
                return;
            }

            throw new ArgumentException($"{dst.GetType().Name} not implemented", nameof(dst));
        }

        public static void SaveToSixLaborsImage(this __ITENSOR tensor, System.IO.FileInfo finfo, bool tensorIsBGR = false)
        {
            SaveToSixLaborsImage(tensor, img => img.Save(finfo.FullName), tensorIsBGR);
        }

        public static void SaveToSixLaborsImage(this __ITENSOR tensor, Action<Image> imageAction, bool tensorIsBGR = false)
        {
            if (tensor == null || tensor.IsEmpty) throw new ArgumentNullException("null or empty", nameof(tensor));
            if (!tensor.IsDense) throw new ArgumentException("is not dense", nameof(tensor));

            switch (tensor)
            {
                case System.Numerics.Tensors.Tensor<float> typedTensor: typedTensor.AsTensorSpan().SaveToSixLaborsImage(imageAction, tensorIsBGR); break;
                default: throw new ArgumentException($"{tensor.GetType().Name} not implemented", nameof(tensor));
            }
        }

        public static Image<TPixel> ToSixLaborsImage<TPixel>(this __ITENSOR tensor, bool tensorIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            if (tensor == null || tensor.IsEmpty) throw new ArgumentNullException("null or empty", nameof(tensor));
            if (!tensor.IsDense) throw new ArgumentException("is not dense", nameof(tensor));

            if (tensor is System.Numerics.Tensors.Tensor<float> ft)
            {
                return ft.AsTensorSpan().ToSixLaborsImage<TPixel>(tensorIsBGR);
            }

            throw new ArgumentException($"{tensor.GetType().Name} not implemented", nameof(tensor));
        }

        public static __ITENSOR ToTensor<TPixel>(this Image img)            
            where TPixel:unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            var pixFactory = default(TPixel);

            __ITENSOR _createByteTensor(int channels, bool dstIsBgr)
            {
                var srcBmp = System.Numerics.Tensors.Tensor.Create(new byte[img.Width * img.Height * channels], new nint[] { img.Height, img.Width, channels });
                img.CopyToTensor(srcBmp.AsTensorSpan(), dstIsBgr);
                return srcBmp;
            }

            __ITENSOR _createFloatTensor(int channels, bool dstIsBgr)
            {
                var srcBmp = System.Numerics.Tensors.Tensor.Create(new float[img.Width * img.Height * channels], new nint[] { img.Height, img.Width, channels });
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

        public static __ITENSOR ToTensor<TElement>(this Image img, int numChannels, bool dstIsBgr)
            where TElement : unmanaged
        {
            if (typeof(TElement) == typeof(Byte))
            {
                var srcBmp = System.Numerics.Tensors.Tensor.Create(new byte[img.Width * img.Height * numChannels], new nint[] { img.Height, img.Width, numChannels });
                img.CopyToTensor(srcBmp.AsTensorSpan(), dstIsBgr);
                return srcBmp;
            }

            if (typeof(TElement) == typeof(Single))
            {
                var srcBmp = System.Numerics.Tensors.Tensor.Create(new float[img.Width * img.Height * numChannels], new nint[] { img.Height, img.Width, numChannels });
                img.CopyToTensor(srcBmp.AsTensorSpan(), dstIsBgr);
                return srcBmp;
            }

            throw new NotImplementedException();
        }

        #endif
    }
}
