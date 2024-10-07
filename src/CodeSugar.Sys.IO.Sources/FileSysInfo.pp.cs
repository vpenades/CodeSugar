// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

using FILE = System.IO.FileInfo;
using DIRECTORY = System.IO.DirectoryInfo;
using FILEORDIR = System.IO.FileSystemInfo;


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
		/// Checks whether a <see cref="FILEORDIR"/> is not null.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
        public static void GuardNotNull(this FILEORDIR info, string name = null)
        {
            if (info == null) throw new ArgumentNullException(name ?? nameof(info));
        }        

        /// <summary>
		/// Checks whether a <see cref="FILEORDIR"/> exists in the file system.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
        public static void GuardExists(this FILEORDIR info, string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);
            if (!info.Exists()) throw new ArgumentException($"'{info.FullName}' does not exist.", name ?? nameof(info));
        }  

        #else        

		/// <summary>
		/// Checks whether a <see cref="FILEORDIR"/> is not null.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardNotNull(this FILEORDIR info, [CallerArgumentExpression("info")] string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);            
        }

		/// <summary>
		/// Checks whether a <see cref="FILEORDIR"/> exists in the file system.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardExists(this FILEORDIR info, [CallerArgumentExpression("info")] string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);
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
        public static bool Exists(this FILEORDIR info)
        {
            if (info == null) return false;
            
            switch(info)
            {
                case null: return false;
                case FILE finfo: finfo.Refresh(); return finfo.Exists;
                case DIRECTORY dinfo: dinfo.Refresh(); return dinfo.Exists;
                default: throw new ArgumentException("Unknown type", nameof(info));
            }
        }

        

        /// <summary>
        /// Tries to get the Alternate Data Stream (ADS) from an existing file.
        /// </summary>
        /// <remarks>
        /// this is supported only on physical NTFS drives.
        /// </remarks>
        public static bool TryGetAlternateDataStream(this FILE baseFile, string adsName, out FILE adsFile)
        {
            GuardNotNull(baseFile);
            GuardIsValidFileName(adsName, true);
            if (baseFile.Name.Contains(':')) throw new ArgumentException($"{baseFile.Name} is already an alternate stream", nameof(baseFile));

            adsFile = null;

            if (!baseFile.Directory.TryGetDriveInfo(out var drive)) return false;
            if (drive.DriveFormat != "NTFS") return false;           
            
            var path = baseFile.FullName + ":" + adsName;
            
            adsFile = new FILE(path);
            adsFile.Refresh();
            return true;
        }

        /// <summary>
        /// Gets the parent directory of the current instance.
        /// </summary>
        [return: NotNull]
        public static DIRECTORY GetParent(this FILEORDIR fsinfo)
        {
            return GetParentOrNull(fsinfo) ?? throw new System.IO.DirectoryNotFoundException();
        }        

        /// <summary>
        /// Gets the parent directory of the current instance, or null if it has no parent.
        /// </summary>
        public static DIRECTORY GetParentOrNull(this FILEORDIR fsinfo)
        {
            if (fsinfo is FILE finfo) return finfo.Directory;
            if (fsinfo is DIRECTORY dinfo) return dinfo.Parent;
            return null;
        }

        /// <summary>
        /// Gets a <see cref="FILE"/> relative to the base directory.
        /// </summary>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="FILE"/> instance.</returns>
        [return: NotNull]
        public static FILE GetFile(this DIRECTORY baseDir, params string[] relativePath)
        {
            var finfo = _CreateFileInfo(baseDir, false, relativePath);
            System.Diagnostics.Debug.Assert(finfo != null && finfo.Exists, $"File {relativePath.Last()} does not exist.");
            return finfo ?? throw new System.IO.FileNotFoundException();
        }

        /// <summary>
        /// Gets a <see cref="FILE"/> relative to the base directory.
        /// </summary>
        /// <remarks>
        /// If the base directory does not exists, it is created.
        /// The requested file may exist in the file system or not.
        /// </remarks>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="FILE"/> instance.</returns>
        [return: NotNull]
        public static FILE UseFile(this DIRECTORY baseDir, params string[] relativePath)
        {
            return _CreateFileInfo(baseDir, true, relativePath) ?? throw new System.IO.FileNotFoundException();
        }

        /// <summary>
        /// Defines a <see cref="FILE"/> relative to the base directory.
        /// </summary>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="FILE"/> instance.</returns>
        [return: NotNull]
        public static FILE DefineFile(this DIRECTORY baseDir, params string[] relativePath)
        {
            return _CreateFileInfo(baseDir, false, relativePath);
        }

        [return: NotNull]
        private static FILE _CreateFileInfo(this DIRECTORY baseDir, bool canCreate, params string[] relativePath)
        {
            GuardNotNull(baseDir);           
            
            // handle special cases for file name
            if (relativePath == null || relativePath.Length == 0) throw new ArgumentNullException(nameof(relativePath));
            var last = System.IO.Path.GetFileName(relativePath[relativePath.Length-1]);
            System.Diagnostics.Debug.Assert(System.Environment.OSVersion.Platform != System.PlatformID.Win32NT || !last.Contains(':'), "Use TryGetAlternateDataStream() instead");            

            GuardIsValidFileName(last, true, nameof(relativePath));

            // concatenate
            var path = ConcatenatePaths(baseDir.FullName, relativePath);
            var finfo = new FILE(path);

            if (canCreate) _EnsureDirectoryExists(finfo.Directory);

            return finfo;
        }



        /// <summary>
        /// Gets an existing <see cref="DIRECTORY"/> relative to the base directory.
        /// </summary>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="DIRECTORY"/> instance.</returns>
        [return: NotNull]
        public static DIRECTORY GetDirectory(this DIRECTORY baseDir, params string[] relativePath)
        {
            return _CreateDirectoryInfo(baseDir, false, false, relativePath)
                ?? throw new System.IO.DirectoryNotFoundException();
        }

        /// <summary>
		/// Uses a <see cref="DIRECTORY"/> relative to the base directory.
		/// </summary>
		/// <param name="baseDir">the base directory</param>
		/// <param name="relativePath">the relative path parts</param>
		/// <returns>a new <see cref="DIRECTORY"/> instance.</returns>
        [return: NotNull]
        public static DIRECTORY UseDirectory(this DIRECTORY baseDir, params string[] relativePath)
        {
            return _CreateDirectoryInfo(baseDir, false, true, relativePath)
                ?? throw new System.IO.DirectoryNotFoundException();
        }

        /// <summary>
		/// Defines a <see cref="DIRECTORY"/> relative to the base directory.
		/// </summary>
		/// <param name="baseDir">the base directory</param>
		/// <param name="relativePath">the relative path parts</param>
		/// <returns>a new <see cref="DIRECTORY"/> instance.</returns>
        [return: NotNull]
        public static DIRECTORY DefineDirectory(this DIRECTORY baseDir, params string[] relativePath)
        {
            return _CreateDirectoryInfo(baseDir, false, false, relativePath)
                ?? throw new System.IO.DirectoryNotFoundException();
        }

        [return: NotNull]
        private static DIRECTORY _CreateDirectoryInfo(this DIRECTORY baseDir, bool mustExist, bool canCreate, params string[] relativePath)
        {
            GuardNotNull(baseDir);

            // concatenate
            var path = ConcatenatePaths(baseDir.FullName, relativePath);
            baseDir = new DIRECTORY(path);            

            if (canCreate) _EnsureDirectoryExists(baseDir);
            else if (mustExist)
            {
                // In release mode, let's be a bit forgiving:                
                System.Diagnostics.Debug.Assert(baseDir.Exists,$"{baseDir.FullName} does not exist. Use 'UseDirectory()' instead.");
            }

            return baseDir;
        }        

        public static void CopyTo(this FILE src, DIRECTORY dst, bool overwrite = false)
        {
            GuardExists(src);
            GuardNotNull(dst);
            var dstf = dst.GetFile(src.Name);
            src.CopyTo(dstf.FullName, overwrite);
        }

        public static void CopyTo(this FILE src, FILE dst, bool overwrite = false)
        {
            GuardExists(src);
            GuardNotNull(dst);
            src.CopyTo(dst.FullName, overwrite);        
            dst.Refresh();
        }

        public static void Rename(this FILE finfo, string newName, bool overwrite)
        {
            GuardNotNull(finfo);

            var newPath = System.IO.Path.Combine(finfo.Directory.FullName, newName);

            finfo.MoveTo(newPath, overwrite);

            finfo.Refresh();
        }

        #if NETSTANDARD
        public static void MoveTo(this FILE finfo, string newPath, bool overwrite)
        {
            var dstInfo = new FILE(newPath);

            if (overwrite && dstInfo.Exists)
            {
                dstInfo.Delete();
            }

            finfo.MoveTo(dstInfo.FullName);
        }
        #endif

        #endregion
    }
}
