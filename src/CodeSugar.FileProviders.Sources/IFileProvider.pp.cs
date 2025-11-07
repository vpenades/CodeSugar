// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

#nullable disable

using __XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using __XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;
using __XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;

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
        public static IFileInfo GetFileInfo(this __XPROVIDER provider,params string[] subpath)
        {
            var path = System.IO.Path.Combine(subpath);
            path = path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            return provider.GetFileInfo(path);
        }


        [return: NotNull]
        public static IDirectoryContents GetDirectoryContents(this __XPROVIDER provider, params string[] subpath)
        {
            var path = System.IO.Path.Combine(subpath);
            path = path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            return provider.GetDirectoryContents(path);
        }

        [return: NotNull]
        public static __XPROVIDER ToIFileProvider(this __XDIRECTORY dir)
        {
            switch(dir)
            {
                case null: return _NullFileProvider;
                case __XPROVIDER provider: return provider; // already a provider
                default: return new _FileProviderOverDirectoryContents(dir);
            }            
        }

        #endregion

        #region nested types

        private static readonly NullFileProvider _NullFileProvider = new NullFileProvider();

        private sealed class _FileProviderOverDirectoryContents : __XPROVIDER
        {
            private readonly __XDIRECTORY _Dir;

            public _FileProviderOverDirectoryContents(__XDIRECTORY dir)
            {
                _Dir = dir;
            }

            public __XINFO GetFileInfo(string subpath)
            {
                return _Dir.FindEntry(subpath);
            }

            public __XDIRECTORY GetDirectoryContents(string subpath)
            {
                return _Dir.FindEntry(subpath) is __XDIRECTORY
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
