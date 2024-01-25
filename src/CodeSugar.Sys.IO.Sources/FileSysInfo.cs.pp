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
    static partial class CodeSugarForSystemIO    
    {
		#region diagnostics

        #if !NET

        /// <summary>
		/// Checks whether a <see cref="SYSTEMENTRY"/> is not null.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardIsValidFileName(string fileName, bool checkForInvalidNames, string name = null)
        {
            name ??= nameof(fileName);
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(name);
            if (fileName.IndexOfAny(_InvalidNameChars) >= 0) throw new ArgumentException($"{fileName} contains invalid chars", name);
            if (checkForInvalidNames)
            {
                if (fileName == "." || fileName == "..") throw new ArgumentException($"{fileName} is an invalid file name", name);
            }
        }

        /// <summary>
		/// Checks whether a <see cref="SYSTEMENTRY"/> is not null.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
        public static void GuardNotNull(this SYSTEMENTRY info, string name = null)
        {
            if (info == null) throw new ArgumentNullException(name ?? nameof(info));
        }        

        /// <summary>
		/// Checks whether a <see cref="SYSTEMENTRY"/> exists in the file system.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
        public static void GuardExists(this SYSTEMENTRY info, string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);
            if (!info.Exists()) throw new ArgumentException($"'{info.FullName}' does not exist.", name ?? nameof(info));
        }  

        #else

        /// <summary>
		/// Checks whether a <see cref="SYSTEMENTRY"/> is not null.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardIsValidFileName(string fileName, bool checkForInvalidNames, [CallerArgumentExpression("fileName")] string name = null)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (fileName.IndexOfAny(_InvalidNameChars) >= 0) throw new ArgumentException($"{fileName} contains invalid chars", nameof(fileName));
            if (checkForInvalidNames)
            {
                if (fileName == "." || fileName == "..") throw new ArgumentException($"{fileName} is an invalid file name", name);
            }
        }

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
        public static bool Exists(this SYSTEMENTRY info)
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
        /// Ensures that <paramref name="directory"/> exists in the file system.
        /// </summary>
        /// <returns>true if it needd to create the directory</returns>
        public static bool EnsureDirectoryExists(this DIRECTORY directory)
        {
            GuardNotNull(directory);

            directory.Refresh();
            return _EnsureDirectoryExists(directory);
        }

        private static bool _EnsureDirectoryExists(this DIRECTORY directory)
        {
            GuardNotNull(directory);
            
            if (directory.Exists) return false;
            directory.Create();
            return true;
        }

        public static DIRECTORY GetSpecialFolder(this System.Environment.SpecialFolder folder)
        {
            var path = System.Environment.GetFolderPath(folder);
            return new DIRECTORY(path);
        }

        public static DIRECTORY GetSpecialFolder(this System.Environment.SpecialFolder folder, System.Environment.SpecialFolderOption options)
        {
            var path = System.Environment.GetFolderPath(folder, options);
            return new DIRECTORY(path);
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
        public static System.IO.DirectoryInfo GetParent(this System.IO.FileSystemInfo fsinfo)
        {
            return GetParentOrNull(fsinfo) ?? throw new NotImplementedException();
        }        

        /// <summary>
        /// Gets the parent directory of the current instance, or null if it has no parent.
        /// </summary>
        public static System.IO.DirectoryInfo GetParentOrNull(this System.IO.FileSystemInfo fsinfo)
        {
            if (fsinfo is System.IO.FileInfo finfo) return finfo.Directory;
            if (fsinfo is System.IO.DirectoryInfo dinfo) return dinfo.Parent;
            return null;
        }     

        /// <summary>
        /// Gets a <see cref="FILE"/> relative to the base directory.
        /// </summary>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="FILE"/> instance.</returns>
        public static FILE GetFile(this DIRECTORY baseDir, params string[] relativePath)
        {
            return _CreateFileInfo(baseDir, false, relativePath);
        }        

        /// <summary>
        /// Gets a <see cref="FILE"/> relative to the base directory.
        /// </summary>
        /// <remarks>
        /// the requested file my axist in the file system or not.
        /// </remarks>
        /// <param name="baseDir">the base directory</param>
        /// <param name="relativePath">the relative path parts</param>
        /// <returns>a new <see cref="FILE"/> instance.</returns>
        public static FILE UseFile(this DIRECTORY baseDir, params string[] relativePath)
        {
            return _CreateFileInfo(baseDir, true, relativePath);
        }
        
        private static FILE _CreateFileInfo(this DIRECTORY baseDir, bool canCreate, params string[] relativePath)
        {
            GuardNotNull(baseDir);
            if (relativePath == null || relativePath.Length == 0) throw new ArgumentNullException(nameof(relativePath));
            
            // handle special cases for file name

            var last = relativePath[relativePath.Length-1];
            System.Diagnostics.Debug.Assert(Environment.OSVersion.Platform != PlatformID.Win32NT || !last.Contains(':'), "Use TryGetAlternateDataStream() instead");
            GuardIsValidFileName(last, true, nameof(relativePath));

            var path = ConcatenatePaths(baseDir.FullName, relativePath);
            var finfo = new FILE(path);

            if (canCreate) _EnsureDirectoryExists(finfo.Directory);
            else
            {
                // In release mode, let's be a bit forgiving:
                System.Diagnostics.Debug.Assert(finfo.Exists,$"{finfo.FullName} does not exist. Use 'UseFile()' instead.");
            }            

            return finfo;
        }

        /// <summary>
		/// Gets a <see cref="DIRECTORY"/> relative to the base directory.
		/// </summary>
		/// <param name="baseDir">the base directory</param>
		/// <param name="relativePath">the relative path parts</param>
		/// <returns>a new <see cref="DIRECTORY"/> instance.</returns>
		public static DIRECTORY GetDirectory(this DIRECTORY baseDir, params string[] relativePath)
        {
            return _CreateDirectoryInfo(baseDir, false, false, relativePath);
        }

        /// <summary>
		/// Uses an existing <see cref="DIRECTORY"/> relative to the base directory.
		/// </summary>
		/// <param name="baseDir">the base directory</param>
		/// <param name="relativePath">the relative path parts</param>
		/// <returns>a new <see cref="DIRECTORY"/> instance.</returns>
		public static DIRECTORY UseDirectory(this DIRECTORY baseDir, params string[] relativePath)
        {
            return _CreateDirectoryInfo(baseDir, false, true, relativePath);
        }        

        private static DIRECTORY _CreateDirectoryInfo(this DIRECTORY baseDir, bool mustExist, bool canCreate, params string[] relativePath)
        {
            GuardNotNull(baseDir);

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
