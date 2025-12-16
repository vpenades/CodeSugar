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

using __TENSORS = System.Numerics.Tensors;


#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.Numerics
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForTensors
    {
        #if NET8_0_OR_GREATER

        /// <summary>
        /// checks whether the given tensor has non default strides
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tensor"></param>
        /// <returns>true if the tensor has strides</returns>
        public static bool IsStrided<T>(this __TENSORS.Tensor<T> tensor)
        {
            return tensor.AsReadOnlyTensorSpan().IsStrided();
        }

        /// <summary>
        /// checks whether the given tensor has non default strides
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tensor"></param>
        /// <returns>true if the tensor has strides</returns>
        public static bool IsStrided<T>(in this __TENSORS.TensorSpan<T> tensor)
        {
            return tensor.AsReadOnlyTensorSpan().IsStrided();
        }

        /// <summary>
        /// checks whether the given tensor has non default strides
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tensor"></param>
        /// <returns>true if the tensor has strides</returns>
        public static bool IsStrided<T>(in this __TENSORS.ReadOnlyTensorSpan<T> tensor)
        {
            var strides = tensor.Strides;
            if (strides.IsEmpty) return false;

            var lengths = tensor.Lengths;
            var k = tensor.FlattenedLength;

            for (int i = 0; i < lengths.Length - 1; ++i)
            {
                k /= lengths[i];
                if (strides[i] != k) return true;                
            }

            return strides[strides.Length - 1] != 1;
        }

        public static void CastTo<TSrc, TDst>(this __TENSORS.TensorSpan<TSrc> src, out __TENSORS.TensorSpan<TDst> dst)
            where TSrc : unmanaged
            where TDst : unmanaged
        {
            if (!TryCastTo(src, out dst))
            {
                if (!src.IsDense) throw new InvalidOperationException($"src is not a dense tensor");
                if (src.IsPinned) throw new InvalidOperationException($"src is pinned");
                throw new InvalidOperationException($"Can't cast {typeof(TSrc).Name} to {typeof(TDst).Name}");
            }
        }

        /// <summary>
        /// Casts a tensor to use another type, using the same memory.
        /// </summary>
        /// <remarks>
        /// Dimensions will be adjusted based on <typeparamref name="TSrc"/> and <typeparamref name="TDst"/> relative size ratio.
        /// </remarks>
        /// <typeparam name="TSrc">The source type</typeparam>
        /// <typeparam name="TDst">The target type</typeparam>
        /// <param name="src">The source tensor</param>
        /// <param name="dst">The casted tensor</param>
        /// <returns>true if success</returns>
        public static bool TryCastTo<TSrc, TDst>(this __TENSORS.TensorSpan<TSrc> src, out __TENSORS.TensorSpan<TDst> dst)
            where TSrc: unmanaged
            where TDst: unmanaged
        {
            ref var srcRef = ref System.Runtime.InteropServices.TensorMarshal.GetReference(src);
            ref var dstRef = ref System.Runtime.CompilerServices.Unsafe.As<TSrc, TDst>(ref srcRef);

            var srcSize = System.Runtime.CompilerServices.Unsafe.SizeOf<TSrc>();
            var dstSize = System.Runtime.CompilerServices.Unsafe.SizeOf<TDst>();

            if (srcSize == dstSize)
            {
                dst = System.Runtime.InteropServices.TensorMarshal.CreateTensorSpan(ref dstRef, src.FlattenedLength, src.Lengths, src.Strides, src.IsPinned);
                return true;
            }

            dst = __TENSORS.TensorSpan<TDst>.Empty;

            if (!src.IsDense || src.IsPinned) return false;

            Span<nint> dstLenghts = stackalloc nint[src.Lengths.Length + (srcSize > dstSize ? 1 : 0)];

            if (!_CalcCastDimensions(srcSize, dstSize, src.Lengths, src.FlattenedLength, dstLenghts, out var dstFlatLen)) return false;

            // remove last dimension if it's redundant
            if (srcSize < dstSize && dstLenghts[dstLenghts.Length - 1] == 1) dstLenghts = dstLenghts.Slice(0, dstLenghts.Length - 1);

            dst = System.Runtime.InteropServices.TensorMarshal.CreateTensorSpan(ref dstRef, dstFlatLen, dstLenghts, Span<nint>.Empty, false);

            return true;
        }

        public static void CastTo<TSrc, TDst>(this __TENSORS.ReadOnlyTensorSpan<TSrc> src, out __TENSORS.ReadOnlyTensorSpan<TDst> dst)
            where TSrc : unmanaged
            where TDst : unmanaged
        {
            if (!TryCastTo(src, out dst))
            {
                if (!src.IsDense) throw new InvalidOperationException($"src is not a dense tensor");
                if (src.IsPinned) throw new InvalidOperationException($"src is pinned");
                throw new InvalidOperationException($"Can't cast {typeof(TSrc).Name} to {typeof(TDst).Name}");
            }
        }

        /// <summary>
        /// Casts a tensor to use another type, using the same memory.
        /// </summary>
        /// <remarks>
        /// Dimensions will be adjusted based on <typeparamref name="TSrc"/> and <typeparamref name="TDst"/> relative size ratio.
        /// </remarks>
        /// <typeparam name="TSrc">The source type</typeparam>
        /// <typeparam name="TDst">The target type</typeparam>
        /// <param name="src">The source tensor</param>
        /// <param name="dst">The casted tensor</param>
        /// <returns>true if success</returns>
        public static bool TryCastTo<TSrc, TDst>(this __TENSORS.ReadOnlyTensorSpan<TSrc> src, out __TENSORS.ReadOnlyTensorSpan<TDst> dst)
            where TSrc : unmanaged
            where TDst : unmanaged
        {
            ref readonly var srcRef = ref System.Runtime.InteropServices.TensorMarshal.GetReference(src);
            ref var tmpRef = ref System.Runtime.CompilerServices.Unsafe.AsRef(in srcRef); // requires langversion 12+
            ref readonly var dstRef = ref System.Runtime.CompilerServices.Unsafe.As<TSrc, TDst>(ref tmpRef);

            var srcSize = System.Runtime.CompilerServices.Unsafe.SizeOf<TSrc>();
            var dstSize = System.Runtime.CompilerServices.Unsafe.SizeOf<TDst>();

            if (srcSize == dstSize)
            {
                dst = System.Runtime.InteropServices.TensorMarshal.CreateReadOnlyTensorSpan(in dstRef, src.FlattenedLength, src.Lengths, src.Strides, src.IsPinned);
                return true;
            }

            dst = __TENSORS.TensorSpan<TDst>.Empty;

            if (!src.IsDense || src.IsPinned) return false;

            Span<nint> dstLenghts = stackalloc nint[src.Lengths.Length + (srcSize > dstSize ? 1 : 0)];

            if (!_CalcCastDimensions(srcSize, dstSize, src.Lengths, src.FlattenedLength, dstLenghts, out var dstFlatLen)) return false;

            // remove last dimension if it's redundant
            if (srcSize < dstSize && dstLenghts[dstLenghts.Length - 1] == 1) dstLenghts = dstLenghts.Slice(0, dstLenghts.Length - 1);

            dst = System.Runtime.InteropServices.TensorMarshal.CreateReadOnlyTensorSpan(in dstRef, dstFlatLen, dstLenghts, Span<nint>.Empty, false);

            return true;
        }

        private static bool _CalcCastDimensions(int srcSize, int dstSize, ReadOnlySpan<nint> srcLenghts, nint srcFlatLen, Span<nint> dstLenghts, out nint dstFlatLen)
        {
            dstFlatLen = default;

            if (srcSize > dstSize) // cast from bigger to smaller (As in Vector4[1] to float[4])
            {
                var mul = Math.DivRem(srcSize, dstSize, out var rem);
                if (mul <= 0 || rem != 0) return false;
                
                dstFlatLen = srcFlatLen * mul;

                srcLenghts.CopyTo(dstLenghts);
                dstLenghts.Slice(srcLenghts.Length).Fill(1);

                dstLenghts[dstLenghts.Length - 1] *= mul;
            }

            if (srcSize < dstSize) // cast from smaller to bigger (As in float[4] to Vector4[1])
            {
                var div = Math.DivRem(dstSize, srcSize, out var rem);
                if (div <= 0 || rem != 0) return false;
                
                dstFlatLen = srcFlatLen / div;                

                srcLenghts.CopyTo(dstLenghts);

                dstLenghts[dstLenghts.Length - 1] /= div;
            }
            
            return true;
        }

        public static __TENSORS.Tensor<T> SliceSubTensor<T>(this __TENSORS.Tensor<T> tensor, nint index0)
        {
            if (tensor.Rank < 2) throw new ArgumentException("Rank is less than 2", nameof(tensor));
            if (index0 < 0 || index0 >= tensor.Lengths[0]) throw new ArgumentOutOfRangeException(nameof(index0));

            Span<NRange> ranges = stackalloc NRange[tensor.Rank];

            ranges[0] = new NRange(index0, index0 + 1);
            for (int i = 1; i < ranges.Length; ++i)
            {
                ranges[i] = new NRange(0, tensor.Lengths[i]);
            }

            return tensor.Slice(ranges).SqueezeDimension(0);
        }        

        public static __TENSORS.TensorSpan<T> SliceSubTensor<T>(in this __TENSORS.TensorSpan<T> tensor, nint index0)
        {
            if (tensor.Rank < 2) throw new ArgumentException("Rank is less than 2", nameof(tensor));
            if (index0 < 0 || index0 >= tensor.Lengths[0]) throw new ArgumentOutOfRangeException(nameof(index0));

            Span<NRange> ranges = stackalloc NRange[tensor.Rank];

            ranges[0] = new NRange(index0, index0 + 1);
            for (int i=1; i < ranges.Length; ++i)
            {
                ranges[i] = new NRange(0, tensor.Lengths[i]);
            }

            return tensor.Slice(ranges).SqueezeDimension(0);

        }

        public static __TENSORS.ReadOnlyTensorSpan<T> SliceSubTensor<T>(in this __TENSORS.ReadOnlyTensorSpan<T> tensor, nint index0)
        {
            if (tensor.Rank < 2) throw new ArgumentException("Rank is less than 2", nameof(tensor));
            if (index0 < 0 || index0 >= tensor.Lengths[0]) throw new ArgumentOutOfRangeException(nameof(index0));

            Span<NRange> ranges = stackalloc NRange[tensor.Rank];

            ranges[0] = new NRange(index0, index0 + 1);
            for (int i = 1; i < ranges.Length; ++i)
            {
                ranges[i] = new NRange(0, tensor.Lengths[i]);
            }

            return tensor.Slice(ranges).SqueezeDimension(0);
        }

        public static Span<T> GetFullSpan<T>(this __TENSORS.Tensor<T> tensor)
        {
            if (tensor.FlattenedLength > int.MaxValue) throw new InvalidOperationException("tensor too large");
            if (tensor.FlattenedLength == 0) return Span<T>.Empty;
            Span<nint> indices = stackalloc nint[tensor.Rank];
            return tensor.GetSpan(indices, (int)tensor.FlattenedLength);
        }

        public static Span<T> GetFullSpan<T>(in this __TENSORS.TensorSpan<T> tensor)
        {
            if (tensor.FlattenedLength > int.MaxValue) throw new InvalidOperationException("tensor too large");
            if (tensor.FlattenedLength == 0) return Span<T>.Empty;
            Span<nint> indices = stackalloc nint[tensor.Rank];            
            return tensor.GetSpan(indices, (int)tensor.FlattenedLength);
        }

        public static ReadOnlySpan<T> GetFullSpan<T>(in this __TENSORS.ReadOnlyTensorSpan<T> tensor)
        {
            if (tensor.FlattenedLength > int.MaxValue) throw new InvalidOperationException("tensor too large");
            if (tensor.FlattenedLength == 0) return Span<T>.Empty;
            Span<nint> indices = stackalloc nint[tensor.Rank];
            return tensor.GetSpan(indices, (int)tensor.FlattenedLength);
        }

        public static Span<T> GetSubSpan<T>(this __TENSORS.Tensor<T> tensor, nint index0)
        {
            var flen = tensor.GetFlattenedLength(1);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static Span<T> GetSubSpan<T>(in this __TENSORS.TensorSpan<T> tensor, nint index0)
        {
            var flen = tensor.GetFlattenedLength(1);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");            
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static ReadOnlySpan<T> GetSubSpan<T>(in this __TENSORS.ReadOnlyTensorSpan<T> tensor, nint index0)
        {
            var flen = tensor.GetFlattenedLength(1);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static Span<T> GetSubSpan<T>(in this __TENSORS.TensorSpan<T> tensor, nint index0, nint index1)
        {
            var flen = tensor.GetFlattenedLength(2);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;
            indices[1] = index1;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static ReadOnlySpan<T> GetSubSpan<T>(in this __TENSORS.ReadOnlyTensorSpan<T> tensor, nint index0, nint index1)
        {
            var flen = tensor.GetFlattenedLength(2);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;
            indices[1] = index1;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static Span<T> GetSubSpan<T>(in this __TENSORS.TensorSpan<T> tensor, nint index0, nint index1, nint index2)
        {
            var flen = tensor.GetFlattenedLength(3);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;
            indices[1] = index1;
            indices[2] = index2;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static ReadOnlySpan<T> GetSubSpan<T>(in this __TENSORS.ReadOnlyTensorSpan<T> tensor, nint index0, nint index1, nint index2)
        {
            var flen = tensor.GetFlattenedLength(3);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;
            indices[1] = index1;
            indices[2] = index2;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static nint GetFlattenedLength<T>(this __TENSORS.Tensor<T> tensor, int index)
        {
            return GetFlattenedLength(tensor.Lengths, index);
        }

        public static nint GetFlattenedLength<T>(in this __TENSORS.TensorSpan<T> tensor, int index)
        {
            return GetFlattenedLength(tensor.Lengths, index);
        }

        public static nint GetFlattenedLength<T>(in this __TENSORS.ReadOnlyTensorSpan<T> tensor, int index)
        {
            return GetFlattenedLength(tensor.Lengths, index);
        }

        public static nint GetFlattenedLength(this ReadOnlySpan<nint> lengths, int index)
        {
            nint len = lengths[index++];

            while (index < lengths.Length)
            {
                len *= lengths[index++];
            }

            return len;
        }     

        #endif
    }
}
