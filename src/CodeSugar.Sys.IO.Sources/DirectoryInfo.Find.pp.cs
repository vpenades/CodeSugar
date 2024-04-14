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
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/IO/Directory.cs
        // https://github.com/dotnet/runtime/tree/main/src/libraries/System.Private.CoreLib/src/System/IO/Enumeration
        // https://github.com/dotnet/runtime/issues/809
        // https://stackoverflow.com/questions/719020/is-there-an-async-version-of-directoryinfo-getfiles-directory-getdirectories-i            
        // https://gist.github.com/jnm2/46a642d2c9f2794ece0b095ba3d96270

        public static async Task<IReadOnlyList<DIRECTORY>> FindAllDirectoriesAsync(this DIRECTORY directoryInfo, Predicate<DIRECTORY> selector, CancellationToken ctoken)
        {
            return await FindAllDirectoriesAsync(directoryInfo, selector,d=>true,ctoken).ConfigureAwait(false);
        }

        public static async Task<IReadOnlyList<DIRECTORY>> FindAllDirectoriesAsync(this DIRECTORY directoryInfo, Predicate<DIRECTORY> selector, Predicate<DIRECTORY> subdirSelector, CancellationToken ctoken)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return Array.Empty<DIRECTORY>();

            selector ??= d => true;

            var result = new List<DIRECTORY>();

            async Task _findAsync(DIRECTORY dinfo)
            {
                ctoken.ThrowIfCancellationRequested();

                List<DIRECTORY> subdirs = null;

                try
                {
                    subdirs = dinfo
                        .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                        .ToList();

                    result.AddRange(subdirs.Where(item => selector(item)));                    
                }
                catch (System.IO.DirectoryNotFoundException) { }
                catch (System.Security.SecurityException) { }

                if (subdirSelector == null) return;

                foreach (var subdir in subdirs.Where(item => subdirSelector(item)))
                {                    
                    await _findAsync(subdir).ConfigureAwait(false);
                }
            }

            await _findAsync(directoryInfo).ConfigureAwait(false);            

            return result;
        }


        public static async Task<DIRECTORY> FindFirstDirectoryAsync(this DIRECTORY directoryInfo, Predicate<DIRECTORY> selector, CancellationToken ctoken)
        {
            return await FindFirstDirectoryAsync(directoryInfo, selector, d => true, ctoken).ConfigureAwait(false);
        }
        public static async Task<DIRECTORY> FindFirstDirectoryAsync(this DIRECTORY directoryInfo, Predicate<DIRECTORY> selector, Predicate<DIRECTORY> subdirSelector, CancellationToken ctoken)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return null;            

            selector ??= d => true;

            async Task<DIRECTORY> _findAsync(DIRECTORY dinfo)
            {
                ctoken.ThrowIfCancellationRequested();                

                var subdirsCache = subdirSelector == null
                    ? null
                    : new List<DIRECTORY>();

                try // first search in top directory, finish searching in a given directory before jumping to another one.
                {
                    var subdirs = dinfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);

                    foreach (var subdir in subdirs)
                    {
                        if (selector(subdir)) return subdir;
                        subdirsCache?.Add(subdir);
                    }
                }                
                catch (System.IO.DirectoryNotFoundException) { }
                catch (System.Security.SecurityException) { }

                if (subdirsCache == null) return null;                

                // search in subdirectories

                foreach (var subdir in subdirsCache)
                {
                    var result = await _findAsync(subdir).ConfigureAwait(false);
                    if (result != null) return result;
                }

                return null;
            }

            return await _findAsync(directoryInfo).ConfigureAwait(false);
        }



        public static async Task<IReadOnlyList<FILE>> FindAllFilesAsync(this DIRECTORY directoryInfo, Predicate<FILE> selector, CancellationToken ctoken)
        {
            return await FindAllFilesAsync(directoryInfo, selector, d => true, ctoken).ConfigureAwait(false);
        }
        public static async Task<IReadOnlyList<FILE>> FindAllFilesAsync(this DIRECTORY directoryInfo, Predicate<FILE> selector, Predicate<DIRECTORY> subdirSelector, CancellationToken ctoken)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return Array.Empty<FILE>();

            selector ??= d => true;            

            var result = new List<FILE>();

            async Task _findAsync(DIRECTORY dinfo)
            {
                ctoken.ThrowIfCancellationRequested();

                try // find files in current directory
                {
                    var files = dinfo
                        .EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                        .Where(item => selector(item));

                    result.AddRange(files);
                }
                catch (System.IO.DirectoryNotFoundException) { }
                catch (System.Security.SecurityException) { }

                if (subdirSelector == null) return;

                ctoken.ThrowIfCancellationRequested();

                try // dig into subdirectories
                {
                    var subdirs = dinfo
                        .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                        .Where(item => subdirSelector(item));

                    foreach (var subdir in subdirs)
                    {
                        await _findAsync(subdir).ConfigureAwait(false);
                    }
                }
                catch (System.IO.DirectoryNotFoundException) { }
                catch (System.Security.SecurityException) { }
            }

            await _findAsync(directoryInfo).ConfigureAwait(false);

            return result;
        }


        public static async Task<FILE> FindFirstFileAsync(this DIRECTORY directoryInfo, Predicate<FILE> selector, CancellationToken ctoken)
        {
            return await FindFirstFileAsync(directoryInfo, selector, d => true, ctoken).ConfigureAwait(false);
        }
        public static async Task<FILE> FindFirstFileAsync(this DIRECTORY directoryInfo, Predicate<FILE> selector, Predicate<DIRECTORY> subdirSelector, CancellationToken ctoken)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return null;

            selector ??= f => true;            

            async Task<FILE> _findAsync(DIRECTORY dinfo)
            {
                ctoken.ThrowIfCancellationRequested();

                try // find files in current directory
                {
                    var result = dinfo
                        .EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                        .FirstOrDefault(item => selector(item));

                    if (result != null) return result;
                }
                catch (System.IO.DirectoryNotFoundException) { }
                catch (System.Security.SecurityException) { }

                if (subdirSelector == null) return null;

                ctoken.ThrowIfCancellationRequested();

                try // dig into subdirectories
                {
                    var subdirs = dinfo
                        .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                        .Where(item => subdirSelector(item));

                    foreach (var subdir in subdirs)
                    {
                        var result = await _findAsync(subdir).ConfigureAwait(false);

                        if (result != null) return result;
                    }
                }
                catch (System.IO.DirectoryNotFoundException) { }
                catch (System.Security.SecurityException) { }

                return null;
            }

            return await _findAsync(directoryInfo).ConfigureAwait(false);
        }        
        
    }
}
