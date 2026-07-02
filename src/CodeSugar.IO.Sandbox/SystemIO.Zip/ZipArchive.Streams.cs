using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

#nullable disable

using __ZIPENTRY = System.IO.Compression.ZipArchiveEntry;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions    
    {
        [return: NotNull]
        public static Func<System.IO.Stream> GetReadStreamFunction([NotNull] this __ZIPENTRY entry)
        {
            GuardReadable(entry);
            return entry.Open;
        }

        [return: NotNull]
        public static Func<System.IO.Stream> GetWriteStreamFunction([NotNull] this __ZIPENTRY entry)
        {
            GuardWriteable(entry);
            return entry.Open;
        }

        public static void CopyToFile(this __ZIPENTRY entry, System.IO.FileInfo dst)
        {
            GuardReadable(entry);
            GuardNotNull(dst);            

            using(var dstS = dst.GetWriteStreamFunction().Invoke())
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