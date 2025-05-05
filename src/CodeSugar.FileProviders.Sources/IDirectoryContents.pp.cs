// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

#nullable disable

using _XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using _XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;
using _MATCHCASING = System.IO.MatchCasing;
using System.Collections;
using Microsoft.Extensions.FileProviders;


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
        #region API        
        

        [return: NotNull]
        public static _XDIRECTORY ToIDirectoryContents(this IEnumerable<_XINFO> files)
        {
            switch(files)
            {
                case null: return NotFoundDirectoryContents.Singleton;
                case _XDIRECTORY xdir: return xdir;
                default: return new _DirectoryCollection(files.ToList());
            }            
        }

        #endregion

        #region nested types

        

        private sealed class _DirectoryCollection : _XDIRECTORY
        {
            public _DirectoryCollection(IReadOnlyList<_XINFO> files)
            {
                _Files = files;
            }

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            private readonly IReadOnlyList<_XINFO> _Files;

            public bool Exists => true;

            public IEnumerator<_XINFO> GetEnumerator()
            {
                return _Files.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _Files.GetEnumerator();
            }
        }

        #endregion
    }
}
