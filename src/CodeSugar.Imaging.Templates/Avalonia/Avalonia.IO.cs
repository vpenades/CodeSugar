// GENERATOR_REQUIRES: Avalonia

#nullable disable

using __SIZE = System.Drawing.Size;
using __STREAMFUNC = System.Func<System.IO.Stream>;

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
