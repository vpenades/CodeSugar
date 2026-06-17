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

using __XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using __XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;
using __XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        [return: NotNull]
        public static __XDIRECTORY GetDirectoryContents(this __XINFO xfile)
        {
            return TryGetDirectoryContents(xfile, out var xdir)
                ? xdir
                : Microsoft.Extensions.FileProviders.NotFoundDirectoryContents.Singleton;
        }

        

        public static bool IsPhysical(this __XINFO entry)
        {            
            return !string.IsNullOrEmpty(entry?.PhysicalPath);
        }
    }
}
