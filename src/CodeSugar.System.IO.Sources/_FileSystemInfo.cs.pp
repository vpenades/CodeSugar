// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
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
    static partial class CodeSugarIO    
    {
		#region diagnostics

        #if !NET

        /// <summary>
		/// Checks whether a <see cref="SYSTEMENTRY"/> is not null.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
        public static void GuardNotNull(this SYSTEMENTRY info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
        }        

        /// <summary>
		/// Checks whether a <see cref="SYSTEMENTRY"/> exists in the file system.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
        public static void GuardExists(this SYSTEMENTRY info)
        {
            if (!info.Exists()) throw new ArgumentException($"'{info.FullName}' does not exist.", nameof(info));
        }  

        #else

		/// <summary>
		/// Checks whether a <see cref="SYSTEMENTRY"/> is not null.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardNotNull(this SYSTEMENTRY info, [CallerArgumentExpression("info")] string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);            
        }

		/// <summary>
		/// Checks whether a <see cref="SYSTEMENTRY"/> exists in the file system.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardExists(this SYSTEMENTRY info, [CallerArgumentExpression("info")] string name = null)
        {
            if (!info.Exists()) throw new ArgumentException($"'{info.FullName}' does not exist.", name);
        }

        #endif

        #endregion

        #region nav tree

        /// <summary>
        /// Gets a value indicating whether <paramref name="info"/> exists in the file system.
        /// </summary>
        /// <param name="info">A <see cref="FILE"/> or a <see cref="DIRECTORY"/> object.</param>
        /// <returns>true if it exists in the file system</returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool Exists(this SYSTEMENTRY info)
        {
            GuardNotNull(info);

            switch(info)
            {
                case null: return false;
                case FILE finfo: return finfo.Exists;
                case DIRECTORY dinfo: return dinfo.Exists;
                default: throw new ArgumentException("Unknown type", nameof(info));
            }
        }

        /// <summary>
        /// Gets a <see cref="FILE"/> derived from a given directory.
        /// </summary>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="FILE"/> instance.</returns>
        public static FILE GetFile(this DIRECTORY baseDir, params string[] relativePath)
        {
            GuardNotNull(baseDir);
            if (relativePath == null || relativePath.Length == 0) throw new ArgumentNullException(nameof(relativePath));

            var last = relativePath[relativePath.Length-1];

            if (last == "." || last == "..") throw new ArgumentException($"{last} is invalid file name", nameof(relativePath));

            var path = _GetPath(baseDir, relativePath);
            return new FILE(path);
        }

		/// <summary>
		/// Gets a <see cref="DIRECTORY"/> derived from a given directory.
		/// </summary>
		/// <param name="baseDir">the base directory</param>
		/// <param name="relativePath">the relative path parts</param>
		/// <returns>a new <see cref="FILE"/> instance.</returns>
		public static DIRECTORY GetDirectory(this DIRECTORY baseDir, params string[] relativePath)
        {
            GuardNotNull(baseDir);

            var path = _GetPath(baseDir, relativePath);
            return new DIRECTORY(path);
        }        

        private static string _GetPath(DIRECTORY dinfo, string[] relativePath)
        {
            GuardNotNull(dinfo);

            if (relativePath == null || relativePath.Length == 0) return dinfo.FullName;

            var path = dinfo.FullName.TrimEnd(_DirectorySeparators);
            foreach (var part in relativePath)
            {
                if (string.IsNullOrWhiteSpace(part)) throw new ArgumentNullException(nameof(relativePath));
                if (part.IndexOfAny(_InvalidChars) >= 0) throw new ArgumentException($"{part} contains invalid chars", nameof(relativePath));

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
        /// Tries to get a composite extension of a file.
        /// </summary>
        /// <param name="fileName">the filename from where to get the extension.</param>
        /// <param name="dots">the number of dots used by the extension.</param>
        /// <param name="extension">the resulting extension.</param>
        /// <returns>true if an extension was found</returns>        
        public static bool TryGetCompositedExtension(this FILE finfo, int dots, out string extension)
        {
            GuardNotNull(finfo);
            
            return TryGetCompositedExtension(finfo.FullName, dots, out extension);
        }

        private static bool TryGetCompositedExtension(string fileName, int dots, out string extension)
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

        #endregion
    }
}
