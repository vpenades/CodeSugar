// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics.CodeAnalysis;

using XFILE = Microsoft.Extensions.FileProviders.IFileInfo;
using MATCHCASING = System.IO.MatchCasing;

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
        #region API

        [return: NotNull]
        public static XFILE ToFileIFileInfo(this System.IO.Compression.ZipArchiveEntry entry)
        {
            return entry == null
                ? __NULLFILE
                : new _ZipArchiveFile(entry);
        }

        #endregion

        #region nested types

        [System.Diagnostics.DebuggerDisplay("{Entry.FullName} {Length}")]
        private readonly struct _ZipArchiveFile : XFILE, IServiceProvider
        {
            #region lifecycle

            public _ZipArchiveFile(System.IO.Compression.ZipArchiveEntry entry)
            {
                Entry = entry;
            }

            #endregion

            #region data

            public System.IO.Compression.ZipArchiveEntry Entry { get; }

            #endregion

            #region properties

            public bool Exists => Entry != null;

            public long Length => Entry?.Length ?? 0;

            public string PhysicalPath => null;

            public string Name => Entry.Name;

            public DateTimeOffset LastModified => Entry.LastWriteTime;

            public bool IsDirectory => false;

            #endregion

            #region API

            public Stream CreateReadStream()
            {
                if (Entry?.Archive == null) throw new ObjectDisposedException(nameof(Entry));
                if (Entry.Archive.Mode == System.IO.Compression.ZipArchiveMode.Create) throw new System.IO.IOException("read is not supported");

                return Entry.Open();
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(MATCHCASING)) return MATCHCASING.CaseSensitive;
                if (serviceType == typeof(StringComparison)) return StringComparison.Ordinal;
                if (serviceType == typeof(System.IO.Compression.ZipArchiveEntry)) return Entry;
                if (serviceType == typeof(System.IO.Compression.ZipArchive)) return Entry?.Archive;
                return null;
            }

            #endregion
        }

        #endregion
    }
}
