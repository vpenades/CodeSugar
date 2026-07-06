using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

#nullable disable

#if __REFERENCES_SYSTEMNUMERICSTENSORS

using __SIZE = System.Drawing.Size;

namespace __CODESUGAR_ROOTNAMESPACE__
{


    partial class CodeSugarImagingExtensions
    {
        public static System.Numerics.Tensors.Tensor<byte> ToTensorRgb24(this Avalonia.Media.Imaging.Bitmap srcBitmap)
        {
            ConvertToRawBitmap(srcBitmap, out var size, out var bpp, out var pixels);

            var hwc = new nint[3];
            hwc[0] = size.Height;
            hwc[1] = size.Width;
            hwc[2] = bpp;

            var tensor = System.Numerics.Tensors.Tensor.Create<byte>(pixels, hwc);

            return tensor;
        }
    }    
}

#endif
