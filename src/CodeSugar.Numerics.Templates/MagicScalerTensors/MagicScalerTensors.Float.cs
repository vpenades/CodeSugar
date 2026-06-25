using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics.Tensors;
using System.Numerics;

using PhotoSauce.MagicScaler;

#nullable disable

using __ITENSOR = System.Numerics.Tensors.ITensor;
using __TENSORSPAN = System.Numerics.Tensors.TensorSpan<float>;
using __READONLYTENSORSPAN = System.Numerics.Tensors.ReadOnlyTensorSpan<float>;

using MAGICSCALER = PhotoSauce.MagicScaler;


namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarNumericsExtensions
    {
        public static void CopyPixelsToTensor(this MAGICSCALER.IPixelSource srcImg, __TENSORSPAN tensor)
        {
            var normalized = Vector3.One / 255f;

            CopyPixelsToTensor(srcImg, tensor, normalized, Vector3.Zero);
        }

        public static void CopyPixelsToTensor(this MAGICSCALER.IPixelSource srcImg, __TENSORSPAN tensor, Vector3 mul, Vector3 add)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;

            // Determine the number of channels from the format
            int channels = GetMagicScalerChannelCount(srcImg.Format);
            if (channels < 1) throw new NotSupportedException($"Pixel format {srcImg.Format} is not supported");

            var dstBmp = new _TensorSpanBitmapHWC<float, Vector3>(tensor);            

            // Allocate buffer for one row of pixels            
            Span<byte> tmpRow = new byte[width * channels];            

            if (srcImg.Format == PixelFormats.Bgr24bpp)
            {                
                for (int y = 0; y < height; y++)
                {
                    srcImg.CopyRowPixels(y, tmpRow.Length, tmpRow);
                    ConvertBGRtoRGB(tmpRow, dstBmp.GetRowSpan(y), mul, add);
                }

                return;
            }

            if (srcImg.Format == PixelFormats.Bgra32bpp)
            {
                for (int y = 0; y < height; y++)
                {
                    srcImg.CopyRowPixels(y, tmpRow.Length, tmpRow);
                    ConvertBGRAtoRGB(tmpRow, dstBmp.GetRowSpan(y), mul, add);
                }
                return;
            }


            throw new NotImplementedException($"Unsupported pixel format: {srcImg.Format}");
        }

        /// <summary>
        /// Determines the number of channels from a PhotoSauce pixel format GUID.
        /// </summary>
        private static int GetMagicScalerChannelCount(Guid format)
        {
            if (format == PixelFormats.Grey8bpp) return 1;
            if (format == PixelFormats.Bgr24bpp) return 3;
            if (format == PixelFormats.Bgra32bpp) return 4;

            return 0; // Unsupported format
        }

        
    }
}
