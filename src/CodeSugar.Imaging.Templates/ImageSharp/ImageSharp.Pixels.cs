using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Numerics.Tensors;
using System.Text;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;

#nullable disable

using __XYZ = System.Numerics.Vector3;
using __XYZW = System.Numerics.Vector4;

using __SIXLABORS = SixLabors.ImageSharp;
using __SIXLABORSPIXFMT = SixLabors.ImageSharp.PixelFormats;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static UInt32 ToImageSharpPackedValue<TPixel>(this TPixel src) where TPixel : unmanaged, IPixel<TPixel>
        {
            Unsafe.SkipInit<Rgba32>(out var dst);
            src.ToRgba32(ref dst);
            return dst.PackedValue;
        }

        public static Rgb24 ToImageSharpRgb24<TPixel>(this TPixel src) where TPixel : unmanaged, IPixel<TPixel>
        {
            Unsafe.SkipInit<Rgba32>(out var dst);
            src.ToRgba32(ref dst);
            return dst.Rgb;
        }

        public static Rgba32 ToImageSharpRgba32<TPixel>(this TPixel src) where TPixel : unmanaged, IPixel<TPixel>
        {
            Unsafe.SkipInit<Rgba32>(out var dst);
            src.ToRgba32(ref dst);
            return dst;
        }

        public static Bgr24 ToImageSharpBgr24<TPixel>(this TPixel src) where TPixel : unmanaged, IPixel<TPixel>
        {
            Unsafe.SkipInit<Rgba32>(out var dst);
            src.ToRgba32(ref dst);
            return dst.Bgr;
        }

        public static Bgra32 ToImageSharpBgra32<TPixel>(this TPixel src) where TPixel : unmanaged, IPixel<TPixel>
        {
            Unsafe.SkipInit<Rgba32>(out var tmp);
            src.ToRgba32(ref tmp);
            Unsafe.SkipInit<Bgra32>(out var dst);
            dst.FromRgba32(tmp);
            return dst;
        }

        public static L8 ToImageSharpL8<TPixel>(this TPixel src) where TPixel : unmanaged, IPixel<TPixel>
        {
            Unsafe.SkipInit<Rgba32>(out var tmp);
            src.ToRgba32(ref tmp);
            Unsafe.SkipInit<L8>(out var dst);
            dst.FromRgba32(tmp);
            return dst;
        }        

        internal static int GetChannelsCount<TPixel>() where TPixel : unmanaged, IPixel<TPixel>
        {
            // PixelTypeInfo.Create<TPixel>(); // this API should be public

            var t = typeof(TPixel);

            if (t == typeof(A8)) return 1;

            if (t == typeof(L8)) return 1;
            if (t == typeof(L16)) return 1;

            if (t == typeof(La16)) return 2;
            if (t == typeof(La32)) return 2;

            if (t == typeof(Rgb24)) return 3;
            if (t == typeof(Rgb48)) return 3;
            
            if (t == typeof(Bgr24)) return 3;
            if (t == typeof(Bgr565)) return 3;

            if (t == typeof(Rgba32)) return 4;
            if (t == typeof(Rgba64)) return 4;
            if (t == typeof(Rgba1010102)) return 4;
            if (t == typeof(RgbaVector)) return 4;

            if (t == typeof(Bgra32)) return 4;
            if (t == typeof(Bgra4444)) return 4;
            if (t == typeof(Bgra5551)) return 4;

            if (t == typeof(Argb32)) return 4;
            if (t == typeof(Abgr32)) return 4;

            throw new NotImplementedException(t.Name);

        }

        internal static bool GetIsBGR<TPixel>() where TPixel : unmanaged, IPixel<TPixel>
        {
            var t = typeof(TPixel);            

            if (t == typeof(A8)) return false;

            if (t == typeof(L8)) return false;
            if (t == typeof(L16)) return false;

            if (t == typeof(La16)) return false;
            if (t == typeof(La32)) return false;

            if (t == typeof(Rgb24)) return false;
            if (t == typeof(Rgb48)) return false;

            if (t == typeof(Bgr24)) return true;
            if (t == typeof(Bgr565)) return true;

            if (t == typeof(Rgba32)) return false;
            if (t == typeof(Rgba64)) return false;
            if (t == typeof(Rgba1010102)) return false;
            if (t == typeof(RgbaVector)) return false;

            if (t == typeof(Bgra32)) return true;
            if (t == typeof(Bgra4444)) return true;
            if (t == typeof(Bgra5551)) return true;

            if (t == typeof(Argb32)) return false;
            if (t == typeof(Abgr32)) return false;

            throw new NotImplementedException(t.Name);
        }

        public static __XYZ ToScaledBGR<TPixel>(this TPixel pixel) where TPixel: unmanaged, IPixel<TPixel>
        {
            var rgba = pixel.ToScaledVector4();
            return new __XYZ(rgba.Z, rgba.Y, rgba.X);
        }
        public static __XYZ ToScaledRGB<TPixel>(this TPixel pixel) where TPixel : unmanaged, IPixel<TPixel>
        {
            var rgba = pixel.ToScaledVector4();
            return new __XYZ(rgba.X, rgba.Y, rgba.Z);
        }

        public static __XYZW ToScaledBGRA<TPixel>(this TPixel pixel) where TPixel : unmanaged, IPixel<TPixel>
        {
            var rgba = pixel.ToScaledVector4();
            return new __XYZW(rgba.Z, rgba.Y, rgba.X, rgba.W);
        }

        public static __XYZW ToScaledRGBA<TPixel>(this TPixel pixel) where TPixel : unmanaged, IPixel<TPixel>
        {
            return pixel.ToScaledVector4();
        }

        /// <summary>
        /// Converts a scalar value (which can be either alpha or luminance) to a sixlabors pixel format
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="l"></param>
        /// <returns></returns>
        private static TPixel _ScalarToImageSharpPixel<TPixel>(float l)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            TPixel pix = default;

            if (typeof(TPixel) == typeof(A8))
            {
                __SIXLABORSPIXFMT.Rgba32 tmpA = new Rgba32(255, 255, 255);
                tmpA.A = (byte)(Math.Clamp(l, 0, 1) * byte.MaxValue);
                pix.FromRgba32(tmpA);
            }
            else
            {
                __SIXLABORSPIXFMT.L16 tmpL = default;
                tmpL.PackedValue = (ushort)(Math.Clamp(l, 0, 1) * ushort.MaxValue);
                pix.FromL16(tmpL);
            }

            return pix;
        }

        private static float _LuminanceOrAlphaToScalar<TPixel>(TPixel pixel)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return (float)(_ToL16(pixel).PackedValue / ushort.MaxValue);
        }

        private static L16 _ToL16<TPixel>(TPixel pixel)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            __SIXLABORSPIXFMT.L16 pix = default;

            switch (pixel)
            {
                case A8 alpha: int a = alpha.PackedValue; return new L16((ushort)((a << 8) + a));
                case Rgb24 color: pix.FromRgb24(color); break;
                case Bgr24 color: pix.FromBgr24(color); break;
                case Rgba32 color: pix.FromRgba32(color); break;
                case Argb32 color: pix.FromArgb32(color); break;
                case Bgra32 color: pix.FromBgra32(color); break;
                default: pix.FromScaledVector4(pixel.ToScaledVector4()); break;
            }

            return pix;
        }        
    }
}
