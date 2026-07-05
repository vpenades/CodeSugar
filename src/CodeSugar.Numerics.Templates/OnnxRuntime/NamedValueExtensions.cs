using System;
using System.Linq;
using System.Numerics;

#nullable disable

using __ONNX = Microsoft.ML.OnnxRuntime;
using __ONNXTENSORS = Microsoft.ML.OnnxRuntime.Tensors;

using __NAMEDVALUE = Microsoft.ML.OnnxRuntime.NamedOnnxValue;
using __NODEMETADATA = Microsoft.ML.OnnxRuntime.NodeMetadata;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarNumericsExtensions
    {
        public static __ONNXTENSORS.DenseTensor<T> AsOnnxDenseTensor<T>(this __NAMEDVALUE nvalue)
        {
            if (nvalue.Value is __ONNXTENSORS.DenseTensor<T> dtensor) return dtensor;

            return nvalue.AsTensor<T>().ToDenseTensor();
        }

        public static __NAMEDVALUE CreateNamedOnnxValue(this __NODEMETADATA metadata, string name, ReadOnlySpan<int> dimensions)
        {
            Span<int> dims = stackalloc int[metadata.Dimensions.Length];
            metadata.InstantiateDimensions(dimensions, dims);

            if (metadata.ElementType == typeof(Boolean)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<Boolean>(dims));
            if (metadata.ElementType == typeof(Char)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<Char>(dims));

            if (metadata.ElementType == typeof(SByte)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<SByte>(dims));
            if (metadata.ElementType == typeof(Byte)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<Byte>(dims));

            if (metadata.ElementType == typeof(Int16)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<Int16>(dims));
            if (metadata.ElementType == typeof(UInt16)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<UInt16>(dims));

            if (metadata.ElementType == typeof(Int32)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<Int32>(dims));
            if (metadata.ElementType == typeof(UInt32)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<UInt32>(dims));

            if (metadata.ElementType == typeof(Int64)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<Int64>(dims));
            if (metadata.ElementType == typeof(UInt64)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<UInt64>(dims));

            if (metadata.ElementType == typeof(Half)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<Half>(dims));
            if (metadata.ElementType == typeof(Single)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<Single>(dims));
            if (metadata.ElementType == typeof(Double)) return __NAMEDVALUE.CreateFromTensor(name, new __ONNXTENSORS.DenseTensor<Double>(dims));

            throw new NotImplementedException(metadata.ElementType.ToString());
        }

        public static System.Numerics.Tensors.ITensor CreateITensor(this __NODEMETADATA metadata, ReadOnlySpan<int> dimensions)
        {
            Span<nint> dims = stackalloc nint[metadata.Dimensions.Length];
            metadata.InstantiateDimensions(dimensions, dims);

            nint flen = 0;
            for(int i=0; i < dims.Length; ++i)
            {
                var d = dims[i];
                if (i == 0) flen = d;
                else flen *= d;
            }

            if (metadata.ElementType == typeof(Boolean)) return System.Numerics.Tensors.Tensor.Create(new Boolean[(int)flen], dims);
            if (metadata.ElementType == typeof(Char)) return System.Numerics.Tensors.Tensor.Create(new Char[(int)flen], dims);

            if (metadata.ElementType == typeof(SByte)) return System.Numerics.Tensors.Tensor.Create(new SByte[(int)flen], dims);
            if (metadata.ElementType == typeof(Byte)) return System.Numerics.Tensors.Tensor.Create(new Byte[(int)flen], dims);

            if (metadata.ElementType == typeof(Int16)) return System.Numerics.Tensors.Tensor.Create(new Int16[(int)flen], dims);
            if (metadata.ElementType == typeof(UInt16)) return System.Numerics.Tensors.Tensor.Create(new UInt16[(int)flen], dims);

            if (metadata.ElementType == typeof(Int32)) return System.Numerics.Tensors.Tensor.Create(new Int32[(int)flen], dims);
            if (metadata.ElementType == typeof(UInt32)) return System.Numerics.Tensors.Tensor.Create(new UInt32[(int)flen], dims);

            if (metadata.ElementType == typeof(Int64)) return System.Numerics.Tensors.Tensor.Create(new Int64[(int)flen], dims);
            if (metadata.ElementType == typeof(UInt64)) return System.Numerics.Tensors.Tensor.Create(new UInt64[(int)flen], dims);

            if (metadata.ElementType == typeof(Half)) return System.Numerics.Tensors.Tensor.Create(new Half[(int)flen], dims);
            if (metadata.ElementType == typeof(Single)) return System.Numerics.Tensors.Tensor.Create(new Single[(int)flen], dims);
            if (metadata.ElementType == typeof(Double)) return System.Numerics.Tensors.Tensor.Create(new Double[(int)flen], dims);

            throw new NotImplementedException(metadata.ElementType.ToString());
        }

        /// <summary>
        /// Fills <paramref name="instanceDimensions"/> with valid, non negative dimensions
        /// </summary>
        public static void InstantiateDimensions(this __NODEMETADATA metadata, ReadOnlySpan<int> fallbackDimensions, Span<int> instanceDimensions)
        {
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));

            var metaDims = metadata.Dimensions;
            if (instanceDimensions.Length != metaDims.Length) throw new ArgumentException($"must have a length of {metaDims.Length}", nameof(instanceDimensions));

            for(int i=0; i < instanceDimensions.Length; ++i)
            {
                var d = metaDims[i];
                if (d <= 0) d = fallbackDimensions.Length <= i ? 1 : fallbackDimensions[i];
                instanceDimensions[i] = d;
            }
        }

        /// <summary>
        /// Fills <paramref name="instanceDimensions"/> with valid, non negative dimensions
        /// </summary>
        public static void InstantiateDimensions(this __NODEMETADATA metadata, ReadOnlySpan<int> fallbackDimensions, Span<nint> instanceDimensions)
        {
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));

            var metaDims = metadata.Dimensions;
            if (instanceDimensions.Length != metaDims.Length) throw new ArgumentException($"must have a length of {metaDims.Length}", nameof(instanceDimensions));

            for (int i = 0; i < instanceDimensions.Length; ++i)
            {
                var d = metaDims[i];
                if (d <= 0) d = fallbackDimensions.Length <= i ? 1 : fallbackDimensions[i];
                instanceDimensions[i] = d;
            }
        }
    }

}