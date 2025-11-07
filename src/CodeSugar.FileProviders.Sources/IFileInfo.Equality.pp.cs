// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using __XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using __XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;
using __MATCHCASING = System.IO.MatchCasing;


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
        public static __MATCHCASING FileSystemPathCasing { get; } = FileSystemIsCaseSensitive ? __MATCHCASING.CaseSensitive : __MATCHCASING.CaseInsensitive;
        public static StringComparison FileSystemPathComparison { get; } = FileSystemIsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;        

        #endregion

        public static bool NameEquals(this __XINFO xfile, string name)
        {
            if (!TryGetStringComparison(xfile, out var cmp)) throw new NotSupportedException();
            return string.Equals(xfile.Name, name, cmp);
        }

        public static bool NameEquals(this __XINFO xfile, string name, __MATCHCASING casing)
        {
            return string.Equals(xfile.Name, name, __ToStringComparison(casing));
        }


        public static bool TryGetStringComparison(this __XDIRECTORY xfile, out StringComparison cmp)
        {
            if (!_TryGetMatchCasing(xfile, out var casing)) { cmp = default; return false; }

            cmp = __ToStringComparison(casing);
            return true;
        }

        public static bool TryGetStringComparison(this __XINFO xfile, out StringComparison cmp)
        {
            if (!_TryGetMatchCasing(xfile, out var casing)) { cmp = default; return false; }

            cmp = __ToStringComparison(casing);
            return true;
        }

        private static bool _TryGetMatchCasing<T>(this T casingSource, out __MATCHCASING casing)
        {
            if (casingSource == null) throw new ArgumentNullException(nameof(casingSource));

            if (casingSource is IServiceProvider srv)
            {
                if (srv.GetService(typeof(__MATCHCASING)) is __MATCHCASING srvCasing)
                {
                    casing = srvCasing;
                    return true;
                }
            }            

            if (casingSource is __XINFO xfile && IsPhysical(xfile))
            {
                casing = __MATCHCASING.PlatformDefault;
                return true;
            }

            if (casingSource is __XDIRECTORY xdir)
            {
                switch(xdir.GetType().FullName)
                {
                    case "Microsoft.Extensions.FileProviders.Internal.PhysicalDirectoryContents":
                        casing = __MATCHCASING.PlatformDefault;
                        return true;
                }                
            }


            casing = default;
            return false;
        }

        private static StringComparison __ToStringComparison(this __MATCHCASING casing)
        {
            switch (casing)
            {
                case __MATCHCASING.PlatformDefault: return FileSystemPathComparison;
                case __MATCHCASING.CaseSensitive: return StringComparison.Ordinal;
                case __MATCHCASING.CaseInsensitive: return StringComparison.OrdinalIgnoreCase;
                default: throw new NotImplementedException();
            }
        }
    }
}
