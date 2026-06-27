using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Transactions;

#nullable disable


namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        public static void GuardIsBitmap(this IReadOnlyTensor tensor, bool requireHWC = true, [CallerArgumentExpression(nameof(tensor))] string name = null)
        {            
            if (!TryInferBitmapSize(tensor, out _, out _, out _, out var isCHW))
            {                
                name ??= nameof(tensor);
                if (tensor == null) throw new ArgumentNullException(name);
                if (tensor.IsEmpty) throw new ArgumentException("Empty", name);
                _ThrowTensorImageException(tensor.Lengths, tensor.Strides, name);
                return;
            }

            if (requireHWC && isCHW) throw new ArgumentException("tensor must be HWC, but found CHW");
        }

        public static void GuardIsBitmap<T>(this TensorSpan<T> tensor, bool requireHWC = true, [CallerArgumentExpression(nameof(tensor))] string name = null)
        {
            if (!TryInferBitmapSize(tensor, out _, out _, out _, out var isCHW))
            {
                name ??= nameof(tensor);                
                if (tensor.IsEmpty) throw new ArgumentException("Empty", name);
                _ThrowTensorImageException(tensor.Lengths, tensor.Strides, name);
                return;
            }

            if (requireHWC && isCHW) throw new ArgumentException("tensor must be HWC, but found CHW");
        }

        public static void GuardIsBitmap<T>(this ReadOnlyTensorSpan<T> tensor, bool requireHWC = true, [CallerArgumentExpression(nameof(tensor))] string name = null)
        {
            if (!TryInferBitmapSize(tensor, out _, out _, out _, out var isCHW))
            {
                name ??= nameof(tensor);
                if (tensor.IsEmpty) throw new ArgumentException("Empty", name);
                _ThrowTensorImageException(tensor.Lengths, tensor.Strides, name);
                return;
            }

            if (requireHWC && isCHW) throw new ArgumentException("tensor must be HWC, but found CHW");
        }

        private static void _ThrowTensorImageException(ReadOnlySpan<nint> lenghts, ReadOnlySpan<nint> strides, string name)
        {            
            if (lenghts.Length < 2 || lenghts.Length > 3) throw new ArgumentOutOfRangeException(name, "Rank must be 2 or 3");

            // check rows are dense

            if (lenghts.Length == 3)
            {
                if (lenghts[2] != strides[1]) throw new ArgumentException("Rows are not dense", name);
                if (strides[2] > 1) throw new ArgumentException("pixels are not dense", name);
            }            

            throw new ArgumentException("not an image", name);
        }

        /// <summary>
        /// Checks if the tensor can be reinterpreted as an image
        /// </summary>
        public static bool TryInferBitmapSize(this IReadOnlyTensor tensor, out nint width, out nint height, out nint channels, out bool isCHW)
        {
            if (tensor == null || tensor.IsEmpty) { width = height = channels = 0; isCHW = false; return false; }
            return _TryInferBitmapSize(tensor.Lengths, tensor.Strides, out width, out height, out channels, out isCHW);
        }

        /// <summary>
        /// Checks if the tensor can be reinterpreted as an image
        /// </summary>
        public static bool TryInferBitmapSize<T>(this TensorSpan<T> tensor, out nint width, out nint height, out nint channels, out bool isCHW)
        {
            if (tensor.IsEmpty) { width = height = channels = 0; isCHW = false; return false; }
            return _TryInferBitmapSize(tensor.Lengths, tensor.Strides, out width, out height, out channels, out isCHW);
        }

        /// <summary>
        /// Checks if the tensor can be reinterpreted as an image
        /// </summary>
        public static bool TryInferBitmapSize<T>(this ReadOnlyTensorSpan<T> tensor, out nint width, out nint height, out nint channels, out bool isCHW)
        {
            if (tensor.IsEmpty) { width = height = channels = 0; isCHW = false; return false; }
            return _TryInferBitmapSize(tensor.Lengths, tensor.Strides, out width, out height, out channels, out isCHW);
        }

        private static bool _TryInferBitmapSize(ReadOnlySpan<nint> lenghts, ReadOnlySpan<nint> strides, out nint width, out nint height, out nint channels, out bool isCHW)
        {
            System.Diagnostics.Debug.Assert(lenghts.Length == strides.Length, "Lengths and Strides mismatch");

            width = 0;
            height = 0;
            channels = 0;
            isCHW = false;            

            if (lenghts.Length < 2 || lenghts.Length > 3) return false;

            if (lenghts[0] <= 0) return false;
            if (lenghts[1] <= 0) return false;

            if (lenghts.Length == 2)
            {
                height = lenghts[0];
                width = lenghts[1];
                channels = 1;
                return true;
            }

            if (lenghts[2] <= 0) return false;

            // CHW

            if (lenghts[0] < lenghts[2]) 
            {
                channels = lenghts[0];
                height = lenghts[1];
                width = lenghts[2];
                isCHW = true;
                return true;
            }

            // HWC

            // check rows are dense            
            if (lenghts[2] != strides[1]) return false;
            if (lenghts[2] > 1 && strides[2] != 1) return false;
            if (lenghts[2] == 1 && strides[2] != 0) return false;
            
            height = lenghts[0];
            width = lenghts[1];
            channels = lenghts[2];
            return true;
        }


        private static void _VerifyTensorIsBitmapHWC<TElement, TPixel>(ReadOnlySpan<nint> lenghts, ReadOnlySpan<nint> strides)
            where TElement : unmanaged
            where TPixel : unmanaged
        {
            if (lenghts.Length < 2 || lenghts.Length > 3) throw new ArgumentOutOfRangeException("must have a rank of 2 or 3", nameof(lenghts));

            var esize = System.Runtime.InteropServices.Marshal.SizeOf<TElement>();
            var psize = System.Runtime.InteropServices.Marshal.SizeOf<TPixel>();

            if (!_TryInferBitmapSize(lenghts, strides, out var w, out var h, out var c, out var isCHW)) throw new ArgumentException("not an image");
            if (isCHW) throw new ArgumentException("tensor must be in the format HWC", nameof(lenghts));

            if (c * esize != psize) throw new ArgumentException("TPixel size and tensor channel size must match", nameof(lenghts));

            if (lenghts.Length > 2)
            {
                if (lenghts[2] != strides[1]) throw new ArgumentException("pixels must be dense", nameof(strides));
                if (strides[2] != 1) throw new ArgumentException("pixel stride must be 1", nameof(strides));
            }
        }
    }



        
}
