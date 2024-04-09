// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Text;
using System.IO;

#nullable disable

using FILE = System.IO.FileInfo;
using PATH = System.IO.Path;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForSystemIO
    {
        public static bool HasExtension(this FILE finfo, string extension)
        {
            if (string.IsNullOrEmpty(extension)) throw new ArgumentNullException(nameof(extension));
            if (!extension.StartsWith('.')) extension = '.' + extension;

            return finfo.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase); // even in case sensitive operating systems, a .jpg is a .jpg            
        }

        public static FILE WithExtension(this FILE finfo, string extension)
        {
            if (finfo == null) return null;

            var path = PATH.ChangeExtension(finfo.FullName, extension);

            return new FILE(path);
        }

        /// <summary>
        /// Tries to get a composite extension of a file.
        /// </summary>
        /// <param name="fileName">the filename from where to get the extension.</param>
        /// <param name="dots">the number of dots used by the extension.</param>
        /// <param name="extension">the resulting extension.</param>
        /// <returns>true if an extension was found</returns>        
        public static bool TryGetCompositedExtension(this FILE finfo, int dots, out string extension)
        {
            GuardNotNull(finfo);
            return TryGetCompositedExtension(finfo.FullName, dots, out extension);
        }

        public static bool TryGetCompositedExtension(string fileName, int dots, out string extension)
        {
            if (dots < 1) throw new ArgumentOutOfRangeException(nameof(dots), "Must be equal or greater than 1");

            var l = fileName.Length - 1;
            var r = -1;

            var invalidChars = PATH.GetInvalidFileNameChars();

            while (dots > 0 && l >= 0)
            {
                var c = fileName[l];

                if (Array.IndexOf(invalidChars, c) >= 0) break;

                if (c == '.')
                {
                    r = l;
                    --dots;
                }

                --l;
            }

            if (dots != 0)
            {
                extension = null;
                return false;
            }

            extension = fileName.Substring(r);
            return true;
        }
    }
}
