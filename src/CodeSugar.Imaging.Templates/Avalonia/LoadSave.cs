using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

#nullable disable

using __SIZE = System.Drawing.Size;
using __STREAMFUNC = System.Func<System.IO.Stream>;

using __ROTENSOR = System.Numerics.Tensors.IReadOnlyTensor;
using __ROTENSORSPANF = System.Numerics.Tensors.ReadOnlyTensorSpan<float>;
using __RWTENSORSPANF = System.Numerics.Tensors.TensorSpan<float>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static Avalonia.Media.Imaging.Bitmap ReadAvaloniaBitmap(this __STREAMFUNC sf)
        {
            using(var s = sf.Invoke())
            {
                return new Avalonia.Media.Imaging.Bitmap(s);
            }
        }        
    }
}
