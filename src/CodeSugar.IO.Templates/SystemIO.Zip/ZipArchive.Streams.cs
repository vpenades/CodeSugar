using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

#nullable disable

using __ZIPENTRY = System.IO.Compression.ZipArchiveEntry;

using __STREAMFUNC = System.Func<System.IO.FileMode, System.IO.Stream>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions    
    {
        [return: NotNull]
        public static __STREAMFUNC GetStreamFunction([NotNull] this __ZIPENTRY entry)
        {
            GuardReadable(entry);            
            return _ToStreamFunc(entry.Open, entry.Open);
        }        

        public static void CopyToFile(this __ZIPENTRY entry, System.IO.FileInfo dst)
        {
            GuardReadable(entry);
            GuardNotNull(dst);            

            using(var dstS = dst.GetWriteStreamFunction(true).OpenWrite())
            {
                using(var srcS = entry.Open())
                {
                    srcS.CopyTo(dstS);
                }
            }
        }

        public static void CopyFromFile(this __ZIPENTRY entry, System.IO.FileInfo src)
        {            
            GuardWriteable(entry);
            GuardExists(src);

            using(var srcS = src.OpenRead())
            {
                using(var dstS = entry.Open())
                {
                    srcS.CopyTo(dstS);
                }
            }
        }        
    }
}