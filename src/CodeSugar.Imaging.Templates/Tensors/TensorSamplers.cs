using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable

using __XY = System.Numerics.Vector2;
using __RGB = System.Numerics.Vector3;
using __RGBA = System.Numerics.Vector4;
using __RGBP = System.Numerics.Vector4;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        private readonly ref struct _StepSampler<TElement, TPixel>
            where TElement: unmanaged, INumber<TElement>
            where TPixel : unmanaged, _IPixelSample
        {
            #region lifecycle
            public _StepSampler(System.Numerics.Tensors.ReadOnlyTensorSpan<TElement> tensor)
            {
                _Bitmap = _ReadOnlyTensorSpanBitmap<TElement, TPixel>.Create(tensor);
                _Width = _Bitmap.Width;
                _Height = _Bitmap.Height;                
            }

            #endregion

            #region data

            private readonly _ReadOnlyTensorSpanBitmap<TElement, TPixel> _Bitmap;
            private readonly float _Width;
            private readonly float _Height;

            private static readonly __XY _Half = __XY.One / 2;

            #endregion

            #region API

            public bool TryGetSample(__XY point, out __RGB pixel)
            {
                pixel = default;

                // center pixel
                point -= _Half;

                if (point.X < 0) return false;
                if (point.X >= _Width) return false;
                if (point.Y < 0) return false;
                if (point.Y >= _Height) return false;

                var xx = (int)MathF.Floor(point.X);
                var yy = (int)MathF.Floor(point.Y);
                pixel = _Bitmap.GetRowSpan(yy)[xx].ToScaledVector3();

                return true;
            }

            public bool TryGetSample(__XY point, out __RGBP pixel)
            {
                pixel = default;

                // center pixel
                point -= _Half;

                if (point.X < 0) return false;
                if (point.X >= _Width) return false;
                if (point.Y < 0) return false;
                if (point.Y >= _Height) return false;

                var xx = (int)MathF.Floor(point.X);
                var yy = (int)MathF.Floor(point.Y);
                pixel = _Bitmap.GetRowSpan(yy)[xx].ToScaledPremul();

                return true;
            }


            #endregion
        }

        private readonly ref struct _BilinearSampler<TElement, TPixel>
            where TElement : unmanaged, INumber<TElement>
            where TPixel : unmanaged, _IPixelSample
        {
            #region lifecycle
            public _BilinearSampler(System.Numerics.Tensors.ReadOnlyTensorSpan<TElement> tensor)
            {
                _Bitmap = _ReadOnlyTensorSpanBitmap<TElement, TPixel>.Create(tensor);
                _Width = _Bitmap.Width;
                _Height = _Bitmap.Height;
            }
            #endregion

            #region data

            private readonly _ReadOnlyTensorSpanBitmap<TElement, TPixel> _Bitmap;

            private readonly int _Width;
            private readonly int _Height;            

            private static readonly __XY _Half = __XY.One / 2;

            #endregion

            #region API

            /// <summary>
            /// Samples the image using bilinear interpolation at normalized coordinates (x, y).
            /// x and y are in pixel space.
            /// </summary>
            /// <param name="point">The X,Y coordinates (in pixel space)</param>
            /// <param name="pixel">The sampled result as Vector4</param>
            /// <returns>False if (x, y) is outside image bounds; otherwise true.</returns>
            public bool TryGetSample(__XY point, out __RGB pixel)
            {
                pixel = default;

                // center pixel
                point -= _Half;

                // Compute the integer coordinates of neighbors
                int x0 = (int)MathF.Floor(point.X);
                int y0 = (int)MathF.Floor(point.Y);
                int x1 = x0 + 1;
                int y1 = y0 + 1;

                // bounds check

                if (x0 >= 0 && x1 < _Width) // fully inside
                {
                    point.X -= x0;
                }
                else // partially inside ?
                {
                    if (x0 == -1) { x0 = x1; point.X = 1; }
                    else if (x1 == _Width) { x1 = x0; point.X = 0; }
                    else return false;
                }

                if (y0 >= 0 && y1 < _Height) // fully inside
                {
                    point.Y -= y0;
                }
                else // partially inside ?
                {
                    if (y0 == -1) { y0 = y1; point.Y = 1; }
                    else if (y1 == _Height) { y1 = y0; point.Y = 0; }
                    else return false;
                }

                // Fetch the 4 pixels
                var r = _Bitmap.GetRowSpan(y0);
                var p00 = r[x0].ToScaledVector3();
                var p10 = r[x1].ToScaledVector3();                

                r = _Bitmap.GetRowSpan(y1);
                var p01 = r[x0].ToScaledVector3();
                var p11 = r[x1].ToScaledVector3();

                // bilinear interpolation
                p00 = __RGB.Lerp(p00, p10, point.X);
                p01 = __RGB.Lerp(p01, p11, point.X);
                pixel = __RGB.Lerp(p00, p01, point.Y);

                return true;
                
            }

            /// <summary>
            /// Samples the image using bilinear interpolation at normalized coordinates (x, y).
            /// x and y are in pixel space.
            /// </summary>
            /// <param name="point">The X,Y coordinates (in pixel space)</param>
            /// <param name="pixel">The sampled result as Vector4</param>
            /// <returns>False if (x, y) is outside image bounds; otherwise true.</returns>
            public bool TryGetSample(__XY point, out __RGBP pixel)
            {
                pixel = default;

                // center pixel
                point -= _Half;

                // Compute the integer coordinates of neighbors
                int x0 = (int)MathF.Floor(point.X);
                int y0 = (int)MathF.Floor(point.Y);
                int x1 = x0 + 1;
                int y1 = y0 + 1;                

                // bounds check

                if (x0 >= 0 && x1 < _Width) // fully inside
                {
                    point.X -= x0;
                }
                else // partially inside ?
                {
                    if (x0 == -1) { x0 = x1; point.X = 1; }
                    else if (x1 == _Width) { x1 = x0; point.X = 0; }
                    else return false;
                }

                if (y0 >= 0 && y1 < _Height) // fully inside
                {
                    point.Y -= y0;
                }
                else // partially inside ?
                {
                    if (y0 == -1) { y0 = y1; point.Y = 1; }
                    else if (y1 == _Height) { y1 = y0; point.Y = 0; }
                    else return false;
                }

                // Fetch the 4 pixels
                var r = _Bitmap.GetRowSpan(y0);
                var p00 = r[x0].ToScaledPremul();
                var p10 = r[x1].ToScaledPremul();                

                r = _Bitmap.GetRowSpan(y1);
                var p01 = r[x0].ToScaledPremul();
                var p11 = r[x1].ToScaledPremul();

                // bilinear interpolation
                p00 = __RGBP.Lerp(p00, p10, point.X);
                p01 = __RGBP.Lerp(p01, p11, point.X);
                pixel = __RGBP.Lerp(p00, p01, point.Y);

                return true;
            }

            #endregion
        }
    }
}
