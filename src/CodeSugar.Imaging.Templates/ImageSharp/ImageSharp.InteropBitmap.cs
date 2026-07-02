using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Numerics.Tensors;
using System.Text;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        private static void CopyRowToPlanes<TPixel>(ReadOnlySpan<TPixel> src, Span<float> dstR, Span<float> dstG, Span<float> dstB)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            System.Diagnostics.Debug.Assert(dstR.Length == dstG.Length, "plane lengths mismatch");
            System.Diagnostics.Debug.Assert(dstR.Length == dstB.Length, "plane lengths mismatch");

            var w = Math.Min(src.Length, dstR.Length);

            for (int i = 0; i < src.Length; ++i)
            {
                var pix = src[i].ToScaledVector4();
                dstR[i] = pix.X;
                dstG[i] = pix.Y;
                dstB[i] = pix.Z;
            }
        }

        private static void CopyRowToPlanes<TPixel>(ReadOnlySpan<TPixel> src, Span<byte> dstR, Span<byte> dstG, Span<byte> dstB)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            System.Diagnostics.Debug.Assert(dstR.Length == dstG.Length, "plane lengths mismatch");
            System.Diagnostics.Debug.Assert(dstR.Length == dstB.Length, "plane lengths mismatch");

            var w = Math.Min(src.Length, dstR.Length);

            Rgba32 pix = default;

            for (int i = 0; i < src.Length; ++i)
            {
                src[i].ToRgba32(ref pix);
                dstR[i] = pix.R;
                dstG[i] = pix.G;
                dstB[i] = pix.B;
            }
        }


        private readonly struct _ImageSharpInteropBitmap<TPixel> : __IInteropBitmap<TPixel>
            where TPixel : unmanaged, IPixel<TPixel>
        {
            public _ImageSharpInteropBitmap(Image<TPixel> image)
            {
                _Image = image;
            }

            private readonly Image<TPixel> _Image;

            public ReadOnlySpan<TPixel> GetReadOnlyRowSpan(int y)
            {
                return GetRowSpan(y);
            }

            public Span<TPixel> GetRowSpan(int y)
            {
                if (y < 0 || y >= _Image.Height) throw new ArgumentOutOfRangeException(nameof(y));
                return _Image.Frames.RootFrame.DangerousGetPixelRowMemory<TPixel>(y).Span;
            }            

            public int Width => _Image.Width;

            public int Height => _Image.Height;
        }
    }
}
