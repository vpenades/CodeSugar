using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.InteropServices;
using System.Text;
using System.Buffers;
using System.Reflection;
using System.Transactions;
using System.Threading;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        public static System.Numerics.Tensors.Tensor<Byte> ReadTensorRgbFrom(this Func<System.IO.Stream> stream)
        {
            using(var s = stream.Invoke())
            {
                return ReadTensorRgbFrom(s);
            }
        }

        public static System.Numerics.Tensors.Tensor<Byte> ReadTensorRgbFrom(this System.IO.Stream stream)
        {
            #if __REFERENCES_SIXLABORSIMAGESHARP
            using (var imageSharpImage = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(stream))
            {
                return imageSharpImage.ToByteTensor();
            }
            #endif

            #if __REFERENCES_SKIASHARP
            using (var skImage = SkiaSharp.SKBitmap.Decode(stream))
            {
                return skImage.ToRgbTensor();
            }
            #endif

            #if __REFERENCES_PHOTOSAUCEMAGICSCALER
            var psmssettings = new PhotoSauce.MagicScaler.ProcessImageSettings();
            return MagicScalerReadTensor<byte>(stream, psmssettings, out var fmt);
            #endif

            #if __REFERENCES_AVALONIA
            using (var avlImage = new Avalonia.Media.Imaging.Bitmap(stream))
            {
                return avlImage.ToTensorRgb24();
            }
            #endif


            throw new NotImplementedException("No imaging service found");
        }


    }
}
