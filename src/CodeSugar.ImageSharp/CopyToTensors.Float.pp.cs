using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics.Tensors;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;

#nullable disable

using __SIXLABORS = SixLabors.ImageSharp;
using __SIXLABORSPIXFMT = SixLabors.ImageSharp.PixelFormats;

using ____XYZ = System.Numerics.Vector3;
using _____XYZW = System.Numerics.Vector4;

#if NET8_0_OR_GREATER
using __TENSORSPAN = System.Numerics.Tensors.TensorSpan<float>;
using __READONLYTENSORSPAN = System.Numerics.Tensors.ReadOnlyTensorSpan<float>;
#endif

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

        #pragma warning disable SYSLIB5001

        public static void CopyTo<TPixel>(Image src, __TENSORSPAN dst, bool dstIsBGR = false)
        {
            switch (src)
            {
                case null: throw new ArgumentNullException(nameof(src));
                case Image<__SIXLABORSPIXFMT.A8> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Abgr32> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Argb32> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr24> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr565> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra32> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra4444> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra5551> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Byte4> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfSingle> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector2> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector4> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L16> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L8> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La16> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La32> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte2> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte4> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort2> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort4> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rg32> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb24> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb48> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba1010102> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba32> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba64> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.RgbaVector> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short2> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short4> srcFmt: CopyTo(srcFmt, dst, dstIsBGR); break;                

                default: throw new NotImplementedException(src.GetType().Name);
            }
        }

        public static void CopyTo<TPixel>(Image<TPixel> src, __TENSORSPAN dst, bool dstIsBGR = false)
            where TPixel: unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            if (src == null) throw new ArgumentNullException(nameof(src));

            dst = dst.Squeeze();

            if (dst.Rank == 2)
            {
                Span<nint> dstIndices = stackalloc nint[2];

                var minRows = Math.Min(src.Height, (int)dst.Lengths[0]);

                for (int y = 0; y < minRows; y++)
                {
                    dstIndices[0] = y;
                    dstIndices[1] = 0;                    

                    var dstRowPix = dst.GetSpan(dstIndices, (int)dst.FlattenedLength);
                    var srcRowPix = src.DangerousGetPixelRowMemory(y).Span;

                    int pixLen = Math.Min(srcRowPix.Length, dstRowPix.Length);

                    // Do faster special cases for Lxx and Axx

                    __SIXLABORSPIXFMT.L16 pix = default;

                    var scale = 1f / (float)ushort.MaxValue;

                    for (int i = 0; i < pixLen; ++i)
                    {
                        pix.FromScaledVector4(srcRowPix[i].ToScaledVector4());
                        dstRowPix[i] = scale * (float)pix.PackedValue;
                    }                    
                }
            }

            if (dst.Rank != 3) throw new ArgumentException("invalid rank or lengths", nameof(dst));

            if (dst.Lengths[2] == 3) // HWC
            {
                Span<nint> dstIndices = stackalloc nint[3];

                var minRows = Math.Min(src.Height, (int)dst.Lengths[0]);

                for (int y = 0; y < minRows; y++)
                {
                    dstIndices[0] = y;
                    dstIndices[1] = 0;
                    dstIndices[2] = 0;

                    var dstRow = dst.GetSpan(dstIndices, (int)dst.FlattenedLength);
                    var dstRowPix = System.Runtime.InteropServices.MemoryMarshal.Cast<float, ____XYZ>(dstRow);

                    var srcRowPix = src.DangerousGetPixelRowMemory(y).Span;

                    int pixLen = Math.Min(srcRowPix.Length, dstRowPix.Length);

                    if (dstIsBGR)
                    {
                        for (int i = 0; i < pixLen; ++i)
                        {
                            var pix = srcRowPix[i].ToScaledVector4();
                            dstRowPix[i] = new ____XYZ(pix.Z, pix.Y, pix.X); // BGR
                        }
                    }
                    else
                    {
                        for (int i = 0; i < pixLen; ++i)
                        {
                            var pix = srcRowPix[i].ToScaledVector4();
                            dstRowPix[i] = new ____XYZ(pix.X, pix.Y, pix.Z); // RGB
                        }
                    }                        
                }

                return;
            }

            if (dst.Lengths[2] == 4) // HWC
            {
                Span<nint> dstIndices = stackalloc nint[3];

                var minRows = Math.Min(src.Height, (int)dst.Lengths[0]);

                for (int y = 0; y < src.Height; y++)
                {
                    dstIndices[0] = y;
                    dstIndices[1] = 0;
                    dstIndices[2] = 0;

                    var dstRow = dst.GetSpan(dstIndices, (int)dst.FlattenedLength);
                    var dstRowPix = System.Runtime.InteropServices.MemoryMarshal.Cast<float, _____XYZW>(dstRow);

                    var srcRowPix = src.DangerousGetPixelRowMemory(y).Span;

                    int pixLen = Math.Min(srcRowPix.Length, dstRowPix.Length);

                    if (dstIsBGR)
                    {
                        for (int i = 0; i < pixLen; ++i)
                        {
                            var pix = srcRowPix[i].ToScaledVector4();
                            dstRowPix[i] = new _____XYZW(pix.Z, pix.Y, pix.X, pix.W); // BGRA
                        }
                    }
                    else
                    {
                        for (int i = 0; i < pixLen; ++i)
                        {
                            dstRowPix[i] = srcRowPix[i].ToScaledVector4(); // RGBA
                        }
                    }
                }

                return;
            }

            if (dst.Lengths[0] == 3) // CHW
            {   
                var minRows = Math.Min(src.Height, (int)dst.Lengths[0]);

                for (int y = 0; y < minRows; y++)
                {
                    GetRowChannels(dst, y, dstIsBGR, out var dstRowR, out var dstRowG, out var dstRowB);

                    var srcRowPix = src.DangerousGetPixelRowMemory(y).Span;

                    int pixLen = Math.Min(srcRowPix.Length, dstRowR.Length);

                    for (int i = 0; i < pixLen; ++i)
                    {
                        var pix = srcRowPix[i].ToScaledVector4();
                        dstRowR[i] = pix.X;
                        dstRowG[i] = pix.Y;
                        dstRowB[i] = pix.Z;
                    }
                }

                return;
            }

            throw new ArgumentException("invalid lengths[2] range", nameof(dst));
        }

        public static void CopyTo(__READONLYTENSORSPAN src, Image dst, bool srcIsBGR = false)
        {
            switch (dst)
            {
                case null: throw new ArgumentNullException(nameof(src));
                case Image<__SIXLABORSPIXFMT.A8> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Abgr32> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Argb32> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr24> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgr565> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra32> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra4444> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Bgra5551> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Byte4> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfSingle> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector2> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.HalfVector4> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L16> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.L8> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La16> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.La32> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte2> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedByte4> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort2> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.NormalizedShort4> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rg32> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb24> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgb48> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba1010102> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba32> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Rgba64> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.RgbaVector> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short2> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;
                case Image<__SIXLABORSPIXFMT.Short4> dstFmt: CopyTo(src, dstFmt, srcIsBGR); break;

                default: throw new NotImplementedException(dst.GetType().Name);
            }
        }

        public static void CopyTo<TPixel>(__READONLYTENSORSPAN src, Image<TPixel> dst, bool srcIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            if (dst == null) throw new ArgumentNullException(nameof(dst));

            src = src.Squeeze();

            if (src.Rank == 2)
            {
                Span<nint> srcIndices = stackalloc nint[2];

                var minRows = Math.Min(dst.Height, (int)src.Lengths[0]);

                for (int y = 0; y < minRows; y++)
                {
                    srcIndices[0] = y;
                    srcIndices[1] = 0;                    

                    var srcRowPix = src.GetSpan(srcIndices, (int)src.FlattenedLength);
                    var dstRowPix = dst.DangerousGetPixelRowMemory(y).Span;

                    int pixLen = Math.Min(srcRowPix.Length, dstRowPix.Length);

                    TPixel pix = default;
                    __SIXLABORSPIXFMT.L16 tmp = default;

                    for (int i = 0; i < pixLen; ++i)
                    {
                        var l = srcRowPix[i];

                        tmp.PackedValue = (ushort)(Math.Clamp(l, 0, 1) * ushort.MaxValue);

                        pix.FromL16(tmp);

                        dstRowPix[i] = pix;
                    }
                }
            }

            if (src.Rank != 3) throw new ArgumentException("invalid rank or lengths", nameof(dst));

            if (src.Lengths[2] == 3)
            {
                Span<nint> srcIndices = stackalloc nint[3];

                var minRows = Math.Min(dst.Height, (int)src.Lengths[0]);

                for (int y = 0; y < minRows; y++)
                {
                    srcIndices[0] = y;
                    srcIndices[1] = 0;
                    srcIndices[2] = 0;

                    var srcRow = src.GetSpan(srcIndices, (int)src.FlattenedLength);
                    var srcRowPix = System.Runtime.InteropServices.MemoryMarshal.Cast<float, ____XYZ>(srcRow);

                    var dstRowPix = dst.DangerousGetPixelRowMemory(y).Span;

                    int pixLen = Math.Min(srcRowPix.Length, dstRowPix.Length);

                    TPixel pix = default;
                    _____XYZW tmp;

                    if (srcIsBGR)
                    {
                        for (int i = 0; i < pixLen; ++i)
                        {
                            var srcPix = srcRowPix[i];
                            tmp = new _____XYZW(srcPix.Z, srcPix.Y, srcPix.X, 1);

                            pix.FromScaledVector4(tmp);

                            dstRowPix[i] = pix;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < pixLen; ++i)
                        {
                            var srcPix = srcRowPix[i];
                            tmp = new _____XYZW(srcPix.X, srcPix.Y, srcPix.Z, 1);

                            pix.FromScaledVector4(tmp);

                            dstRowPix[i] = pix;
                        }
                    }
                }

                return;
            }

            if (src.Lengths[2] == 4)
            {
                Span<nint> srcIndices = stackalloc nint[3];

                var minRows = Math.Min(dst.Height, (int)src.Lengths[0]);

                for (int y = 0; y < minRows; y++)
                {
                    srcIndices[0] = y;
                    srcIndices[1] = 0;
                    srcIndices[2] = 0;

                    var srcRow = src.GetSpan(srcIndices, (int)src.FlattenedLength);
                    var srcRowPix = System.Runtime.InteropServices.MemoryMarshal.Cast<float, _____XYZW>(srcRow);

                    var dstRowPix = dst.DangerousGetPixelRowMemory(y).Span;

                    int pixLen = Math.Min(srcRowPix.Length, dstRowPix.Length);

                    TPixel pix = default;                    

                    if (srcIsBGR)
                    {
                        _____XYZW tmp;

                        for (int i = 0; i < pixLen; ++i)
                        {
                            var srcPix = srcRowPix[i];
                            tmp = new _____XYZW(srcPix.Z, srcPix.Y, srcPix.X, srcPix.W);

                            pix.FromScaledVector4(tmp);

                            dstRowPix[i] = pix;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < pixLen; ++i)
                        {
                            pix.FromScaledVector4(srcRowPix[i]);

                            dstRowPix[i] = pix;
                        }
                    }
                }

                return;
            }

            if (src.Lengths[0] == 3) // CHW
            {
                var minRows = Math.Min(dst.Height, (int)src.Lengths[0]);

                for (int y = 0; y < minRows; y++)
                {                    
                    GetRowChannels(src, y, srcIsBGR, out var srcRowR, out var srcRowG, out var srcRowB);

                    var dstRowPix = dst.DangerousGetPixelRowMemory(y).Span;

                    int pixLen = Math.Min(dstRowPix.Length, srcRowR.Length);

                    _____XYZW tmp = default;
                    tmp.W = 1;

                    TPixel pix = default;

                    for (int i = 0; i < pixLen; ++i)
                    {
                        tmp.X = srcRowR[i];
                        tmp.Y = srcRowG[i];
                        tmp.Z = srcRowB[i];

                        pix.FromScaledVector4(tmp);

                        dstRowPix[i] = pix;
                    }
                }

                return;
            }

            throw new ArgumentException("invalid lengths[2] range", nameof(dst));
        }

        private static void GetRowChannels(__READONLYTENSORSPAN tensor, int y, bool IsBGR, out ReadOnlySpan<float> channelR, out ReadOnlySpan<float> channelG, out ReadOnlySpan<float> channelB)
        {
            System.Diagnostics.Debug.Assert(tensor.Rank == 3, ".Rank must be 3");
            System.Diagnostics.Debug.Assert(tensor.Lengths[0] == 3, ".Lengths[0] must be 3");

            Span<nint> indices = stackalloc nint[3];

            int len = (int)tensor.Lengths[1];

            for(int i = 2; i < tensor.Lengths.Length; ++i)
            {
                len *= (int)tensor.Lengths[i];
            }

            indices[1] = y;
            indices[2] = 0;

            indices[0] = IsBGR ? 2 : 0;            
            channelR = tensor.GetSpan(indices, len);

            indices[0] = 1;            
            channelG = tensor.GetSpan(indices, len);

            indices[0] = IsBGR ? 0 : 2;            
            channelB = tensor.GetSpan(indices, len);
        }

        private static void GetRowChannels(__TENSORSPAN tensor, int y, bool IsBGR, out Span<float> channelR, out Span<float> channelG, out Span<float> channelB)
        {
            System.Diagnostics.Debug.Assert(tensor.Rank == 3, ".Rank must be 3");
            System.Diagnostics.Debug.Assert(tensor.Lengths[0] == 3, ".Lengths[0] must be 3");

            Span<nint> indices = stackalloc nint[3];

            int len = (int)tensor.Lengths[1];

            for (int i = 2; i < tensor.Lengths.Length; ++i)
            {
                len *= (int)tensor.Lengths[i];
            }

            indices[1] = y;
            indices[2] = 0;

            indices[0] = IsBGR ? 2 : 0;
            channelR = tensor.GetSpan(indices, len);

            indices[0] = 1;
            channelG = tensor.GetSpan(indices, len);

            indices[0] = IsBGR ? 0 : 2;
            channelB = tensor.GetSpan(indices, len);
        }

        #pragma warning disable SYSLIB5001

        #endif
    }

}
