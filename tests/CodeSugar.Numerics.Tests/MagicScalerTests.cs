using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PhotoSauce.MagicScaler;

using TUnit;

namespace CodeSugar
{
    internal class MagicScalerTests
    {
        [Test]
        public async Task LoadTest()
        {
            var icon = ResourceInfo.From("CodeSugar.png");

            // magic scaler setup
            var settings = new ProcessImageSettings { Width = 800 };            
            using var pipeline = MagicImageProcessor.BuildPipeline(icon.FilePath, settings);

            var pixels = pipeline.PixelSource!;

            var tensor = new System.Drawing.Size(pixels.Width, pixels.Height).CreateCompatibleTensorHWC<float>(3);

            pixels.CopyPixelsToTensor(tensor);

            AttachmentInfo.From("CodeSugar.ImageSharp.png").WriteObjectEx(x=> tensor.SaveToSixLaborsImage(x,false));
        }
    }
}
