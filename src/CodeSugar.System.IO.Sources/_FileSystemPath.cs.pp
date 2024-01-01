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

        public static bool ArePathsEqual(string pathX, string pathY)
        {
            return ArePathsEqual(pathX, pathY, FileSystemStringComparison);
        }        

        public static bool ArePathsEqual(string pathX, string pathY, StringComparison comparer)
        {
            if (pathX == pathY) return true;
            if (pathX == null) return false;
            if (pathY == null) return false;               

            pathX = GetNormalizedPath(pathX);
            pathY = GetNormalizedPath(pathY);              

            return string.Equals(pathX, pathY, comparer);
        }

        public static int GetPathHashCode(string path)
        {
            return GetPathHashCode(path, FileSystemStringComparison);
        }

        public static int GetPathHashCode(string path, StringComparison comparer)
        {
            return path == null ? 0 : GetNormalizedPath(path).GetHashCode(comparer);
        }
    }
}
