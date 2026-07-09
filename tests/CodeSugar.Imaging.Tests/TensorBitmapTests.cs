using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using InteropTypes.Numerics;
using InteropTypes.TensorBitmaps;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using SkiaSharp;

using TUnit;

namespace CodeSugar
{
    internal class TensorBitmapTests
    {
        [Test]
        public async Task ImageSharpRoundtripTest()
        {
            using var img = Image.Load<Rgb24>(ResourceInfo.From("shannon.jpg"));

            var tbmp = img.ToResizedTensorBitmap<Rgb24, byte, Rgb24>(256,192);

            var crop = tbmp.GetCropped(new System.Drawing.Rectangle(5, 5, 16, 16));

            var fillColor = new Rgb24(0, 255, 0);
            for (int i = 0; i < crop.Height; i++)
            {
                var row = crop.GetRowPixelsSpan(i);
                row.Fill(fillColor);
            }

            using var img2 = tbmp.ToImageSharp();

            AttachmentInfo.From("shannon.modified.jpg").WriteObject(img2.Save);
        }

        [Test]
        public async Task SkiaSharpRoundtripTest()
        {
            // https://github.com/mono/SkiaSharp/issues/2607#issuecomment-1812437697
            if (OperatingSystem.IsLinux()) return;

            using var img = SkiaSharp.SKBitmap.Decode(ResourceInfo.From("shannon.jpg"));

            var tbmp = img.ToResizedTensorBitmap<byte, Rgb24ForTesting>(256, 192);

            var crop = tbmp.GetCropped(new System.Drawing.Rectangle(5, 5, 16, 16));

            var fillColor = new Rgb24ForTesting(0, 255, 0);
            for (int i = 0; i < crop.Height; i++)
            {
                var row = crop.GetRowPixelsSpan(i);
                row.Fill(fillColor);
            }

            using var img2 = tbmp.ToSkiaSharp();            

            AttachmentInfo.From("shannon.modified.jpg").WriteObjectEx(x => x.WriteSkiaSharpBitmap(img2, SKEncodedImageFormat.Jpeg)  );
        }


        [Test]
        public void TestTensorsFittingImageSharp()
        {
            var icon = ResourceInfo.From("CodeSugar.png");

            using var img = SixLabors.ImageSharp.Image.Load<Rgba32>(icon.FilePath);

            static TensorSpanBitmap<float, Vector3> bitmapRequest(int width, int height)
            {
                return TensorBitmap<float, Vector3>.Create(100, 300, KnownPixelFormats.RgbF32);
            }            

            void _bitmapWrite(System.IO.Stream s, float overflowAmount)
            {
                var result = img.FitPixelsTo(bitmapRequest, overflowAmount);

                result.AsReadOnlyTensorSpanBitmap().WritePngToStream(s);                
            }            

            AttachmentInfo.From("bitmap.min.png").WriteToStream(s => _bitmapWrite(s, 0));
            AttachmentInfo.From("bitmap.max.png").WriteToStream(s => _bitmapWrite(s, 1));

            static TensorSpanPlanes3<float> planesRequest(int width, int height)
            {
                return TensorSpanPlanes3<float>.Create(100, 300, KnownPixelFormats.RgbF32);
            }

            void _planesWrite(System.IO.Stream s, float overflowAmount)
            {
                var result = img.FitPixelsTo(planesRequest, overflowAmount);

                var tmp = TensorBitmap<byte, Rgb24>.Create(result.Width, result.Height, KnownPixelFormats.Rgb8);
                result.CopyPixelsTo(tmp);

                tmp.AsReadOnlyTensorSpanBitmap().WritePngToStream(s);
            }

            AttachmentInfo.From("planes.min.png").WriteToStream(s => _planesWrite(s, 0));
            AttachmentInfo.From("planes.max.png").WriteToStream(s => _planesWrite(s, 1));

        }
    }
}
