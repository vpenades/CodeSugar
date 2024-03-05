// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

#nullable disable

using FILE = System.IO.FileInfo;
using DIRECTORY = System.IO.DirectoryInfo;
using SYSTEMENTRY = System.IO.FileSystemInfo;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForSystemIO
    {
        
        public static async Task<IReadOnlyList<DIRECTORY>> FindAllDirectoriesAsync(this DIRECTORY directoryInfo, Predicate<DIRECTORY> condition, CancellationToken ctoken)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return Array.Empty<DIRECTORY>();            

            // https://github.com/dotnet/runtime/issues/809
            // https://stackoverflow.com/questions/719020/is-there-an-async-version-of-directoryinfo-getfiles-directory-getdirectories-i            
            // https://gist.github.com/jnm2/46a642d2c9f2794ece0b095ba3d96270

            var result = new List<DIRECTORY>();

            async Task _findAsync(DIRECTORY dinfo)
            {
                ctoken.ThrowIfCancellationRequested();

                var subdirs = dinfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);

                foreach (var subdir in subdirs)
                {
                    if (condition(subdir)) result.Add(subdir);
                    await _findAsync(subdir).ConfigureAwait(false);
                }
            }

            await _findAsync(directoryInfo).ConfigureAwait(false);            

            return result;
        }

        public static async Task<DIRECTORY> FindFirstDirectoryAsync(this DIRECTORY directoryInfo, Predicate<DIRECTORY> condition, CancellationToken ctoken)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return null;

            // https://github.com/dotnet/runtime/issues/809
            // https://stackoverflow.com/questions/719020/is-there-an-async-version-of-directoryinfo-getfiles-directory-getdirectories-i            

            async Task<DIRECTORY> _findAsync(DIRECTORY dinfo)
            {
                ctoken.ThrowIfCancellationRequested();

                var subdirs = dinfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);

                foreach (var subdir in subdirs)
                {
                    if (condition(subdir)) return subdir;                    

                    var result = await _findAsync(subdir).ConfigureAwait(false);
                    if (result != null) return result;
                }

                return null;
            }

            return await _findAsync(directoryInfo).ConfigureAwait(false);
        }

        public static async Task<IReadOnlyList<FILE>> FindAllFilesAsync(this DIRECTORY directoryInfo, Predicate<FILE> condition, CancellationToken ctoken)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return Array.Empty<FileInfo>();

            // https://github.com/dotnet/runtime/issues/809
            // https://stackoverflow.com/questions/719020/is-there-an-async-version-of-directoryinfo-getfiles-directory-getdirectories-i            

            var result = new List<FILE>();

            async Task _findAsync(DIRECTORY dinfo)
            {
                ctoken.ThrowIfCancellationRequested();

                var files = dinfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    if (condition(file)) result.Add(file);
                }

                ctoken.ThrowIfCancellationRequested();

                var subdirs = dinfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
                foreach (var subdir in subdirs)
                {
                    await _findAsync(subdir).ConfigureAwait(false);
                }
            }

            await _findAsync(directoryInfo).ConfigureAwait(false);

            return result;
        }

        public static async Task<FILE> FindFirstFileAsync(this DIRECTORY directoryInfo, Predicate<FILE> condition, CancellationToken ctoken)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return null;

            // https://github.com/dotnet/runtime/issues/809
            // https://stackoverflow.com/questions/719020/is-there-an-async-version-of-directoryinfo-getfiles-directory-getdirectories-i            

            async Task<FILE> _findAsync(DIRECTORY dinfo)
            {
                ctoken.ThrowIfCancellationRequested();

                var files = dinfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    if (condition(file)) return file;
                }

                ctoken.ThrowIfCancellationRequested();

                var subdirs = dinfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
                foreach (var subdir in subdirs)
                {
                    var result = await _findAsync(subdir).ConfigureAwait(false);
                    if (result != null) return result;
                }

                return null;
            }

            return await _findAsync(directoryInfo).ConfigureAwait(false);
        }
        
    }
}
