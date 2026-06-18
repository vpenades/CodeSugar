// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

#nullable disable

using __FINFO = System.IO.FileInfo;
using __DINFO = System.IO.DirectoryInfo;
using __SINFO = System.IO.FileSystemInfo;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        #region diagnostics        

        /// <summary>
        /// Checks whether a <see cref="__SINFO"/> is not null.
        /// </summary>        
        /// <exception cref="ArgumentNullException"></exception>
        public static void GuardNotNull(this __SINFO info, [CallerArgumentExpression(nameof(info))] string name = null)
        {
            if (info == null) throw new ArgumentNullException(name ?? nameof(info));            
        }

		/// <summary>
		/// Checks whether a <see cref="__SINFO"/> exists in the file system.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardExists(this __SINFO info, [CallerArgumentExpression(nameof(info))] string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);
            if (!info.RefreshedExists()) throw new ArgumentException($"'{info.FullName}' does not exist.", name ?? nameof(info));
        }        

        #endregion

        #region tree navigation

        /// <summary>
        /// Gets a value indicating whether <paramref name="info"/> exists in the file system.
        /// </summary>
        /// <param name="info">A <see cref="__FINFO"/> or a <see cref="__DINFO"/> object.</param>
        /// <returns>true if it exists in the file system</returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool RefreshedExists(this __SINFO info)
        {
            if (info == null) return false;
            
            switch(info)
            {
                case null: return false;
                case __FINFO finfo: return finfo.RefreshedExists();
                case __DINFO dinfo: return dinfo.RefreshedExists();
                default: throw new ArgumentException("Unknown type", nameof(info));
            }
        }

        public static bool PhysicallyExists(this __SINFO info)
        {
            if (info == null) return false;

            switch (info)
            {
                case null: return false;
                case __FINFO finfo: return finfo.PhysicallyExists();
                case __DINFO dinfo: return dinfo.PhysicallyExists();
                default: throw new ArgumentException("Unknown type", nameof(info));
            }
        }



        /// <summary>
        /// Tries to get the Alternate Data Stream (ADS) from an existing file.
        /// </summary>
        /// <remarks>
        /// this is supported only on physical NTFS drives.
        /// </remarks>
        public static bool TryGetAlternateDataStream([DisallowNull] this __FINFO baseFile, string adsName, out __FINFO adsFile)
        {
            GuardNotNull(baseFile);
            GuardIsValidFileName(adsName, true);
            if (baseFile.Name.Contains(':')) throw new ArgumentException($"{baseFile.Name} is already an alternate stream", nameof(baseFile));

            adsFile = null;

            if (!baseFile.Directory.TryGetDriveInfo(out var drive)) return false;
            if (drive.DriveFormat != "NTFS") return false;           
            
            var path = baseFile.FullName + ":" + adsName;
            
            adsFile = new __FINFO(path);
            return true;
        }

        /// <summary>
        /// Gets the parent directory of the current instance.
        /// </summary>
        [return: NotNull]
        public static __DINFO GetParent([DisallowNull] this __SINFO fsinfo)
        {
            return GetParentOrNull(fsinfo) ?? throw new System.IO.DirectoryNotFoundException();
        }        

        /// <summary>
        /// Gets the parent directory of the current instance, or null if it has no parent.
        /// </summary>
        public static __DINFO GetParentOrNull([DisallowNull] this __SINFO fsinfo)
        {
            if (fsinfo is __FINFO finfo) return finfo.Directory;
            if (fsinfo is __DINFO dinfo) return dinfo.Parent;
            return null;
        }        

        /// <summary>
        /// Gets a <see cref="__FINFO"/> relative to the base directory.
        /// </summary>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="__FINFO"/> instance.</returns>
        [return: NotNull]
        public static __FINFO GetFileInfo([DisallowNull] this __DINFO baseDir, params string[] relativePath)
        {
            var finfo = _CreateFileInfo(baseDir, false, relativePath);
            System.Diagnostics.Debug.Assert(finfo != null && finfo.PhysicallyExists(), $"File {relativePath.Last()} does not exist.");
            return finfo ?? throw new System.IO.FileNotFoundException();
        }        

        /// <summary>
        /// Gets a <see cref="__FINFO"/> relative to the base directory.
        /// </summary>
        /// <remarks>
        /// If the base directory does not exists, it is created.
        /// The requested file may exist in the file system or not.
        /// </remarks>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="__FINFO"/> instance.</returns>
        [return: NotNull]
        public static __FINFO UseFileInfo([DisallowNull] this __DINFO baseDir, params string[] relativePath)
        {
            return _CreateFileInfo(baseDir, true, relativePath) ?? throw new System.IO.FileNotFoundException();
        }

        

        public static __FINFO DefineRandomFileInfo([DisallowNull] this __DINFO baseDir, int maxRetries)
        {
            while(true)
            {
                var f = DefineRandomFileInfo(baseDir);
                if (!CachedExists(f)) return f;
                if (maxRetries <= 0) break;
                --maxRetries;
            }

            return null;            
        }

        public static __FINFO DefineRandomFileInfo([DisallowNull] this __DINFO baseDir)
        {
            return baseDir.DefineFileInfo(System.IO.Path.GetRandomFileName());
        }

        [return: NotNull]
        public static __FINFO DefineFileInfo(this Environment.SpecialFolder folder, params string[] relativePath)
        {
            return folder.GetSpecialFolder().DefineFileInfo(relativePath);
        }

        /// <summary>
        /// Defines a <see cref="__FINFO"/> relative to the base directory.
        /// </summary>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="__FINFO"/> instance.</returns>
        [return: NotNull]
        public static __FINFO DefineFileInfo([DisallowNull] this __DINFO baseDir, params string[] relativePath)
        {
            return _CreateFileInfo(baseDir, false, relativePath);
        }

        [return: NotNull]
        private static __FINFO _CreateFileInfo([DisallowNull] this __DINFO baseDir, bool canCreate, params string[] relativePath)
        {
            GuardNotNull(baseDir);           
            
            // handle special cases for file name
            if (relativePath == null || relativePath.Length == 0) throw new ArgumentNullException(nameof(relativePath));
            var last = System.IO.Path.GetFileName(relativePath[relativePath.Length-1]);
            System.Diagnostics.Debug.Assert(System.Environment.OSVersion.Platform != System.PlatformID.Win32NT || !last.Contains(':'), "Use TryGetAlternateDataStream() instead");            

            GuardIsValidFileName(last, true, nameof(relativePath));

            // concatenate
            var path = ConcatenatePaths(baseDir.FullName, relativePath);
            var finfo = new __FINFO(path);

            if (canCreate) _EnsureDirectoryExists(finfo.Directory);

            return finfo;
        }

        /// <summary>
        /// Gets an existing <see cref="__DINFO"/> relative to the base directory.
        /// </summary>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="__DINFO"/> instance.</returns>
        [return: NotNull]
        public static __DINFO GetDirectoryInfo([DisallowNull] this __DINFO baseDir, params string[] relativePath)
        {
            return _CreateDirectoryInfo(baseDir, false, false, relativePath)
                ?? throw new System.IO.DirectoryNotFoundException();
        }

        [return: NotNull]
        public static __DINFO UseDirectoryInfo([DisallowNull] this Environment.SpecialFolder folder, params string[] relativePath)
        {
            return folder.GetSpecialFolder().UseDirectoryInfo(relativePath);
        }

        /// <summary>
		/// Uses a <see cref="__DINFO"/> relative to the base directory.
		/// </summary>
		/// <param name="baseDir">the base directory</param>
		/// <param name="relativePath">the relative path parts</param>
		/// <returns>a new <see cref="__DINFO"/> instance.</returns>
        [return: NotNull]
        public static __DINFO UseDirectoryInfo([DisallowNull] this __DINFO baseDir, params string[] relativePath)
        {
            return _CreateDirectoryInfo(baseDir, false, true, relativePath)
                ?? throw new System.IO.DirectoryNotFoundException();
        }

        [return: NotNull]
        public static __DINFO DefineDirectoryInfo([DisallowNull] this Environment.SpecialFolder folder, params string[] relativePath)
        {
            return folder.GetSpecialFolder().DefineDirectoryInfo(relativePath);
        }

        /// <summary>
		/// Defines a <see cref="__DINFO"/> relative to the base directory.
		/// </summary>
		/// <param name="baseDir">the base directory</param>
		/// <param name="relativePath">the relative path parts</param>
		/// <returns>a new <see cref="__DINFO"/> instance.</returns>
        [return: NotNull]
        public static __DINFO DefineDirectoryInfo([DisallowNull] this __DINFO baseDir, params string[] relativePath)
        {
            return _CreateDirectoryInfo(baseDir, false, false, relativePath)
                ?? throw new System.IO.DirectoryNotFoundException();
        }

        [return: NotNull]
        private static __DINFO _CreateDirectoryInfo([DisallowNull] this __DINFO baseDir, bool mustExist, bool canCreate, params string[] relativePath)
        {
            GuardNotNull(baseDir);

            // concatenate
            var path = ConcatenatePaths(baseDir.FullName, relativePath);
            baseDir = new __DINFO(path);            

            if (canCreate) _EnsureDirectoryExists(baseDir);
            else if (mustExist)
            {
                // In release mode, let's be a bit forgiving:                
                System.Diagnostics.Debug.Assert(baseDir.PhysicallyExists(),$"{baseDir.FullName} does not exist. Use 'UseDirectory()' instead.");
            }

            return baseDir;
        }        

        public static void CopyTo([DisallowNull] this __FINFO src, [DisallowNull] __DINFO dst, bool overwrite = false)
        {
            GuardExists(src);
            GuardNotNull(dst);
            var dstf = dst.DefineFileInfo(src.Name);
            src.CopyTo(dstf.FullName, overwrite);
        }

        public static void CopyTo([DisallowNull] this __FINFO src, [DisallowNull] __FINFO dst, bool overwrite = false)
        {
            GuardExists(src);
            GuardNotNull(dst);
            src.CopyTo(dst.FullName, overwrite);        
            dst.Refresh();
        }

        public static void Rename([DisallowNull] this __FINFO finfo, string newName, bool overwrite)
        {
            GuardNotNull(finfo);

            var newPath = System.IO.Path.Combine(finfo.Directory.FullName, newName);

            finfo.MoveTo(newPath, overwrite);
            System.Diagnostics.Debug.Assert(finfo.CachedExists() == System.IO.File.Exists(newPath));
        }

        #if NETSTANDARD
        public static void MoveTo([DisallowNull] this __FINFO finfo, [DisallowNull] string newPath, bool overwrite)
        {
            var dstInfo = new __FINFO(newPath);

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
