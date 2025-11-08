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

using __XY = System.Numerics.Vector2;
using __XYZ = System.Numerics.Vector3;
using __XYZW = System.Numerics.Vector4;


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

        public static bool DangerousTryGetSpanTensor<TPixel>(this Image<TPixel> src, out System.Numerics.Tensors.TensorSpan<TPixel> dst)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            if (!src.DangerousTryGetSinglePixelMemory(out var memory))
            {
                dst = System.Numerics.Tensors.TensorSpan<TPixel>.Empty;                
                return false;
            }

            Span<nint> size = stackalloc nint[2];
            size[0] = src.Height;
            size[1] = src.Width;

            dst = new System.Numerics.Tensors.TensorSpan<TPixel>(memory.Span, size);

            return true;
        }

        public static unsafe bool DangerousTryGetSpanTensor<TPixel>(this Image<TPixel> src, out System.Numerics.Tensors.TensorSpan<Byte> dst)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            if (!src.DangerousTryGetSinglePixelMemory(out var memory))
            {
                dst = System.Numerics.Tensors.TensorSpan<byte>.Empty;
                return false;
            }

            var imgData = System.Runtime.InteropServices.MemoryMarshal.Cast<TPixel, byte>(memory.Span);

            Span<nint> size = stackalloc nint[3];
            size[0] = src.Height;
            size[1] = src.Width;
            size[2] = sizeof(TPixel);

            dst = new System.Numerics.Tensors.TensorSpan<byte>(imgData, size);

            return true;
        }

        public static void CopyToTensor(this Image src, System.Numerics.Tensors.ITensor dst, bool dstIsBGR = false)
        {
            if (dst == null || dst.IsEmpty) throw new ArgumentNullException("null or empty",nameof(dst));
            if (dst.IsReadOnly) throw new ArgumentException("is read only", nameof(dst));
            if (!dst.IsDense) throw new ArgumentException("is not dense", nameof(dst));
            
            if (dst is System.Numerics.Tensors.Tensor<float> ft)
            {
                var fts = ft.AsTensorSpan();
                CopyToTensor(src, fts, dstIsBGR);
                return;
            }

            throw new ArgumentException($"{dst.GetType().Name} not implemented", nameof(dst));
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

        public static void CopyToTensor<TPixel>(this Image<TPixel> src, __TENSORSPAN dst, bool dstIsBGR = false)
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

                var rowLen = (int)(dst.Lengths[1] * dst.Lengths[2]);

                for (int y = 0; y < minRows; y++)
                {
                    dstIndices[0] = y;
                    dstIndices[1] = 0;
                    dstIndices[2] = 0;

                    var dstRow = dst.GetSpan(dstIndices, rowLen);
                    var dstRowPix = System.Runtime.InteropServices.MemoryMarshal.Cast<float, __XYZ>(dstRow);

                    var srcRowPix = src.DangerousGetPixelRowMemory(y).Span;

                    int pixLen = Math.Min(srcRowPix.Length, dstRowPix.Length);

                    if (dstIsBGR)
                    {
                        for (int i = 0; i < pixLen; ++i)
                        {
                            var pix = srcRowPix[i].ToScaledVector4();
                            dstRowPix[i] = new __XYZ(pix.Z, pix.Y, pix.X); // BGR
                        }
                    }
                    else
                    {
                        for (int i = 0; i < pixLen; ++i)
                        {
                            var pix = srcRowPix[i].ToScaledVector4();
                            dstRowPix[i] = new __XYZ(pix.X, pix.Y, pix.Z); // RGB
                        }
                    }                        
                }

                return;
            }

            if (dst.Lengths[2] == 4) // HWC
            {
                Span<nint> dstIndices = stackalloc nint[3];

                var minRows = Math.Min(src.Height, (int)dst.Lengths[0]);

                var rowLen = (int)(dst.Lengths[1] * dst.Lengths[2]);

                for (int y = 0; y < src.Height; y++)
                {
                    dstIndices[0] = y;
                    dstIndices[1] = 0;
                    dstIndices[2] = 0;

                    var dstRow = dst.GetSpan(dstIndices, rowLen);
                    var dstRowPix = System.Runtime.InteropServices.MemoryMarshal.Cast<float, __XYZW>(dstRow);

                    var srcRowPix = src.DangerousGetPixelRowMemory(y).Span;

                    int pixLen = Math.Min(srcRowPix.Length, dstRowPix.Length);

                    if (dstIsBGR)
                    {
                        for (int i = 0; i < pixLen; ++i)
                        {
                            var pix = srcRowPix[i].ToScaledVector4();
                            dstRowPix[i] = new __XYZW(pix.Z, pix.Y, pix.X, pix.W); // BGRA
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

        public static void CopyToTensor(this Image src, System.Numerics.Matrix3x2 srcXform, System.Numerics.Tensors.ITensor dst, bool dstIsBGR = false)
        {
            if (dst == null || dst.IsEmpty) throw new ArgumentNullException("null or empty", nameof(dst));
            if (dst.IsReadOnly) throw new ArgumentException("is read only", nameof(dst));
            if (!dst.IsDense) throw new ArgumentException("is not dense", nameof(dst));

            if (dst is System.Numerics.Tensors.Tensor<float> ft)
            {
                var fts = ft.AsTensorSpan();
                CopyToTensor(src, srcXform, fts, dstIsBGR);
                return;
            }

            throw new ArgumentException($"{dst.GetType().Name} not implemented", nameof(dst));
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

        public static void CopyToTensor<TPixel>(this Image<TPixel> src, __TENSORSPAN dst, System.Numerics.Matrix3x2 srcXform, bool dstIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            var sampler = new _ClampedBilinearSampler<TPixel>(src);

            _CopyToTensor(sampler, srcXform, dst, dstIsBGR);
        }        

        private static void _CopyToTensor<TPixel>(this _ClampedBilinearSampler<TPixel> src, System.Numerics.Matrix3x2 srcXform, __TENSORSPAN dst, bool dstIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            var xformer = new _SamplerTransform(srcXform);

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

                    int pixLen = Math.Min(src.Width, dstRowPix.Length);                    

                    __SIXLABORSPIXFMT.L16 pix = default;

                    var scale = 1f / (float)ushort.MaxValue;

                    for (int x = 0; x < pixLen; ++x)
                    {
                        var v = xformer.Transform(x, y);
                        src.CopySampleTo(v.X, v.Y, ref pix);

                        dstRowPix[x] = scale * (float)pix.PackedValue;
                    }
                }
            }

            if (dst.Rank != 3) throw new ArgumentException("invalid rank or lengths", nameof(dst));

            if (dst.Lengths[2] == 3) // HWC
            {
                Span<nint> dstIndices = stackalloc nint[3];

                var minRows = Math.Min(src.Height, (int)dst.Lengths[0]);

                var rowLen = (int)(dst.Lengths[1] * dst.Lengths[2]);

                for (int y = 0; y < minRows; y++)
                {
                    dstIndices[0] = y;
                    dstIndices[1] = 0;
                    dstIndices[2] = 0;

                    var dstRow = dst.GetSpan(dstIndices, rowLen);
                    var dstRowPix = System.Runtime.InteropServices.MemoryMarshal.Cast<float, __XYZ>(dstRow);                    

                    int pixLen = Math.Min(src.Width, dstRowPix.Length);

                    if (dstIsBGR)
                    {
                        for (int x = 0; x < pixLen; ++x)
                        {
                            var v = xformer.Transform(x, y);
                            var pix = src.GetScaledVectorSample(v.X, v.Y);
                            
                            dstRowPix[x] = new __XYZ(pix.Z, pix.Y, pix.X); // BGR
                        }
                    }
                    else
                    {
                        for (int x = 0; x < pixLen; ++x)
                        {
                            var v = xformer.Transform(x, y);
                            var pix = src.GetScaledVectorSample(v.X, v.Y);

                            dstRowPix[x] = new __XYZ(pix.X, pix.Y, pix.Z); // RGB
                        }
                    }
                }

                return;
            }

            if (dst.Lengths[2] == 4) // HWC
            {
                Span<nint> dstIndices = stackalloc nint[3];

                var minRows = Math.Min(src.Height, (int)dst.Lengths[0]);

                var rowLen = (int)(dst.Lengths[1] * dst.Lengths[2]);

                for (int y = 0; y < minRows; y++)
                {
                    dstIndices[0] = y;
                    dstIndices[1] = 0;
                    dstIndices[2] = 0;

                    var dstRow = dst.GetSpan(dstIndices, rowLen);
                    var dstRowPix = System.Runtime.InteropServices.MemoryMarshal.Cast<float, __XYZW>(dstRow);

                    int pixLen = Math.Min(src.Width, dstRowPix.Length);

                    if (dstIsBGR)
                    {
                        for (int x = 0; x < pixLen; ++x)
                        {
                            var v = xformer.Transform(x, y);
                            var pix = src.GetScaledVectorSample(v.X, v.Y);

                            dstRowPix[x] = new __XYZW(pix.Z, pix.Y, pix.X, pix.W); // BGRA
                        }
                    }
                    else
                    {
                        for (int x = 0; x < pixLen; ++x)
                        {
                            var v = xformer.Transform(x, y);
                            var pix = src.GetScaledVectorSample(v.X, v.Y);

                            dstRowPix[x] = pix; // RGBA
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

                    int pixLen = Math.Min(src.Width, dstRowR.Length);

                    for (int x = 0; x < pixLen; ++x)
                    {
                        var v = xformer.Transform(x, y);
                        var pix = src.GetScaledVectorSample(v.X, v.Y);

                        dstRowR[x] = pix.X;
                        dstRowG[x] = pix.Y;
                        dstRowB[x] = pix.Z;
                    }
                }

                return;
            }

            throw new ArgumentException("invalid lengths[2] range", nameof(dst));
        }

        public static void CopyToSixLaborsImage(this ITensor src, Image dst, bool srcIsBGR = false)
        {
            if (src == null || src.IsEmpty) throw new ArgumentNullException("null or empty", nameof(dst));
            
            if (!src.IsDense) throw new ArgumentException("is not dense", nameof(dst));

            if (src is System.Numerics.Tensors.Tensor<float> ft)
            {
                var fts = ft.AsTensorSpan();
                CopyToSixLaborsImage(fts, dst, srcIsBGR);
                return;
            }

            throw new ArgumentException($"{dst.GetType().Name} not implemented", nameof(dst));
        }

        public static void CopyToSixLaborsImage(this __TENSORSPAN src, Image dst, bool srcIsBGR = false)
        {
            CopyToSixLaborsImage(src.AsReadOnlyTensorSpan(), dst, srcIsBGR);
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

        public static void CopyToSixLaborsImage<TPixel>(this __TENSORSPAN src, Image<TPixel> dst, bool srcIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            CopyToSixLaborsImage(src.AsReadOnlyTensorSpan(), dst, srcIsBGR);
        }

        public static void CopyToSixLaborsImage<TPixel>(this __READONLYTENSORSPAN src, Image<TPixel> dst, bool srcIsBGR = false)
            where TPixel : unmanaged, __SIXLABORSPIXFMT.IPixel<TPixel>
        {
            if (dst == null) throw new ArgumentNullException(nameof(dst));

            src = src.Squeeze();

            if (src.Rank == 2)
            {
                Span<nint> srcIndices = stackalloc nint[2];

                var minRows = Math.Min(dst.Height, (int)src.Lengths[0]);

                var rowLen = (int)(src.Lengths[1] * src.Lengths[2]);

                for (int y = 0; y < minRows; y++)
                {
                    srcIndices[0] = y;
                    srcIndices[1] = 0;                    

                    var srcRowPix = src.GetSpan(srcIndices, rowLen);
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

                var rowLen = (int)(src.Lengths[1] * src.Lengths[2]);

                for (int y = 0; y < minRows; y++)
                {
                    srcIndices[0] = y;
                    srcIndices[1] = 0;
                    srcIndices[2] = 0;

                    var srcRow = src.GetSpan(srcIndices, rowLen);
                    var srcRowPix = System.Runtime.InteropServices.MemoryMarshal.Cast<float, __XYZ>(srcRow);

                    var dstRowPix = dst.DangerousGetPixelRowMemory(y).Span;

                    int pixLen = Math.Min(srcRowPix.Length, dstRowPix.Length);

                    TPixel pix = default;
                    __XYZW tmp;

                    if (srcIsBGR)
                    {
                        for (int i = 0; i < pixLen; ++i)
                        {
                            var srcPix = srcRowPix[i];
                            tmp = new __XYZW(srcPix.Z, srcPix.Y, srcPix.X, 1);

                            pix.FromScaledVector4(tmp);

                            dstRowPix[i] = pix;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < pixLen; ++i)
                        {
                            var srcPix = srcRowPix[i];
                            tmp = new __XYZW(srcPix.X, srcPix.Y, srcPix.Z, 1);

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
                    var srcRowPix = System.Runtime.InteropServices.MemoryMarshal.Cast<float, __XYZW>(srcRow);

                    var dstRowPix = dst.DangerousGetPixelRowMemory(y).Span;

                    int pixLen = Math.Min(srcRowPix.Length, dstRowPix.Length);

                    TPixel pix = default;                    

                    if (srcIsBGR)
                    {
                        __XYZW tmp;

                        for (int i = 0; i < pixLen; ++i)
                        {
                            var srcPix = srcRowPix[i];
                            tmp = new __XYZW(srcPix.Z, srcPix.Y, srcPix.X, srcPix.W);

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

                    __XYZW tmp = default;
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

            int rowLen = (int)tensor.Lengths[1];            

            indices[1] = y;
            indices[2] = 0;

            indices[0] = IsBGR ? 2 : 0;            
            channelR = tensor.GetSpan(indices, rowLen);

            indices[0] = 1;            
            channelG = tensor.GetSpan(indices, rowLen);

            indices[0] = IsBGR ? 0 : 2;            
            channelB = tensor.GetSpan(indices, rowLen);
        }

        private static void GetRowChannels(__TENSORSPAN tensor, int y, bool IsBGR, out Span<float> channelR, out Span<float> channelG, out Span<float> channelB)
        {
            System.Diagnostics.Debug.Assert(tensor.Rank == 3, ".Rank must be 3");
            System.Diagnostics.Debug.Assert(tensor.Lengths[0] == 3, ".Lengths[0] must be 3");

            Span<nint> indices = stackalloc nint[3];

            int rowLen = (int)tensor.Lengths[1];            

            indices[1] = y;
            indices[2] = 0;

            indices[0] = IsBGR ? 2 : 0;
            channelR = tensor.GetSpan(indices, rowLen);

            indices[0] = 1;
            channelG = tensor.GetSpan(indices, rowLen);

            indices[0] = IsBGR ? 0 : 2;
            channelB = tensor.GetSpan(indices, rowLen);
        }

        #pragma warning disable SYSLIB5001

        #endif
    }

}
