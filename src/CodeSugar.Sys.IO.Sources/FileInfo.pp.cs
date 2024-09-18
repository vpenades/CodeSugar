// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using FILE = System.IO.FileInfo;
using BYTESSEGMENT = System.ArraySegment<byte>;

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
        public static void ThrowNotFound(this FILE dinfo, Exception innerException = null)
        {
            GuardNotNull(dinfo);
            if (innerException == null) throw new System.IO.FileNotFoundException(dinfo.FullName);
            else throw new System.IO.FileNotFoundException(dinfo.FullName, innerException);
        }
    }
}
