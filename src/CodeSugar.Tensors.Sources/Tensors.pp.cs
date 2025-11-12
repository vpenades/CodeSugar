using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable

#if NET8_0_OR_GREATER
using __XYZ = System.Numerics.Vector3;
using __TENSORSPAN = System.Numerics.Tensors.TensorSpan<float>;
using __READONLYTENSORSPAN = System.Numerics.Tensors.ReadOnlyTensorSpan<float>;
#endif


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

        public static void CastTo<TSrc, TDst>(this System.Numerics.Tensors.TensorSpan<TSrc> src, out System.Numerics.Tensors.TensorSpan<TDst> dst)
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
        public static bool TryCastTo<TSrc, TDst>(this System.Numerics.Tensors.TensorSpan<TSrc> src, out System.Numerics.Tensors.TensorSpan<TDst> dst)
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

            dst = System.Numerics.Tensors.TensorSpan<TDst>.Empty;

            if (!src.IsDense || src.IsPinned) return false;

            Span<nint> dstLenghts = stackalloc nint[src.Lengths.Length + (srcSize > dstSize ? 1 : 0)];

            if (!_CalcCastDimensions(srcSize, dstSize, src.Lengths, src.FlattenedLength, dstLenghts, out var dstFlatLen)) return false;

            // remove last dimension if it's redundant
            if (srcSize < dstSize && dstLenghts[dstLenghts.Length - 1] == 1) dstLenghts = dstLenghts.Slice(0, dstLenghts.Length - 1);

            dst = System.Runtime.InteropServices.TensorMarshal.CreateTensorSpan(ref dstRef, dstFlatLen, dstLenghts, Span<nint>.Empty, false);

            return true;
        }

        public static void CastTo<TSrc, TDst>(this System.Numerics.Tensors.ReadOnlyTensorSpan<TSrc> src, out System.Numerics.Tensors.ReadOnlyTensorSpan<TDst> dst)
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
        public static bool TryCastTo<TSrc, TDst>(this System.Numerics.Tensors.ReadOnlyTensorSpan<TSrc> src, out System.Numerics.Tensors.ReadOnlyTensorSpan<TDst> dst)
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

            dst = System.Numerics.Tensors.TensorSpan<TDst>.Empty;

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
        

        public static Span<T> GetFullSpan<T>(this System.Numerics.Tensors.TensorSpan<T> tensor)
        {
            if (tensor.FlattenedLength > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            return tensor.GetSpan(indices, (int)tensor.FlattenedLength);
        }

        public static ReadOnlySpan<T> GetFullSpan<T>(this System.Numerics.Tensors.ReadOnlyTensorSpan<T> tensor)
        {
            if (tensor.FlattenedLength > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            return tensor.GetSpan(indices, (int)tensor.FlattenedLength);
        }

        public static Span<T> GetSubSpan<T>(this System.Numerics.Tensors.TensorSpan<T> tensor, nint index0)
        {
            var flen = tensor.GetFlattenedLength(1);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");            
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static ReadOnlySpan<T> GetSubSpan<T>(this System.Numerics.Tensors.ReadOnlyTensorSpan<T> tensor, nint index0)
        {
            var flen = tensor.GetFlattenedLength(1);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static Span<T> GetSubSpan<T>(this System.Numerics.Tensors.TensorSpan<T> tensor, nint index0, nint index1)
        {
            var flen = tensor.GetFlattenedLength(2);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;
            indices[1] = index1;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static ReadOnlySpan<T> GetSubSpan<T>(this System.Numerics.Tensors.ReadOnlyTensorSpan<T> tensor, nint index0, nint index1)
        {
            var flen = tensor.GetFlattenedLength(2);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;
            indices[1] = index1;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static Span<T> GetSubSpan<T>(this System.Numerics.Tensors.TensorSpan<T> tensor, nint index0, nint index1, nint index2)
        {
            var flen = tensor.GetFlattenedLength(3);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;
            indices[1] = index1;
            indices[2] = index2;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static ReadOnlySpan<T> GetSubSpan<T>(this System.Numerics.Tensors.ReadOnlyTensorSpan<T> tensor, nint index0, nint index1, nint index2)
        {
            var flen = tensor.GetFlattenedLength(3);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;
            indices[1] = index1;
            indices[2] = index2;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static nint GetFlattenedLength<T>(this System.Numerics.Tensors.TensorSpan<T> tensor, int index)
        {
            nint len = tensor.Lengths[index++];

            while (index < tensor.Lengths.Length)
            {
                len *= tensor.Lengths[index++];
            }

            return len;
        }

        public static nint GetFlattenedLength<T>(this System.Numerics.Tensors.ReadOnlyTensorSpan<T> tensor, int index)
        {
            nint len = tensor.Lengths[index++];

            while (index < tensor.Lengths.Length)
            {
                len *= tensor.Lengths[index++];
            }

            return len;
        }

        public static void ApplyMultiplyAddToPixelElements(this Tensor<float> tensor, __XYZ mul, __XYZ add)
        {
            tensor.AsTensorSpan().ApplyMultiplyAddToPixelElements(mul, add);
        }

        public static void ApplyMultiplyAddToPixelElements(this __TENSORSPAN tensor, __XYZ mul, __XYZ add)
        {
            tensor = tensor.Squeeze();

            if (!_TryInferImageSize(tensor, out _, out _, out int channels, out bool isCHW)) throw new ArgumentException("can't infer image size", nameof(tensor));

            if (isCHW)
            {
                channels = Math.Min(3, channels);                

                for (nint i = 0; i < channels; ++i)
                {
                    var emul = i == 0 ? mul.X : (i == 1 ? mul.Y : mul.Z);
                    var eadd = i == 0 ? add.X : (i == 1 ? add.Y : add.Z);                    
                    
                    var span = tensor.GetSubSpan(i);

                    System.Numerics.Tensors.TensorPrimitives.Multiply(span, emul, span);
                    System.Numerics.Tensors.TensorPrimitives.Add(span, eadd, span);
                }
                return;
            }

            if (channels == 3)
            {
                var span = tensor.GetFullSpan();
                var vvv = System.Runtime.InteropServices.MemoryMarshal.Cast<float, System.Numerics.Vector3>(span);

                for (int i = 0; i < vvv.Length; ++i)
                {
                    vvv[i] = vvv[i] * mul + add;
                }

                return;
            }

            throw new NotImplementedException();
        }

        private static bool _TryInferImageSize(__READONLYTENSORSPAN tensor, out int width, out int height, out int channels, out bool isCHW)
        {
            tensor = tensor.Squeeze();
            width = 0;
            height = 0;
            channels = 0;
            isCHW = false;

            if (tensor.Rank == 2)
            {
                height = (int)tensor.Lengths[0];
                width = (int)tensor.Lengths[1];
                channels = 1;                
                return true;
            }

            if (tensor.Rank != 3) return false;

            if (tensor.Lengths[2] <= 4 && tensor.Lengths[0] > tensor.Lengths[2]) // check is HWC
            {
                height = (int)tensor.Lengths[0];
                width = (int)tensor.Lengths[1];
                channels = (int)tensor.Lengths[2];
                return true;
            }

            if (tensor.Lengths[0] <= 4 && tensor.Lengths[1] > tensor.Lengths[0]) // check is CHW
            {
                channels = (int)tensor.Lengths[0];
                height = (int)tensor.Lengths[1];
                width = (int)tensor.Lengths[2];
                isCHW = true;
                return true;
            }

            return false;
        }        

        #endif
    }
}
