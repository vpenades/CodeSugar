using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#if __REFERENCES_SIXLABORSIMAGESHARP
using SixLabors.ImageSharp;
#endif

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        public static InteropTypes.TensorBitmaps.TensorBitmap<byte, TPixel> ReadByteTensorBitmapFrom<TPixel>(this Func<System.IO.Stream> stream)
            where TPixel : unmanaged
        {
            using(var s = stream.Invoke()) { return ReadByteTensorBitmapFrom<TPixel>(s); }
        }

        public static InteropTypes.TensorBitmaps.TensorBitmap<float, TPixel> ReadFloatTensorBitmapFrom<TPixel>(this Func<System.IO.Stream> stream)
            where TPixel : unmanaged
        {
            using (var s = stream.Invoke()) { return ReadFloatTensorBitmapFrom<TPixel>(s); }
        }

        public static void WritePngToStream<TElement, TPixel>(this InteropTypes.TensorBitmaps.ReadOnlyTensorSpanBitmap<TElement, TPixel> bitmap, Func<System.IO.Stream> stream)
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged
        {
            using (var s = stream.Invoke()) { WritePngToStream(bitmap, s); }
        }

        public static InteropTypes.TensorBitmaps.TensorBitmap<byte, TPixel> ReadByteTensorBitmapFrom<TPixel>(this System.IO.Stream stream)
            where TPixel:unmanaged
        {
            var pixelSize = Unsafe.SizeOf<TPixel>();

            #if __REFERENCES_SIXLABORSIMAGESHARP

            if (pixelSize == 1) // as Luminance
            {
                using (var imageSharpImage = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.L8>(stream))
                {
                    return imageSharpImage.ToTensorBitmap<byte, SixLabors.ImageSharp.PixelFormats.L8>().Cast<TPixel>();
                }
            }

            if (pixelSize == 3) // as RGB
            {
                using (var imageSharpImage = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(stream))
                {
                    var bitmap = imageSharpImage.ToTensorBitmap<byte, SixLabors.ImageSharp.PixelFormats.Rgb24>().Cast<TPixel>();
                }
            }

            if (pixelSize == 4) // as RGBA
            {
                using (var imageSharpImage = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(stream))
                {
                    var bitmap = imageSharpImage.ToTensorBitmap<byte, SixLabors.ImageSharp.PixelFormats.Rgba32>().Cast<TPixel>();
                }
            }

            #endif

            #if __REFERENCES_SKIASHARP

            using (var skImage = SkiaSharp.SKBitmap.Decode(stream))
            {                
                return skImage.ToTensorBitmap<byte,TPixel>();
            }

            #endif

            throw new NotImplementedException("No imaging service found");
        }

        public static InteropTypes.TensorBitmaps.TensorBitmap<float, TPixel> ReadFloatTensorBitmapFrom<TPixel>(this System.IO.Stream stream)
            where TPixel:unmanaged
        {
            var pixelSize = Unsafe.SizeOf<TPixel>();

            #if __REFERENCES_SIXLABORSIMAGESHARP            

            if (pixelSize == 16) // as RGBA
            {
                using (var imageSharpImage = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.RgbaVector>(stream))
                {
                    var bitmap = imageSharpImage.ToTensorBitmap<float, SixLabors.ImageSharp.PixelFormats.RgbaVector>();

                    return bitmap.Cast<TPixel>();
                }
            }            

            #endif

            #if __REFERENCES_SKIASHARP

            using (var skImage = SkiaSharp.SKBitmap.Decode(stream))
            {                
                return skImage.ToTensorBitmap<float,TPixel>();
            }

            #endif

            throw new NotImplementedException("No imaging service found");
        }

        public static void WritePngToStream<TElement, TPixel>(this InteropTypes.TensorBitmaps.ReadOnlyTensorSpanBitmap<TElement, TPixel> bitmap, System.IO.Stream stream)
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged
        {
            #if __REFERENCES_SIXLABORSIMAGESHARP

            if (bitmap.Format == InteropTypes.TensorBitmaps.KnownPixelFormats.Alpha8)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.A8>();
                WriteToImageSharpStream(typedBitmap, stream);
            }

            if (bitmap.Format == InteropTypes.TensorBitmaps.KnownPixelFormats.Luminance8)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.L8>();
                WriteToImageSharpStream(typedBitmap, stream);
            }

            if (bitmap.Format == InteropTypes.TensorBitmaps.KnownPixelFormats.Rgb888)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.Rgb24>();
                WriteToImageSharpStream(typedBitmap, stream);
            }

            if (bitmap.Format == InteropTypes.TensorBitmaps.KnownPixelFormats.Bgr888)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.Bgr24>();
                WriteToImageSharpStream(typedBitmap, stream);
            }

            if (bitmap.Format == InteropTypes.TensorBitmaps.KnownPixelFormats.Rgba8888)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.Rgba32>();
                WriteToImageSharpStream(typedBitmap, stream);
            }

            if (bitmap.Format == InteropTypes.TensorBitmaps.KnownPixelFormats.Bgra8888)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.Bgra32>();
                WriteToImageSharpStream(typedBitmap, stream);
            }

            if (bitmap.Format == InteropTypes.TensorBitmaps.KnownPixelFormats.Argb8888)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.Argb32>();
                WriteToImageSharpStream(typedBitmap, stream);
            }

            if (bitmap.Format == InteropTypes.TensorBitmaps.KnownPixelFormats.RgbaF32)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.RgbaVector>();
                WriteToImageSharpStream(typedBitmap, stream);
            }            

            #endif
        }

        #if __REFERENCES_SIXLABORSIMAGESHARP
        public static void WriteToImageSharpStream<TElement,TPixel>(this InteropTypes.TensorBitmaps.ReadOnlyTensorSpanBitmap<TElement,TPixel> bitmap, System.IO.Stream stream, SixLabors.ImageSharp.Formats.IImageEncoder encoder = null)
            where TElement: unmanaged, INumber<TElement>
            where TPixel: unmanaged, SixLabors.ImageSharp.PixelFormats.IPixel<TPixel>
        {
            using var image = bitmap.ToImageSharp();

            if (encoder == null) image.SaveAsPng(stream);
            else image.Save(stream, encoder);            
        }

        #endif
    }
}
