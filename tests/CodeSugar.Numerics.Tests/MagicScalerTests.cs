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

            var tensor = icon.File.GetReadStreamFunction().MagicScalerReadTensor<float>(settings, out _);            

            AttachmentInfo.From("CodeSugar.ImageSharp.png").WriteObjectEx(x=> tensor.ImageSharpSaveTo(x,true));
        }
    }
}
