using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable

using __RGB = System.Numerics.Vector3;
using __RGBA = System.Numerics.Vector4;
using __RGBP = System.Numerics.Vector4;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        private interface _IPixelSample
        {
            bool HasAlpha { get; }

            void Assign(__RGB rgb);
            void Assign(__RGBP rgbp);

            __RGB ToScaledVector3();
            __RGBA ToScaledVector4();

            [MethodImpl(AGRESSIVE)]
            __RGBP ToScaledPremul() => _Premultiply(ToScaledVector4());

            [MethodImpl(AGRESSIVE)]
            private static __RGBP _Premultiply(__RGBA value)
            {
                value.X *= value.W;
                value.Y *= value.W;
                value.Z *= value.W;
                return value;
            }
        }

        [System.Diagnostics.DebuggerDisplay("{Red} {Green} {Blue}")]
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        private struct _RGB : _IPixelSample
        {
            #region info

            public bool HasAlpha => false;

            #endregion

            #region data

            public Byte Red;
            public Byte Green;
            public Byte Blue;

            #endregion

            #region API

            [MethodImpl(AGRESSIVE)]
            public void Assign(__RGB rgb)
            {
                rgb *= 255f;

                Red = (Byte)rgb.X;
                Green = (Byte)rgb.Y;
                Blue = (Byte)rgb.Z;
            }

            [MethodImpl(AGRESSIVE)]
            public void Assign(__RGBP rgbp)
            {
                var scale = rgbp.W == 0 ? 0 : 255f / rgbp.W;
                Red = (Byte)(rgbp.X * scale);
                Green = (Byte)(rgbp.Y * scale);
                Blue = (Byte)(rgbp.Z * scale);
            }

            [MethodImpl(AGRESSIVE)]
            public __RGB ToScaledVector3() { return new __RGB(Red, Green, Blue) / 255f; }

            [MethodImpl(AGRESSIVE)]
            public __RGBA ToScaledVector4() { return new __RGBA(Red, Green, Blue, 255f) / 255f; }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("{Red} {Green} {Blue}")]
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        private struct _RGBf : _IPixelSample
        {
            #region info

            public bool HasAlpha => false;

            #endregion

            #region data

            public Single Red;
            public Single Green;
            public Single Blue;

            #endregion

            #region API

            [MethodImpl(AGRESSIVE)]
            public void Assign(__RGB rgb)
            {
                Red = rgb.X;
                Green = rgb.Y;
                Blue = rgb.Z;
            }

            [MethodImpl(AGRESSIVE)]
            public void Assign(__RGBP rgbp)
            {
                var scale = rgbp.W == 0 ? 0 : 1f / rgbp.W;
                Red = rgbp.X * scale;
                Green = rgbp.Y * scale;
                Blue = rgbp.Z * scale;
            }

            [MethodImpl(AGRESSIVE)]
            public __RGB ToScaledVector3() { return new __RGB(Red, Green, Blue); }

            [MethodImpl(AGRESSIVE)]
            public __RGBA ToScaledVector4() { return new __RGBA(Red, Green, Blue, 1f); }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("{Red} {Green} {Blue} {Alpha}")]
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        private struct _RGBA : _IPixelSample
        {
            #region info

            public bool HasAlpha => true;

            #endregion

            #region data

            public Byte Red;
            public Byte Green;
            public Byte Blue;
            public Byte Alpha;

            #endregion

            #region API

            [MethodImpl(AGRESSIVE)]
            public void Assign(__RGB rgb)
            {
                rgb *= 255f;

                Red = (Byte)rgb.X;
                Green = (Byte)rgb.Y;
                Blue = (Byte)rgb.Z;
                Alpha = 255;
            }

            [MethodImpl(AGRESSIVE)]
            public void Assign(__RGBP rgbp)
            {
                var scale = rgbp.W == 0 ? 0 : 255f / rgbp.W;
                Red = (Byte)(rgbp.X * scale);
                Green = (Byte)(rgbp.Y * scale);
                Blue = (Byte)(rgbp.Z * scale);
                Alpha = (Byte)(rgbp.W * 255f);
            }

            [MethodImpl(AGRESSIVE)]
            public __RGB ToScaledVector3() { return new __RGB(Red, Green, Blue) / 255f; }

            [MethodImpl(AGRESSIVE)]
            public __RGBA ToScaledVector4() { return new __RGBA(Red, Green, Blue, Alpha) / 255f; }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("{Red} {Green} {Blue} {Alpha}")]
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        private struct _RGBAf : _IPixelSample
        {
            #region info

            public bool HasAlpha => true;

            #endregion

            #region data

            public Single Red;
            public Single Green;
            public Single Blue;
            public Single Alpha;

            #endregion

            #region API

            [MethodImpl(AGRESSIVE)]
            public void Assign(__RGB rgb)
            {
                Red = rgb.X;
                Green = rgb.Y;
                Blue = rgb.Z;
                Alpha = 255;
            }

            [MethodImpl(AGRESSIVE)]
            public void Assign(__RGBP rgbp)
            {
                var scale = rgbp.W == 0 ? 0 : 1f / rgbp.W;
                Red = rgbp.X * scale;
                Green = rgbp.Y * scale;
                Blue = rgbp.Z * scale;
                Alpha = rgbp.W;
            }

            [MethodImpl(AGRESSIVE)]
            public void ComposeNormal(__RGBP rgbp)
            {
                // premultiply
                Red *= Alpha;
                Green *= Alpha;
                Blue *= Alpha;

                // alpha composition:            
                var ia = 1f - rgbp.W;
                Red = rgbp.X + Red * ia;
                Green = rgbp.Y + Green * ia;
                Blue = rgbp.Z + Blue * ia;
                Alpha = rgbp.W + Alpha * ia;

                // unpremultiply
                var scale = Alpha == 0 ? 0f : 1f / Alpha;
                Red *= scale;
                Green *= scale;
                Blue *= scale;
            }

            [MethodImpl(AGRESSIVE)]
            public __RGB ToScaledVector3() { return new __RGB(Red, Green, Blue); }

            [MethodImpl(AGRESSIVE)]
            public __RGBA ToScaledVector4() { return new __RGBA(Red, Green, Blue, Alpha); }            

            #endregion
        }


        [MethodImpl(AGRESSIVE)]
        private static __RGBA _ComposeScaledVectorNormal(this __RGBA backPixel, __RGBA forePixel)
        {
            // premultiply
            forePixel.X *= forePixel.W;
            forePixel.Y *= forePixel.W;
            forePixel.Z *= forePixel.W;

            // premultiply
            backPixel.X *= backPixel.W;
            backPixel.Y *= backPixel.W;
            backPixel.Z *= backPixel.W;

            // alpha composition:            
            var ia = 1f - forePixel.W;
            backPixel.X = forePixel.X + backPixel.X * ia;
            backPixel.Y = forePixel.Y + backPixel.Y * ia;
            backPixel.Z = forePixel.Z + backPixel.Z * ia;
            backPixel.W = forePixel.W + backPixel.W * ia;

            // unpremultiply
            var scale = backPixel.W == 0 ? 0f : 1f / backPixel.W;            
            backPixel.X *= scale;
            backPixel.Y *= scale;
            backPixel.Z *= scale;            

            return backPixel;
        }
    }
}
