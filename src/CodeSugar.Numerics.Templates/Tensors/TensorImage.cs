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

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarNumericsExtensions
    {
        #region API

        public delegate Span<TPixel> TensorBitmapRowEvalDelegate<TPixel>(int index) where TPixel : unmanaged;        


        public static Tensor<T> CreateCompatibleTensorHWC<T>(this System.Drawing.Size rect, int numChannels)
            where T:unmanaged
        {
            var lll = new nint[3];
            lll[0] = rect.Height;
            lll[1] = rect.Width;
            lll[2] = numChannels;

            var buff = new T[rect.Width * rect.Height * numChannels];

            return Tensor.Create<T>(buff, lll);
        }

        /// <summary>
        /// tries to get a delegate that can be used to query bitmap rows of a tensor representing a bitmap.
        /// </summary>
        /// <typeparam name="TElement">The element type of the tensor</typeparam>
        /// <typeparam name="TPixel">The pixel type of the bitmap</typeparam>
        /// <param name="bitmap">The tensor representing a bitmap</param>
        /// <param name="bitmapSize">The size of the bitmap</param>
        /// <param name="rowEvaluator">The delegate to query the rows</param>
        /// <returns>true on success</returns>
        public static bool TryGetBitmapRowEvaluatorHWC<TElement,TPixel>(System.Numerics.Tensors.Tensor<TElement> bitmap, out System.Drawing.Size bitmapSize, out TensorBitmapRowEvalDelegate<TPixel> rowEvaluator)
            where TElement : unmanaged
            where TPixel : unmanaged
        {
            try
            {             
                var wrapper = new _TensorBitmapHWC<TElement, TPixel>(bitmap);

                bitmapSize = new System.Drawing.Size(wrapper.Width, wrapper.Height);
                rowEvaluator = wrapper.GetRowSpan;

                return true;
            }
            catch
            {
                bitmapSize = System.Drawing.Size.Empty;
                rowEvaluator = default;
                return false;
            }
        }

        #endregion

        #region nested types

        /// <summary>
        /// Wraps a <see cref="System.Numerics.Tensors.Tensor<TElement>"/> and exposes it as a bitmap
        /// </summary>        
        private readonly struct _TensorBitmapHWC<TElement, TPixel>
            where TElement : unmanaged
            where TPixel : unmanaged
        {
            #region lifecycle
            public _TensorBitmapHWC(System.Numerics.Tensors.Tensor<TElement> bitmap)
            {
                _VerifyTensorIsBitmapHWC<TElement, TPixel>(bitmap.Lengths, bitmap.Strides);

                _Bitmap = bitmap;

                Height = (int)bitmap.Lengths[0];
                Width = (int)bitmap.Lengths[1];

                _RawWidth = (int)(bitmap.Lengths[1] * bitmap.Lengths[2]);
            }

            #endregion

            #region data

            private readonly System.Numerics.Tensors.Tensor<TElement> _Bitmap;
            public int Width { get; }
            public int Height { get; }

            private readonly int _RawWidth;

            #endregion

            #region API

            public Span<TPixel> GetRowSpan(int y)
            {
                Span<nint> iii = stackalloc nint[3];
                iii[0] = y;
                iii[1] = 0;
                iii[2] = 0;

                var row = _Bitmap.GetSpan(iii, _RawWidth);

                return System.Runtime.InteropServices.MemoryMarshal.Cast<TElement, TPixel>(row);
            }

            #endregion
        }

        /// <summary>
        /// Wraps a <see cref="System.Numerics.Tensors.TensorSpan<TElement>"/> and exposes it as a bitmap
        /// </summary>        
        private readonly ref struct _TensorSpanBitmapHWC<TElement, TPixel>
            where TElement: unmanaged
            where TPixel : unmanaged
        {
            #region lifecycle

            public static bool TryCreate(System.Numerics.Tensors.TensorSpan<TElement> tensor, _TensorSpanBitmapHWC<TElement, TPixel> bitmap)
            {
                try
                {
                    bitmap = new _TensorSpanBitmapHWC<TElement, TPixel>(tensor);
                    return true;
                }
                catch
                {
                    bitmap = default;
                    return false;
                }

            }

            public _TensorSpanBitmapHWC(System.Numerics.Tensors.TensorSpan<TElement> bitmap)
            {
                _VerifyTensorIsBitmapHWC<TElement, TPixel>(bitmap.Lengths, bitmap.Strides);

                _Bitmap = bitmap;

                Height = (int)bitmap.Lengths[0];
                Width = (int)bitmap.Lengths[1];

                _RawWidth = (int)(bitmap.Lengths[1] * bitmap.Lengths[2]);
            }

            #endregion

            #region data

            private readonly System.Numerics.Tensors.TensorSpan<TElement> _Bitmap;
            public int Width { get; }
            public int Height { get; }

            private readonly int _RawWidth;

            #endregion

            #region API

            public Span<TPixel> GetRowSpan(int y)
            {
                Span<nint> iii = stackalloc nint[3];
                iii[0] = y;
                iii[1] = 0;
                iii[2] = 0;

                var row = _Bitmap.GetSpan(iii, _RawWidth);

                return System.Runtime.InteropServices.MemoryMarshal.Cast<TElement, TPixel>(row);
            }

            #endregion
        }

        /// <summary>
        /// Wraps a <see cref="System.Numerics.Tensors.ReadOnlyTensorSpan<TElement>"/> and exposes it as a bitmap
        /// </summary>        
        private readonly ref struct _ReadOnlyTensorSpanBitmapHWC<TElement, TPixel>
            where TElement : unmanaged
            where TPixel : unmanaged
        {
            #region lifecycle
            public _ReadOnlyTensorSpanBitmapHWC(System.Numerics.Tensors.ReadOnlyTensorSpan<TElement> bitmap)
            {
                _VerifyTensorIsBitmapHWC<TElement, TPixel>(bitmap.Lengths, bitmap.Strides);

                _Bitmap = bitmap;

                Height = (int)bitmap.Lengths[0];
                Width = (int)bitmap.Lengths[1];

                _RawWidth = (int)(bitmap.Lengths[1]* bitmap.Lengths[2]);
            }

            #endregion

            #region data

            private readonly System.Numerics.Tensors.ReadOnlyTensorSpan<TElement> _Bitmap;
            public int Width { get; }
            public int Height { get; }

            private readonly int _RawWidth;

            #endregion

            #region API

            public ReadOnlySpan<TPixel> GetRowSpan(int y)
            {
                Span<nint> iii = stackalloc nint[3];
                iii[0] = y;
                iii[1] = 0;
                iii[2] = 0;

                var row = _Bitmap.GetSpan(iii, _RawWidth);

                return System.Runtime.InteropServices.MemoryMarshal.Cast<TElement, TPixel>(row);
            }

            #endregion
        }

        private static void _VerifyTensorIsBitmapHWC<TElement, TPixel>(ReadOnlySpan<nint> lenghts, ReadOnlySpan<nint> strides)
            where TElement : unmanaged
            where TPixel : unmanaged
        {
            var esize = System.Runtime.InteropServices.Marshal.SizeOf<TElement>();
            var psize = System.Runtime.InteropServices.Marshal.SizeOf<TPixel>();

            if (lenghts.Length != 3) throw new ArgumentException("tensor must be in the format HWC", nameof(lenghts));

            if (lenghts[2] * esize != psize) throw new ArgumentException("TPixel size and tensor channel size must match", nameof(lenghts));
            if (lenghts[2] != strides[1]) throw new ArgumentException("pixels must be dense", nameof(strides));
            if (strides[2] != 1) throw new ArgumentException("pixel stride must be 1", nameof(strides));
        }

        #endregion
    }
}
