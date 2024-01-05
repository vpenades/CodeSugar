// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

using RTINTEROPSVCS = System.Runtime.InteropServices;

#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("SharpGLTF.CodeGen", "1.0.0.0")]
    internal static partial class CodeSugarIO    
    {
    #region constants

        private static Dictionary<string, System.IO.DriveInfo> _InternedFixedDrives;

        private static readonly char[] _DirectorySeparators = _GetDirectorySeparators();

        private static readonly char[] _InvalidNameChars = System.IO.Path.GetInvalidFileNameChars();    

        private static readonly char[] _InvalidPathChars = System.IO.Path.GetInvalidPathChars();        
        
        #if NETSTANDARD
        private static string _processPath;
        #endif

        #endregion

        #region Properties

        public static string ProcessPath
        {
            get
            {            
                #if NETSTANDARD

                string _getProcessPath() => System.Diagnostics.Process.GetCurrentProcess()?.MainModule?.FileName;

                string processPath = _processPath;
                if (processPath == null)
                {
                    // The value is cached both as a performance optimization and to ensure that the API always returns
                    // the same path in a given process.
                    System.Threading.Interlocked.CompareExchange(ref _processPath, _getProcessPath() ?? String.Empty, null);
                    processPath = _processPath;
                    System.Diagnostics.Debug.Assert(processPath != null);
                }
                return (processPath.Length != 0) ? processPath : null;                
                #else
                return System.Environment.ProcessPath;
                #endif                
            }
        }

        /// <summary>
        /// This file points to the file location where the application started.
        /// </summary>
        /// <remarks>
        /// This is not neccesarily the location of the application binaries are being executed. If the executable is a SingleFileExe,
        /// this location points to the facade exe, but the actual binaries are located somewhere in the temp path. <see cref="ApplicationDirectory" />        
        /// </remarks>
        public static System.IO.FileInfo ProcessPathFile
        {
            get
            {
                var path = ProcessPath;                
                return path == null ? null : new System.IO.FileInfo(path);
            }
        }

        /// <summary>
        /// This directory points to the directory that contains the actual binaries of the application.
        /// </summary>
        /// <remarks>
        /// This directory is suited to retrieve application assets.
        /// </remarks>
        public static System.IO.DirectoryInfo ApplicationDirectory => new System.IO.DirectoryInfo(System.AppContext.BaseDirectory);

        public static bool FileSystemIsCaseSensitive { get; } = _CheckFileSystemCaseSensitive();

        public static StringComparison FileSystemStringComparison => FileSystemIsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        public static StringComparer FileSystemStringComparer => FileSystemIsCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

        private static char[] _GetDirectorySeparators()
        {
            return System.IO.Path.DirectorySeparatorChar == System.IO.Path.AltDirectorySeparatorChar
                ? new char[] { System.IO.Path.DirectorySeparatorChar }
                : new char[] { System.IO.Path.DirectorySeparatorChar , System.IO.Path.AltDirectorySeparatorChar };
        }

        private static bool _CheckFileSystemCaseSensitive()
        {
            // credits: https://stackoverflow.com/a/56773947

            if (RTINTEROPSVCS.RuntimeInformation.IsOSPlatform(RTINTEROPSVCS.OSPlatform.Windows) ||
                RTINTEROPSVCS.RuntimeInformation.IsOSPlatform(RTINTEROPSVCS.OSPlatform.OSX))  // HFS+ (the Mac file-system) is usually configured to be case insensitive.
            {
                return false;
            }
            else if (RTINTEROPSVCS.RuntimeInformation.IsOSPlatform(RTINTEROPSVCS.OSPlatform.Linux))
            {
                return true;
            }
            else if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                return true;
            }
            else
            {
               // A default.
               return false;
            }
        }

        #endregion
    }
}
