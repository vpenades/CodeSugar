using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics.Tensors;

#nullable disable

using MAGICSCALER = PhotoSauce.MagicScaler;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarNumericsExtensions
    {

        static void CopyRowPixels(this MAGICSCALER.IPixelSource srcImg, int y, int cbStride, Span<byte> dst)
        {
            srcImg.CopyPixels(new System.Drawing.Rectangle(0, y, srcImg.Width, 1), cbStride, dst);
        }

    }
}
