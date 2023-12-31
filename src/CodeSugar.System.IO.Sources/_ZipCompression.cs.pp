// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

#nullable disable

using ZIPENTRY = System.IO.Compression.ZipArchiveEntry;
using BYTESSEGMENT = System.ArraySegment<byte>;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarIO
    {
        #if !NET

        public static void GuardReadable(this ZIPENTRY entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            if (entry.Archive.Mode != System.IO.Compression.ZipArchiveMode.Read) throw new ArgumentException("Can't read from strean", nameof(entry));
        }

        public static void GuardWriteable(this ZIPENTRY entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            if (entry.Archive.Mode != System.IO.Compression.ZipArchiveMode.Create) throw new ArgumentException("Can't read from strean", nameof(entry));
        }

        #else

        public static void GuardReadable(this ZIPENTRY entry, [CallerArgumentExpression("entry")] string name = null)
        {
            if (entry == null) throw new ArgumentNullException(name);
            if (entry.Archive.Mode != System.IO.Compression.ZipArchiveMode.Read) throw new ArgumentException("Can't read from strean", name);
        }

        public static void GuardWriteable(this ZIPENTRY entry, [CallerArgumentExpression("entry")] string name = null)
        {
            if (entry == null) throw new ArgumentNullException(name);
            if (entry.Archive.Mode != System.IO.Compression.ZipArchiveMode.Create) throw new ArgumentException("Can't read from strean", name);
        }

        #endif
    }
}
