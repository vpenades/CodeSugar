// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using _FINFO = System.IO.FileInfo;
using _DINFO = System.IO.DirectoryInfo;
using _SINFO = System.IO.FileSystemInfo;

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
        /// Gets the <see cref="_SINFO.FullName"/> wrapped with quotes '"'.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="onlyIfRequired">if true, it will wrap with quotes only if the path contains namespaces</param>
        /// <returns>the path, with quotes</returns>
        public static string FullNameWithQuotes(this _SINFO entry, bool onlyIfRequired = true)
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
        public static bool ShellOpenMedia(this _FINFO finfo)
        {
            var psi = __GetProcessStartMedia(finfo);
            if (psi == null) return false;

            System.Diagnostics.Process.Start(psi)?.Dispose();

            return true;
        }

        public static bool ShellShowInExplorer(this _FINFO finfo)
        {
            if (System.Environment.OSVersion.Platform != System.PlatformID.Win32NT) return false;

            if (finfo == null) return false;

            if (!finfo.Exists)
            {
                return ShellShowInExplorer(finfo.Directory);
            }

            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "explorer.exe",
                UseShellExecute = false,
                Arguments = "/select, " + '"' + finfo.FullName + '"'
            };

            System.Diagnostics.Process.Start(psi)?.Dispose();

            return true;
        }

        public static bool ShellShowInExplorer(this _DINFO dirInfo)
        {
            if (System.Environment.OSVersion.Platform != System.PlatformID.Win32NT) return false;

            if (dirInfo == null) return false;
            if (!dirInfo.Exists) return false;

            var psi = __GetProcessStartInfo(dirInfo);
            if (psi != null) System.Diagnostics.Process.Start(psi)?.Dispose();

            return true;
        }        

        private static System.Diagnostics.ProcessStartInfo GetProcessStartWeb(this Uri uri, bool allowLocalFiles = false)
        {
            if (uri == null) return null;
            if (!allowLocalFiles && uri.IsFile) throw new ArgumentException("local files not supported");

            return new System.Diagnostics.ProcessStartInfo()
            {
                FileName = uri.OriginalString,
                UseShellExecute = true,
                ErrorDialog = false
            };
        }

        /// <summary>
        /// Creates a <see cref="System.Diagnostics.ProcessStartInfo"/> for media (audio/video/documents)
        /// </summary>
        /// <param name="finfo">an existing file</param>
        /// <returns>A <see cref="System.Diagnostics.ProcessStartInfo"/> object, or null if the file does not exist or it's a script/executable</returns>
        private static System.Diagnostics.ProcessStartInfo __GetProcessStartMedia(this _FINFO finfo)
        {
            if (finfo == null || !finfo.Exists) return null;

            if (HasAnyExtension(finfo, ".lnk", ".url")) { System.Diagnostics.Debug.Fail("Resolve shortcut before calling"); return null; }

            if (HasExecutableOrScriptExtension(finfo)) return null;            

            return new System.Diagnostics.ProcessStartInfo()
            {
                FileName = finfo.FullName,
                WorkingDirectory = finfo.Directory.FullName,
                UseShellExecute = true,
                ErrorDialog = false
            };
        }

        private static System.Diagnostics.ProcessStartInfo __GetProcessStartInfo(this _DINFO dirInfo)
        {
            if (dirInfo == null || !dirInfo.Exists) return null;

            return new System.Diagnostics.ProcessStartInfo()
            {
                FileName = dirInfo.FullName,
                UseShellExecute = true,
                Verb = "open",
                ErrorDialog = false
            };
        }        
    }
}
