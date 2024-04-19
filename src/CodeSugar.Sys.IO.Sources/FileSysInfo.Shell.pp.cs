// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using FILE = System.IO.FileInfo;
using DIRECTORY = System.IO.DirectoryInfo;
using SYSTEMENTRY = System.IO.FileSystemInfo;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarForSystemIO
    {
        /// <summary>
        /// Gets the <see cref="SYSTEMENTRY.FullName"/> wrapped with quotes '"'.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="onlyIfRequired">if true, it will wrap with quotes only if the path contains namespaces</param>
        /// <returns>the path, with quotes</returns>
        public static string FullNameWithQuotes(this SYSTEMENTRY entry, bool onlyIfRequired = true)
        {
            GuardNotNull(entry);

            var name = entry.FullName;
            bool hasWhiteSpaces = name.Any(c => char.IsWhiteSpace(c));
            if (hasWhiteSpaces || !onlyIfRequired) name = "\"" + name + "\"";
            return name;
        }

        /// <summary>
        /// Opens a file only if it's a media file, like an image, a video or a text document. Executables and scripts are explicitly omitted
        /// </summary>
        /// <param name="finfo">the file to open</param>
        public static void ShellOpenMedia(this FILE finfo)
        {
            var psi = GetProcessStartMedia(finfo);
            if (psi != null) System.Diagnostics.Process.Start(psi)?.Dispose();
        }

        public static void ShellShowInExplorer(this FILE finfo)
        {
            if (System.Environment.OSVersion.Platform != System.PlatformID.Win32NT) return;

            if (finfo == null || !finfo.Exists) return;

            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "explorer.exe",
                UseShellExecute = false,
                Arguments = "/select, " + '"' + finfo.FullName + '"'
            };

            System.Diagnostics.Process.Start(psi)?.Dispose();
        }

        public static void ShellShowInExplorer(this DIRECTORY dirInfo)
        {
            if (System.Environment.OSVersion.Platform != System.PlatformID.Win32NT) return;

            var psi = GetProcessStartInfo(dirInfo);
            if (psi != null) System.Diagnostics.Process.Start(psi)?.Dispose();
        }        

        private static System.Diagnostics.ProcessStartInfo GetProcessStartWeb(this Uri uri, bool allowLocalFiles = false)
        {
            if (uri == null) return null;
            if (!allowLocalFiles && uri.IsFile) throw new ArgumentException("local files not supported");

            return new System.Diagnostics.ProcessStartInfo()
            {
                FileName = uri.OriginalString,
                UseShellExecute = true
            };
        }

        private static System.Diagnostics.ProcessStartInfo GetProcessStartMedia(this FILE finfo)
        {
            if (finfo == null || !finfo.Exists) return null;
            if (finfo.HasExecutableOrScriptExtension()) return null;

            return new System.Diagnostics.ProcessStartInfo()
            {
                FileName = finfo.FullName,
                UseShellExecute = true                
            };
        }

        private static System.Diagnostics.ProcessStartInfo GetProcessStartInfo(this DIRECTORY dirInfo)
        {
            if (dirInfo == null || !dirInfo.Exists) return null;

            return new System.Diagnostics.ProcessStartInfo()
            {
                FileName = dirInfo.FullName,
                UseShellExecute = true,
                Verb = "open"
            };
        }        
    }
}
