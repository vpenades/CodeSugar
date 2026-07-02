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
            if (!OperatingSystem.IsWindows()) return; // needs codecs configuration

            var icon = ResourceInfo.From("CodeSugar.png");

            // magic scaler setup
            var settings = new ProcessImageSettings { Width = 800 };            

            var tensor = icon.File.GetReadStreamFunction().MagicScalerReadTensor<float>(settings, out _);            

            AttachmentInfo.From("CodeSugar.ImageSharp.png").WriteObjectEx(x=> tensor.ImageSharpSaveTo(x,true));
        }
    }
}
