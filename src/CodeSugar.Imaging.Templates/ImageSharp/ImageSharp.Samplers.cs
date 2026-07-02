using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Threading;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using __XY = System.Numerics.Vector2;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        

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
                ? (Func<__XY, Vector4>)(p => new _ImageSharpBilinearSampler<TPixel>(image).TryGetScaledVectorSample(p, out var pixel) ? pixel : default)
                : (Func<__XY, Vector4>)(p => new _ImageSharpStepSampler<TPixel>(image).TryGetScaledVectorSample(p, out var pixel) ? pixel : default);
        }

        

        private readonly struct _ImageSharpStepSampler<TPixel>
            where TPixel : unmanaged, IPixel<TPixel>
        {
            public _ImageSharpStepSampler(Image<TPixel> image)
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

        private readonly struct _ImageSharpBilinearSampler<TPixel>
            where TPixel : unmanaged, IPixel<TPixel>
        {
            public _ImageSharpBilinearSampler(Image<TPixel> image)
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
            /// <param name="point">The X,Y coordinates (in pixel space)</param>
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

                pixel = Vector4.Lerp(p00, p01, dy)._ImageSharpUnpremultiply();

                return true;
            }
        }

        private readonly struct _ImageSharpClampedBilinearSampler<TPixel>
            where TPixel : unmanaged, IPixel<TPixel>
        {
            public _ImageSharpClampedBilinearSampler(Image<TPixel> image)
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
            /// <param name="point">The X,Y coordinates (in pixel space)</param>
            /// <returns>the sampled value</returns>
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

                return Vector4.Lerp(p00, p01, dy)._ImageSharpUnpremultiply();
            }

            public void CopySampleTo<TDstPixel>(__XY point, ref TDstPixel dst)
                where TDstPixel : unmanaged, IPixel<TDstPixel>
            {
                dst.FromScaledVector4( GetScaledVectorSample(point) );
            }
        }
    }
}
