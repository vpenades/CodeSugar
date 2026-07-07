using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using SkiaSharp;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        private readonly struct _SkiaSharpInternalBitmap<TPixel> : __IInternalBitmap<TPixel>
            where TPixel : unmanaged
        {
            public _SkiaSharpInternalBitmap(SKBitmap image)
            {
                if (image == null) throw new ArgumentNullException(nameof(image));

                if (Unsafe.SizeOf<TPixel>() != _Image.BytesPerPixel)
                {
                    throw new InvalidOperationException($"image has {_Image.BytesPerPixel} bytes per pixel, but TPixel is {Unsafe.SizeOf<TPixel>()}");
                }               

                _Image = image;
            }

            private readonly SKBitmap _Image;

            public ReadOnlySpan<TPixel> GetReadOnlyRowSpan(int y)
            {
                return GetRowSpan(y);
            }

            public Span<TPixel> GetRowSpan(int y)
            {
                if (y < 0 || y >= _Image.Height) throw new ArgumentOutOfRangeException(nameof(y));
                if (_Image.IsImmutable) throw new InvalidOperationException("SKBitmap is immutable");

                var pixels = _Image.GetPixelSpan();
                pixels = pixels.Slice(_Image.RowBytes * y);

                return System.Runtime.InteropServices.MemoryMarshal.Cast<byte, TPixel>(pixels).Slice(_Image.Width);
            }            

            public int Width => _Image.Width;

            public int Height => _Image.Height;
        }
    }
}
