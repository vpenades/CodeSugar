using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Numerics.Tensors;
using System.Text;
using System.Threading;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using __XY = System.Numerics.Vector2;

#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESIXLABORSNAMESPACE
namespace SixLabors.ImageSharp
#else
namespace $rootnamespace$
#endif
{
    internal static partial class CodeSugarForImageSharp
    {
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

        /// <summary>
        /// Gets a function that can be used to randomly sample pixels from an image.
        /// </summary>
        /// <typeparam name="TPixel">The pixel type</typeparam>
        /// <param name="image">The source image</param>
        /// <param name="bilinear">true to enable bilinear filtering</param>
        /// <returns>The sampled pixel, or default value if the coordinates are outside the image</returns>
        public static Func<__XY, Vector4> GetScaledVector4Sampler<TPixel>(this Image<TPixel> image, bool bilinear = true)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return bilinear
                ? (p => new _BilinearSampler<TPixel>(image).TryGetScaledVectorSample(p, out var pixel) ? pixel : default)
                : (p => new _StepSampler<TPixel>(image).TryGetScaledVectorSample(p, out var pixel) ? pixel : default);
        }

        

        private readonly struct _StepSampler<TPixel>
            where TPixel : unmanaged, IPixel<TPixel>
        {
            public _StepSampler(Image<TPixel> image)
            {
                if (image == null) throw new ArgumentNullException(nameof(image));
                _Buffer = image.Frames.RootFrame.PixelBuffer;
                _Width = image.Width;
                _Height = image.Height;
            }

            private readonly SixLabors.ImageSharp.Memory.Buffer2D<TPixel> _Buffer;
            private readonly float _Width;
            private readonly float _Height;

            private static readonly __XY _Half = __XY.One / 2;

            public bool TryGetScaledVectorSample(__XY point, out Vector4 pixel)
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
                pixel = _Buffer.DangerousGetRowSpan(yy)[xx].ToScaledVector4();

                return true;
            }
        }

        private readonly struct _BilinearSampler<TPixel>
            where TPixel : unmanaged, IPixel<TPixel>
        {
            public _BilinearSampler(Image<TPixel> image)
            {
                if (image == null) throw new ArgumentNullException(nameof(image));
                _Buffer = image.Frames.RootFrame.PixelBuffer;
                _Width = image.Width;
                _Height = image.Height;
            }

            private readonly SixLabors.ImageSharp.Memory.Buffer2D<TPixel> _Buffer;

            private readonly int _Width;
            private readonly int _Height;

            private static readonly __XY _Half = __XY.One / 2;

            public int Width => _Width;
            public int Height => _Height;

            /// <summary>
            /// Samples the image using bilinear interpolation at normalized coordinates (x, y).
            /// x and y are in pixel space.
            /// </summary>
            /// <param name="x">The X coordinate (float, pixel space)</param>
            /// <param name="y">The Y coordinate (float, pixel space)</param>
            /// <param name="pixel">The sampled result as Vector4</param>
            /// <returns>False if (x, y) is outside image bounds; otherwise true.</returns>
            public bool TryGetScaledVectorSample(__XY point, out Vector4 pixel)
            {
                pixel = default;

                // center pixel
                point -= _Half;

                // Compute the integer coordinates of neighbors
                int x0 = (int)MathF.Floor(point.X);
                int y0 = (int)MathF.Floor(point.Y);
                int x1 = x0 + 1;
                int y1 = y0 + 1;

                // If outside image bounds, fail
                if (x0 < 0 || x1 >= _Width || y0 < 0 || y1 >= _Height) return false;

                // Interpolation weights
                float dx = point.X - x0;
                float dy = point.Y - y0;

                // Fetch the 4 pixels
                var r = _Buffer.DangerousGetRowSpan(y0);
                var p00 = r[x0].ToPremultipliedVector4();
                var p10 = r[x1].ToPremultipliedVector4();

                r = _Buffer.DangerousGetRowSpan(y1);
                var p01 = r[x0].ToPremultipliedVector4();
                var p11 = r[x1].ToPremultipliedVector4();

                // Bilinear interpolation
                p00 = Vector4.Lerp(p00, p10, dx);
                p01 = Vector4.Lerp(p01, p11, dx);

                pixel = Vector4.Lerp(p00, p01, dy)._Unpremultiply();

                return true;
            }
        }

        private readonly struct _ClampedBilinearSampler<TPixel>
            where TPixel : unmanaged, IPixel<TPixel>
        {
            public _ClampedBilinearSampler(Image<TPixel> image)
            {
                if (image == null) throw new ArgumentNullException(nameof(image));
                _Buffer = image.Frames.RootFrame.PixelBuffer;
                _Width = image.Width;
                _Height = image.Height;
            }

            private readonly SixLabors.ImageSharp.Memory.Buffer2D<TPixel> _Buffer;

            private readonly int _Width;
            private readonly int _Height;

            private static readonly __XY _Half = __XY.One / 2;

            public int Width => _Width;
            public int Height => _Height;

            /// <summary>
            /// Samples the image using bilinear interpolation at normalized coordinates (x, y).
            /// x and y are in pixel space.
            /// </summary>
            /// <param name="x">The X coordinate (float, pixel space)</param>
            /// <param name="y">The Y coordinate (float, pixel space)</param>
            /// <param name="pixel">The sampled result as Vector4</param>
            /// <returns>False if (x, y) is outside image bounds; otherwise true.</returns>
            public Vector4 GetScaledVectorSample(__XY point)
            {
                // center pixel
                point -= _Half;

                // Compute the integer coordinates of neighbors
                int x0 = Math.Clamp((int)MathF.Floor(point.X), 0, _Width - 1);
                int y0 = Math.Clamp((int)MathF.Floor(point.Y), 0, _Height - 1);
                int x1 = Math.Min(x0 + 1, _Width - 1);
                int y1 = Math.Min(y0 + 1, _Height - 1);

                // Interpolation weights
                float dx = point.X - x0;
                float dy = point.Y - y0;

                // Fetch the 4 pixels
                var r = _Buffer.DangerousGetRowSpan(y0);
                var p00 = r[x0].ToPremultipliedVector4();
                var p10 = r[x1].ToPremultipliedVector4();

                r = _Buffer.DangerousGetRowSpan(y1);
                var p01 = r[x0].ToPremultipliedVector4();
                var p11 = r[x1].ToPremultipliedVector4();

                // Bilinear interpolation
                p00 = Vector4.Lerp(p00, p10, dx);
                p01 = Vector4.Lerp(p01, p11, dx);

                return Vector4.Lerp(p00, p01, dy)._Unpremultiply();
            }

            public void CopySampleTo<TDstPixel>(__XY point, ref TDstPixel dst)
                where TDstPixel : unmanaged, IPixel<TDstPixel>
            {
                dst.FromScaledVector4( GetScaledVectorSample(point) );
            }
        }
    }
}
