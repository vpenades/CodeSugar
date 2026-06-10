using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics.Tensors;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

using __SIXLABORS = SixLabors.ImageSharp;
using __SIXLABORSPIXFMT = SixLabors.ImageSharp.PixelFormats;

using __XY = System.Numerics.Vector2;
using __XYZ = System.Numerics.Vector3;
using __XYZW = System.Numerics.Vector4;

#if NET8_0_OR_GREATER
using __TENSORSPAN = System.Numerics.Tensors.TensorSpan<byte>;
using __READONLYTENSORSPAN = System.Numerics.Tensors.ReadOnlyTensorSpan<byte>;
#endif


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
        #if NET8_0_OR_GREATER       

        public static bool DangerousTryGetSpanTensor<TPixel>(this Image<TPixel> src, out __TENSORSPAN dst)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            if (!src.DangerousTryGetSinglePixelMemory(out var memory))
            {
                dst = __TENSORSPAN.Empty;
                return false;
            }

            var imgData = System.Runtime.InteropServices.MemoryMarshal.Cast<TPixel, byte>(memory.Span);

            Span<nint> size = stackalloc nint[3];
            size[0] = src.Height;
            size[1] = src.Width;
            size[2] = System.Runtime.CompilerServices.Unsafe.SizeOf<TPixel>();

            dst = new __TENSORSPAN(imgData, size);

            return true;
        }

        public static void CopyToTensor(this Image src, __TENSORSPAN dst, bool dstIsBGR = false)
        {
            switch (src)
            {
                case null: throw new ArgumentNullException(nameof(src));
                case Image<__SIXLABORSPIXFMT.A8> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Abgr32> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Argb32> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr24> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr565> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra32> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra4444> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra5551> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Byte4> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfSingle> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector2> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector4> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L16> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L8> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La16> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La32> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte2> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte4> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort2> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort4> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rg32> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb24> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb48> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba1010102> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba32> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba64> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.RgbaVector> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short2> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short4> srcFmt: CopyToTensor(srcFmt, dst, dstIsBGR); break;

                default: throw new NotImplementedException(src.GetType().Name);
            }
        }

        public static void CopyToTensor(this Image src, System.Numerics.Matrix3x2 srcXform, __TENSORSPAN dst, bool dstIsBGR = false)
        {
            switch (src)
            {
                case null: throw new ArgumentNullException(nameof(src));
                case Image<__SIXLABORSPIXFMT.A8> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Abgr32> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Argb32> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr24> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr565> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra32> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra4444> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra5551> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Byte4> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfSingle> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector2> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector4> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L16> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L8> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La16> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La32> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte2> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte4> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort2> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort4> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rg32> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb24> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb48> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba1010102> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba32> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba64> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.RgbaVector> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short2> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short4> srcFmt: CopyToTensor(srcFmt, srcXform, dst, dstIsBGR); break;

                default: throw new NotImplementedException(src.GetType().Name);
            }
        }

        public static void CopyToSixLaborsImage(this __READONLYTENSORSPAN src, Image dst, bool srcIsBGR = false)
        {
            switch (dst)
            {
                case null: throw new ArgumentNullException(nameof(src));
                case Image<__SIXLABORSPIXFMT.A8> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Abgr32> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Argb32> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr24> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr565> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra32> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra4444> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra5551> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Byte4> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfSingle> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector2> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector4> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L16> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L8> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La16> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La32> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte2> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte4> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort2> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort4> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rg32> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb24> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb48> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba1010102> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba32> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba64> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.RgbaVector> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short2> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short4> dstFmt: CopyToSixLaborsImage(src, dstFmt, srcIsBGR); break;

                default: throw new NotImplementedException(dst.GetType().Name);
            }
        }


        public static void CopyToTensor<TPixel>(this Image<TPixel> src, __TENSORSPAN dst, bool dstIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            throw new NotImplementedException();
        }

        public static void CopyToTensor<TPixel>(this Image<TPixel> src, System.Numerics.Matrix3x2 srcXform, __TENSORSPAN dst, bool dstIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            throw new NotImplementedException();
        }

        public static void CopyToSixLaborsImage<TPixel>(this __READONLYTENSORSPAN src, Image<TPixel> dst, bool srcIsBGR = false)
           where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            throw new NotImplementedException();
        }

        #endif
    }

}
