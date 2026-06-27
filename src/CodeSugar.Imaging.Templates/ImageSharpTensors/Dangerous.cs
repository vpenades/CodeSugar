using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

#nullable disable

using __SIXLABORS = SixLabors.ImageSharp;
using __SIXLABORSPIXFMT = SixLabors.ImageSharp.PixelFormats;

using __RWTENSORSPANB = System.Numerics.Tensors.TensorSpan<byte>;
using __ROTENSORSPANB = System.Numerics.Tensors.ReadOnlyTensorSpan<byte>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static bool DangerousTryGetDenseBytesTensor<TPixel>(this Image<TPixel> src, out __RWTENSORSPANB dst)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return DangerousTryGetDensePixelsTensor(src, out dst);
        }

        public static bool DangerousTryGetDensePixelsTensor<TPixel>(this Image<TPixel> src, out System.Numerics.Tensors.TensorSpan<TPixel> dst)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return DangerousTryGetDensePixelsTensor<TPixel, TPixel>(src, out dst);
        }

        public static bool DangerousTryGetDensePixelsTensor<TPixelIn,TPixelOut>(this Image<TPixelIn> src, out System.Numerics.Tensors.TensorSpan<TPixelOut> dst)
            where TPixelIn : unmanaged, IPixel<TPixelIn>
            where TPixelOut: unmanaged
        {
            dst = System.Numerics.Tensors.TensorSpan<TPixelOut>.Empty;

            var elements = Unsafe.SizeOf<TPixelIn>() / Unsafe.SizeOf<TPixelOut>();
            if (elements * Unsafe.SizeOf<TPixelOut>() != Unsafe.SizeOf<TPixelIn>()) return false;

            if (src == null || src.Width == 0 || src.Height == 0) return false;
            if (!src.DangerousTryGetSinglePixelMemory(out var memory)) return false;

            var imgData = System.Runtime.InteropServices.MemoryMarshal.Cast<TPixelIn,TPixelOut>(memory.Span);

            dst = imgData.AsTensorBitmapHWC(src.Width, src.Height, elements);

            return true;
        }
    }
}
