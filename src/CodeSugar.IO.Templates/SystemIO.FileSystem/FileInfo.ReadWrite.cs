using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Threading;

#nullable disable

using __FINFO = System.IO.FileInfo;
using __READSTREAM = System.IO.Stream;
using __WRITESTREAM = System.IO.Stream;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions    
    {
        [return: NotNull]
        public static Func<__READSTREAM> GetReadStreamFunction([NotNull] this __FINFO finfo)
        {
            GuardExists(finfo);
            return finfo.OpenRead;
        }        

        [return: NotNull]
        public static Func<__WRITESTREAM> GetWriteStreamFunction([NotNull] this __FINFO finfo, bool syncFile = true)
        {
            GuardNotNull(finfo);

            __WRITESTREAM openWriteBlind()
            {
                EnsureDirectoryExists(finfo.Directory);
                return finfo.Create();
            }

            __WRITESTREAM openWriteRefresh()
            {
                EnsureDirectoryExists(finfo.Directory);
                return finfo.Create().WithDisposeObserver(finfo.Refresh);
            }

            return syncFile
                ? (Func<__WRITESTREAM>)openWriteRefresh
                : (Func<__WRITESTREAM>)openWriteBlind;
        }
    }
}