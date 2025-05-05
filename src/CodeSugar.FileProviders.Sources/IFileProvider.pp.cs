// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.FileProviders;

#nullable disable

using _XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using _XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;
using _XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;


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
        public static IFileInfo GetFileInfo(this _XPROVIDER provider,params string[] subpath)
        {
            var path = System.IO.Path.Combine(subpath);
            path = path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            return provider.GetFileInfo(path);
        }


        [return: NotNull]
        public static IDirectoryContents GetDirectoryContents(this _XPROVIDER provider, params string[] subpath)
        {
            var path = System.IO.Path.Combine(subpath);
            path = path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            return provider.GetDirectoryContents(path);
        }

        [return: NotNull]
        public static _XPROVIDER ToIFileProvider(this _XDIRECTORY dir)
        {
            switch(dir)
            {
                case null: return _NullFileProvider;
                case _XPROVIDER provider: return provider; // already a provider
                default: return new _FileProviderOverDirectoryContents(dir);
            }            
        }

        #endregion

        #region nested types

        private static readonly NullFileProvider _NullFileProvider = new NullFileProvider();

        private sealed class _FileProviderOverDirectoryContents : IFileProvider
        {
            private readonly _XDIRECTORY _Dir;

            public _FileProviderOverDirectoryContents(_XDIRECTORY dir)
            {
                _Dir = dir;
            }

            public _XINFO GetFileInfo(string subpath)
            {
                return _Dir.FindEntry(subpath);
            }

            public _XDIRECTORY GetDirectoryContents(string subpath)
            {
                return _Dir.FindEntry(subpath) is _XDIRECTORY
                    ? _Dir
                    : Microsoft.Extensions.FileProviders.NotFoundDirectoryContents.Singleton;
            }

            public IChangeToken Watch(string filter)
            {
                return NullChangeToken.Singleton;
            }
        }

        #endregion
    }
}
