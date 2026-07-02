using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Numerics.Tensors;
using System.Runtime.Intrinsics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;



using SixLabors.ImageSharp.PixelFormats;

using TUnit;

using XYZ = System.Numerics.Vector3;
using XYZW = System.Numerics.Vector4;

namespace CodeSugar
{
    internal class SystemTensorsImagingTests
    {
        [Test]
        public async Task TestTensorIsBitmap()
        {
            var tensor1 = Tensor.Create<byte>(new byte[256 * 200 * 1], new nint[] { 256, 200, 1 });
            await Assert.That(tensor1.TryInferBitmapSize(out var w, out var h, out var c, out var isCHW)).IsTrue();
            await Assert.That(w).IsEqualTo(200);
            await Assert.That(h).IsEqualTo(256);
            await Assert.That(c).IsEqualTo(1);
            await Assert.That(isCHW).IsFalse();

            var tensor2 = Tensor.Create<byte>(new byte[256 * 200 * 3], new nint[] { 256, 200, 3 });
            await Assert.That(tensor2.TryInferBitmapSize(out w, out h, out c, out isCHW)).IsTrue();
            await Assert.That(w).IsEqualTo(200);
            await Assert.That(h).IsEqualTo(256);
            await Assert.That(c).IsEqualTo(3);
            await Assert.That(isCHW).IsFalse();

            var tensor3 = Tensor.Create<byte>(new byte[256 * 200 * 3], new nint[] { 3, 256, 200 });
            await Assert.That(tensor3.TryInferBitmapSize(out w, out h, out c, out isCHW)).IsTrue();
            await Assert.That(w).IsEqualTo(200);
            await Assert.That(h).IsEqualTo(256);
            await Assert.That(c).IsEqualTo(3);
            await Assert.That(isCHW).IsTrue();
        }

        [Test]
        [Arguments("RGB", "RGB")]
        [Arguments("RGB", "BGR")]
        [Arguments("RGB", "RGBA")]
        [Arguments("BGR", "RGBA")]
        [Arguments("RGB", "BGRA")]
        [Arguments("RGB", "ARGB")]
        [Arguments("RGBA", "RGBA")]
        [Arguments("RGBA", "RGB")]
        [Arguments("BGRA", "RGB")]
        [Arguments("ARGB", "RGB")]
        [Arguments("RGBA", "RGBA")]
        [Arguments("RGBA", "BGRA")]
        [Arguments("ARGB", "BGRA")]
        public async Task TestColorConversion(string srcFmt, string dstFmt)
        {
            foreach (var itemsLen in new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 50, 100, 128, 256, 1000, 1001, 1002, 5000, 20000 })
            {
                var mul = new XYZW(1, 2, 3, 4);
                var add = new XYZW(4, 3, 2, 1);

                mul = XYZW.One;
                add = XYZW.Zero;

                var srcBytes = new Byte[itemsLen * srcFmt.Length]; new Random().NextBytes(srcBytes);

                var dstImplPixels = new float[itemsLen * dstFmt.Length];
                var dstRefPixels = new float[itemsLen * dstFmt.Length];

                _ConvertPixelsImpl(srcBytes, srcFmt, dstImplPixels, dstFmt, mul, add);
                _ConvertPixelsRef(srcBytes, srcFmt, dstRefPixels, dstFmt, mul, add);

                await Assert.That(dstImplPixels).IsSequenceEqualTo(dstRefPixels);
            }
        }

        private static void _ConvertPixelsImpl(ReadOnlySpan<byte> src, string srcFmt, Span<float> dst, string dstFmt, XYZW mul, XYZW add)
        {
            var dstXYZ = System.Runtime.InteropServices.MemoryMarshal.Cast<float, XYZ>(dst);
            var dstXYZW = System.Runtime.InteropServices.MemoryMarshal.Cast<float, XYZW>(dst);

            var mul3 = new XYZ(mul.X, mul.Y, mul.Z);
            var add3 = new XYZ(add.X, add.Y, add.Z);

            switch (srcFmt)
            {
                case "RGB":
                    switch (dstFmt)
                    {
                        case "RGB": src.ConvertRGBtoRGB(dstXYZ, mul3, add3); break;
                        case "BGR": src.ConvertRGBtoBGR(dstXYZ, mul3, add3); break;
                        case "RGBA": src.ConvertRGBtoRGBA(dstXYZW, mul, add); break;
                        case "BGRA": src.ConvertRGBtoBGRA(dstXYZW, mul, add); break;
                        case "ARGB": src.ConvertRGBtoARGB(dstXYZW, mul, add); break;
                    }
                    break;
                case "BGR":
                    switch (dstFmt)
                    {
                        case "RGB": src.ConvertBGRtoRGB(dstXYZ, mul3, add3); break;
                        case "BGR": src.ConvertBGRtoBGR(dstXYZ, mul3, add3); break;
                        case "RGBA": src.ConvertBGRtoRGBA(dstXYZW, mul, add); break;
                        case "BGRA": src.ConvertBGRtoBGRA(dstXYZW, mul, add); break;
                        case "ARGB": src.ConvertBGRtoARGB(dstXYZW, mul, add); break;
                    }
                    break;
                case "RGBA":
                    switch (dstFmt)
                    {
                        case "RGB": src.ConvertRGBAtoRGB(dstXYZ, mul3, add3); break;
                        case "BGR": src.ConvertRGBAtoBGR(dstXYZ, mul3, add3); break;
                        case "RGBA": src.ConvertRGBAtoRGBA(dstXYZW, mul, add); break;
                        case "BGRA": src.ConvertRGBAtoBGRA(dstXYZW, mul, add); break;
                        case "ARGB": src.ConvertRGBAtoARGB(dstXYZW, mul, add); break;
                    }
                    break;
                case "BGRA":
                    switch (dstFmt)
                    {
                        case "RGB": src.ConvertBGRAtoRGB(dstXYZ, mul3, add3); break;
                        case "BGR": src.ConvertBGRAtoBGR(dstXYZ, mul3, add3); break;
                        case "RGBA": src.ConvertBGRAtoRGBA(dstXYZW, mul, add); break;
                        case "BGRA": src.ConvertBGRAtoBGRA(dstXYZW, mul, add); break;
                        case "ARGB": src.ConvertBGRAtoARGB(dstXYZW, mul, add); break;
                    }
                    break;
                case "ARGB":
                    switch (dstFmt)
                    {
                        case "RGB": src.ConvertARGBtoRGB(dstXYZ, mul3, add3); break;
                        case "BGR": src.ConvertARGBtoBGR(dstXYZ, mul3, add3); break;
                        case "RGBA": src.ConvertARGBtoRGBA(dstXYZW, mul, add); break;
                        case "BGRA": src.ConvertARGBtoBGRA(dstXYZW, mul, add); break;
                        case "ARGB": src.ConvertARGBtoARGB(dstXYZW, mul, add); break;
                    }
                    break;
            }
        }

        private static void _ConvertPixelsRef(ReadOnlySpan<byte> src, string srcFmt, Span<float> dst, string dstFmt, XYZW mul, XYZW add)
        {
            IReadOnlyList<int> _ComponentIndices(string fmt)
            {
                switch (fmt)
                {
                    case "RGB": return new int[] { 0, 1, 2 };
                    case "BGR": return new int[] { 2, 1, 0 };
                    case "RGBA": return new int[] { 0, 1, 2, 3 };
                    case "BGRA": return new int[] { 2, 1, 0, 3 };
                    case "ARGB": return new int[] { 1, 2, 3, 0 };
                    default: throw new NotImplementedException();
                }
            }

            var srcIdx = _ComponentIndices(srcFmt);
            var dstIdx = _ComponentIndices(dstFmt);

            var len = Math.Min(src.Length / srcIdx.Count, dst.Length / dstIdx.Count);

            dst.Fill(255); // missing alphas

            // transfer bytes

            for (int i = 0; i < len; ++i)
            {
                for (int j = 0; j < dstIdx.Count; ++j)
                {
                    // get from src
                    var val = j < srcIdx.Count
                        ? src[i * srcIdx.Count + srcIdx[j]]
                        : 255;

                    // set to dst
                    dst[i * dstIdx.Count + dstIdx[j]] = val;
                }
            }

            // convert to float, and apply MAD
            switch (dstIdx.Count)
            {
                case 4:
                    var dstXYZW = System.Runtime.InteropServices.MemoryMarshal.Cast<float, XYZW>(dst);
                    for (int i = 0; i < dstXYZW.Length; ++i)
                    {
                        dstXYZW[i] *= mul;
                        dstXYZW[i] += add;
                    }
                    break;
                case 3:
                    var dstXYZ = System.Runtime.InteropServices.MemoryMarshal.Cast<float, XYZ>(dst);
                    var mul3 = new XYZ(mul.X, mul.Y, mul.Z);
                    var add3 = new XYZ(add.X, add.Y, add.Z);
                    for (int i = 0; i < dstXYZ.Length; ++i)
                    {
                        dstXYZ[i] *= mul3;
                        dstXYZ[i] += add3;
                    }
                    break;
            }
        }

        [Test]
        public async Task TestTensorDrawing()
        {
            // load image
            var icon = ResourceInfo.From("CodeSugar.png");
            using var img = SixLabors.ImageSharp.Image.Load<Rgb24>(icon.FilePath);

            var srcBmp = img.ToTensor<Rgb24, float>(3, false);
            await Assert.That(srcBmp.TryInferBitmapSize(out var srcW, out var srcH, out var srcC, out _)).IsTrue();

            var crc = srcBmp.GetContentCrc32Checksum();
            await Assert.That(crc).IsEqualTo(4160516094u);

            // prepare render target

            var dstBmp = System.Numerics.Tensors.Tensor.Create(new float[512 * 512 * 3], new nint[] { 512, 512, 3 });

            // draw

            var xform = System.Numerics.Matrix3x2.CreateScale(2, 2) * System.Numerics.Matrix3x2.CreateRotation(0.1f);
            await Assert.That(xform.GetChecksum()).IsEqualTo(1408182021u);

            dstBmp.AsTensorSpan().DrawRgbPixelsOverRgb(xform, srcBmp, true);

            // save result

            AttachmentInfo.From("dstBmp.png").WriteObjectEx(f => dstBmp.ImageSharpSaveTo(f));

            // crc check
            
            crc = dstBmp.GetContentCrc32Checksum();

            #if NET8_0
            await Assert.That(crc).IsEqualTo(4282507014u); // it seems there's a slight difference in net 8 
            #endif

            #if NET10_0_OR_GREATER
            await Assert.That(crc).IsEqualTo(595744082u);
            #endif
        }
    }
}
