using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Threading;

#nullable disable

using __ZIPENTRY = System.IO.Compression.ZipArchiveEntry;

using __STREAMFUNC = System.Func<System.IO.FileMode, System.IO.Stream>;
using __STREAMTASK = System.Func<System.IO.FileMode, System.Threading.CancellationToken, System.Threading.Tasks.Task<System.IO.Stream>>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions    
    {
        [return: NotNull]
        public static __STREAMFUNC GetStreamFunction([NotNull] this __ZIPENTRY entry)
        {
            return _ToStreamFunc(entry.OpenRead, entry.OpenWrite);
        }

        public static System.IO.Stream OpenRead([NotNull] this __ZIPENTRY entry)
        {
            GuardReadable(entry);
            return entry.Open();
        }

        public static System.IO.Stream OpenWrite([NotNull] this __ZIPENTRY entry)
        {
            GuardWriteable(entry);
            return entry.Open();
        }

        

        [return: NotNull]
        public static __STREAMTASK GetStreamTask([NotNull] this __ZIPENTRY entry)
        {
            return _ToStreamTask(entry.OpenReadAsync, entry.OpenWriteAsync);
        }

        public static async ValueTask<System.IO.Stream> OpenReadAsync([NotNull] this __ZIPENTRY entry, CancellationToken token = default)
        {
            GuardReadable(entry);
            return await _OpenAsync(entry, token).ConfigureAwait(false);
        }

        public static async ValueTask<System.IO.Stream> OpenWriteAsync([NotNull] this __ZIPENTRY entry, CancellationToken token = default)
        {
            GuardWriteable(entry);
            return await _OpenAsync(entry, token).ConfigureAwait(false);
        }

        private static async Task<System.IO.Stream> _OpenAsync(__ZIPENTRY entry, CancellationToken token)
        {
            #if NET10_0_OR_GREATER
            return await entry.OpenAsync(token).ConfigureAwait(false);
            #else
            token.ThrowIfCancellationRequested();
            return await Task.FromResult(entry.Open()).ConfigureAwait(false);
            #endif
        }

        



        public static void CopyToFile(this __ZIPENTRY entry, System.IO.FileInfo dst)
        {            
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