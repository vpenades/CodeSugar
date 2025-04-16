// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using _FINFO = System.IO.FileInfo;
using _BYTESSEGMENT = System.ArraySegment<byte>;

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
        public static void ThrowNotFound(this _FINFO dinfo, Exception innerException = null)
        {
            GuardNotNull(dinfo);
            if (innerException == null) throw new System.IO.FileNotFoundException(dinfo.FullName);
            else throw new System.IO.FileNotFoundException(dinfo.FullName, innerException);
        }
    }
}
