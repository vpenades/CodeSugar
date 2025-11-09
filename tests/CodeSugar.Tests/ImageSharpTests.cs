using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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



        #if NET8_0_OR_GREATER

        [Test]
        public void TestTensorsInterop()
        {
            var icon = ResourceInfo.From("CodeSugar.png");

            using var img = SixLabors.ImageSharp.Image.Load<Rgba32>(icon.FilePath);

            var hwcTensor = System.Numerics.Tensors.Tensor.Create<float>(new float[img.Width * img.Height * 3], new nint[] { img.Height, img.Width, 3 });
            var chwTensor = System.Numerics.Tensors.Tensor.Create<float>(new float[img.Width * img.Height * 3], new nint[] { 3, img.Height, img.Width });

            img.CopyToTensor(hwcTensor);
            img.CopyToTensor(chwTensor);
            
            AttachmentInfo.From("hwcTensor.png").WriteObjectEx(f => hwcTensor.SaveToSixLaborsImage(f));
            AttachmentInfo.From("chwTensor.png").WriteObjectEx(f => chwTensor.SaveToSixLaborsImage(f));
        }

        [Test]
        public void TestTensorsAffineTransformInterop()
        {
            var icon = ResourceInfo.From("CodeSugar.png");

            using var img = SixLabors.ImageSharp.Image.Load<Rgba32>(icon.FilePath);

            int dstw = 64;
            int dsth = 128;
            var hwcTensor = System.Numerics.Tensors.Tensor.Create<float>(new float[dstw * dsth * 3], new nint[] { dsth, dstw, 3 });
            var chwTensor = System.Numerics.Tensors.Tensor.Create<float>(new float[dstw * dsth * 3], new nint[] { 3, dsth, dstw });

            var xform = Matrix3x2.CreateScale((float)dstw / img.Width, (float)dsth / img.Height);

            img.CopyToTensor(hwcTensor, xform);
            img.CopyToTensor(chwTensor, xform);

            AttachmentInfo.From("hwcTensor.png").WriteObjectEx(f => hwcTensor.SaveToSixLaborsImage(f));
            AttachmentInfo.From("chwTensor.png").WriteObjectEx(f => chwTensor.SaveToSixLaborsImage(f));
        }

        #endif
    }
}
