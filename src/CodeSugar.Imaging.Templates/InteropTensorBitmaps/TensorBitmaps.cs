using System;
using System.Runtime.CompilerServices;
using System.Numerics;

#nullable disable

using __TBPIXELFORMAT = InteropTypes.TensorBitmaps.TensorPixelFormat;
using __TBCOMPONENT = InteropTypes.TensorBitmaps.TensorPixelComponent;

#if __REFERENCES_SIXLABORSIMAGESHARP
using __IMAGESHARPPIXFMT = SixLabors.ImageSharp.PixelFormats;
#endif

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarImagingExtensions
    {
        public static bool TryInferTensorBitmapPixelFormat(this (Type type, int length) pixelInfo, out __TBPIXELFORMAT fmt)
        {            
            // by type            

            var t = pixelInfo.type;

            if (t == typeof(byte)) { fmt = new __TBPIXELFORMAT(__TBCOMPONENT.Luminance255); return true; }
            if (t == typeof(float)) { fmt = new __TBPIXELFORMAT(__TBCOMPONENT.LuminanceScalar); return true; }

            if (t == typeof(int)) { fmt = __TBPIXELFORMAT.Rgba32; return true; }
            if (t == typeof(uint)) { fmt = __TBPIXELFORMAT.Rgba32; return true; }
            if (t == typeof(Vector3)) { fmt = __TBPIXELFORMAT.Rgb96f; return true; }
            if (t == typeof(Vector4)) { fmt = __TBPIXELFORMAT.Rgba128f; return true; }
            if (t == typeof(System.Runtime.Intrinsics.Vector128<float>)) { fmt = __TBPIXELFORMAT.Rgba128f; return true; }

            #if __REFERENCES_SIXLABORSIMAGESHARP    
            if (t == typeof(__IMAGESHARPPIXFMT.L8)) { fmt = new __TBPIXELFORMAT(__TBCOMPONENT.Luminance255); return true; }
            if (t == typeof(__IMAGESHARPPIXFMT.A8)) { fmt = new __TBPIXELFORMAT(__TBCOMPONENT.Alpha255); return true; }
            if (t == typeof(__IMAGESHARPPIXFMT.Rgb24)) { fmt = __TBPIXELFORMAT.Rgb24; return true; }
            if (t == typeof(__IMAGESHARPPIXFMT.Rgba32)) { fmt = __TBPIXELFORMAT.Rgba32; return true; }
            if (t == typeof(__IMAGESHARPPIXFMT.RgbaVector)) { fmt = __TBPIXELFORMAT.Rgba128f; return true; }

            if (t == typeof(__IMAGESHARPPIXFMT.Bgr24)) { fmt = __TBPIXELFORMAT.Bgr24; return true; }
            if (t == typeof(__IMAGESHARPPIXFMT.Bgra32)) { fmt = __TBPIXELFORMAT.Bgra32; return true; }

            if (t == typeof(__IMAGESHARPPIXFMT.Argb32)) { fmt = __TBPIXELFORMAT.Argb32; return true; }
            if (t == typeof(__IMAGESHARPPIXFMT.Abgr32)) { fmt = new __TBPIXELFORMAT(__TBCOMPONENT.Alpha255, __TBCOMPONENT.Blue255, __TBCOMPONENT.Green255,__TBCOMPONENT.Red255); return true; }
            #endif

            #if __REFERENCES_SKIASHARP
            if (t == typeof(SkiaSharp.SKColor) && Unsafe.SizeOf<SkiaSharp.SKColor>() == 4) { fmt = __TBPIXELFORMAT.Bgra32; return true; }
            #endif

            // by known names

            if (pixelInfo.length == 1)
            {
                if (t.FullName.StartsWith("SixLabors.ImageSharp.PixelFormats.", StringComparison.Ordinal))
                {
                    switch (t.Name)
                    {
                        case "L8": { fmt = new __TBPIXELFORMAT(__TBCOMPONENT.Luminance255); return true; }
                        case "A8": { fmt = new __TBPIXELFORMAT(__TBCOMPONENT.Alpha255); return true; }                        
                    }
                }
            }

            if (pixelInfo.length == 3)
            {
                if (t.FullName.StartsWith("SixLabors.ImageSharp.PixelFormats.", StringComparison.Ordinal))
                {
                    switch (t.Name)
                    {
                        case "Rgb24": { fmt = __TBPIXELFORMAT.Rgb24; return true; }
                        case "Bgr24": { fmt = __TBPIXELFORMAT.Bgr24; return true; }                        
                    }
                }
            }

            if (pixelInfo.length == 4)
            {
                if (t.FullName.Equals("SkiaSharp.SKColor", StringComparison.Ordinal)) { fmt = __TBPIXELFORMAT.Bgra32; return true; }
                if (t.FullName.Equals("UnityEngine.Color32", StringComparison.Ordinal)) { fmt = __TBPIXELFORMAT.Rgba32; return true; }
                if (t.FullName.Equals("Microsoft.XNA.Framework.Color", StringComparison.Ordinal)) { fmt = __TBPIXELFORMAT.Rgba32; return true; }                

                if (t.FullName.StartsWith("SixLabors.ImageSharp.PixelFormats.", StringComparison.Ordinal))
                {
                    switch (t.Name)
                    {
                        case "Rgba32": { fmt = __TBPIXELFORMAT.Rgba32; return true; }
                        case "Bgra32": { fmt = __TBPIXELFORMAT.Bgra32; return true; }
                        case "Argb32": { fmt = __TBPIXELFORMAT.Argb32; return true; }
                        case "Abgr32": { fmt = new __TBPIXELFORMAT(__TBCOMPONENT.Alpha255, __TBCOMPONENT.Blue255, __TBCOMPONENT.Green255, __TBCOMPONENT.Red255); return true; }
                    }
                }
            }

            if (t.FullName.StartsWith("SixLabors.ImageSharp.PixelFormats.", StringComparison.Ordinal))
            {
                switch (t.Name)
                {                    
                    case "RgbaVector": { fmt = __TBPIXELFORMAT.Rgba128f; return true; }                    
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
            if (l == 1 && !(nameHasRGB && nameHasYUV)) { fmt = new __TBPIXELFORMAT(__TBCOMPONENT.Luminance255); return true; }            

            if (l == 3)
            {
                if (t.Name.Contains("rgb", StringComparison.OrdinalIgnoreCase)) { fmt = __TBPIXELFORMAT.Rgb24; return true; }
                if (t.Name.Contains("bgr", StringComparison.OrdinalIgnoreCase)) { fmt = __TBPIXELFORMAT.Bgr24; return true; }

                // notice that "Green" also has a R, in case we find "RedGreenBlue" "BlueGreenRed"
                if (_compIdx('r') < _compIdx('g') && _compIdx('g') < _compIdx('b')) { fmt = __TBPIXELFORMAT.Rgb24; return true; }
            }

            if (l == 4)
            {
                if (t.Name.Contains("rgba", StringComparison.OrdinalIgnoreCase)) { fmt = __TBPIXELFORMAT.Rgba32; return true; }
                if (t.Name.Contains("bgra", StringComparison.OrdinalIgnoreCase)) { fmt = __TBPIXELFORMAT.Bgra32; return true; }
                if (t.Name.Contains("argb", StringComparison.OrdinalIgnoreCase)) { fmt = __TBPIXELFORMAT.Argb32; return true; }

                // notice that "Green" also has a R, in case we find "RedGreenBlue" "BlueGreenRed"
                if (_compIdx('r') < _compIdx('g') && _compIdx('g') < _compIdx('b')) { fmt = __TBPIXELFORMAT.Rgb24; return true; }
            }

            if (l == 12)
            {
                if (t.Name.Contains("rgb", StringComparison.OrdinalIgnoreCase)) { fmt = __TBPIXELFORMAT.Rgb96f; return true; }
                if (t.Name.Contains("bgr", StringComparison.OrdinalIgnoreCase)) { fmt = __TBPIXELFORMAT.Bgr96f; return true; }
            }

            // a length of 16 may be too many things for a safe bet.

            fmt = default;
            return false;
        }
    }
}
