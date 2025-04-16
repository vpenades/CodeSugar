// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using _XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using _XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;
using _MATCHCASING = System.IO.MatchCasing;


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
        #region constants

        public static bool FileSystemIsCaseSensitive { get; } = _CheckFileSystemCaseSensitive();
        public static _MATCHCASING FileSystemPathCasing { get; } = FileSystemIsCaseSensitive ? _MATCHCASING.CaseSensitive : _MATCHCASING.CaseInsensitive;
        public static StringComparison FileSystemPathComparison { get; } = FileSystemIsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;        

        #endregion

        public static bool NameEquals(this _XINFO xfile, string name)
        {
            if (!TryGetStringComparison(xfile, out var cmp)) throw new NotSupportedException();
            return string.Equals(xfile.Name, name, cmp);
        }

        public static bool NameEquals(this _XINFO xfile, string name, _MATCHCASING casing)
        {
            return string.Equals(xfile.Name, name, __ToStringComparison(casing));
        }


        public static bool TryGetStringComparison(this _XDIRECTORY xfile, out StringComparison cmp)
        {
            if (!_TryGetMatchCasing(xfile, out var casing)) { cmp = default; return false; }

            cmp = __ToStringComparison(casing);
            return true;
        }

        public static bool TryGetStringComparison(this _XINFO xfile, out StringComparison cmp)
        {
            if (!_TryGetMatchCasing(xfile, out var casing)) { cmp = default; return false; }

            cmp = __ToStringComparison(casing);
            return true;
        }

        private static bool _TryGetMatchCasing<T>(this T casingSource, out _MATCHCASING casing)
        {
            if (casingSource == null) throw new ArgumentNullException(nameof(casingSource));

            if (casingSource is IServiceProvider srv)
            {
                if (srv.GetService(typeof(_MATCHCASING)) is _MATCHCASING srvCasing)
                {
                    casing = srvCasing;
                    return true;
                }
            }            

            if (casingSource is _XINFO xfile && IsPhysical(xfile))
            {
                casing = _MATCHCASING.PlatformDefault;
                return true;
            }

            if (casingSource is _XDIRECTORY xdir)
            {
                switch(xdir.GetType().FullName)
                {
                    case "Microsoft.Extensions.FileProviders.Internal.PhysicalDirectoryContents":
                        casing = _MATCHCASING.PlatformDefault;
                        return true;
                }                
            }


            casing = default;
            return false;
        }

        private static StringComparison __ToStringComparison(this _MATCHCASING casing)
        {
            switch (casing)
            {
                case _MATCHCASING.PlatformDefault: return FileSystemPathComparison;
                case _MATCHCASING.CaseSensitive: return StringComparison.Ordinal;
                case _MATCHCASING.CaseInsensitive: return StringComparison.OrdinalIgnoreCase;
                default: throw new NotImplementedException();
            }
        }
    }
}
