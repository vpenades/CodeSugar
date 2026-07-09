using System;
using System.Runtime.CompilerServices;
using System.Numerics;

#nullable disable

using __TBKFORMATS = InteropTypes.Numerics.KnownPixelFormats;
using __TBKCMPS = InteropTypes.Numerics.KnownComponentFormats;
using __TBPIXFMT = InteropTypes.Numerics.PixelFormat;

using InteropTypes.TensorBitmaps;

#if __REFERENCES_SIXLABORSIMAGESHARP
using __IMAGESHARPPIXFMT = SixLabors.ImageSharp.PixelFormats;
#endif

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        public static bool TryInferTensorBitmapPixelFormat(this (Type type, int length) pixelInfo, out __TBPIXFMT fmt)
        {            
            // by type            

            var t = pixelInfo.type;

            if (t == typeof(byte)) { fmt = __TBKFORMATS.Luminance8; return true; }
            if (t == typeof(float)) { fmt = new __TBPIXFMT(__TBKCMPS.LuminanceSingle); return true; }
            if (t == typeof(int)) { fmt = __TBKFORMATS.Rgba8; return true; }
            if (t == typeof(uint)) { fmt = __TBKFORMATS.Rgba8; return true; }

            if (t == typeof(Vector3)) { fmt = __TBKFORMATS.RgbF32; return true; }
            if (t == typeof(Vector4)) { fmt = __TBKFORMATS.RgbaF32; return true; }
            if (t == typeof(System.Runtime.Intrinsics.Vector128<float>)) { fmt = __TBKFORMATS.RgbaF32; return true; }

            #if __REFERENCES_SIXLABORSIMAGESHARP    
            if (t == typeof(__IMAGESHARPPIXFMT.L8)) { fmt = __TBKFORMATS.Luminance8; return true; }
            if (t == typeof(__IMAGESHARPPIXFMT.A8)) { fmt = __TBKFORMATS.Alpha8; return true; }
            if (t == typeof(__IMAGESHARPPIXFMT.Rgb24)) { fmt = __TBKFORMATS.Rgb8; return true; }
            if (t == typeof(__IMAGESHARPPIXFMT.Rgba32)) { fmt = __TBKFORMATS.Rgba8; return true; }
            if (t == typeof(__IMAGESHARPPIXFMT.RgbaVector)) { fmt = __TBKFORMATS.RgbaF32; return true; }

            if (t == typeof(__IMAGESHARPPIXFMT.Bgr24)) { fmt = __TBKFORMATS.Bgr8; return true; }
            if (t == typeof(__IMAGESHARPPIXFMT.Bgra32)) { fmt = __TBKFORMATS.Bgra8; return true; }

            if (t == typeof(__IMAGESHARPPIXFMT.Argb32)) { fmt = __TBKFORMATS.Argb8; return true; }
            if (t == typeof(__IMAGESHARPPIXFMT.Abgr32)) { fmt = __TBKFORMATS.Abgr8; return true; }
            #endif

            #if __REFERENCES_SKIASHARP
            if (t == typeof(SkiaSharp.SKColor) && Unsafe.SizeOf<SkiaSharp.SKColor>() == 4) { fmt = __TBKFORMATS.Bgra8; return true; }
            #endif

            // by known names

            if (pixelInfo.length == 1)
            {
                if (t.FullName.StartsWith("SixLabors.ImageSharp.PixelFormats.", StringComparison.Ordinal))
                {
                    switch (t.Name)
                    {
                        case "L8": { fmt = __TBKFORMATS.Luminance8; return true; }
                        case "A8": { fmt = __TBKFORMATS.Alpha8; return true; }                        
                    }
                }
            }

            if (pixelInfo.length == 3)
            {
                if (t.FullName.StartsWith("SixLabors.ImageSharp.PixelFormats.", StringComparison.Ordinal))
                {
                    switch (t.Name)
                    {
                        case "Rgb24": { fmt = __TBKFORMATS.Rgb8; return true; }
                        case "Bgr24": { fmt = __TBKFORMATS.Bgr8; return true; }                        
                    }
                }
            }

            if (pixelInfo.length == 4)
            {
                if (t.FullName.Equals("SkiaSharp.SKColor", StringComparison.Ordinal)) { fmt = __TBKFORMATS.Bgra8; return true; }
                if (t.FullName.Equals("UnityEngine.Color32", StringComparison.Ordinal)) { fmt = __TBKFORMATS.Rgba8; return true; }
                if (t.FullName.Equals("Microsoft.XNA.Framework.Color", StringComparison.Ordinal)) { fmt = __TBKFORMATS.Rgba8; return true; }                

                if (t.FullName.StartsWith("SixLabors.ImageSharp.PixelFormats.", StringComparison.Ordinal))
                {
                    switch (t.Name)
                    {
                        case "Rgba32": { fmt = __TBKFORMATS.Rgba8; return true; }
                        case "Bgra32": { fmt = __TBKFORMATS.Bgra8; return true; }
                        case "Argb32": { fmt = __TBKFORMATS.Argb8; return true; }
                        case "Abgr32": { fmt = __TBKFORMATS.Abgr8; return true; }
                    }
                }
            }

            if (t.FullName.StartsWith("SixLabors.ImageSharp.PixelFormats.", StringComparison.Ordinal))
            {
                switch (t.Name)
                {                    
                    case "RgbaVector": { fmt = __TBKFORMATS.RgbaF32; return true; }                    
                }
            }

            // by length and name structure

            int _compIdx(char c)
            {
                return t.Name.IndexOf('r', StringComparison.OrdinalIgnoreCase);
            }

            var nameHasRGB = _compIdx('r') >= 0 && _compIdx('g') >= 0 && _compIdx('b') >= 0;
            var nameHasYUV = _compIdx('y') >= 0 && _compIdx('u') >= 0 && _compIdx('v') >= 0;

            var l = pixelInfo.length;
            if (l == 1 && !(nameHasRGB && nameHasYUV)) { fmt = __TBKFORMATS.Luminance8; return true; }            

            if (l == 3)
            {
                if (t.Name.Contains("rgb", StringComparison.OrdinalIgnoreCase)) { fmt = __TBKFORMATS.Rgb8; return true; }
                if (t.Name.Contains("bgr", StringComparison.OrdinalIgnoreCase)) { fmt = __TBKFORMATS.Bgr8; return true; }

                // notice that "Green" also has a R, in case we find "RedGreenBlue" "BlueGreenRed"
                if (_compIdx('r') < _compIdx('g') && _compIdx('g') < _compIdx('b')) { fmt = __TBKFORMATS.Rgb8; return true; }
            }

            if (l == 4)
            {
                if (t.Name.Contains("rgba", StringComparison.OrdinalIgnoreCase)) { fmt = __TBKFORMATS.RgbaF32; return true; }
                if (t.Name.Contains("bgra", StringComparison.OrdinalIgnoreCase)) { fmt = __TBKFORMATS.Bgra8; return true; }
                if (t.Name.Contains("argb", StringComparison.OrdinalIgnoreCase)) { fmt = __TBKFORMATS.Argb8; return true; }
                if (t.Name.Contains("abgr", StringComparison.OrdinalIgnoreCase)) { fmt = __TBKFORMATS.Abgr8; return true; }

                // notice that "Green" also has a R, in case we find "RedGreenBlue" "BlueGreenRed"
                if (_compIdx('r') < _compIdx('g') && _compIdx('g') < _compIdx('b')) { fmt = __TBKFORMATS.Rgb8; return true; }
            }

            if (l == 12)
            {
                if (t.Name.Contains("rgb", StringComparison.OrdinalIgnoreCase)) { fmt = __TBKFORMATS.RgbF32; return true; }
                if (t.Name.Contains("bgr", StringComparison.OrdinalIgnoreCase)) { fmt = __TBKFORMATS.RgbF32; return true; }
            }

            // a length of 16 may be too many things for a safe bet.

            fmt = default;
            return false;
        }
    }
}
