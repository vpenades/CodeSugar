using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using __IOPATH = System.IO.Path;
using __PATHCASING = System.IO.MatchCasing;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        #region diagnostics        

        /// <summary>
		/// Checks whether a path is valid.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardIsValidFileName(string fileName, bool checkForInvalidNames, [CallerArgumentExpression(nameof(fileName))] string paramName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(paramName);
            if (fileName.IndexOfAny(_InvalidNameChars) >= 0) throw new ArgumentException($"'{fileName}' contains invalid chars", paramName ?? nameof(fileName));

            // files and directories with leading/trailing white spaces can be
            // effectively created, but in practice it messes with windows explorer.
            if (Char.IsWhiteSpace(fileName[0]) || Char.IsWhiteSpace(fileName[fileName.Length - 1])) throw new ArgumentException($"'{fileName}' has leading or trailing white spaces", paramName ?? nameof(fileName));

            if (checkForInvalidNames)
            {
                if (fileName == "." || fileName == "..") throw new ArgumentException($"'{fileName}' is an invalid file name", paramName ?? nameof(fileName));
            }
        }

        #endregion

        

        public static bool IsDirectorySeparatorChar(Char character)
        {
            return character == __IOPATH.DirectorySeparatorChar || character == __IOPATH.AltDirectorySeparatorChar;        
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

            if (__IOPATH.IsPathFullyQualified(path)) return path;

            path = __IOPATH.GetFullPath(path);            

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
                .Replace(__IOPATH.AltDirectorySeparatorChar, __IOPATH.DirectorySeparatorChar)
                .TrimEnd(__IOPATH.DirectorySeparatorChar);
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

                path = __IOPATH.Combine(path, part);
            }

            return path;
        }            
    }
}
