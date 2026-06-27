using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable

using __XY = System.Numerics.Vector2;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        private readonly struct _PixelSamplerTransform
        {
            #region lifecycle
            public _PixelSamplerTransform(Matrix3x2 transform)
            {
                if (!Matrix3x2.Invert(transform, out _InverseTransform))
                {
                    _InverseTransform = default;
                    System.Diagnostics.Debug.Fail("Invalid transform matrix");
                }

                _ForwardTransform = transform;

                var w = new __XY(transform.M11, transform.M12);
                var h = new __XY(transform.M21, transform.M22);
                PixelArea = Math.Abs(__XY.Dot(w, h));
                PixelLength = MathF.Sqrt(PixelArea);
            }

            #endregion

            #region data

            private readonly Matrix3x2 _ForwardTransform;
            private readonly Matrix3x2 _InverseTransform;

            private static readonly __XY _Half = __XY.One / 2;

            #endregion

            #region API

            /// <summary>
            /// Given the current transform, this is the area covered by a single pixel
            /// </summary>
            public float PixelArea { get; }

            /// <summary>
            /// Given the current transform, this is the length of a square pixel from side to side
            /// </summary>
            public float PixelLength { get; }

            /// <summary>
            /// Given the size of the source bitmap and this transform, it returns the rectangle in the destination bitmap
            /// </summary>            
            public System.Drawing.Rectangle GetCoverage(System.Drawing.Size bitmapSize)
            {
                var min = new __XY(float.MaxValue);
                var max = new __XY(float.MinValue);

                var p = __XY.Zero;

                p = __XY.Transform(p, _ForwardTransform);
                min = __XY.Min(min, p);
                max = __XY.Max(max, p);

                p.X = bitmapSize.Width;
                p.Y = 0;
                p = __XY.Transform(p, _ForwardTransform);
                min = __XY.Min(min, p);
                max = __XY.Max(max, p);

                p.X = bitmapSize.Width;
                p.Y = bitmapSize.Height;
                p = __XY.Transform(p, _ForwardTransform);
                min = __XY.Min(min, p);
                max = __XY.Max(max, p);

                p.X = 0;
                p.Y = bitmapSize.Height;
                p = __XY.Transform(p, _ForwardTransform);
                min = __XY.Min(min, p);
                max = __XY.Max(max, p);

                var r = System.Drawing.RectangleF.FromLTRB(float.Floor(min.X), float.Floor(min.Y), float.Ceiling(max.X), float.Ceiling(max.Y));

                return System.Drawing.Rectangle.Round(r);
            }

            /// <summary>
            /// Given a point in the destionation bitmap, it gets the point to sample in the source bitmap
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public __XY Transform(int x, int y)
            {
                return __XY.Transform(new __XY(x, y) + _Half, _InverseTransform);
            }

            #endregion
        }
    }
}
