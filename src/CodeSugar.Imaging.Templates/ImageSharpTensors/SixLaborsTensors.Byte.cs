using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics.Tensors;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

#nullable disable

using __SIXLABORS = SixLabors.ImageSharp;
using __SIXLABORSPIXFMT = SixLabors.ImageSharp.PixelFormats;

using __RWTENSORSPANB = System.Numerics.Tensors.TensorSpan<byte>;
using __ROTENSORSPANB = System.Numerics.Tensors.ReadOnlyTensorSpan<byte>;

using System.Threading.Channels;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static Tensor<byte> ToByteTensor<TPixel>(this Image<TPixel> img)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            var size = new System.Drawing.Size(img.Width, img.Height);
            var chns = GetChannelsCount<TPixel>();
            var isBgr = GetIsBGR<TPixel>();

            var srcBmp = size.CreateTensorBitmapHWC<byte>(chns);
            img.CopyToTensor(srcBmp.AsTensorSpan(), isBgr);
            return srcBmp;
        }
    }

}
