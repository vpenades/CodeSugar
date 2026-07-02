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
using __STREAM = System.IO.Stream;
using __STREAMTASK = System.Threading.Tasks.Task<System.IO.Stream>;


namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions    
    {
        [return: NotNull]
        public static Func<__STREAM> GetReadStreamFunction([NotNull] this __FINFO finfo)
        {
            GuardExists(finfo);
            return finfo.OpenRead;
        }        

        [return: NotNull]
        public static Func<__STREAM> GetWriteStreamFunction([NotNull] this __FINFO finfo, bool syncFile = true)
        {
            GuardNotNull(finfo);

            __STREAM openWriteBlind()
            {
                EnsureDirectoryExists(finfo.Directory);
                return finfo.Create();
            }

            __STREAM openWriteRefresh()
            {
                EnsureDirectoryExists(finfo.Directory);
                return finfo.Create().WithDisposeObserver(finfo.Refresh);
            }

            return syncFile
                ? (Func<__STREAM>)openWriteRefresh
                : (Func<__STREAM>)openWriteBlind;
        }
    }
}