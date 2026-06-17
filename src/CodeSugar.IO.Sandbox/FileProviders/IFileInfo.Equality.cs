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


namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        public static bool NameEquals(this __XINFO xfile, string name)
        {
            if (!TryGetStringComparison(xfile, out var cmp)) throw new NotSupportedException();
            return string.Equals(xfile.Name, name, cmp);
        }

        public static bool NameEquals(this __XINFO xfile, string name, __MATCHCASING casing)
        {
            var cmp = GetStringComparison(casing);
            return string.Equals(xfile.Name, name, cmp);
        }

        /// <summary>
        /// Tries to determine of <paramref name="left"/> and <paramref name="right"/> represent the same resource.
        /// </summary>
        /// <param name="left">A resource reference</param>
        /// <param name="right">A resource reference</param>
        /// <param name="casing">if resources define physical paths, the casing to use for comparing the paths</param>
        /// <returns></returns>
        public static bool IsSameResourceAs(this __XINFO left, __XINFO right, __MATCHCASING casing)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;
            if (right == null) return false;

            if (object.ReferenceEquals(left, right)) return true;

            if (left.IsDirectory != right.IsDirectory) return false;

            if (!string.IsNullOrEmpty(left.PhysicalPath) && !string.IsNullOrEmpty(right.PhysicalPath))
            {
                var leftPath = System.IO.Path.GetFullPath(left.PhysicalPath).Replace('\\', '/').TrimEnd('/');
                var rightPath = System.IO.Path.GetFullPath(right.PhysicalPath).Replace('\\', '/').TrimEnd('/');

                return string.Equals(leftPath, rightPath, GetStringComparison(casing));
            }

            // do this only AFTER comparing PhysicalPath
            if (left.GetType() != right.GetType()) return false;

            return left.Equals(right);
        }


        public static bool TryGetStringComparison(this __XDIRECTORY xfile, out StringComparison cmp)
        {
            if (!_TryGetMatchCasing(xfile, out var casing)) { cmp = default; return false; }

            cmp = GetStringComparison(casing);
            return true;
        }

        public static bool TryGetStringComparison(this __XINFO xfile, out StringComparison cmp)
        {
            if (!_TryGetMatchCasing(xfile, out var casing)) { cmp = default; return false; }

            cmp = GetStringComparison(casing);
            return true;
        }

        private static bool _TryGetMatchCasing<T>(T casingSource, out __MATCHCASING casing)
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
    }
}
