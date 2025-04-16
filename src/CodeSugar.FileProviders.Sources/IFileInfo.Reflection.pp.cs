// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Collections;

#nullable disable

using _XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using _XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;
using _XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForFileProviders        
    {
        [return: NotNull]
        public static _XDIRECTORY GetDirectoryContents(this _XINFO xfile)
        {
            return TryGetDirectoryContents(xfile, out var xdir)
                ? xdir
                : Microsoft.Extensions.FileProviders.NotFoundDirectoryContents.Singleton;
        }

        /// <summary>
        /// If <paramref name="xfile"/> is a directory, it tries to return its <paramref name="xdir"/>
        /// </summary>
        /// <param name="xfile">the input file</param>
        /// <param name="xdir">the output directory contents</param>
        /// <returns>true on success</returns>
        public static bool TryGetDirectoryContents(this _XINFO xfile, out _XDIRECTORY xdir)
        {
            // notice that we're not handling .Exist here, because if the file does not exist,
            // the returned IDirectoryInfo returned should also have the .Exist to false

            xdir = null;
            if (xfile == null || !xfile.IsDirectory) return false;

            switch (xfile)
            {
                // IFileInfo implements IDirectoryContents on Microsoft.Extensions.FileProviders.Physical.9.0.0 and up.
                case _XDIRECTORY asDC:
                    xdir = asDC;
                    return true;

                // it may be odd for a IFileInfo to also implement IFileProvider, but it may happen in the wild.
                case _XPROVIDER asFP:
                    xdir = asFP.GetDirectoryContents(string.Empty);
                    return true;

                // easter egg: some implementations may choose to expose the IDirectoryContents as a service.
                case System.IServiceProvider asSrv:
                    xdir = asSrv.GetService(typeof(_XDIRECTORY)) as _XDIRECTORY;
                    if (xdir != null) return true;
                    break;                
            }

            // fallback for physical IFileInfo directories not implementing IDirectoryContents
            // (Microsoft.Extensions.FileProviders.Physical.8.0.0 and below)
            if (IsPhysical(xfile) && System.IO.Path.IsPathFullyQualified(xfile.PhysicalPath))
            {
                System.Diagnostics.Debug.Assert(xfile.IsDirectory);
                var dinfo = new DirectoryInfo(xfile.PhysicalPath);
                xdir = ToIFileInfo(dinfo) as _XDIRECTORY;
            }

            return xdir != null;
        }

        public static bool IsPhysical(this _XINFO entry)
        {            
            return !string.IsNullOrEmpty(entry?.PhysicalPath);
        }
    }
}
