#nullable disable

using System;
using System.Runtime.CompilerServices;

using __SIXLABORS = SixLabors.ImageSharp;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarImagingExtensions
    {
        public static void GuardNotNullOrEmpty(this __SIXLABORS.Image image, [CallerArgumentExpression(nameof(image))] string name = null)
        {
            if (image != null && image.Width > 0 && image.Height > 0) return;
            name ??= nameof(image);

            if (image == null) throw new ArgumentNullException(name);
            throw new ArgumentException("empty", name);
        }
    }
}
