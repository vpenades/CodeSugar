using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PhotoSauce.MagicScaler;

using SkiaSharp;

using TUnit;

namespace CodeSugar
{
    internal class SkiaSharpTests
    {
        [Test]
        public async Task LoadTest()
        {
            var icon = ResourceInfo.From("CodeSugar.png");

            using var srcBmp = SKBitmap.Decode(icon.FilePath);            

            var tensor = new System.Drawing.Size(srcBmp.Width, srcBmp.Height).CreateCompatibleTensorHWC<float>(3);

            srcBmp.CopyPixelsToTensor(tensor, false);

            AttachmentInfo.From("CodeSugar.ImageSharp.png").WriteObjectEx(x => tensor.SaveToSixLaborsImage(x, false));
        }
    }
}
