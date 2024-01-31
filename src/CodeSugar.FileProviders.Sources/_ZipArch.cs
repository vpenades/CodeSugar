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
        private readonly struct _ZipArchiveFile : IFileInfo
        {
            public _ZipArchiveFile(System.IO.Compression.ZipArchiveEntry entry)
            {
                Entry = entry;
            }

            public readonly System.IO.Compression.ZipArchiveEntry Entry;

            public bool Exists => Entry != null;

            public long Length => Entry?.Length ?? 0;

            public string? PhysicalPath => null;

            public string Name => Entry.Name;

            public DateTimeOffset LastModified => Entry.LastWriteTime;

            public bool IsDirectory => false;

            public Stream CreateReadStream()
            {
                return Entry.Open();
            }
        }
    }
}
