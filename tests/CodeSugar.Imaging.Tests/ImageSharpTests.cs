using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using TUnit;

namespace CodeSugar
{
    internal class ImageSharpTests
    {
        
        [Test]
        public void TestDrawAffineTransform()
        {
            var iconPath = ResourceInfo.From("CodeSugar.png");
            using var icon = SixLabors.ImageSharp.Image.Load<Rgba32>(iconPath.FilePath);

            using var target = new SixLabors.ImageSharp.Image<Rgb24>(512,512);

            target.Mutate(dc => dc.Fill(Color.Red));

            target.Mutate(dc => dc.DrawImage(icon, Matrix3x2.CreateTranslation(-50, -50)));

            var xform
                = Matrix3x2.CreateScale(2,1)
                * Matrix3x2.CreateRotation(0.2f)
                * Matrix3x2.CreateTranslation(5,5);            

            target.Mutate(dc => dc.DrawImage(icon, xform));

            AttachmentInfo.From("affineTransform1.png").WriteObject(target.SaveAsPng);
        }

        [Test]
        public async Task TestTensorsAlpha()
        {
            var hwcTensor = new System.Drawing.Size(256, 256).CreateTensorBitmapHWC<float>(1);

            using var image = hwcTensor.ToImageSharp<A8>();
            await Assert.That(image.Width).IsEqualTo(256);
            await Assert.That(image.Height).IsEqualTo(256);

        }

        [Test]
        public void TestTensorsInterop()
        {
            var icon = ResourceInfo.From("CodeSugar.png");

            using var img = SixLabors.ImageSharp.Image.Load<Rgba32>(icon.FilePath);

            var size = new System.Drawing.Size(img.Width, img.Height);

            var hwcTensor = size.CreateTensorBitmapHWC<float>(3);
            var chwTensor = size.CreateTensorBitmapCHW<float>(3);

            img.CopyToTensor<Rgba32, float>(hwcTensor);
            img.CopyToTensor<Rgba32, float>(chwTensor);            

            AttachmentInfo.From("hwcTensor.png").WriteObjectEx(f => hwcTensor.ImageSharpSaveTo(f));
            AttachmentInfo.From("chwTensor.png").WriteObjectEx(f => chwTensor.ImageSharpSaveTo(f));

            hwcTensor.ApplyMultiplyAddToPixelElements(System.Numerics.Vector3.One *2, -System.Numerics.Vector3.One);
            AttachmentInfo.From("hwcTensor_scaled.png").WriteObjectEx(f => hwcTensor.ImageSharpSaveTo(f));

            chwTensor.ApplyMultiplyAddToPixelElements(System.Numerics.Vector3.One *2, -System.Numerics.Vector3.One);
            AttachmentInfo.From("chwTensor_scaled.png").WriteObjectEx(f => chwTensor.ImageSharpSaveTo(f));
        }        
    }
}