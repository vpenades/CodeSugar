using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Numerics.Tensors;
using System.Text;
using System.Threading;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
        public static void DrawImage<TSrcPixel>(this IImageProcessingContext source, Image<TSrcPixel> foreground, System.Numerics.Matrix3x2 foregroundTransform, float opacity = 1)
            where TSrcPixel : unmanaged, IPixel<TSrcPixel>
        {
            opacity = Math.Clamp(opacity, 0, 1);

            // unused
            // var pixelBlender = PixelOperations<RgbaVector>.Instance.GetPixelBlender(new GraphicsOptions());
            

            if (foregroundTransform.M11 == 1 && foregroundTransform.M12 == 0 && foregroundTransform.M21 == 0 && foregroundTransform.M22 == 1) // translation only
            {
                var x = (int)MathF.Floor(foregroundTransform.M31);
                var y = (int)MathF.Floor(foregroundTransform.M32);

                source.DrawImage(foreground, new Point(x, y), opacity);
                return;
            }

            // var p = new Point((int)cmd.Transform.Translation.X, (int)cmd.Transform.Translation.Y);
            // target.Mutate(ctx => ctx.DrawImage(cmd.Image, p, 1));
            // continue;

            // https://github.com/SixLabors/ImageSharp/discussions/1764
            // https://github.com/SixLabors/ImageSharp.Drawing/discussions/124
            // but it seems it only works on vectors
            // var xform = System.Numerics.Matrix3x2.CreateScale(1, 1);
            // xform.Translation = -new System.Numerics.Vector2(cmd.X - x, cmd.Y - y);
            // target.Mutate(ctx => ctx.SetDrawingTransform(xform).DrawImage(cmd.Image, 1));

            /*
            var brush = new ImageBrush(sprite);
            var options = new DrawingOptions();
            options.Transform = spriteTransform;
            var rect = new RectangleF(0, 0, sprite.Width, sprite.Height);
            // target.Mutate(ctx => ctx.Fill(options, brush, rect));
            */

            // works for fonts and shapes, but NOT for images
            // target.Mutate(ctx => ctx.SetDrawingTransform(spriteTransform).DrawImage(sprite, 1));

            // low level fallback

            var xformer = new _SamplerTransform(foregroundTransform);
            var sampler = new _BilinearSampler<TSrcPixel>(foreground);

            void _processRowWithAlpha(Span<Vector4> dstSpan, Point value)
            {
                for (int i = 0; i < dstSpan.Length; i++)
                {
                    var v = xformer.Transform(value.X + i, value.Y);
                    if (!sampler.TryGetScaledVectorSample(v, out var forePixel)) continue;                    

                    forePixel.W *= opacity;

                    dstSpan[i] = ComposeScaledVectorNormal(dstSpan[i], forePixel);
                }
            }

            void _processRowWithOpaque(Span<Vector4> dstSpan, Point value)
            {
                for (int i = 0; i < dstSpan.Length; i++)
                {
                    var v = xformer.Transform(value.X + i, value.Y);
                    if (!sampler.TryGetScaledVectorSample(v, out var forePixel)) continue;

                    dstSpan[i] = forePixel;
                }
            }

            // TODO: calculate ProcessPixelRowsAsVector4 rectangle to minimize processing area

            if (HasAlphaChannel<TSrcPixel>() || opacity != 1)
            {
                source.ProcessPixelRowsAsVector4(_processRowWithAlpha);
            }
            else
            {
                source.ProcessPixelRowsAsVector4(_processRowWithOpaque);
            }
        }

        
    }
}
