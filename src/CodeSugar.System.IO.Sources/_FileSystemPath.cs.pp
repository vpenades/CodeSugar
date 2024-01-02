// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using FILE = System.IO.FileInfo;
using DIRECTORY = System.IO.DirectoryInfo;
using SYSTEMENTRY = System.IO.FileSystemInfo;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarIO
    {
        /// <summary>
        /// ensures that the path uses Path.DirectorySeparatorChar and it does not end with a path separator.
        /// </summary>
        /// <returns>
        /// a normalized path that is suited to be used for path string comparison.
        /// </returns>
        public static string GetNormalizedPath(string path)
        {
            if (path == null) return null;

            return path
                .Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar)
                .TrimEnd(System.IO.Path.DirectorySeparatorChar);
        }

        public static string ConcatenatePaths(string absolutePath, string[] relativePath)
        {
            if (string.IsNullOrWhiteSpace(absolutePath)) throw new ArgumentNullException(nameof(absolutePath));

            if (relativePath == null || relativePath.Length == 0) return absolutePath;

            var path = absolutePath.TrimEnd(_DirectorySeparators);
            foreach (var part in relativePath)
            {
                GuardIsValidFileName(part, false, nameof(relativePath));

                if (part == ".") continue;

                if (part == "..")
                {
                    var idx = path.LastIndexOfAny(_DirectorySeparators);
                    if (idx < 0) throw new ArgumentException("invalid ..", nameof(relativePath));
                    path = path.Substring(0, idx);                    
                    continue;
                }

                path = System.IO.Path.Combine(path, part);
            }

            return path;
        }        

        /// <summary>
        /// determines if two file system paths are equal.
        /// </summary>
        public static bool ArePathsEqual(string pathX, string pathY)
        {
            return ArePathsEqual(pathX, pathY, FileSystemStringComparison);
        }        

        /// <summary>
        /// determines if two file system paths are equal.
        /// </summary>
        public static bool ArePathsEqual(string pathX, string pathY, StringComparison comparer)
        {
            if (pathX == pathY) return true;
            if (pathX == null) return false;
            if (pathY == null) return false;               

            pathX = GetNormalizedPath(pathX);
            pathY = GetNormalizedPath(pathY);              

            return string.Equals(pathX, pathY, comparer);
        }

        /// <summary>
        /// calculates the hash code of a path, using the same rules used for path equality.
        /// </summary>
        public static int GetPathHashCode(string path)
        {
            return GetPathHashCode(path, FileSystemStringComparison);
        }

        /// <summary>
        /// calculates the hash code of a path, using the same rules used for path equality.
        /// </summary>
        public static int GetPathHashCode(string path, StringComparison comparer)
        {
            return path == null ? 0 : GetNormalizedPath(path).GetHashCode(comparer);
        }

        public static bool PathEndsWith(string path, string tail, StringComparison comparer)
        {
            while(tail.Length > 0)
            {
                var x = tail[tail.Length - 1];
                tail = tail.Substring(0, tail.Length - 1);

                var y = path[path.Length - 1];
                path = path.Substring(0, path.Length - 1);

                if (x == '?') continue;

                switch(comparer)
                {
                    case StringComparison.Ordinal: break;                    
                    case StringComparison.CurrentCulture: break;
                    case StringComparison.InvariantCulture: break;
                    case StringComparison.OrdinalIgnoreCase:                    
                    case StringComparison.CurrentCultureIgnoreCase:
                    case StringComparison.InvariantCultureIgnoreCase:
                        x = char.ToUpperInvariant(x);
                        y = char.ToUpperInvariant(y);
                        break;
                    default: throw new NotSupportedException();
                }                

                if (x != y) return false;
            }

            return true;
        }
    }
}
