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

using __STREAMFUNC = System.Func<System.IO.FileMode, System.IO.Stream>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions    
    {
        [return: NotNull]
        public static __STREAMFUNC GetStreamFunction([NotNull] this __FINFO finfo)
        {
            GuardNotNull(finfo);            

            System.IO.Stream open(System.IO.FileMode mode)
            {
                switch(mode)
                {
                    case System.IO.FileMode.Open: GuardExists(finfo); return finfo.OpenRead();
                    case System.IO.FileMode.Append: GuardExists(finfo); return finfo.Open(System.IO.FileMode.Append);
                    case System.IO.FileMode.Create: return finfo.Create();                    
                    default: return finfo.Open(mode);                        
                }
            }

            return open;            
        }

        [return: NotNull]
        public static __STREAMFUNC GetWriteStreamFunction([NotNull] this __FINFO finfo, bool syncFile)
            {
            GuardNotNull(finfo);

            System.IO.Stream openWriteBlind()
            {
                EnsureDirectoryExists(finfo.Directory);
                return finfo.Create();
            }

            System.IO.Stream openWriteRefresh()
            {
                EnsureDirectoryExists(finfo.Directory);
                return finfo.Create().WithDisposeObserver(finfo.Refresh);
            }            

            var f = syncFile
                ? (Func<System.IO.Stream>)openWriteRefresh
                : (Func<System.IO.Stream>)openWriteBlind;

            return _ToStreamFunc(null, f);
        }        
    }
}