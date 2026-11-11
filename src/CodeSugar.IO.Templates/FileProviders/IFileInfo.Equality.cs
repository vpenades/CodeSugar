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
        public static int GetIFileInfoHashCode<T>(this __MATCHCASING casing, T xfile)
            where T: __XINFO
        {
            if (xfile == null) return 0;
            if (IsPhysical(xfile)) return casing.GetFullPathHashCode(xfile.PhysicalPath);
            return xfile.GetHashCode();
        }

        /// <summary>
        /// Tries to determine of <paramref name="left"/> and <paramref name="right"/> represent the same resource.
        /// </summary>
        /// <param name="left">A resource reference</param>
        /// <param name="right">A resource reference</param>
        /// <param name="casing">if resources define physical paths, the casing to use for comparing the paths</param>
        /// <returns></returns>
        [Obsolete("Use AreEqualIFileInfos", true)]
        public static bool IsSameResourceAs(this __XINFO left, __XINFO right, __MATCHCASING casing)
        {
            return AreEqualIFileInfos(casing, left, right);
        }

        public static bool AreEqualIFileInfos<TLeft,TRight>(this __MATCHCASING casing, TLeft left, TRight right)
            where TLeft: __XINFO
            where TRight: __XINFO
        {
            if (left == null && right == null) return true;
            if (left == null) return false;
            if (right == null) return false;

            if (left.IsDirectory != right.IsDirectory) return false;

            if (object.ReferenceEquals(left, right)) return true;            

            if (IsPhysical(left) && IsPhysical(right))
            {
                // this is weak because both objects may have the same physical paths
                // representing the same file system resource. But maybe one object might
                // have runtime metadata and the other don't, so we might be discarding
                // a rich object vs a poor object.

                return casing.AreFullPathsEqual(left.PhysicalPath, right.PhysicalPath);
            }

            // do this ONLY AFTER being sure that the files
            // do not represent physical file system resources.

            if (left.GetType() != right.GetType()) return false;

            return left.Equals(right);
        }

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
