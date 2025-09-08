// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarForSystemIO
    {
        public static bool TryGetFileInfo(this Uri uri, out FileInfo fileInfo)
        {
            fileInfo = null;

            // Only file:// schemes are supported
            if (uri == null || !uri.IsAbsoluteUri || uri.Scheme != Uri.UriSchemeFile) return false;            

            // Create FileInfo object
            fileInfo = new FileInfo(uri.LocalPath);            

            return true;
        }

        public static bool TryGetDirectoryInfo(this Uri uri, out DirectoryInfo dirInfo)
        {
            dirInfo = null;

            // Only file:// schemes are supported
            if (uri == null || !uri.IsAbsoluteUri || uri.Scheme != Uri.UriSchemeFile) return false;            

            // Create FileInfo object
            dirInfo = new DirectoryInfo(uri.LocalPath);

            return true;
        }
    }
}