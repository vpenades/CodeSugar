using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        public static void ImageSharpReadRawBitmap<TPixel>(this __STREAMFUNC streamFunc, out __SIZE bitmapSize, out byte[] bitmapPixels)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (var s = streamFunc.Invoke())
            {
                var image = SixLabors.ImageSharp.Image.Load<TPixel>(s);
                ConvertToRawBitmap(image.Frames.RootFrame.PixelBuffer, out bitmapSize, out bitmapPixels);
            }
        }

        public static (__SIZE size, TPixel[] pixels) ImageSharpReadRawBitmap<TPixel>(this __STREAMFUNC streamFunc)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (var s = streamFunc.Invoke())
            {
                var image = SixLabors.ImageSharp.Image.Load<TPixel>(s);
                return ConvertToRawBitmap(image.Frames.RootFrame.PixelBuffer);
            }
        }

        public static void ConvertToRawBitmap<TPixel>(this __SIXLABORS.Memory.Buffer2D<TPixel> buffer, out __SIZE bitmapSize, out byte[] bitmapPixels)
            where TPixel: unmanaged, IPixel<TPixel>
        {
            var rowLen = buffer.Width * Unsafe.SizeOf<TPixel>();

            bitmapPixels = new byte[buffer.Height * rowLen];
            bitmapSize = new __SIZE(buffer.Width, buffer.Height);

            for (int y = 0; y < buffer.Height; ++y)
            {
                var srcRow = System.Runtime.InteropServices.MemoryMarshal.Cast<TPixel, byte>(buffer.DangerousGetRowSpan(y));
                var dstRow = bitmapPixels.AsSpan(y * rowLen, rowLen);
                srcRow.CopyTo(dstRow);
            }
        }

        public static (__SIZE size, TPixel[] pixels) ConvertToRawBitmap<TPixel>(this __SIXLABORS.Memory.Buffer2D<TPixel> buffer)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var bitmapPixels = new TPixel[buffer.Height * buffer.Width];
            var bitmapSize = new __SIZE(buffer.Width, buffer.Height);

            for (int y = 0; y < buffer.Height; ++y)
            {
                var srcRow = buffer.DangerousGetRowSpan(y);
                var dstRow = bitmapPixels.AsSpan(y * buffer.Width);
                srcRow.CopyTo(dstRow);
            }

            return (bitmapSize, bitmapPixels);
        }

        #if __REFERENCES_SYSTEMNUMERICSTENSORS

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

        #endif
    }
}
