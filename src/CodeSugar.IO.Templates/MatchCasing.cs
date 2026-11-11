using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using __IOPATH = System.IO.Path;
using __PATHCASING = System.IO.MatchCasing;
using __RTINTEROPSVCS = System.Runtime.InteropServices;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        #region constants

        private static readonly bool __FileSystemIsCaseSensitive = _CheckFileSystemCaseSensitive();        

        public static readonly StringComparer __FileSystemStringComparer = __FileSystemIsCaseSensitive
            ? StringComparer.Ordinal
            : StringComparer.OrdinalIgnoreCase;
        private static readonly StringComparer[] __MatchCasingStringComparer = new StringComparer[]
        {
            __FileSystemStringComparer,
            StringComparer.Ordinal,
            StringComparer.OrdinalIgnoreCase
        };

        private static readonly StringComparison __FileSystemStringComparison = __FileSystemIsCaseSensitive
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;
        private static readonly StringComparison[] __MatchCasingStringComparison = new StringComparison[]
        {
            __FileSystemStringComparison,
            StringComparison.Ordinal,
            StringComparison.OrdinalIgnoreCase
        };

        #endregion

        #region Casing

        private static bool _CheckFileSystemCaseSensitive()
        {
            // credits: https://stackoverflow.com/a/56773947

            if (__RTINTEROPSVCS.RuntimeInformation.IsOSPlatform(__RTINTEROPSVCS.OSPlatform.Windows) ||
                __RTINTEROPSVCS.RuntimeInformation.IsOSPlatform(__RTINTEROPSVCS.OSPlatform.OSX))  // HFS+ (the Mac file-system) is usually configured to be case insensitive.
            {
                return false;
            }
            else if (__RTINTEROPSVCS.RuntimeInformation.IsOSPlatform(__RTINTEROPSVCS.OSPlatform.Linux))
            {
                return true;
            }
            else if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                return true;
            }
            else
            {
                // A default.
                return false;
            }
        }

        public static StringComparer GetStringComparer(this __PATHCASING casing)
        {
            return __MatchCasingStringComparer[(int)casing];
        }

        public static StringComparison GetStringComparison(this __PATHCASING casing)
        {
            return __MatchCasingStringComparison[(int)casing];
        }

        /// <summary>
        /// determines if two paths are equal.
        /// </summary>
        [Obsolete("Use AreFullPathsEqual")]
        public static bool ArePathsEqual(this __PATHCASING casing, string pathX, string pathY)
        {
            return AreFullPathsEqual(casing, pathX, pathY);
        }

        /// <summary>
        /// determines if two FULLY QUALIFIED file system paths are equal.
        /// </summary>
        public static bool AreFullPathsEqual(this __PATHCASING casing, string pathX, string pathY)
        {
            return _AreFullPathsEqual(GetStringComparison(casing), pathX, pathY);
        }
        
        private static bool _AreFullPathsEqual(StringComparison casing, string pathX, string pathY)
        {
            if (pathX == pathY) return true;
            if (pathX == null) return false;
            if (pathY == null) return false;

            pathX = GetNormalizedFullyQualifiedPath(pathX);
            pathY = GetNormalizedFullyQualifiedPath(pathY);

            return string.Equals(pathX, pathY, casing);
        }

        private static int _CompareFullPaths(StringComparison casing, string pathX, string pathY)
        {
            if (pathX == pathY) return 0;
            if (pathX == null) return 1;
            if (pathY == null) return -1;

            pathX = GetNormalizedFullyQualifiedPath(pathX);
            pathY = GetNormalizedFullyQualifiedPath(pathY);

            return string.Compare(pathX, pathY, casing);
        }

        public static bool PathStartsWith(this __PATHCASING casing, string path, string head)
        {
            if (path == head) return true;
            if (path == null) return false;
            if (head == null) return true;

            path = GetNormalizedFullyQualifiedPath(path);
            head = GetNormalizedPath(head);

            return path.StartsWith(head, GetStringComparison(casing));
        }

        public static bool PathEndsWith(this __PATHCASING casing, string path, string tail)
        {
            if (path == tail) return true;
            if (path == null) return false;
            if (tail == null) return true;

            path = GetNormalizedFullyQualifiedPath(path);
            tail = GetNormalizedPath(tail);

            return path.EndsWith(tail, GetStringComparison(casing));

        }

        public static bool PathEndsWith(this __PATHCASING casing, string path, string tail, bool tailHasWildcards)
        {
            if (path == tail) return true;
            if (path == null) return false;
            if (tail == null) return true;

            while (tail.Length > 0)
            {
                var path_curr = path[path.Length - 1];
                path = path.Substring(0, path.Length - 1);

                var tail_curr = tail[tail.Length - 1];
                tail = tail.Substring(0, tail.Length - 1);

                if (tail_curr == '?') continue; // tail supports wildcards

                switch (GetStringComparison(casing))
                {
                    case StringComparison.Ordinal: break;
                    case StringComparison.CurrentCulture: break;
                    case StringComparison.InvariantCulture: break;
                    case StringComparison.OrdinalIgnoreCase:
                    case StringComparison.CurrentCultureIgnoreCase:
                    case StringComparison.InvariantCultureIgnoreCase:
                        tail_curr = char.ToUpperInvariant(tail_curr);
                        path_curr = char.ToUpperInvariant(path_curr);
                        break;
                    default: throw new NotSupportedException();
                }

                if (tail_curr != path_curr) return false;
            }

            return true;
        }

        /// <summary>
        /// calculates the hash code of a path, using the same rules used for path equality.
        /// </summary>
        [Obsolete("Use GetFullPathHashCode")]
        public static int GetPathHashCode(this __PATHCASING casing, string path)
        {
            if (string.IsNullOrEmpty(path)) return 0;
            path = GetNormalizedFullyQualifiedPath(path);
            return path.GetHashCode(GetStringComparison(casing));
        }

        /// <summary>
        /// calculates the hash code of a FULLY QUALIFIED path, using the same rules used for path equality.
        /// </summary>
        public static int GetFullPathHashCode(this __PATHCASING casing, string path)
        {
            return _GetFullPathHashCode(GetStringComparison(casing), path);
        }
        
        private static int _GetFullPathHashCode(StringComparison casing, string path)
        {
            if (string.IsNullOrEmpty(path)) return 0;
            path = GetNormalizedFullyQualifiedPath(path);
            return path.GetHashCode(casing);
        }

        #endregion

        #region nested types

        /// <summary>
        /// Used to determine if a given path is in the directory defined by <see cref="FullPath"/>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Diagnostics.DebuggerDisplay("{_PathSelector,nq}")]
        internal readonly struct _PathSlicer
        {
            #region lifecycle

            public _PathSlicer(string fullDirPath, __PATHCASING casing)
            {
                FullPath = _SanitizePath(fullDirPath ?? string.Empty).TrimEnd('/');
                Casing = GetStringComparison(casing);
            }

            public _PathSlicer(string fullDirPath, StringComparison casing)
            {
                FullPath = _SanitizePath(fullDirPath ?? string.Empty).TrimEnd('/');
                Casing = casing;
            }

            #endregion

            #region data

            /// <summary>
            /// Represents a full directory path.
            /// </summary>
            public string FullPath { get; }
            public StringComparison Casing { get; }

            #endregion

            #region API

            /// <summary>
            /// Given a collection of full paths, it returns
            /// </summary>
            /// <param name="paths"></param>
            /// <returns></returns>
            public IEnumerable<(string name, bool isDirectory)> Filter(IEnumerable<string> paths)
            {
                HashSet<string> dirs = null;

                foreach (var itemKey in paths)
                {
                    var xkey = itemKey;

                    if (!_TryTrimBasePath(ref xkey)) continue;

                    var idx = xkey.IndexOf('/');
                    if (idx >= 0)
                    {
                        dirs ??= new HashSet<string>();
                        dirs.Add(xkey.Substring(0, idx));
                    }
                    else yield return (xkey, false);
                }

                if (dirs != null)
                {
                    foreach (var d in dirs) yield return (d, true);
                }
            }

            public bool Contains(string itemKey)
            {
                if (!_TryTrimBasePath(ref itemKey)) return false;

                return !itemKey.TrimEnd('/').Contains('/');
            }

            private bool _TryTrimBasePath(ref string itemKey)
            {
                itemKey = _SanitizePath(itemKey);

                if (!itemKey.StartsWith(FullPath, Casing)) return false;

                itemKey = itemKey
                    .Substring(FullPath.Length)
                    .TrimStart('/');

                if (itemKey == "/") itemKey = string.Empty;

                return !string.IsNullOrEmpty(itemKey);
            }

            private static string _SanitizePath(string path)
            {
                return path
                    .Replace('\\', '/')
                    .TrimStart('/');
            }

            #endregion
        }

        #endregion
    }
}
