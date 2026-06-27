using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics.Tensors;

#nullable disable

using MAGICSCALER = PhotoSauce.MagicScaler;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        static void CopyRowPixels(this MAGICSCALER.IPixelSource srcImg, int y, int cbStride, Span<float> dst)
        {
            const float scale = 1f / 255f;

            Span<byte> bytes = stackalloc byte[dst.Length];

            CopyRowPixels(srcImg, y, cbStride, bytes);

            for (int i = 0; i < dst.Length; i++)
            {
                dst[i] = (float)bytes[i] * scale;
            }            
        }

        static void CopyRowPixels(this MAGICSCALER.IPixelSource srcImg, int y, int cbStride, Span<byte> dst)
        {
            srcImg.CopyPixels(new System.Drawing.Rectangle(0, y, srcImg.Width, 1), cbStride, dst);
        }
    }
}
