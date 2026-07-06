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

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

#nullable disable

using __SIZE = System.Drawing.Size;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static void ConvertToRawBitmap(this Avalonia.Media.Imaging.Bitmap bitmap, out __SIZE bitmapSize, out int bpp, out byte[] bitmapPixels)
        {
            var pw = bitmap.PixelSize.Width;
            var ph = bitmap.PixelSize.Height;

            bitmapSize = new __SIZE(pw, ph);

            var avrect = new Avalonia.PixelRect(0, 0, pw, ph);

            bpp = bitmap.Format.Value.BitsPerPixel / 8;

            bitmapPixels = new byte[bpp * pw * ph];

            bitmap.CopyPixels(bitmapPixels, bpp * pw);
        }

        public static void CopyPixels(this Avalonia.Media.Imaging.Bitmap src, Byte[] dst, int stride)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (dst == null) throw new ArgumentNullException(nameof(dst));            

            var r = new Avalonia.PixelRect(0, 0, src.PixelSize.Width, src.PixelSize.Height);

            if (stride < r.Width * src.Format.Value.BitsPerPixel / 8) throw new ArgumentOutOfRangeException(nameof(stride));

            var h = GCHandle.Alloc(dst, GCHandleType.Pinned);            

            try
            {
                src.CopyPixels(r, h.AddrOfPinnedObject(), dst.Length, stride);
            }
            finally
            {
                h.Free();
            }
        }
    }
}
