using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var tbmp = img.ToResizedTensorBitmap<byte, Rgb24>(256,192);

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
    }
}
