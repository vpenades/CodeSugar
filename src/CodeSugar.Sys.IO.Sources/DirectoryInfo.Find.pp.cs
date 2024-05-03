// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


#nullable disable

using FILE = System.IO.FileInfo;
using FILEFILTER = System.Predicate<System.IO.FileInfo>;
using DIRECTORY = System.IO.DirectoryInfo;
using DIRECTORYFILTER = System.Predicate<System.IO.DirectoryInfo>;
using SEARCHOPTION = System.IO.SearchOption;
using CTOKEN = System.Threading.CancellationToken;
using PPROGRESS = System.IProgress<int>;

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

        public static async Task<IReadOnlyList<DIRECTORY>> FindAllDirectoriesAsync(this DIRECTORY directoryInfo, DIRECTORYFILTER resultFilter, SEARCHOPTION option, CTOKEN ctoken, PPROGRESS percentProgress = null)
        {
            return await FindAllDirectoriesAsync(directoryInfo, resultFilter, d=> option == SEARCHOPTION.AllDirectories, ctoken, percentProgress).ConfigureAwait(false);
        }
        public static async Task<IReadOnlyList<DIRECTORY>> FindAllDirectoriesAsync(this DIRECTORY directoryInfo, DIRECTORYFILTER resultFilter, DIRECTORYFILTER subdirFilter, CTOKEN ctoken, PPROGRESS percentProgress = null)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return Array.Empty<DIRECTORY>();
            if (resultFilter == null) throw new ArgumentNullException(nameof(resultFilter));
            if (subdirFilter == null) throw new ArgumentNullException(nameof(subdirFilter));

            var result = new List<DIRECTORY>();

            async Task _findAsync(DIRECTORY dinfo, int start, int count)
            {
                if (dinfo == null || !dinfo.Exists) return;

                ctoken.ThrowIfCancellationRequested();

                __ReportDirectoryScanProgress(percentProgress, start, dinfo);

                List<DIRECTORY> subdirs = null;

                try
                {
                    foreach (var info in dinfo.EnumerateDirectories())
                    {
                        if (resultFilter(info)) result.Add(info);
                        if (subdirFilter(info)) { subdirs ??= new List<DIRECTORY>(); subdirs.Add(info); }
                    }
                }
                catch (System.IO.DirectoryNotFoundException ex) { __ReportDirectoryScanException(percentProgress,ex); }
                catch (System.Security.SecurityException ex) { __ReportDirectoryScanException(percentProgress, ex); }

                if (subdirs == null) return;

                for (int i = 0; i < subdirs.Count; ++i)
                {
                    var ss = start + count * (i + 0) / subdirs.Count;
                    var cc = start + count * (i + 1) / subdirs.Count - ss;

                    await _findAsync(subdirs[i], ss, cc).ConfigureAwait(false);
                }
            }

            await _findAsync(directoryInfo, 0, 100).ConfigureAwait(false);            

            return result;
        }


        public static async Task<DIRECTORY> FindFirstDirectoryAsync(this DIRECTORY directoryInfo, DIRECTORYFILTER resultFilter, SEARCHOPTION option, CTOKEN ctoken, PPROGRESS percentProgress = null)
        {
            return await FindFirstDirectoryAsync(directoryInfo, resultFilter, d => option == SEARCHOPTION.AllDirectories, ctoken, percentProgress).ConfigureAwait(false);
        }
        public static async Task<DIRECTORY> FindFirstDirectoryAsync(this DIRECTORY directoryInfo, DIRECTORYFILTER resultFilter, DIRECTORYFILTER subdirFilter, CTOKEN ctoken, PPROGRESS percentProgress = null)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return null;
            if (resultFilter == null) throw new ArgumentNullException(nameof(resultFilter));
            if (subdirFilter == null) throw new ArgumentNullException(nameof(subdirFilter));            

            async Task<DIRECTORY> _findAsync(DIRECTORY dinfo, int start, int count)
            {
                if (dinfo == null || !dinfo.Exists) return null;

                ctoken.ThrowIfCancellationRequested();

                __ReportDirectoryScanProgress(percentProgress, start, dinfo);

                List<DIRECTORY> subdirs = null;

                try
                {
                    foreach (var info in dinfo.EnumerateDirectories())
                    {
                        if (resultFilter(info)) return info;
                        if (subdirFilter(info)) { subdirs ??= new List<DIRECTORY>(); subdirs.Add(info); }
                    }
                }
                catch (System.IO.DirectoryNotFoundException ex) { __ReportDirectoryScanException(percentProgress, ex); }
                catch (System.Security.SecurityException ex) { __ReportDirectoryScanException(percentProgress, ex); }

                if (subdirs == null) return null;

                for (int i = 0; i < subdirs.Count; ++i)
                {
                    var ss = start + count * (i + 0) / subdirs.Count;
                    var cc = start + count * (i + 1) / subdirs.Count - ss;

                    var rr = await _findAsync(subdirs[i], ss, cc).ConfigureAwait(false);
                    if (rr != null) return rr;
                }

                return null;
            }

            return await _findAsync(directoryInfo, 0, 100).ConfigureAwait(false);
        }


        public static async Task<IReadOnlyList<FILE>> FindAllFilesAsync(this DIRECTORY directoryInfo, FILEFILTER resultFilter, SEARCHOPTION option, CTOKEN ctoken, PPROGRESS percentProgress = null)
        {
            return await FindAllFilesAsync(directoryInfo, resultFilter, d => option == SEARCHOPTION.AllDirectories, ctoken, percentProgress).ConfigureAwait(false);
        }
        public static async Task<IReadOnlyList<FILE>> FindAllFilesAsync(this DIRECTORY directoryInfo, FILEFILTER resultFilter, DIRECTORYFILTER subdirFilter, CTOKEN ctoken, PPROGRESS percentProgress = null)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return Array.Empty<FILE>();
            if (resultFilter == null) throw new ArgumentNullException(nameof(resultFilter));
            if (subdirFilter == null) throw new ArgumentNullException(nameof(subdirFilter));            

            var result = new List<FILE>();

            async Task _findAsync(DIRECTORY dinfo, int start, int count)
            {
                if (dinfo == null || !dinfo.Exists) return;

                ctoken.ThrowIfCancellationRequested();

                __ReportDirectoryScanProgress(percentProgress, start, dinfo);

                List<DIRECTORY> subdirs = null;

                try
                {
                    foreach (var info in dinfo.EnumerateFileSystemInfos())
                    {
                        if (info is FILE finfo && resultFilter(finfo)) result.Add(finfo);
                        else if (info is DIRECTORY subd && subdirFilter(subd)) { subdirs ??= new List<DIRECTORY>(); subdirs.Add(subd); }
                    }
                }
                catch (System.IO.DirectoryNotFoundException ex) { __ReportDirectoryScanException(percentProgress, ex); }
                catch (System.Security.SecurityException ex) { __ReportDirectoryScanException(percentProgress, ex); }

                if (subdirs == null) return;

                for (int i = 0; i < subdirs.Count; ++i)
                {
                    var ss = start + count * (i + 0) / subdirs.Count;
                    var cc = start + count * (i + 1) / subdirs.Count - ss;

                    await _findAsync(subdirs[i], ss, cc).ConfigureAwait(false);
                }
            }

            await _findAsync(directoryInfo, 0, 100).ConfigureAwait(false);

            return result;
        }
                

        public static async Task<FILE> FindFirstFileAsync(this DIRECTORY directoryInfo, FILEFILTER resultFilter, SEARCHOPTION option, CTOKEN ctoken, PPROGRESS percentProgress = null)
        {
            return await FindFirstFileAsync(directoryInfo, resultFilter, d => option == SEARCHOPTION.AllDirectories, ctoken, percentProgress).ConfigureAwait(false);
        }
        public static async Task<FILE> FindFirstFileAsync(this DIRECTORY directoryInfo, FILEFILTER resultFilter, DIRECTORYFILTER subdirFilter, CTOKEN ctoken, PPROGRESS percentProgress = null)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return null;
            if (resultFilter == null) throw new ArgumentNullException(nameof(resultFilter));
            if (subdirFilter == null) throw new ArgumentNullException(nameof(subdirFilter));            

            async Task<FILE> _findAsync(DIRECTORY dinfo, int start, int count)
            {
                if (dinfo == null || !dinfo.Exists) return null;

                ctoken.ThrowIfCancellationRequested();

                __ReportDirectoryScanProgress(percentProgress, start, dinfo);

                List<DIRECTORY> subdirs = null;

                try
                {
                    foreach (var info in dinfo.EnumerateFileSystemInfos())
                    {
                        if (info is FILE finfo && resultFilter(finfo)) return finfo;
                        else if (info is DIRECTORY subd && subdirFilter(subd)) { subdirs ??= new List<DIRECTORY>(); subdirs.Add(subd); }
                    }
                }
                catch (System.IO.DirectoryNotFoundException ex) { __ReportDirectoryScanException(percentProgress, ex); }
                catch (System.Security.SecurityException ex) { __ReportDirectoryScanException(percentProgress, ex); }

                if (subdirs == null) return null;

                for (int i = 0; i < subdirs.Count; ++i)
                {
                    var ss = start + count * (i + 0) / subdirs.Count;
                    var cc = start + count * (i + 1) / subdirs.Count - ss;

                    var rr = await _findAsync(subdirs[i], ss, cc).ConfigureAwait(false);
                    if (rr != null) return rr;
                }

                return null;
            }

            return await _findAsync(directoryInfo, 0, 100).ConfigureAwait(false);
        }


        private static void __ReportDirectoryScanProgress(PPROGRESS percentProgress, int percent, DIRECTORY dinfo)
        {
            if (percentProgress == null) return;            

            System.Diagnostics.Debug.Assert(percent >= 0 && percent <= 100, $"Percent {percent} is out of bounds");

            if (percentProgress is IProgress<(int, string)> mixedProgress)
            {
                mixedProgress.Report((percent, dinfo.FullName));
                return;
            }

            percentProgress.Report(percent);
            if (percentProgress is IProgress<string> textProgress) textProgress.Report(dinfo.FullName);
        }
        private static void __ReportDirectoryScanException(PPROGRESS percentProgress, Exception ex)
        {
            if (ex == null) return;
            switch(percentProgress)
            {
                case null: break;
                case IProgress<Exception> pex: pex.Report(ex); break;
                case IProgress<string> ptxt: ptxt.Report(ex.Message); break;
            }
        }        
    }
}
