using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.Serialization;


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

        #pragma warning disable SYSLIB5001

        public static void CastTo<TSrc, TDst>(this System.Numerics.Tensors.TensorSpan<TSrc> src, out System.Numerics.Tensors.TensorSpan<TDst> dst)
            where TSrc : unmanaged
            where TDst : unmanaged
        {
            if (!TryCastTo(src, out dst)) throw new InvalidOperationException($"Can't cast {typeof(TSrc).Name} to {typeof(TDst).Name}");
        }

        public static bool TryCastTo<TSrc, TDst>(this System.Numerics.Tensors.TensorSpan<TSrc> src, out System.Numerics.Tensors.TensorSpan<TDst> dst)
            where TSrc: unmanaged
            where TDst: unmanaged
        {
            var srcSize = System.Runtime.CompilerServices.Unsafe.SizeOf<TSrc>();
            var dstSize = System.Runtime.CompilerServices.Unsafe.SizeOf<TDst>();

            ref var srcRef = ref System.Runtime.InteropServices.TensorMarshal.GetReference(src);
            ref var dstRef = ref System.Runtime.CompilerServices.Unsafe.As<TSrc, TDst>(ref srcRef);            

            if (srcSize == dstSize)
            {
                dst = System.Runtime.InteropServices.TensorMarshal.CreateTensorSpan(ref dstRef, src.FlattenedLength, src.Lengths, src.Strides, src.IsPinned);
                return true;
            }

            dst = System.Numerics.Tensors.TensorSpan<TDst>.Empty;

            if (!src.IsDense) return false;
            if (src.IsPinned) return false;

            if (srcSize > dstSize) // cast from bigger to smaller (As in Vector4 to float)
            {
                var count = Math.DivRem(srcSize, dstSize, out var rem);

                if (count > 0 && rem == 0)
                {
                    var flatLen = src.FlattenedLength * count;

                    Span<nint> lenghts = stackalloc nint[src.Lengths.Length + 1];
                    src.Lengths.CopyTo(lenghts);
                    lenghts[lenghts.Length - 1] = count;                    

                    dst = System.Runtime.InteropServices.TensorMarshal.CreateTensorSpan(ref dstRef, flatLen, lenghts, Span<nint>.Empty,false);
                    return true;
                }
            }

            if (srcSize < dstSize) // cast from smaller to bigger (As in float to Vector4)
            {
                var count = Math.DivRem(dstSize, srcSize, out var rem);                

                if (count > 0 && rem == 0)
                {
                    var flatLen = src.FlattenedLength / count;

                    var lastLen = src.Lengths[src.Lengths.Length - 1] / count;

                    if (lastLen == 1) // we can remove the last dimension
                    {
                        Span<nint> lenghts = stackalloc nint[src.Lengths.Length -1];
                        src.Lengths.Slice(0, src.Lengths.Length -1).CopyTo(lenghts);

                        dst = System.Runtime.InteropServices.TensorMarshal.CreateTensorSpan(ref dstRef, flatLen, lenghts, Span<nint>.Empty, false);
                        return true;
                    }
                    else
                    {
                        Span<nint> lenghts = stackalloc nint[src.Lengths.Length];
                        src.Lengths.CopyTo(lenghts);
                        lenghts[src.Lengths.Length - 1] = lastLen;

                        dst = System.Runtime.InteropServices.TensorMarshal.CreateTensorSpan(ref dstRef, flatLen, lenghts, Span<nint>.Empty, false);
                        return true;
                    }                        
                }
            }

            return false;
        }

        public static nint GetFlattenedLength<T>(this System.Numerics.Tensors.TensorSpan<T> tensor, int index)
        {
            nint len = tensor.Lengths[index++];

            while(index < tensor.Lengths.Length)
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

        public static Span<T> GetSpan<T>(this System.Numerics.Tensors.TensorSpan<T> tensor, nint index0, nint index1, nint index2)
        {
            var flen = tensor.GetFlattenedLength(3);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;
            indices[1] = index1;
            indices[2] = index2;

            return tensor.GetSpan(indices, (int)flen);
        }

        public static ReadOnlySpan<T> GetSpan<T>(this System.Numerics.Tensors.ReadOnlyTensorSpan<T> tensor, nint index0, nint index1, nint index2)
        {
            var flen = tensor.GetFlattenedLength(3);
            if (flen > int.MaxValue) throw new InvalidOperationException("tensor too large");
            Span<nint> indices = stackalloc nint[tensor.Rank];
            indices[0] = index0;
            indices[1] = index1;
            indices[2] = index2;

            return tensor.GetSpan(indices, (int)flen);
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

        #pragma warning disable SYSLIB5001

        #endif
    }
}
