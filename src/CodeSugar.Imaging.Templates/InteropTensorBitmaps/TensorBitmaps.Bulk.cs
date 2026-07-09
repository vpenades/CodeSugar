using System;
using System.Numerics;
using System.Runtime.CompilerServices;

using InteropTypes.Numerics;
using InteropTypes.TensorBitmaps;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        /// <summary>
        /// Request a tensor bitmap to a client.
        /// </summary>        
        /// <returns>
        /// A bitmap which may or may not have the requeted dimensions due to the underlaying model's constraints
        /// </returns>        
        public delegate TensorSpanBitmap<TElement, TPixel> __TensorSpanBitmapRequestDelegate<TElement, TPixel>(int requestedWidth, int requestedHeight)
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged;

        /// <summary>
        /// Request a tensor bitmap to a client.
        /// </summary>        
        /// <returns>
        /// A bitmap which may or may not have the requeted dimensions due to the underlaying model's constraints
        /// </returns>        
        public delegate TensorSpanPlanes3<TElement> __TensorSpanPlanes3RequestDelegate<TElement>(int requestedWidth, int requestedHeight)
            where TElement : unmanaged, INumber<TElement>;            

        public static TensorBitmap<TDstElement, TDstPixel> ConvertTo<TSrcElement, TSrcPixel, TDstElement, TDstPixel>(this ReadOnlyTensorSpanBitmap<TSrcElement, TSrcPixel> source, PixelFormat dstFmt)
            where TSrcElement : unmanaged, INumber<TSrcElement>
            where TDstElement : unmanaged, INumber<TDstElement>
            where TSrcPixel : unmanaged
            where TDstPixel : unmanaged
        {
            var dst = TensorBitmap<TDstElement, TDstPixel>.Create(source.Width, source.Height, dstFmt);
            source.CopyPixelsTo(dst);
            return dst;
        }

        public static void CopyPixelsToCenter<TSrcElement,TSrcPixel,TDstElement,TDstPixel>(this ReadOnlyTensorSpanBitmap<TSrcElement,TSrcPixel> source, TensorSpanBitmap<TDstElement,TDstPixel> target)
            where TSrcElement: unmanaged, INumber<TSrcElement>
            where TDstElement : unmanaged, INumber<TDstElement>
            where TSrcPixel: unmanaged
            where TDstPixel: unmanaged
        {
            // calculate source and target crops, so they fit at the center
            var w = Math.Min(source.Width, target.Width);
            var h = Math.Min(source.Height, target.Height);
            
            source = GetCroppedAtCenter(source, w, h);
            target = GetCroppedAtCenter(target, w, h);

            source.CopyPixelsTo(target);
        }

        public static void CopyPixelsToCenter<TSrcElement, TSrcPixel, TDstElement>(this ReadOnlyTensorSpanBitmap<TSrcElement, TSrcPixel> source, TensorSpanPlanes3<TDstElement> target)
            where TSrcElement : unmanaged, INumber<TSrcElement>
            where TDstElement : unmanaged, INumber<TDstElement>
            where TSrcPixel : unmanaged            
        {
            // calculate source and target crops, so they fit at the center
            var w = Math.Min(source.Width, target.Width);
            var h = Math.Min(source.Height, target.Height);

            source = GetCroppedAtCenter(source, w, h);
            target = GetCroppedAtCenter(target, w, h);

            target.CopyPixelsFrom(source);
        }

        private static ReadOnlyTensorSpanBitmap<TElement, TPixel> GetCroppedAtCenter<TElement, TPixel>(this ReadOnlyTensorSpanBitmap<TElement, TPixel> source, int w, int h)
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged
        {
            w = Math.Min(source.Width, w);
            h = Math.Min(source.Height, h);

            var x = Math.Max(0, source.Width - w) / 2;
            var y = Math.Max(0, source.Height - h) / 2;

            var r = new System.Drawing.Rectangle(x, y, w, h);

            return source.GetCropped(r);
        }

        private static TensorSpanPlanes3<TElement> GetCroppedAtCenter<TElement>(this TensorSpanPlanes3<TElement> source, int w, int h)
            where TElement : unmanaged, INumber<TElement>            
        {
            w = Math.Min(source.Width, w);
            h = Math.Min(source.Height, h);

            var x = Math.Max(0, source.Width - w) / 2;
            var y = Math.Max(0, source.Height - h) / 2;

            var r = new System.Drawing.Rectangle(x, y, w, h);

            return source.GetCropped(r);
        }

        private static TensorSpanBitmap<TSrcElement, TSrcPixel> GetCroppedAtCenter<TSrcElement, TSrcPixel>(this TensorSpanBitmap<TSrcElement, TSrcPixel> source, int w, int h)
            where TSrcElement : unmanaged, INumber<TSrcElement>
            where TSrcPixel : unmanaged
        {
            w = Math.Min(source.Width, w);
            h = Math.Min(source.Height, h);

            var x = Math.Max(0, source.Width - w) / 2;
            var y = Math.Max(0, source.Height - h) / 2;

            var r = new System.Drawing.Rectangle(x, y, w, h);

            return source.GetCropped(r);
        }
    }
}
