using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        private interface __IReadOnlyInternalBitmap
        {
            public int Width { get; }
            public int Height { get; }
        }

        private interface __IReadOnlyInternalBitmap<TPixel> : __IReadOnlyInternalBitmap
        {
            public ReadOnlySpan<TPixel> GetReadOnlyRowSpan(int y);

            public void CopyTo<TPixelOut>(__IInternalBitmap<TPixelOut> dst, Func<TPixel, TPixelOut> pixelConverter)
            {
                var rowsCount = Math.Min(dst.Height, Height);

                for (int y = 0; y < rowsCount; y++)
                {
                    var srcRow = GetReadOnlyRowSpan(y);
                    var dstRow = dst.GetRowSpan(y);
                    int rowLen = Math.Min(srcRow.Length, dstRow.Length);

                    for (int i = 0; i < rowLen; ++i)
                    {
                        dstRow[i] = pixelConverter(srcRow[i]);
                    }
                }
            }
        }

        private interface __IInternalBitmap<TPixel> : __IReadOnlyInternalBitmap<TPixel>
        {
            public Span<TPixel> GetRowSpan(int y);           

            public void CopyFrom<TPixelIn>(__IReadOnlyInternalBitmap<TPixelIn> src, Func<TPixelIn, TPixel> pixelConverter)
            {
                var rowsCount = Math.Min(src.Height, Height);

                for (int y = 0; y < rowsCount; y++)
                {
                    var srcRow = src.GetReadOnlyRowSpan(y);
                    var dstRow = GetRowSpan(y);
                    int rowLen = Math.Min(srcRow.Length, dstRow.Length);

                    for (int i = 0; i < rowLen; ++i)
                    {
                        dstRow[i] = pixelConverter(srcRow[i]);
                    }
                }
            }
        }
    }
}
