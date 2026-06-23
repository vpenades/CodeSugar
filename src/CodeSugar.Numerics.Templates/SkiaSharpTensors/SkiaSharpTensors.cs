using System;
using System.Collections.Generic;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Text;

using SkiaSharp;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarNumericsExtensions
    {        
        private static int GetByteStride(SKColorType colorType)
        {
            return colorType switch
            {
                SKColorType.Gray8 => 1,
                SKColorType.Rgb565 => 2,
                SKColorType.Argb4444 => 2,
                SKColorType.Rgba8888 => 4,
                SKColorType.Bgra8888 => 4,
                SKColorType.Rgb888x => 4,
                SKColorType.Rgb101010x => 4,
                SKColorType.Rgba1010102 => 4,
                SKColorType.Bgra1010102 => 4,
                _ => 0
            };
        }

        private static int GetChannelCount(SKColorType colorType)
        {
            return colorType switch
            {
                SKColorType.Gray8 => 1,
                SKColorType.Rgb565 => 3,
                SKColorType.Argb4444 => 4,
                SKColorType.Rgba8888 => 4,
                SKColorType.Bgra8888 => 4,
                SKColorType.Rgb888x => 3,
                SKColorType.Rgb101010x => 3,
                SKColorType.Rgba1010102 => 4,
                SKColorType.Bgra1010102 => 4,
                _ => 0
            };
        }

        static bool IsBGRFormat(SKColorType colorType)
        {
            switch (colorType)
            {
                case SKColorType.Bgra8888:
                case SKColorType.Bgra1010102:
                case SKColorType.Bgr101010x:
                case SKColorType.Bgr101010xXR:
                    return true;
                default: return false;
            }
        }


        [System.Diagnostics.DebuggerDisplay("{X} {Y} {Z} {Alpha}")]
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        private struct _SkiaSharpPremulPixel
        {
            #region data

            public Byte X;
            public Byte Y;
            public Byte Z;
            public Byte Alpha;

            #endregion

            #region API

            [MethodImpl(AGRESSIVE)]
            public static Vector3 ToVector3Forward(in _SkiaSharpPremulPixel pixel)
            {
                if (pixel.Alpha == 0) return Vector3.Zero;

                // One reciprocal, three multiplications
                uint reciprocal = (255 << 16) / (uint)pixel.Alpha;  // Fixed-point reciprocal (16.16 or similar)

                var xx = (pixel.X * reciprocal) >> 16;
                var yy = (pixel.Y * reciprocal) >> 16;
                var zz = (pixel.Z * reciprocal) >> 16;

                return new Vector3(xx, yy, zz);
            }

            [MethodImpl(AGRESSIVE)]
            public static Vector4 ToVector4Forward(in _SkiaSharpPremulPixel pixel)
            {
                if (pixel.Alpha == 0) return Vector4.Zero;

                // One reciprocal, three multiplications
                uint reciprocal = (255 << 16) / (uint)pixel.Alpha;  // Fixed-point reciprocal (16.16 or similar)

                var xx = (pixel.X * reciprocal) >> 16;
                var yy = (pixel.Y * reciprocal) >> 16;
                var zz = (pixel.Z * reciprocal) >> 16;

                return new Vector4(xx, yy, zz, pixel.Alpha);
            }

            [MethodImpl(AGRESSIVE)]
            public static Vector3 ToVector3Reverse(in _SkiaSharpPremulPixel pixel)
            {
                if (pixel.Alpha == 0) return Vector3.Zero;

                // One reciprocal, three multiplications
                uint reciprocal = (255 << 16) / (uint)pixel.Alpha;  // Fixed-point reciprocal (16.16 or similar)

                var xx = (pixel.X * reciprocal) >> 16;
                var yy = (pixel.Y * reciprocal) >> 16;
                var zz = (pixel.Z * reciprocal) >> 16;

                return new Vector3(zz, yy, xx);
            }

            [MethodImpl(AGRESSIVE)]
            public static Vector4 ToVector4Reverse(in _SkiaSharpPremulPixel pixel)
            {
                if (pixel.Alpha == 0) return Vector4.Zero;

                // One reciprocal, three multiplications
                uint reciprocal = (255 << 16) / (uint)pixel.Alpha;  // Fixed-point reciprocal (16.16 or similar)

                var xx = (pixel.X * reciprocal) >> 16;
                var yy = (pixel.Y * reciprocal) >> 16;
                var zz = (pixel.Z * reciprocal) >> 16;

                return new Vector4(zz, yy, xx, pixel.Alpha);
            }

            [MethodImpl(AGRESSIVE)]
            public static Vector3 ToScaledVector3(in _SkiaSharpPremulPixel pixel)
            {
                return ToVector3Forward(pixel) / 255f;
            }

            [MethodImpl(AGRESSIVE)]
            public static Vector4 ToScaledVector4(in _SkiaSharpPremulPixel pixel)
            {
                return ToVector4Forward(pixel) / 255f;
            }            

            #endregion
        }
    }
}
