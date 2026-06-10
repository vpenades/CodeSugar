// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable

using __XY = System.Numerics.Vector2;
using __RGB = System.Numerics.Vector3;
using __RGBA = System.Numerics.Vector4;
using __RGBP = System.Numerics.Vector4;

using __METHODOPTIONS = System.Runtime.CompilerServices.MethodImplOptions;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.Numerics.Tensors
#else
namespace $rootnamespace$
#endif
{
    internal static partial class CodeSugarForTensors
    {
        public static void DrawRgbPixelsOverRgb(this System.Numerics.Tensors.TensorSpan<float> dstBitmap, System.Numerics.Matrix3x2 srcTransform, System.Numerics.Tensors.ITensor srcBitmap, bool useBilinear)
        {
            switch(srcBitmap)
            {
                case System.Numerics.Tensors.Tensor<byte> typedSrc:
                    switch(useBilinear)
                    {
                        case false: DrawRgbPixelsOverRgbStep(dstBitmap, srcTransform, typedSrc.AsReadOnlyTensorSpan()); break;
                        case true: DrawRgbPixelsOverRgbBilinear(dstBitmap, srcTransform, typedSrc.AsReadOnlyTensorSpan()); break;
                    } break;
                case System.Numerics.Tensors.Tensor<float> typedSrc:
                    switch (useBilinear)
                    {
                        case false: DrawRgbPixelsOverRgbStep(dstBitmap, srcTransform, typedSrc.AsReadOnlyTensorSpan()); break;
                        case true: DrawRgbPixelsOverRgbBilinear(dstBitmap, srcTransform, typedSrc.AsReadOnlyTensorSpan()); break;
                    }
                    break;
            }            
        }        

        private readonly ref struct _StepSampler<TElement, TPixel>
            where TElement: unmanaged
            where TPixel : unmanaged, _IPixelSample
        {
            #region lifecycle
            public _StepSampler(System.Numerics.Tensors.ReadOnlyTensorSpan<TElement> bitmap)
            {
                _Bitmap = new _ReadOnlyTensorSpanBitmapHWC<TElement, TPixel>(bitmap);
                _Width = _Bitmap.Width;
                _Height = _Bitmap.Height;                
            }

            #endregion

            #region data

            private readonly _ReadOnlyTensorSpanBitmapHWC<TElement, TPixel> _Bitmap;
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
            where TElement : unmanaged
            where TPixel : unmanaged, _IPixelSample
        {
            #region lifecycle
            public _BilinearSampler(System.Numerics.Tensors.ReadOnlyTensorSpan<TElement> bitmap)
            {
                _Bitmap = new _ReadOnlyTensorSpanBitmapHWC<TElement, TPixel>(bitmap);
                _Width = _Bitmap.Width;
                _Height = _Bitmap.Height;
            }
            #endregion

            #region data

            private readonly _ReadOnlyTensorSpanBitmapHWC<TElement, TPixel> _Bitmap;

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

        private readonly struct _SamplerTransform
        {
            public _SamplerTransform(Matrix3x2 transform)
            {
                if (!Matrix3x2.Invert(transform, out _InverseTransform))
                {
                    _InverseTransform = default;
                    System.Diagnostics.Debug.Fail("Invalid transform matrix");
                }

                var w = new __XY(transform.M11, transform.M12);
                var h = new __XY(transform.M21, transform.M22);
                PixelArea = Math.Abs(__XY.Dot(w, h));
                PixelSize = MathF.Sqrt(PixelArea);
            }

            private readonly Matrix3x2 _InverseTransform;

            private static readonly __XY _Half = __XY.One / 2;

            public float PixelArea { get; }
            public float PixelSize { get; }

            public __XY Transform(int x, int y)
            {
                return __XY.Transform(new __XY(x, y) + _Half, _InverseTransform);
            }
        }        
    }
}
