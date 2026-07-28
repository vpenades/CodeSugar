// GENERATOR_REQUIRES: SixLabors.ImageSharp System.Numerics.Tensors

using System;
using System.Runtime.CompilerServices;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

#nullable disable

using __SIXLABORS = SixLabors.ImageSharp;

using __SIZE = System.Drawing.Size;
using __STREAMFUNC = System.Func<System.IO.Stream>;

using __ROTENSOR = System.Numerics.Tensors.IReadOnlyTensor;
using __ROTENSORSPANF = System.Numerics.Tensors.ReadOnlyTensorSpan<float>;
using __RWTENSORSPANF = System.Numerics.Tensors.TensorSpan<float>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static System.Numerics.Tensors.Tensor<Byte> ImageSharpReadBytesTensor<TPixel>(this __STREAMFUNC streamFunc)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (var s = streamFunc.Invoke())
            {
                var image = SixLabors.ImageSharp.Image.Load<TPixel>(s);
                return ConvertToBytesTensor(image.Frames.RootFrame.PixelBuffer);
            }
        }

        public static System.Numerics.Tensors.Tensor<TPixel> ImageSharpReadTensor<TPixel>(this __STREAMFUNC streamFunc)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (var s = streamFunc.Invoke())
            {
                var image = SixLabors.ImageSharp.Image.Load<TPixel>(s);
                return ConvertToTensor(image.Frames.RootFrame.PixelBuffer);
            }
        }

        public static System.Numerics.Tensors.Tensor<Byte> ConvertToBytesTensor<TPixel>(this __SIXLABORS.Memory.Buffer2D<TPixel> buffer)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            ConvertToRawBitmap(buffer, out var bitmapSize, out var bitmapPixels);

            var lenghts = new nint[3];
            lenghts[0] = bitmapSize.Height;
            lenghts[1] = bitmapSize.Width;
            lenghts[2] = Unsafe.SizeOf<TPixel>();

            return System.Numerics.Tensors.Tensor.Create(bitmapPixels, lenghts);
        }

        public static System.Numerics.Tensors.Tensor<TPixel> ConvertToTensor<TPixel>(this __SIXLABORS.Memory.Buffer2D<TPixel> buffer)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var (raws,rawp) = ConvertToRawBitmap(buffer);

            var lenghts = new nint[2];
            lenghts[0] = raws.Height;
            lenghts[1] = raws.Width;

            return System.Numerics.Tensors.Tensor.Create(rawp, lenghts);
        }

        public static void ImageSharpSaveTo(this __ROTENSOR tensor, System.IO.FileInfo finfo, bool tensorIsBGR = false)
        {
            ImageSharpAction(tensor, img => img.Save(finfo.FullName), tensorIsBGR);
        }        
    }
}
