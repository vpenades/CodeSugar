// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Text;
using System.IO;
using System.Linq;

#nullable disable

using _FINFO = System.IO.FileInfo;
using _IOPATH = System.IO.Path;

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
        public static bool HasAnyExtension(this _FINFO finfo, params string[] extensions)
        {
            return extensions.Any(ext => HasExtension(finfo, ext));
        }

        public static bool HasExtension(this _FINFO finfo, string extension)
        {
            if (string.IsNullOrEmpty(extension)) throw new ArgumentNullException(nameof(extension));
            if (!extension.StartsWith('.')) extension = '.' + extension;

            return finfo.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase); // even in case sensitive operating systems, a .jpg is a .jpg            
        }

        public static _FINFO WithExtension(this _FINFO finfo, string extension)
        {
            if (finfo == null) return null;

            var path = _IOPATH.ChangeExtension(finfo.FullName, extension);

            return new _FINFO(path);
        }

        /// <summary>
        /// Tries to get a composite extension of a file.
        /// </summary>
        /// <param name="fileName">the filename from where to get the extension.</param>
        /// <param name="dots">the number of dots used by the extension.</param>
        /// <param name="extension">the resulting extension.</param>
        /// <returns>true if an extension was found</returns>        
        public static bool TryGetCompositedExtension(this _FINFO finfo, int dots, out string extension)
        {
            GuardNotNull(finfo);
            return TryGetCompositedExtension(finfo.FullName, dots, out extension);
        }

        public static bool TryGetCompositedExtension(string fileName, int dots, out string extension)
        {
            if (dots < 1) throw new ArgumentOutOfRangeException(nameof(dots), "Must be equal or greater than 1");

            var l = fileName.Length - 1;
            var r = -1;

            var invalidChars = _IOPATH.GetInvalidFileNameChars();

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

        // https://www.lifewire.com/list-of-executable-file-extensions-2626061

        private static readonly string[] _WindowsExecutableExtensions = new[] { "ex_", "exe", "dll", "com", "scr", "gadget", "u3p", "vbe", "jar", "jse" };

        // although a .lnk file is essentially a windows link, it also supports command line arguments,
        // which make it, in practice, fully scriptable and is being used to download and install malware

        private static readonly string[] _WindowsScriptExtensions = new[] { "lnk", "bat", "cmd", "sh", "ps1", "reg", "rgs", "sct", "shb", "vb", "vbs", "vbscript", "ws", "wsf", "wsh" };
        private static readonly string[] _WindowsShellExtensions = new[] { "cpl", "inf", "ins", "job", "msc", "pif", "shs" };
        private static readonly string[] _WindowsInstallerExtensions = new[] { "inx", "isu", "cab", "msi", "msp", "mst", "paf" };

        public static bool HasExecutableOrScriptExtension(this _FINFO finfo)
        {
            if (finfo == null) return false;
            var ext = finfo.Extension.ToLowerInvariant().Trim('.');

            if (_WindowsExecutableExtensions.Contains(ext)) return true;
            if (_WindowsScriptExtensions.Contains(ext)) return true;
            if (_WindowsShellExtensions.Contains(ext)) return true;
            if (_WindowsInstallerExtensions.Contains(ext)) return true;

            return false;
        }
    }
}
