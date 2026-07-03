using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [global::System.CodeDom.Compiler.GeneratedCode("CodeSugar.CodeGen", "1.0.0.0")]
    internal static partial class CodeSugarExtensions    
    {
        #region constants

        private static System.Collections.Concurrent.ConcurrentDictionary<string, System.IO.DriveInfo> _InternedFixedDrives;

        private static readonly char[] _DirectorySeparators = _GetDirectorySeparators();

        private static readonly char[] _InvalidNameChars = System.IO.Path.GetInvalidFileNameChars();    

        private static readonly char[] _InvalidPathChars = System.IO.Path.GetInvalidPathChars();

        private static char[] _GetDirectorySeparators()
        {
            // this would be valid only when working with actual file system paths, otherwise it's
            // dangerous because at some point an OS with '/' as main separator may process paths
            // that come from windows using '\'

            return System.IO.Path.DirectorySeparatorChar == System.IO.Path.AltDirectorySeparatorChar
                ? new char[] { System.IO.Path.DirectorySeparatorChar }
                : new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar };
        }

        #if NETSTANDARD
        private static string _processPath;
        #endif

        #endregion

        #region Properties

        public static string ProcessPath => _GetProcessPath();

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

        #endregion

        #region helpers

        private static string _GetProcessPath()
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

          

        #endregion
    }
}
