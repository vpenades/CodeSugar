// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#nullable disable

using __RTINTEROPSVCS = System.Runtime.InteropServices;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [global::System.CodeDom.Compiler.GeneratedCode("CodeSugar.CodeGen", "1.0.0.0")]
    internal static partial class CodeSugarForFileProviders
    {
        #region helpers        

        private static bool _CheckFileSystemCaseSensitive()
        {
            // credits: https://stackoverflow.com/a/56773947

            if (__RTINTEROPSVCS.RuntimeInformation.IsOSPlatform(__RTINTEROPSVCS.OSPlatform.Windows) ||
                __RTINTEROPSVCS.RuntimeInformation.IsOSPlatform(__RTINTEROPSVCS.OSPlatform.OSX))  // HFS+ (the Mac file-system) is usually configured to be case insensitive.
            {
                return false;
            }
            else if (__RTINTEROPSVCS.RuntimeInformation.IsOSPlatform(__RTINTEROPSVCS.OSPlatform.Linux))
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
