using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

#nullable disable

using __FINFO = System.IO.FileInfo;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions    
    {
        [return: NotNull]
        public static Func<System.IO.Stream> GetReadStreamFunction([NotNull] this __FINFO finfo)
        {
            GuardExists(finfo);
            return finfo.OpenRead;
        }

        [return: NotNull]
        public static Func<System.IO.Stream> GetWriteStreamFunction([NotNull] this __FINFO finfo, bool syncFile = true)
        {
            GuardNotNull(finfo);

            EnsureDirectoryExists(finfo.Directory);

            if (!syncFile) return finfo.Create;

            System.IO.Stream openWrite()
            {                
                return finfo.Create().WithDisposeObserver(() => finfo.Refresh());
            }

            return openWrite;
        }
    }
}