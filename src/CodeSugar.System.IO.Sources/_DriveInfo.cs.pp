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
        /// <summary>
        /// Tries to get the DriveInfo from a given system file object.
        /// </summary>
        /// <remarks>
        /// it will fail if the path points to a network drive, as in: <c>\\NetworkDevice\path\</c>
        /// </remarks>
        public static bool TryGetDriveInfo(this System.IO.FileSystemInfo fsinfo, out System.IO.DriveInfo dinfo)
        {
            if (fsinfo == null) { dinfo = null; return false; }

            var root = System.IO.Path.GetPathRoot(fsinfo.FullName);

            // system drive

            if (root.Length <= 3 && Char.IsLetter(root[0]) && root[1] ==':')
            {
                root = root.ToUpperInvariant();
                dinfo = _GetInternedDriveInfo(root) ?? new System.IO.DriveInfo(root);
                return true;
            }

            // network drive

            dinfo = null;
            return false;        
        }

        public static string GetDriveOrNetworkName(this System.IO.DirectoryInfo dinfo)
        {
            if (dinfo == null) return null;
            var path = dinfo.Root.FullName.TrimEnd(_DirectorySeparators);

            if (path.EndsWith(':')) // if it's a system drive, return it
            {
                return path.ToUpperInvariant();
            }

            // network drive
            
            var idx = path.LastIndexOfAny(_DirectorySeparators);
            if (idx >= 3) path = path.Substring(0, idx);
            
            return path;
        }

        // this is a helper method that allows reusing tha same System.IO.DriveInfo instanced mapped to System Drives.
        private static System.IO.DriveInfo _GetInternedDriveInfo(string root)
        {
            int idx;

            if (_InternedFixedDrives == null) // initialize
            {
                _InternedFixedDrives = new System.IO.DriveInfo[32];

                foreach(var dinfo in System.IO.DriveInfo.GetDrives())
                {
                    if (dinfo.DriveType != System.IO.DriveType.Fixed) continue;
                    idx = _GetLetterIndex(dinfo.Name);
                    if (idx < 0 || idx >= _InternedFixedDrives.Length) continue;
                    _InternedFixedDrives[idx] = dinfo;
                }
            }

            idx = _GetLetterIndex(root);
            if (idx < 0 || idx >= _InternedFixedDrives.Length) return null;
            return _InternedFixedDrives[idx];
        }

        private static int _GetLetterIndex(string root)
        {
            if (string.IsNullOrEmpty(root)) return -1;

            var driveLetter = Char.ToUpperInvariant(root[0]);
            if (driveLetter < 'A') return -1;

            var idx = (int)(driveLetter - 'A');            
            return idx;
        }
    }
}
