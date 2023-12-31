﻿// Copyright (c) CodeSugar 2024 Vicente Penades

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
        public static bool IsDirectorySeparatorChar(Char character)
        {
            return character == System.IO.Path.DirectorySeparatorChar || character == System.IO.Path.AltDirectorySeparatorChar;        
        }

        public static bool PathStartsWithNetworkDrivePrefix(string path)
        {
            if (path == null || path.Length < 2) return false;

            if (!IsDirectorySeparatorChar(path[0])) return false;
            if (!IsDirectorySeparatorChar(path[1])) return false;
            return true;
        }
        

        /// <summary>
        /// ensures that the path uses Path.DirectorySeparatorChar and it does not end with a path separator.
        /// </summary>
        /// <returns>
        /// a normalized path that is suited to be used for path string comparison.
        /// </returns>
        public static string GetNormalizedPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return String.Empty;

            if (path.IndexOfAny(_InvalidPathChars) >= 0) throw new ArgumentException("invalid chars", nameof(path));

            path = path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

            // unlike GetNormalizedFullName, this method can handle both absolute and relative paths

            if (PathStartsWithNetworkDrivePrefix(path))
            {
                path = path.TrimEnd(System.IO.Path.DirectorySeparatorChar);
            }
            else
            {
                path = path.Trim(System.IO.Path.DirectorySeparatorChar);
            }

            return path;
        }

        public static string[] SplitPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            // subtract network prefix

            string networkPrefix = null;

            if (PathStartsWithNetworkDrivePrefix(path))
            {
                networkPrefix = path.Substring(0,2);
                path = path.Substring(2);
            }

            // sanitize
            
            path = path.Trim(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            var parts = path.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            // restore network prefix

            if (networkPrefix != null)
            {
                parts[0] = networkPrefix + parts[0];
            }

            return parts;            
        }

        public static (string path, string name) SplitDirectoryAndName(string path)        
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            // subtract network prefix

            string networkPrefix = null;

            if (PathStartsWithNetworkDrivePrefix(path))
            {
                networkPrefix = path.Substring(0,2);
                path = path.Substring(2);
            }

            // sanitize

            path = path.Trim(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            // find last separator

            var idx = Math.Max(path.LastIndexOf(System.IO.Path.DirectorySeparatorChar), path.LastIndexOf(System.IO.Path.AltDirectorySeparatorChar));

            if (idx < 0) return (null, path);

            var name = path.Substring(idx + 1);
            path = path.Substring(0, idx);

            // restore network prefix

            if (networkPrefix != null)
            {
                path = networkPrefix + path;
            }

            return (path, name);            
        }

        public static string ConcatenatePaths(string basePath, string[] relativePath)
        {
            if (string.IsNullOrWhiteSpace(basePath)) throw new ArgumentNullException(nameof(basePath));

            if (relativePath == null || relativePath.Length == 0) return basePath;

            var path = basePath.TrimEnd(_DirectorySeparators);
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

        public static bool TryGetCompositedExtension(string fileName, int dots, out string extension)
        {
            if (dots < 1) throw new ArgumentOutOfRangeException(nameof(dots), "Must be equal or greater than 1");

            var l = fileName.Length - 1;
            var r = -1;

            var invalidChars = System.IO.Path.GetInvalidFileNameChars();

            while (dots > 0 && l >= 0)
            {
                var c = fileName[l];

                if (Array.IndexOf(invalidChars, c) >= 0) break;

                if (c == '.')
                {
                    r = l;
                    --dots;
                }

                --l;
            }

            if (dots != 0)
            {
                extension = null;
                return false;
            }

            extension = fileName.Substring(r);
            return true;
        }
    }
}
