// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#nullable disable

using _DRIVE = System.IO.DriveInfo;
using _DINFO = System.IO.DirectoryInfo;

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
        /// <summary>
        /// Tries to get the DriveInfo from a given system file object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// it will fail if the path points to a network drive, as in: <c>\\NetworkDevice\path\</c>
        /// </para>
        /// <para>
        /// This only works on Windows.
        /// </para>
        /// </remarks>
        public static bool TryGetDriveInfo(this _DINFO dinfo, out _DRIVE drive)
        {
            drive = null;

            if (dinfo == null) return false;
            var root = dinfo.Root.GetNormalizedFullName();            

            if (string.IsNullOrWhiteSpace(root)) return false;
            if (root.Length < 2) return false;            

            // network drive

            if (root[0] == System.IO.Path.DirectorySeparatorChar && root[1] == System.IO.Path.DirectorySeparatorChar) return false;                            

            // system drive

            var interned = _TryGetInternedDriveInfo(root);
            if (interned != null) { drive = interned; return true; }

            drive = new _DRIVE(root);                
            return true;
        }

        /// <summary>
        /// Gets the drive or network name of the given object.
        /// </summary>
        public static string GetDriveOrNetworkName(this _DINFO dinfo)
        {
            if (dinfo == null) return null;
            var root = dinfo.Root.GetNormalizedFullName();

            var interned = _TryGetInternedDriveInfo(root);
            if (interned != null) return interned.Name;

            return root;            
        }

        // this is a helper method that allows reusing tha same System.IO.DriveInfo instanced mapped to System Drives.
        private static _DRIVE _TryGetInternedDriveInfo(string root)
        {
            if (_InternedFixedDrives == null) // initialize
            {
                _InternedFixedDrives = new Dictionary<string, _DRIVE>(GetStringComparer(MatchCasing.PlatformDefault));

                foreach(var d in _DRIVE.GetDrives())
                {
                    if (!d.IsReady) continue;

                    if (d.DriveType != System.IO.DriveType.Fixed) continue;                                    
                    _InternedFixedDrives[d.Name] = d;
                }
            }

            return _InternedFixedDrives.TryGetValue(root, out var drive) ? drive : null;            
        }        
    }
}
