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
using System.Runtime.CompilerServices;
using System.Security;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        #region API

        public delegate Span<TPixel> TensorBitmapRowEvalDelegate<TPixel>(int index) where TPixel : unmanaged;

        public static TensorSpan<TElement> AsTensorBitmapHWC<TElement>(this Span<TElement> data, int width, int height, int numChannels)
            where TElement : unmanaged
        {
            var size = width * height * numChannels;

            if (data.Length < size) throw new ArgumentException("data length mismatch", nameof(data));

            if (numChannels == 1)
            {
                Span<nint> hw = stackalloc nint[2];
                hw[0] = height;
                hw[1] = width;
                return new TensorSpan<TElement>(data, hw);
            }
            else
            {
                Span<nint> hwc = stackalloc nint[3];
                hwc[0] = height;
                hwc[1] = width;
                hwc[2] = numChannels;
                return new TensorSpan<TElement>(data, hwc);
            }
        }

        public static ReadOnlyTensorSpan<TElement> AsTensorBitmapHWC<TElement>(this ReadOnlySpan<TElement> data, int width, int height, int numChannels)
            where TElement : unmanaged
        {
            var size = width * height * numChannels;

            if (data.Length < size) throw new ArgumentException("data length mismatch", nameof(data));

            if (numChannels == 1)
            {
                Span<nint> hw = stackalloc nint[2];
                hw[0] = height;
                hw[1] = width;
                return new ReadOnlyTensorSpan<TElement>(data, hw);
            }
            else
            {
                Span<nint> hwc = stackalloc nint[3];
                hwc[0] = height;
                hwc[1] = width;
                hwc[2] = numChannels;
                return new ReadOnlyTensorSpan<TElement>(data, hwc);
            }
        }

        public static Tensor<T> CreateTensorBitmapHWC<T>(this System.Drawing.Size rect, int numChannels)
            where T:unmanaged
        {
            var buff = new T[rect.Width * rect.Height * numChannels];

            if (numChannels == 1)
            {
                var hw = new nint[2];
                hw[0] = rect.Height;
                hw[1] = rect.Width;
                return Tensor.Create(buff, hw);
            }
            else
            {
                var hwc = new nint[3];
                hwc[0] = rect.Height;
                hwc[1] = rect.Width;
                hwc[2] = numChannels;
                return Tensor.Create(buff, hwc);
            }
        }

        public static Tensor<T> CreateTensorBitmapCHW<T>(this System.Drawing.Size rect, int numChannels)
            where T : unmanaged
        {
            var buff = new T[rect.Width * rect.Height * numChannels];

            if (numChannels == 1)
            {
                var hw = new nint[2];
                hw[0] = rect.Height;
                hw[1] = rect.Width;
                return Tensor.Create(buff, hw);
            }
            else
            {
                var chw = new nint[3];
                chw[0] = numChannels;
                chw[1] = rect.Height;
                chw[2] = rect.Width;
                return Tensor.Create(buff, chw);
            }
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
            if (_TensorBitmap<TElement, TPixel>.TryCreate(bitmap, out var wrapper))
            {
                bitmapSize = new System.Drawing.Size(wrapper.Width, wrapper.Height);
                rowEvaluator = wrapper.GetRowSpan;
                return true;
            }
            
            bitmapSize = System.Drawing.Size.Empty;
            rowEvaluator = default;
            return false;            
        }

        #endregion

        #region nested types

        /// <summary>
        /// Wraps a <see cref="System.Numerics.Tensors.Tensor<TElement>"/> and exposes it as a bitmap
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("_TensorBitmapHWC {Width}x{Height}x{typeof(TPixel).Name,nq}")]
        private readonly struct _TensorBitmap<TElement, TPixel> : __IInteropBitmap<TPixel>
            where TElement : unmanaged
            where TPixel : unmanaged
        {
            #region lifecycle

            public static _TensorBitmap<TElement, TPixel> Create(System.Numerics.Tensors.Tensor<TElement> tensor)
            {
                if (!TryCreate(tensor, out var bitmap)) GuardIsBitmap(tensor);
                return bitmap;
            }

            public static bool TryCreate(System.Numerics.Tensors.Tensor<TElement> tensor, out _TensorBitmap<TElement, TPixel> bitmap)
            {
                bitmap = default;
                if (tensor.IsEmpty) { return false; }

                tensor = tensor.SqueezeIfRequired();

                if (!TryInferBitmapSize(tensor, out _, out _, out var channels, out var isCHW) || isCHW) return false;

                var typeElements = Unsafe.SizeOf<TPixel>() / Unsafe.SizeOf<TElement>();
                if (channels != typeElements) return false;

                bitmap = new _TensorBitmap<TElement, TPixel>(tensor);
                return true;
            }

            private _TensorBitmap(System.Numerics.Tensors.Tensor<TElement> bitmap)
            {
                _VerifyTensorIsBitmapHWC<TElement, TPixel>(bitmap.Lengths, bitmap.Strides);

                _Bitmap = bitmap;

                Height = (int)bitmap.Lengths[0];
                Width = (int)bitmap.Lengths[1];

                _RowLength = (int)(bitmap.Lengths[1] * bitmap.Strides[1]);
            }

            #endregion

            #region data

            private readonly System.Numerics.Tensors.Tensor<TElement> _Bitmap;
            public int Width { get; }
            public int Height { get; }

            private readonly int _RowLength;

            #endregion

            #region API

            public ReadOnlySpan<TPixel> GetReadOnlyRowSpan(int y) { return GetRowSpan(y); }

            public Span<TPixel> GetRowSpan(int y)
            {
                Span<nint> iii = stackalloc nint[_Bitmap.Rank];
                iii[0] = y;

                var row = _Bitmap.GetSpan(iii, _RowLength);

                return System.Runtime.InteropServices.MemoryMarshal.Cast<TElement, TPixel>(row);
            }            

            #endregion
        }

        /// <summary>
        /// Wraps a <see cref="System.Numerics.Tensors.TensorSpan<TElement>"/> and exposes it as a bitmap
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("_TensorSpanBitmapHWC {Width}x{Height}x{typeof(TPixel).Name,nq}")]
        private readonly ref struct _TensorSpanBitmap<TElement, TPixel>
            where TElement: unmanaged
            where TPixel : unmanaged
        {
            #region lifecycle

            public static bool TryCreatePlanes(System.Numerics.Tensors.TensorSpan<TElement> tensor, out _TensorSpanBitmap<TElement, TElement> x, out _TensorSpanBitmap<TElement, TElement> y, out _TensorSpanBitmap<TElement, TElement> z)
            {
                x = y = z = default;

                if (tensor.IsEmpty) return false;
                tensor = tensor.SqueezeIfRequired();

                if (!TryInferBitmapSize(tensor, out _, out _, out var channels, out var isCHW) || !isCHW || channels != 3) return false;

                var typeElements = Unsafe.SizeOf<TPixel>() / Unsafe.SizeOf<TElement>();
                if (typeElements != 3) return false;

                var xyz = tensor.GetDimensionSpan(0);

                x = new _TensorSpanBitmap<TElement, TElement>(xyz[0]);
                y = new _TensorSpanBitmap<TElement, TElement>(xyz[1]);
                z = new _TensorSpanBitmap<TElement, TElement>(xyz[2]);
                return true;
            }

            public static _TensorSpanBitmap<TElement, TPixel> Create(System.Numerics.Tensors.TensorSpan<TElement> tensor)
            {
                if (!TryCreate(tensor, out var bitmap)) GuardIsBitmap(tensor);
                return bitmap;
            }

            public static bool TryCreate(System.Numerics.Tensors.TensorSpan<TElement> tensor, out _TensorSpanBitmap<TElement, TPixel> bitmap)
            {
                bitmap = default;

                if (tensor.IsEmpty) return false;
                tensor = tensor.SqueezeIfRequired();

                if (!TryInferBitmapSize(tensor, out _, out _, out var channels, out var isCHW) || isCHW) return false;

                var typeElements = Unsafe.SizeOf<TPixel>() / Unsafe.SizeOf<TElement>();
                if (channels != typeElements) return false;

                bitmap = new _TensorSpanBitmap<TElement, TPixel>(tensor);
                return true;
            }

            private _TensorSpanBitmap(System.Numerics.Tensors.TensorSpan<TElement> bitmap)
            {
                _VerifyTensorIsBitmapHWC<TElement, TPixel>(bitmap.Lengths, bitmap.Strides);

                _Bitmap = bitmap;

                Height = (int)bitmap.Lengths[0];
                Width = (int)bitmap.Lengths[1];

                _RowLength = (int)(bitmap.Lengths[1] * bitmap.Strides[1]);
            }

            #endregion

            #region data

            private readonly System.Numerics.Tensors.TensorSpan<TElement> _Bitmap;
            public int Width { get; }
            public int Height { get; }

            private readonly int _RowLength;

            #endregion

            #region API

            public void UpdateAllPixels(Func<TPixel,TPixel> pixelFunc)
            {
                for(int y=0; y < this.Height; ++y)
                {
                    var row = GetRowSpan(y);

                    for(int x=0; x < row.Length; ++x)
                    {
                        row[x] = pixelFunc(row[x]);
                    }
                }
            }

            public Span<TPixel> GetRowSpan(int y)
            {
                Span<nint> iii = stackalloc nint[_Bitmap.Rank];
                iii[0] = y;

                var row = _Bitmap.GetSpan(iii, _RowLength);

                return System.Runtime.InteropServices.MemoryMarshal.Cast<TElement, TPixel>(row);
            }

            public void CopyFrom<TPixelIn>(__IReadOnlyInteropBitmap<TPixelIn> src, Func<TPixelIn, TPixel> pixelConverter)
            {
                var rowsCount = Math.Min(src.Height, Height);

                for (int y = 0; y < rowsCount; y++)
                {
                    var srcRow = src.GetReadOnlyRowSpan(y);
                    var dstRow = GetRowSpan(y);
                    int rowLen = Math.Min(srcRow.Length, dstRow.Length);

                    for (int i = 0; i < rowLen; ++i)
                    {
                        dstRow[i] = pixelConverter(srcRow[i]);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Wraps a <see cref="System.Numerics.Tensors.ReadOnlyTensorSpan<TElement>"/> and exposes it as a bitmap
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("_ReadOnlyTensorSpanBitmapHWC {Width}x{Height}x{typeof(TPixel).Name,nq}")]
        private readonly ref struct _ReadOnlyTensorSpanBitmap<TElement, TPixel>
            where TElement : unmanaged
            where TPixel : unmanaged
        {
            #region lifecycle

            public static _ReadOnlyTensorSpanBitmap<TElement, TPixel> Create(System.Numerics.Tensors.ReadOnlyTensorSpan<TElement> tensor)
            {
                if (!TryCreate(tensor, out var bitmap)) GuardIsBitmap(tensor);
                return bitmap;
            }

            public static bool TryCreate(System.Numerics.Tensors.ReadOnlyTensorSpan<TElement> tensor, out _ReadOnlyTensorSpanBitmap<TElement, TPixel> bitmap)
            {
                bitmap = default;                
                tensor = tensor.SqueezeIfRequired();

                if (!TryInferBitmapSize(tensor, out _, out _, out var channels, out var isCHW) || isCHW) return false;

                var typeElements = Unsafe.SizeOf<TPixel>() / Unsafe.SizeOf<TElement>();
                if (channels != typeElements) return false;

                bitmap = new _ReadOnlyTensorSpanBitmap<TElement, TPixel>(tensor);
                return true;
            }

            private _ReadOnlyTensorSpanBitmap(System.Numerics.Tensors.ReadOnlyTensorSpan<TElement> bitmap)
            {
                _VerifyTensorIsBitmapHWC<TElement, TPixel>(bitmap.Lengths, bitmap.Strides);

                _Bitmap = bitmap;

                Height = (int)bitmap.Lengths[0];
                Width = (int)bitmap.Lengths[1];

                _RowLength = (int)(bitmap.Lengths[1]* bitmap.Strides[1]);
            }

            #endregion

            #region data

            private readonly System.Numerics.Tensors.ReadOnlyTensorSpan<TElement> _Bitmap;
            public int Width { get; }
            public int Height { get; }

            private readonly int _RowLength;

            #endregion

            #region API

            public ReadOnlySpan<TPixel> GetRowSpan(int y)
            {
                Span<nint> iii = stackalloc nint[_Bitmap.Rank];
                iii[0] = y;

                var row = _Bitmap.GetSpan(iii, _RowLength);

                return System.Runtime.InteropServices.MemoryMarshal.Cast<TElement, TPixel>(row);
            }

            public void CopyTo<TPixelOut>(__IInteropBitmap<TPixelOut> dst, Func<TPixel, TPixelOut> pixelConverter)
            {
                var rowsCount = Math.Min(dst.Height, Height);

                for (int y = 0; y < rowsCount; y++)
                {
                    var srcRow = GetRowSpan(y);
                    var dstRow = dst.GetRowSpan(y);
                    int rowLen = Math.Min(srcRow.Length, dstRow.Length);

                    for (int i = 0; i < rowLen; ++i)
                    {
                        dstRow[i] = pixelConverter(srcRow[i]);
                    }
                }
            }

            #endregion            
        }        

        #endregion
    }
}
