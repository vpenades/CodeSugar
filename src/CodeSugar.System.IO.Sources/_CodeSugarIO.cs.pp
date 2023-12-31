// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

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

        private static readonly char[] _DirectorySeparators = new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar };

        private static readonly char[] _InvalidChars = System.IO.Path.GetInvalidFileNameChars();

        #endregion
    }
}
