using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.InteropServices;
using System.Text;
using System.Buffers;
using System.Reflection;
using System.Transactions;
using System.Threading;

#nullable disable

using __XYZ = System.Numerics.Vector3;
using __RWTENSOR = System.Numerics.Tensors.ITensor;
using __ROTENSOR = System.Numerics.Tensors.IReadOnlyTensor;
using __RWTENSORSPANB = System.Numerics.Tensors.TensorSpan<byte>;
using __ROTENSORSPANB = System.Numerics.Tensors.ReadOnlyTensorSpan<byte>;
using __RWTENSORSPANF = System.Numerics.Tensors.TensorSpan<float>;
using __ROTENSORSPANF = System.Numerics.Tensors.ReadOnlyTensorSpan<float>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        private static void _CopyTensorPlanesToBitmap<TPixelOut>(__ROTENSORSPANF src, bool srcIsBGR, __IInteropBitmap<TPixelOut> dst, Func<__XYZ, TPixelOut> pixConverter)
        {
            if (!TryInferBitmapSize(src, out var srcWidth, out var srcHeight, out var srcChannels, out var srcIsCHW))
            {
                GuardIsBitmap(src, false);
            }

            if (!srcIsCHW) throw new ArgumentException("tensor is not made of planes");

            var numRows = Math.Min(dst.Height, srcHeight);
            var numCols = Math.Min(dst.Width, srcWidth);

            for (int y = 0; y < numRows; y++)
            {
                GetBitmapPlaneRows(src, y, srcIsBGR, out var srcRowR, out var srcRowG, out var srcRowB);

                var dstRowPix = dst.GetRowSpan(y);

                var rgb = __XYZ.Zero;
                
                for (int i = 0; i < numCols; ++i)
                {
                    rgb.X = srcRowR[i];
                    rgb.Y = srcRowG[i];
                    rgb.Z = srcRowB[i];                   

                    dstRowPix[i] = pixConverter(rgb);
                }
            }
        }

        public static void GetBitmapPlaneRows(__ROTENSORSPANF tensor, int y, bool IsBGR, out ReadOnlySpan<float> channelR, out ReadOnlySpan<float> channelG, out ReadOnlySpan<float> channelB)
        {
            if (tensor.IsEmpty) throw new ArgumentNullException(nameof(tensor));
            GuardIsPlanarBitmap(tensor.Lengths, tensor.Strides);            

            int rowLen = (int)tensor.Lengths[2];

            Span<nint> indices = stackalloc nint[3];
            indices[1] = y;
            indices[2] = 0;

            indices[0] = IsBGR ? 2 : 0;
            channelR = tensor.GetSpan(indices, rowLen);

            indices[0] = 1;
            channelG = tensor.GetSpan(indices, rowLen);

            indices[0] = IsBGR ? 0 : 2;
            channelB = tensor.GetSpan(indices, rowLen);
        }        

        public static void GetBitmapPlaneRows(__RWTENSORSPANF tensor, int y, bool IsBGR, out Span<float> channelR, out Span<float> channelG, out Span<float> channelB)
        {
            if (tensor.IsEmpty) throw new ArgumentNullException(nameof(tensor));
            GuardIsPlanarBitmap(tensor.Lengths, tensor.Strides);

            int rowLen = (int)tensor.Lengths[2];

            Span<nint> indices = stackalloc nint[3];
            indices[1] = y;
            indices[2] = 0;

            indices[0] = IsBGR ? 2 : 0;
            channelR = tensor.GetSpan(indices, rowLen);

            indices[0] = 1;
            channelG = tensor.GetSpan(indices, rowLen);

            indices[0] = IsBGR ? 0 : 2;
            channelB = tensor.GetSpan(indices, rowLen);
        }

        private static void GuardIsPlanarBitmap(ReadOnlySpan<nint> lengths, ReadOnlySpan<nint> strides)
        {
            if (lengths.Length != 3) throw new ArgumentOutOfRangeException("must have Rank 3", nameof(lengths));
            if (lengths[0] < 3 || lengths[0] > 4) throw new ArgumentOutOfRangeException("only 3 and 4 planes are supported", nameof(lengths));
        }
    }
}
