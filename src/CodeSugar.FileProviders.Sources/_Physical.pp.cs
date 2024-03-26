// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.FileProviders;

#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    
    internal static partial class CodeSugarForFileProviders    
    {
        private readonly struct _PhysicalFile : IFileInfo
            {
                public _PhysicalFile(System.IO.FileInfo finfo) { File = finfo; }

                public readonly System.IO.FileInfo File;

                public bool Exists => File?.Exists ?? false;

                public long Length => File?.Length ?? 0;

                public string PhysicalPath => File?.FullName;

                public string Name => File?.Name;

                public DateTimeOffset LastModified => File.LastWriteTime;

                public bool IsDirectory => false;

                public Stream CreateReadStream() { return File.OpenRead(); }
            }
    }
}
