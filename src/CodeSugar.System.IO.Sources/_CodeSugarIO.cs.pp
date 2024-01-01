// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

using RTI = System.Runtime.InteropServices;

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

        private static System.IO.DriveInfo[] _InternedFixedDrives;

        private static readonly char[] _DirectorySeparators = _GetDirectorySeparators();

        private static readonly char[] _InvalidChars = System.IO.Path.GetInvalidFileNameChars();        

        #endregion

        #region api

        public static bool FileSystemIsCaseSensitive { get; } = _CheckFileSystemCaseSensitive();

        public static StringComparison FileSystemStringComparison => FileSystemIsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        private static char[] _GetDirectorySeparators()
        {
            return System.IO.Path.DirectorySeparatorChar == System.IO.Path.AltDirectorySeparatorChar
                ? new char[] { System.IO.Path.DirectorySeparatorChar }
                : new char[] { System.IO.Path.DirectorySeparatorChar , System.IO.Path.AltDirectorySeparatorChar };
        }

        private static bool _CheckFileSystemCaseSensitive()
        {
            if (RTI.RuntimeInformation.IsOSPlatform(RTI.OSPlatform.Windows) ||
                RTI.RuntimeInformation.IsOSPlatform(RTI.OSPlatform.OSX))  // HFS+ (the Mac file-system) is usually configured to be case insensitive.
            {
                return false;
            }
            else if (RTI.RuntimeInformation.IsOSPlatform(RTI.OSPlatform.Linux))
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
