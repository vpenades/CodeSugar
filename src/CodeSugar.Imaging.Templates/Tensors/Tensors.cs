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
        public static Tensor<T> ToTensor<T>(this TensorSpan<T> source)
        {
            return ToTensor(source.AsReadOnlyTensorSpan());
        }

        public static Tensor<T> ToTensor<T>(this ReadOnlyTensorSpan<T> source)
        {            
            var destination = Tensor.CreateFromShape<T>(source.Lengths);
            
            source.CopyTo(destination.AsTensorSpan());

            return destination;
        }

        public static Tensor<T> SqueezeIfRequired<T>(this Tensor<T> tensor)
        {
            if (tensor == null) return tensor;
            return tensor.Lengths.Contains(1)
                ? tensor.Squeeze()
                : tensor;
        }

        public static TensorSpan<T> SqueezeIfRequired<T>(this TensorSpan<T> tensor)
        {
            return tensor.Lengths.Contains(1)
                ? tensor.Squeeze()
                : tensor;
        }

        public static ReadOnlyTensorSpan<T> SqueezeIfRequired<T>(this ReadOnlyTensorSpan<T> tensor)
        {
            return tensor.Lengths.Contains(1)
                ? tensor.Squeeze()
                : tensor;
        }
    }
}
