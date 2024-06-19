// Copyright (c) CodeSugar 2024 Vicente Penades

using System;

#nullable disable

using PATH = System.IO.Path;
using CASING = System.IO.MatchCasing;
using System.Runtime.CompilerServices;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarForSystemIO
    {
        #region diagnostics        

        /// <summary>
		/// Checks whether a path is valid.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardIsValidFileName(string fileName, bool checkForInvalidNames,
            #if NET
            [CallerArgumentExpression("fileName")]
            #endif
            string paramName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(paramName);
            if (fileName.IndexOfAny(_InvalidNameChars) >= 0) throw new ArgumentException($"'{fileName}' contains invalid chars", paramName);

            // files and directories with leading/trailing white spaces can be
            // effectively created, but in practice it messes with windows explorer.
            if (Char.IsWhiteSpace(fileName[0]) || Char.IsWhiteSpace(fileName[fileName.Length - 1])) throw new ArgumentException($"'{fileName}' has leading or trailing white spaces", paramName);

            if (checkForInvalidNames)
            {
                if (fileName == "." || fileName == "..") throw new ArgumentException($"'{fileName}' is an invalid file name", paramName);
            }
        }        

        #endregion

        public static StringComparer GetStringComparer(this CASING casing)
        {
            switch (casing)
            {
                case CASING.CaseInsensitive: return StringComparer.OrdinalIgnoreCase;
                case CASING.CaseSensitive: return StringComparer.Ordinal;
                case CASING.PlatformDefault: return FileSystemStringComparer;
                default: throw new ArgumentOutOfRangeException(casing.ToString(), nameof(casing));
            }
        }

        public static StringComparison GetStringComparison(this CASING casing)
        {
            switch (casing)
            {
                case CASING.CaseInsensitive: return StringComparison.OrdinalIgnoreCase;
                case CASING.CaseSensitive: return StringComparison.Ordinal;
                case CASING.PlatformDefault: return FileSystemStringComparison;
                default: throw new ArgumentOutOfRangeException(casing.ToString(), nameof(casing));
            }
        }

        public static bool IsDirectorySeparatorChar(Char character)
        {
            return character == PATH.DirectorySeparatorChar || character == PATH.AltDirectorySeparatorChar;        
        }

        public static bool PathStartsWithNetworkDrivePrefix(string path)
        {
            if (path == null || path.Length < 2) return false;

            if (!IsDirectorySeparatorChar(path[0])) return false;
            if (!IsDirectorySeparatorChar(path[1])) return false;
            return true;
        }
        

        /// <summary>
        /// Ensures that the path uses the appropiate Path.DirectorySeparatorChar and it does not end with a path separator.
        /// </summary>
        /// <returns>
        /// A normalized path that is suited to be used for path string comparison.
        /// </returns>
        public static string GetNormalizedFullyQualifiedPath(string path)
        {
            path = GetNormalizedPath(path);

            if (PATH.IsPathFullyQualified(path)) return path;

            path = PATH.GetFullPath(path);            

            /*
            path = PathStartsWithNetworkDrivePrefix(path)
                ? path.TrimEnd(PATH.DirectorySeparatorChar)
                : path.Trim(PATH.DirectorySeparatorChar);*/

            return path;
        }

        /// <summary>
        /// Ensures that the path uses the appropiate Path.DirectorySeparatorChar and it does not end with a path separator.
        /// </summary>
        /// <remarks>
        /// This funcion can be used for both absolute and relative paths.
        /// </remarks>
        public static string GetNormalizedPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return String.Empty;

            if (path.IndexOfAny(_InvalidPathChars) >= 0) throw new ArgumentException("invalid chars", nameof(path));

            return path
                .Replace(PATH.AltDirectorySeparatorChar, PATH.DirectorySeparatorChar)
                .TrimEnd(PATH.DirectorySeparatorChar);
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
            
            path = path.Trim(_DirectorySeparators);
            var parts = path.Split(_DirectorySeparators);

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

            path = path.Trim(_DirectorySeparators);

            // find last separator

            var idx = path.LastIndexOfAny(_DirectorySeparators);

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

        public static string ConcatenatePaths(string basePath, params string[] relativePath)
        {
            if (string.IsNullOrWhiteSpace(basePath)) throw new ArgumentNullException(nameof(basePath));

            if (relativePath == null || relativePath.Length == 0) return basePath;

            if (relativePath.Length == 1)
            {
                var rp = relativePath[0].Trim(_DirectorySeparators);
                if (rp.IndexOfAny(_DirectorySeparators)>=0)
                {
                    var parts = SplitPath(rp);
                    if (parts.Length != 1) return ConcatenatePaths(basePath, parts);
                }
            }

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

                path = PATH.Combine(path, part);
            }

            return path;
        }                  

        /// <summary>
        /// determines if two file system paths are equal.
        /// </summary>
        public static bool ArePathsEqual(this CASING casing, string pathX, string pathY)
        {
            if (pathX == pathY) return true;
            if (pathX == null) return false;
            if (pathY == null) return false;               

            pathX = GetNormalizedFullyQualifiedPath(pathX);
            pathY = GetNormalizedFullyQualifiedPath(pathY);              

            return string.Equals(pathX, pathY, GetStringComparison(casing));
        }

        public static bool PathStartsWith(this CASING casing, string path, string head)
        {
            if (path == null && head == null) return true;
            if (path == null) return false;
            if (head == null) return true;

            path = GetNormalizedFullyQualifiedPath(path);
            head = GetNormalizedPath(head);

            return path.StartsWith(head, GetStringComparison(casing));
        }

        public static bool PathEndsWith(this CASING casing, string path, string tail)
        {
            if (path == null && tail == null) return true;
            if (path == null) return false;
            if (tail == null) return true;

            path = GetNormalizedFullyQualifiedPath(path);
            tail = GetNormalizedPath(tail);

            return path.EndsWith(tail, GetStringComparison(casing));

        }

        public static bool PathEndsWith(this CASING casing,  string path, string tail, bool tailHasWildcards)
        {
            if (path == null && tail == null) return true;
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
        public static int GetPathHashCode(this CASING casing, string path)
        {
            if (string.IsNullOrEmpty(path)) return 0;
            path = GetNormalizedFullyQualifiedPath(path);

            return path.GetHashCode(GetStringComparison(casing));
        }
    }
}
