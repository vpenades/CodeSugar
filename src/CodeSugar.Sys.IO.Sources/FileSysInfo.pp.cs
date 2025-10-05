// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

using _FINFO = System.IO.FileInfo;
using _DINFO = System.IO.DirectoryInfo;
using _SINFO = System.IO.FileSystemInfo;


#nullable disable


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
        #region diagnostics

        #if !NET

        /// <summary>
		/// Checks whether a <see cref="_SINFO"/> is not null.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
        public static void GuardNotNull(this _SINFO info, string name = null)
        {
            if (info == null) throw new ArgumentNullException(name ?? nameof(info));
        }        

        /// <summary>
		/// Checks whether a <see cref="_SINFO"/> exists in the file system.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
        public static void GuardExists(this _SINFO info, string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);
            if (!info.RefreshedExists()) throw new ArgumentException($"'{info.FullName}' does not exist.", name ?? nameof(info));
        }  

        #else

        /// <summary>
        /// Checks whether a <see cref="_SINFO"/> is not null.
        /// </summary>        
        /// <exception cref="ArgumentNullException"></exception>
        public static void GuardNotNull(this _SINFO info, [CallerArgumentExpression("info")] string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);            
        }

		/// <summary>
		/// Checks whether a <see cref="_SINFO"/> exists in the file system.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardExists(this _SINFO info, [CallerArgumentExpression("info")] string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);
            if (!info.RefreshedExists()) throw new ArgumentException($"'{info.FullName}' does not exist.", name);
        }

        #endif

        #endregion

        #region nav tree

        /// <summary>
        /// Gets a value indicating whether <paramref name="info"/> exists in the file system.
        /// </summary>
        /// <param name="info">A <see cref="_FINFO"/> or a <see cref="_DINFO"/> object.</param>
        /// <returns>true if it exists in the file system</returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool RefreshedExists(this _SINFO info)
        {
            if (info == null) return false;
            
            switch(info)
            {
                case null: return false;
                case _FINFO finfo: return finfo.RefreshedExists();
                case _DINFO dinfo: return dinfo.RefreshedExists();
                default: throw new ArgumentException("Unknown type", nameof(info));
            }
        }

        public static bool PhysicallyExists(this _SINFO info)
        {
            if (info == null) return false;

            switch (info)
            {
                case null: return false;
                case _FINFO finfo: return finfo.PhysicallyExists();
                case _DINFO dinfo: return dinfo.PhysicallyExists();
                default: throw new ArgumentException("Unknown type", nameof(info));
            }
        }



        /// <summary>
        /// Tries to get the Alternate Data Stream (ADS) from an existing file.
        /// </summary>
        /// <remarks>
        /// this is supported only on physical NTFS drives.
        /// </remarks>
        public static bool TryGetAlternateDataStream(this _FINFO baseFile, string adsName, out _FINFO adsFile)
        {
            GuardNotNull(baseFile);
            GuardIsValidFileName(adsName, true);
            if (baseFile.Name.Contains(':')) throw new ArgumentException($"{baseFile.Name} is already an alternate stream", nameof(baseFile));

            adsFile = null;

            if (!baseFile.Directory.TryGetDriveInfo(out var drive)) return false;
            if (drive.DriveFormat != "NTFS") return false;           
            
            var path = baseFile.FullName + ":" + adsName;
            
            adsFile = new _FINFO(path);
            return true;
        }

        /// <summary>
        /// Gets the parent directory of the current instance.
        /// </summary>
        [return: NotNull]
        public static _DINFO GetParent(this _SINFO fsinfo)
        {
            return GetParentOrNull(fsinfo) ?? throw new System.IO.DirectoryNotFoundException();
        }        

        /// <summary>
        /// Gets the parent directory of the current instance, or null if it has no parent.
        /// </summary>
        public static _DINFO GetParentOrNull(this _SINFO fsinfo)
        {
            if (fsinfo is _FINFO finfo) return finfo.Directory;
            if (fsinfo is _DINFO dinfo) return dinfo.Parent;
            return null;
        }

        [Obsolete("Use GetFileInfo", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static _FINFO GetFile(this _DINFO baseDir, params string[] relativePath)
        {
            return GetFileInfo(baseDir, relativePath);
        }

        /// <summary>
        /// Gets a <see cref="_FINFO"/> relative to the base directory.
        /// </summary>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="_FINFO"/> instance.</returns>
        [return: NotNull]
        public static _FINFO GetFileInfo(this _DINFO baseDir, params string[] relativePath)
        {
            var finfo = _CreateFileInfo(baseDir, false, relativePath);
            System.Diagnostics.Debug.Assert(finfo != null && finfo.PhysicallyExists(), $"File {relativePath.Last()} does not exist.");
            return finfo ?? throw new System.IO.FileNotFoundException();
        }

        [Obsolete("UseFileInfo", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static _FINFO UseFile(this _DINFO baseDir, params string[] relativePath)
        {
            return UseFileInfo(baseDir, relativePath);
        }

        /// <summary>
        /// Gets a <see cref="_FINFO"/> relative to the base directory.
        /// </summary>
        /// <remarks>
        /// If the base directory does not exists, it is created.
        /// The requested file may exist in the file system or not.
        /// </remarks>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="_FINFO"/> instance.</returns>
        [return: NotNull]
        public static _FINFO UseFileInfo(this _DINFO baseDir, params string[] relativePath)
        {
            return _CreateFileInfo(baseDir, true, relativePath) ?? throw new System.IO.FileNotFoundException();
        }

        [Obsolete("Use DefineFileInfo", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static _FINFO DefineFile(this _DINFO baseDir, params string[] relativePath)
        {
            return DefineFileInfo(baseDir, relativePath);
        }

        /// <summary>
        /// Defines a <see cref="_FINFO"/> relative to the base directory.
        /// </summary>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="_FINFO"/> instance.</returns>
        [return: NotNull]
        public static _FINFO DefineFileInfo(this _DINFO baseDir, params string[] relativePath)
        {
            return _CreateFileInfo(baseDir, false, relativePath);
        }

        [return: NotNull]
        private static _FINFO _CreateFileInfo(this _DINFO baseDir, bool canCreate, params string[] relativePath)
        {
            GuardNotNull(baseDir);           
            
            // handle special cases for file name
            if (relativePath == null || relativePath.Length == 0) throw new ArgumentNullException(nameof(relativePath));
            var last = System.IO.Path.GetFileName(relativePath[relativePath.Length-1]);
            System.Diagnostics.Debug.Assert(System.Environment.OSVersion.Platform != System.PlatformID.Win32NT || !last.Contains(':'), "Use TryGetAlternateDataStream() instead");            

            GuardIsValidFileName(last, true, nameof(relativePath));

            // concatenate
            var path = ConcatenatePaths(baseDir.FullName, relativePath);
            var finfo = new _FINFO(path);

            if (canCreate) _EnsureDirectoryExists(finfo.Directory);

            return finfo;
        }

        [Obsolete("Use GetDirectoryInfo", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [return: NotNull]
        public static _DINFO GetDirectory(this _DINFO baseDir, params string[] relativePath)
        {
            return GetDirectoryInfo(baseDir, relativePath);
        }


        /// <summary>
        /// Gets an existing <see cref="_DINFO"/> relative to the base directory.
        /// </summary>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="_DINFO"/> instance.</returns>
        [return: NotNull]
        public static _DINFO GetDirectoryInfo(this _DINFO baseDir, params string[] relativePath)
        {
            return _CreateDirectoryInfo(baseDir, false, false, relativePath)
                ?? throw new System.IO.DirectoryNotFoundException();
        }

        [Obsolete("Use UseDirectoryInfo", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [return: NotNull]
        public static _DINFO UseDirectory(this _DINFO baseDir, params string[] relativePath)
        {
            return UseDirectoryInfo(baseDir, relativePath);
        }

        /// <summary>
		/// Uses a <see cref="_DINFO"/> relative to the base directory.
		/// </summary>
		/// <param name="baseDir">the base directory</param>
		/// <param name="relativePath">the relative path parts</param>
		/// <returns>a new <see cref="_DINFO"/> instance.</returns>
        [return: NotNull]
        public static _DINFO UseDirectoryInfo(this _DINFO baseDir, params string[] relativePath)
        {
            return _CreateDirectoryInfo(baseDir, false, true, relativePath)
                ?? throw new System.IO.DirectoryNotFoundException();
        }

        [Obsolete("Use DefineDirectoryInfo", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [return: NotNull]
        public static _DINFO DefineDirectory(this _DINFO baseDir, params string[] relativePath)
        {
            return DefineDirectoryInfo(baseDir, relativePath);
        }

        /// <summary>
		/// Defines a <see cref="_DINFO"/> relative to the base directory.
		/// </summary>
		/// <param name="baseDir">the base directory</param>
		/// <param name="relativePath">the relative path parts</param>
		/// <returns>a new <see cref="_DINFO"/> instance.</returns>
        [return: NotNull]
        public static _DINFO DefineDirectoryInfo(this _DINFO baseDir, params string[] relativePath)
        {
            return _CreateDirectoryInfo(baseDir, false, false, relativePath)
                ?? throw new System.IO.DirectoryNotFoundException();
        }

        [return: NotNull]
        private static _DINFO _CreateDirectoryInfo(this _DINFO baseDir, bool mustExist, bool canCreate, params string[] relativePath)
        {
            GuardNotNull(baseDir);

            // concatenate
            var path = ConcatenatePaths(baseDir.FullName, relativePath);
            baseDir = new _DINFO(path);            

            if (canCreate) _EnsureDirectoryExists(baseDir);
            else if (mustExist)
            {
                // In release mode, let's be a bit forgiving:                
                System.Diagnostics.Debug.Assert(baseDir.PhysicallyExists(),$"{baseDir.FullName} does not exist. Use 'UseDirectory()' instead.");
            }

            return baseDir;
        }        

        public static void CopyTo(this _FINFO src, _DINFO dst, bool overwrite = false)
        {
            GuardExists(src);
            GuardNotNull(dst);
            var dstf = dst.DefineFileInfo(src.Name);
            src.CopyTo(dstf.FullName, overwrite);
        }

        public static void CopyTo(this _FINFO src, _FINFO dst, bool overwrite = false)
        {
            GuardExists(src);
            GuardNotNull(dst);
            src.CopyTo(dst.FullName, overwrite);        
            dst.Refresh();
        }

        public static void Rename(this _FINFO finfo, string newName, bool overwrite)
        {
            GuardNotNull(finfo);

            var newPath = System.IO.Path.Combine(finfo.Directory.FullName, newName);

            finfo.MoveTo(newPath, overwrite);
            System.Diagnostics.Debug.Assert(finfo.CachedExists() == System.IO.File.Exists(newPath));
        }

        #if NETSTANDARD
        public static void MoveTo(this _FINFO finfo, string newPath, bool overwrite)
        {
            var dstInfo = new _FINFO(newPath);

            if (overwrite && dstInfo.RefreshedExists())
            {
                dstInfo.Delete();
                System.Diagnostics.Debug.Assert(dstInfo.CachedExists() == false);
            }

            finfo.MoveTo(dstInfo.FullName);
            System.Diagnostics.Debug.Assert(finfo.CachedExists() == dstInfo.PhysicallyExists());
        }
        #endif

        #endregion
    }
}
