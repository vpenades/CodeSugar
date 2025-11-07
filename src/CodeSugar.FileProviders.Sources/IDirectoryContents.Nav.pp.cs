// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

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
        [return: NotNull]
        public static __XINFO FindEntry(this __XINFO xfile, params string[] path)
        {
            GuardNotNull(xfile);
            if (!TryGetStringComparison(xfile, out var cmp)) throw new NotSupportedException("name comparer not found");
            return __FindEntry(xfile, cmp, path);
        }

        [return: NotNull]
        public static __XINFO FindEntry(this __XINFO xfile, __MATCHCASING casing, params string[] path)
        {
            GuardNotNull(xfile);
            var cmp = __ToStringComparison(casing);
            return __FindEntry(xfile, cmp, path);
        }

        [return: NotNull]
        public static __XINFO FindEntry(this __XDIRECTORY xdir, params string[] path)
        {
            GuardNotNull(xdir);
            if (!TryGetStringComparison(xdir, out var cmp)) throw new NotSupportedException("name comparer not found");
            return __FindEntry(xdir, cmp, path);
        }

        [return: NotNull]
        public static __XINFO FindEntry(this __XDIRECTORY xdir, __MATCHCASING casing, params string[] path)
        {
            GuardNotNull(xdir);
            var cmp = __ToStringComparison(casing);
            return __FindEntry(xdir, cmp, path);
        }

        [return: NotNull]
        private static __XINFO __FindEntry(this __XINFO xfile, StringComparison nameComparer, params string[] path)
        {
            // when path is empty, return self.
            if (path.Length == 0 || (path.Length == 1 && string.IsNullOrEmpty(path[0]))) return xfile;

            var dc = GetDirectoryContents(xfile);
            System.Diagnostics.Debug.Assert(dc != null);

            return __FindEntry(dc, nameComparer, path);
        }

        [return: NotNull]
        private static __XINFO __FindEntry(this __XDIRECTORY xdir, StringComparison nameComparer, params string[] path)
        {
            if (xdir == null || !xdir.Exists) return __NULLFILE;

            // when path is empty, return a best effort to get an self xfile
            if (path.Length == 0 || (path.Length == 1 && string.IsNullOrEmpty(path[0])))
            {
                var entry = xdir as __XINFO;

                return entry != null
                    ? entry
                    // TODO: it it can't be cast back, we might want to wrap a IDirectoryContents as an IFileInfo
                    : new Microsoft.Extensions.FileProviders.NotFoundFileInfo(path[path.Length - 1]);
            }

            for (int i = 0; i < path.Length; ++i)
            {
                var name = path[i];

                // validate path name
                if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(path));
                if (name.Contains(System.IO.Path.DirectorySeparatorChar)) throw new ArgumentException(nameof(path));
                if (name.Contains(System.IO.Path.AltDirectorySeparatorChar)) throw new ArgumentException(nameof(path));

                bool isLast = i == path.Length - 1;
                bool found = false;

                foreach (var entry in xdir)
                {
                    if (!string.Equals(entry.Name, name, nameComparer)) continue;

                    if (isLast) return entry;
                    else if (!entry.IsDirectory) continue; // skip files

                    xdir = GetDirectoryContents(entry);
                    System.Diagnostics.Debug.Assert(xdir != null);
                    if (!xdir.Exists) return new Microsoft.Extensions.FileProviders.NotFoundFileInfo(name);

                    found = true;
                    break;
                }

                if (!found) break;
            }

            return new Microsoft.Extensions.FileProviders.NotFoundFileInfo(path[path.Length - 1]);
        }
    }
}
