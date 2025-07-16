// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

#nullable disable

using _FINFO = System.IO.FileInfo;
using _DINFO = System.IO.DirectoryInfo;
using _SPECIALFOLDER = System.Environment.SpecialFolder;
using _PATHCASING = System.IO.MatchCasing;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForSystemIO
    {
        public static void ThrowNotFound(this _DINFO dinfo, Exception innerException = null)
        {
            GuardNotNull(dinfo);
            if (innerException == null) throw new System.IO.DirectoryNotFoundException(dinfo.FullName);
            else throw new System.IO.DirectoryNotFoundException(dinfo.FullName, innerException);
        }

        public static bool IsTempPath(this _PATHCASING casing, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            
            var temp = System.IO.Path.GetTempPath().TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            if (path.Length < temp.Length) return false;

            path = path.Substring(0, temp.Length);            

            return ArePathsEqual(casing, temp, path);
        }

        /// <summary>
        /// Checks whether the directory is, or is inside the current Temp path.
        /// </summary>
        /// <param name="directory">The direcyory to check</param>
        /// <returns>true if it's a temporary path</returns>
        public static bool IsTemp(this _DINFO directory)
        {
            if (directory == null) return false;

            return IsTempPath(_PATHCASING.PlatformDefault, directory.FullName);
        }

        /// <summary>
        /// Ensures that <paramref name="directory"/> exists in the file system.
        /// </summary>
        /// <returns>the same directory passed as argument, so it can be used fluently.</returns>
        public static _DINFO EnsureCreated(this _DINFO directory)
        {
            GuardNotNull(directory);

            directory.Refresh();
            _EnsureDirectoryExists(directory);
            return directory;
        }

        /// <summary>
        /// Checks if the directory exists, refreshing the state beforehand
        /// </summary>
        /// <param name="directory">the directory to check</param>
        /// <returns>true if it exists</returns>
        public static bool RefreshedExists(this _DINFO directory)
        {
            // https://github.com/dotnet/corefx/pull/40677
            // https://github.com/dotnet/runtime/issues/31425
            // https://github.com/dotnet/runtime/issues/117709

            if (directory == null) return false;            
            directory.Refresh();
            return CachedExists(directory);            
        }

        /// <summary>
        /// Checks if the directory exists, using the internal cache
        /// </summary>
        /// <param name="directory">the directory to check</param>
        /// <returns>true if it exists</returns>
        public static bool CachedExists(this _DINFO directory)
        {
            // https://github.com/dotnet/corefx/pull/40677
            // https://github.com/dotnet/runtime/issues/31425
            // https://github.com/dotnet/runtime/issues/117709

            if (directory == null) return false;
            #pragma warning disable            
            return directory.Exists;
            #pragma warning restore
        }        

        /// <summary>
        /// Checks if the directory exists, bypassing internal cache
        /// </summary>
        /// <param name="directory">the directory to check</param>
        /// <returns>true if it exists</returns>
        public static bool PhysicallyExists(this _DINFO directory)
        {
            if (directory == null) return false;
            return System.IO.Directory.Exists(directory.FullName);
        }

        /// <summary>
        /// Ensures that <paramref name="directory"/> exists in the file system.
        /// </summary>
        /// <returns>true if it needd to create the directory</returns>
        public static bool EnsureDirectoryExists(this _DINFO directory)
        {
            GuardNotNull(directory);

            directory.Refresh();
            return _EnsureDirectoryExists(directory);
        }

        private static bool _EnsureDirectoryExists(this _DINFO directory)
        {
            GuardNotNull(directory);

            if (directory.RefreshedExists()) return false;

            // prevent creation of directories with leading/trailing whitespaces
            // (which can actually exist, but mess up windows explorer navigation because explorer removes the whitespaces)
            GuardIsValidFileName(directory.Name, false, nameof(directory));

            directory.Create();

            System.Diagnostics.Debug.Assert(directory.CachedExists());

            return true;
        }

        public static _DINFO GetSpecialFolder(this _SPECIALFOLDER folder)
        {
            var path = System.Environment.GetFolderPath(folder);
            return new _DINFO(path);
        }

        public static _DINFO GetSpecialFolder(this _SPECIALFOLDER folder, System.Environment.SpecialFolderOption options)
        {
            var path = System.Environment.GetFolderPath(folder, options);
            return new _DINFO(path);
        }

    }
}
