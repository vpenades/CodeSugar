// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using XFILE = Microsoft.Extensions.FileProviders.IFileInfo;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForFileProviders
    {
        #region constants

        private static readonly XFILE __NULLFILE = new Microsoft.Extensions.FileProviders.NotFoundFileInfo("NULL");

        #endregion
    }
}
