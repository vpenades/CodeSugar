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
using __TENSOR = System.Numerics.Tensors.Tensor<float>;
using __TENSORSPAN = System.Numerics.Tensors.TensorSpan<float>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        public static void DrawRgbPixelsOverRgb(this __TENSORSPAN dstBitmap, System.Numerics.Matrix3x2 srcTransform, System.Numerics.Tensors.ITensor srcBitmap, bool useBilinear)
        {
            switch (srcBitmap)
            {
                case System.Numerics.Tensors.Tensor<byte> typedSrc:
                    switch (useBilinear)
                    {
                        case false: DrawRgbPixelsOverRgbStep(dstBitmap, srcTransform, typedSrc.AsReadOnlyTensorSpan()); break;
                        case true: DrawRgbPixelsOverRgbBilinear(dstBitmap, srcTransform, typedSrc.AsReadOnlyTensorSpan()); break;
                    }
                    break;
                case System.Numerics.Tensors.Tensor<float> typedSrc:
                    switch (useBilinear)
                    {
                        case false: DrawRgbPixelsOverRgbStep(dstBitmap, srcTransform, typedSrc.AsReadOnlyTensorSpan()); break;
                        case true: DrawRgbPixelsOverRgbBilinear(dstBitmap, srcTransform, typedSrc.AsReadOnlyTensorSpan()); break;
                    }
                    break;
            }
        }

        public static void ApplyMultiplyAddToPixelElements(this __TENSOR tensor, __XYZ mul, __XYZ add)
        {
            tensor.AsTensorSpan().ApplyMultiplyAddToPixelElements(mul, add);
        }

        public static void ApplyMultiplyAddToPixelElements(this __TENSORSPAN tensor, __XYZ mul, __XYZ add)
        {
            tensor = tensor.SqueezeIfRequired();

            if (_TensorSpanBitmap<float, __XYZ>.TryCreate(tensor, out var bmp3))
            {
                bmp3.UpdateAllPixels(p => p * mul + add);
                return;
            }

            if (_TensorSpanBitmap<float, __XYZ>.TryCreatePlanes(tensor, out var r, out var g, out var b))
            {
                r.UpdateAllPixels(p => p * mul.X + add.X);
                g.UpdateAllPixels(p => p * mul.Y + add.Y);
                b.UpdateAllPixels(p => p * mul.Z + add.Z);
                return;
            }            

            throw new NotImplementedException();
        }        
    }
}
