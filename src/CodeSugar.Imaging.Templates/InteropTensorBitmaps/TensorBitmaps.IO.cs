using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#if __REFERENCES_SIXLABORSIMAGESHARP
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
#endif

using __KNOWNFMTS = InteropTypes.Numerics.KnownPixelFormats;

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
            if (!(typeof(TPixel),Unsafe.SizeOf<TPixel>()).TryInferTensorBitmapPixelFormat(out var fmt))
            {
                throw new InvalidOperationException("could not infer format from pixel");
            }

            #if __REFERENCES_SIXLABORSIMAGESHARP

            static InteropTypes.TensorBitmaps.TensorBitmap<byte, TPixel> _LoadImageSharp<TPixelIn>(System.IO.Stream stream, InteropTypes.Numerics.PixelFormat dstFmt)
                where TPixelIn:unmanaged, IPixel<TPixelIn>
            {
                using (var imageSharpImage = Image.Load<TPixelIn>(stream))
                {
                    return imageSharpImage.ToTensorBitmap<TPixelIn, byte, TPixel>(dstFmt);
                }
            }

            if (fmt == __KNOWNFMTS.Luminance8) return _LoadImageSharp<L8>(stream, __KNOWNFMTS.Luminance8);
            if (fmt == __KNOWNFMTS.Alpha8) return _LoadImageSharp<A8>(stream, __KNOWNFMTS.Alpha8);
            if (fmt == __KNOWNFMTS.Rgb8) return _LoadImageSharp<Rgb24>(stream, __KNOWNFMTS.Rgb8);
            if (fmt == __KNOWNFMTS.Bgr8) return _LoadImageSharp<Bgr24>(stream, __KNOWNFMTS.Bgr8);
            if (fmt == __KNOWNFMTS.Rgba8) return _LoadImageSharp<Rgba32>(stream, __KNOWNFMTS.Rgba8);
            if (fmt == __KNOWNFMTS.Bgra8) return _LoadImageSharp<Bgra32>(stream, __KNOWNFMTS.Bgra8);
            if (fmt == __KNOWNFMTS.Argb8) return _LoadImageSharp<Argb32>(stream, __KNOWNFMTS.Bgra8);
            if (fmt == __KNOWNFMTS.Abgr8) return _LoadImageSharp<Abgr32>(stream, __KNOWNFMTS.Abgr8);            

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
            if (!(typeof(TPixel), Unsafe.SizeOf<TPixel>()).TryInferTensorBitmapPixelFormat(out var fmt))
            {
                throw new InvalidOperationException("could not infer format from pixel");
            }

            #if __REFERENCES_SIXLABORSIMAGESHARP

            static InteropTypes.TensorBitmaps.TensorBitmap<float, TPixel> _LoadImageSharp<TPixelIn>(System.IO.Stream stream, InteropTypes.Numerics.PixelFormat dstFmt)
                where TPixelIn : unmanaged, IPixel<TPixelIn>
            {
                using (var imageSharpImage = Image.Load<TPixelIn>(stream))
                {
                    return imageSharpImage.ToTensorBitmap<TPixelIn, float, TPixel>(dstFmt);
                }
            }            

            if (fmt == __KNOWNFMTS.RgbF32) return _LoadImageSharp<Rgb24>(stream, __KNOWNFMTS.RgbF32);
            if (fmt == __KNOWNFMTS.RgbaF32) return _LoadImageSharp<Rgba32>(stream, __KNOWNFMTS.RgbaF32);

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

            if (bitmap.Format == __KNOWNFMTS.Alpha8)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.A8>();
                WriteToImageSharpStream(typedBitmap, stream);
                return;
            }

            if (bitmap.Format == __KNOWNFMTS.Luminance8)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.L8>();
                WriteToImageSharpStream(typedBitmap, stream);
                return;
            }

            if (bitmap.Format == __KNOWNFMTS.Rgb8)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.Rgb24>();
                WriteToImageSharpStream(typedBitmap, stream);
                return;
            }

            if (bitmap.Format == __KNOWNFMTS.Bgr8)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.Bgr24>();
                WriteToImageSharpStream(typedBitmap, stream);
                return;
            }

            if (bitmap.Format == __KNOWNFMTS.Rgba8)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.Rgba32>();
                WriteToImageSharpStream(typedBitmap, stream);
                return;
            }

            if (bitmap.Format == __KNOWNFMTS.Bgra8)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.Bgra32>();
                WriteToImageSharpStream(typedBitmap, stream);
                return;
            }

            if (bitmap.Format == __KNOWNFMTS.Argb8)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.Argb32>();
                WriteToImageSharpStream(typedBitmap, stream);
                return;
            }

            if (bitmap.Format == __KNOWNFMTS.RgbaF32)
            {
                var typedBitmap = bitmap.Cast<SixLabors.ImageSharp.PixelFormats.RgbaVector>();
                WriteToImageSharpStream(typedBitmap, stream);
                return;
            }

            if (bitmap.Format == __KNOWNFMTS.RgbF32 || bitmap.Format == __KNOWNFMTS.BgrF32)
            {
                var typedBitmap = bitmap.ConvertTo<TElement,TPixel,byte,Rgb24>(__KNOWNFMTS.Rgb8);
                WriteToImageSharpStream(typedBitmap.AsReadOnlyTensorSpanBitmap(), stream);
                return;
            }

            #endif

            throw new NotImplementedException("unsupported format");
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
