// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using XFILE = Microsoft.Extensions.FileProviders.IFileInfo;
using XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;

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

        public static bool IsPhysical(this XFILE entry)
        {
            if (entry == null) return false;
            return !string.IsNullOrEmpty(entry.PhysicalPath);
        }

        public static XFILE FindEntry(this XFILE baseDir, params string[] relativePath)
        {
            if (baseDir == null) throw new ArgumentNullException(nameof(baseDir));            
            if (relativePath == null || relativePath.Length == 0) throw new ArgumentNullException(nameof(relativePath));

            if (baseDir.IsDirectory)
            {
                if (baseDir is XDIRECTORY xdir) return FindEntry(xdir, relativePath);

                if (IsPhysical(baseDir))
                {
                    throw new NotImplementedException("we can use a facade");            
                }
            }
            
            throw new ArgumentException("Not a directory.", nameof(baseDir));
        }

        public static XFILE FindEntry(this XDIRECTORY baseDir, params string[] relativePath)
        {
            if (baseDir == null) throw new ArgumentNullException(nameof(baseDir));
            if (!baseDir.Exists) throw new ArgumentException("Does not exist.", nameof(baseDir));
            if (relativePath == null || relativePath.Length == 0) throw new ArgumentNullException(nameof(relativePath));

            var comparer = StringComparison.Ordinal;

            if (baseDir is XFILE baseFile && IsPhysical(baseFile))
            {
                // check file system comparer
            }            

            var subFile = baseDir.FirstOrDefault(item => item.Name.Equals(relativePath[0], comparer));            

            if (relativePath.Length == 1) return subFile;

            if (subFile == null) return null;

            var subPath = relativePath.Skip(1).ToArray();
            return FindEntry(subFile, subPath);
        }
    }
}
