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
using __MATCHCASING = System.IO.MatchCasing;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        #region API

        [return: NotNull]
        public static __XINFO GetFileInfo(this __XPROVIDER provider, params string[] subpath)
        {
            var path = System.IO.Path.Combine(subpath);
            path = path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            return provider.GetFileInfo(path);
        }


        [return: NotNull]
        public static __XDIRECTORY GetDirectoryContents(this __XPROVIDER provider, params string[] subpath)
        {
            var path = System.IO.Path.Combine(subpath);
            path = path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            return provider.GetDirectoryContents(path);
        }

        [return: NotNull]
        public static __XPROVIDER ToIFileProvider(this __XDIRECTORY dir, __MATCHCASING casing)
        {
            switch (dir)
            {
                case null: return _NullFileProvider;
                case __XPROVIDER provider: return provider; // already a provider
                default: return new _FileProviderOverDirectoryContents(dir, casing);
            }
        }

        #endregion

        #region nested types

        private static readonly NullFileProvider _NullFileProvider = new NullFileProvider();

        [System.Diagnostics.DebuggerDisplay("{_Dir}")]
        private sealed class _FileProviderOverDirectoryContents : __XPROVIDER
        {
            #region lifecycle
            public _FileProviderOverDirectoryContents(__XDIRECTORY dir, __MATCHCASING casing)
            {
                _Dir = dir;
                _Casing = casing;
            }

            #endregion

            #region data

            private readonly __XDIRECTORY _Dir;
            private readonly __MATCHCASING _Casing;

            #endregion

            #region API

            public __XINFO GetFileInfo(string subpath)
            {
                var parts = subpath.Replace('\\','/').Split('/');

                return _Dir.FindEntry(_Casing, parts);
            }

            public __XDIRECTORY GetDirectoryContents(string subpath)
            {
                subpath ??= string.Empty;

                var parts = subpath.Replace('\\', '/').Split('/');                

                return _Dir.FindEntry(_Casing, subpath) is __XDIRECTORY xdir
                    ? xdir
                    : Microsoft.Extensions.FileProviders.NotFoundDirectoryContents.Singleton;
            }

            public IChangeToken Watch(string filter)
            {
                return NullChangeToken.Singleton;
            }

            #endregion
        }

        #endregion
    }
}
