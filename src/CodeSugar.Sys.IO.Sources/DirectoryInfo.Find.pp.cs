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
using PPROGRESS = System.IProgress<int>;
using CTOKEN = System.Threading.CancellationToken;


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

            var context = new __DirectoryScanStateSlice(100, percentProgress, ctoken);

            await _findAsync(directoryInfo, context).ConfigureAwait(false);

            context.ReportCompleted();

            return result;

            async Task _findAsync(DIRECTORY dinfo, __DirectoryScanStateSlice slice)
            {
                if (dinfo == null || !dinfo.Exists) return;                

                await slice.UpdateAsync(dinfo).ConfigureAwait(false);

                List<DIRECTORY> subdirs = null;

                try
                {
                    foreach (var info in dinfo.EnumerateDirectories())
                    {
                        if (resultFilter(info)) result.Add(info);
                        if (subdirFilter(info)) { subdirs ??= new List<DIRECTORY>(); subdirs.Add(info); }
                    }
                }
                catch (System.IO.DirectoryNotFoundException ex) { slice.ReportException(ex); }
                catch (System.Security.SecurityException ex) { slice.ReportException(ex); }

                if (subdirs == null) return;

                for (int i = 0; i < subdirs.Count; ++i)
                {
                    await _findAsync(subdirs[i], slice.Next(i,subdirs.Count)).ConfigureAwait(false);
                }
            }            
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

            var context = new __DirectoryScanStateSlice(100, percentProgress, ctoken);

            var result = await _findAsync(directoryInfo, context).ConfigureAwait(false);

            context.ReportCompleted();

            return result;

            async Task<DIRECTORY> _findAsync(DIRECTORY dinfo, __DirectoryScanStateSlice slice)
            {
                if (dinfo == null || !dinfo.Exists) return null;                

                await slice.UpdateAsync(dinfo).ConfigureAwait(false);

                List<DIRECTORY> subdirs = null;

                try
                {
                    foreach (var info in dinfo.EnumerateDirectories())
                    {
                        if (resultFilter(info)) return info;
                        if (subdirFilter(info)) { subdirs ??= new List<DIRECTORY>(); subdirs.Add(info); }
                    }
                }
                catch (System.IO.DirectoryNotFoundException ex) { slice.ReportException(ex); }
                catch (System.Security.SecurityException ex) { slice.ReportException(ex); }

                if (subdirs == null) return null;

                for (int i = 0; i < subdirs.Count; ++i)
                {
                    var rr = await _findAsync(subdirs[i], slice.Next(i,subdirs.Count)).ConfigureAwait(false);
                    if (rr != null) return rr;
                }

                return null;
            }            
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

            var context = new __DirectoryScanStateSlice(100, percentProgress, ctoken);

            await _findAsync(directoryInfo, context).ConfigureAwait(false);

            context.ReportCompleted();

            return result;

            async Task _findAsync(DIRECTORY dinfo, __DirectoryScanStateSlice slice)
            {
                if (dinfo == null || !dinfo.Exists) return;                

                await slice.UpdateAsync(dinfo).ConfigureAwait(false);

                List<DIRECTORY> subdirs = null;

                try
                {
                    foreach (var info in dinfo.EnumerateFileSystemInfos())
                    {
                        if (info is FILE finfo && resultFilter(finfo)) result.Add(finfo);
                        else if (info is DIRECTORY subd && subdirFilter(subd)) { subdirs ??= new List<DIRECTORY>(); subdirs.Add(subd); }
                    }
                }
                catch (System.IO.DirectoryNotFoundException ex) { slice.ReportException(ex); }
                catch (System.Security.SecurityException ex) { slice.ReportException(ex); }

                if (subdirs == null) return;

                for (int i = 0; i < subdirs.Count; ++i)
                {
                    await _findAsync(subdirs[i], slice.Next(i,subdirs.Count)).ConfigureAwait(false);
                }
            }            
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

            var context = new __DirectoryScanStateSlice(100, percentProgress, ctoken);

            var result = await _findAsync(directoryInfo, context).ConfigureAwait(false);

            context.ReportCompleted();

            return result;

            async Task<FILE> _findAsync(DIRECTORY dinfo, __DirectoryScanStateSlice slice)
            {
                if (dinfo == null || !dinfo.Exists) return null;                

                await slice.UpdateAsync(dinfo).ConfigureAwait(false);

                List<DIRECTORY> subdirs = null;

                try
                {
                    foreach (var info in dinfo.EnumerateFileSystemInfos())
                    {
                        if (info is FILE finfo && resultFilter(finfo)) return finfo;
                        else if (info is DIRECTORY subd && subdirFilter(subd)) { subdirs ??= new List<DIRECTORY>(); subdirs.Add(subd); }
                    }
                }
                catch (System.IO.DirectoryNotFoundException ex) { slice.ReportException(ex); }
                catch (System.Security.SecurityException ex) { slice.ReportException(ex); }

                if (subdirs == null) return null;

                for (int i = 0; i < subdirs.Count; ++i)
                {
                    var rr = await _findAsync(subdirs[i], slice.Next(i,subdirs.Count)).ConfigureAwait(false);
                    if (rr != null) return rr;
                }

                return null;
            }            
        }


        private readonly struct __DirectoryScanStateSlice
        {
            public __DirectoryScanStateSlice(int count, PPROGRESS progress, CTOKEN token)
            {
                _Start = 0;
                _Count = count;
                _Progress = progress;
                _Token = token;
            }

            private __DirectoryScanStateSlice(int start, int count, PPROGRESS progress, CTOKEN token)
            {
                _Start = start;
                _Count = count;
                _Progress = progress;
                _Token = token;
            }

            private readonly PPROGRESS _Progress;
            private readonly CTOKEN _Token;

            private readonly int _Start;
            private readonly int _Count;

            public async Task UpdateAsync(DIRECTORY dinfo)
            {
                _Token.ThrowIfCancellationRequested();
                _ReportProgress(_Progress, _Start, dinfo.FullName);

                await Task.Yield();
            }

            public void ReportCompleted()
            {
                _ReportProgress(_Progress, 100, string.Empty);
            }

            private static void _ReportProgress(PPROGRESS progress, int percent, string msg)
            {
                System.Diagnostics.Debug.Assert(percent >= 0 && percent <= 100, $"Percent {percent} is out of bounds");

                switch (progress)
                {
                    case null: break;

                    case IProgress<(int, string)> mixedProgress:
                        mixedProgress.Report((percent, msg));
                        break;

                    case IProgress<string> textProgress:
                        progress.Report(percent);
                        textProgress.Report(msg);
                        break;

                    default: progress.Report(percent); break;
                }
            }

            public void ReportException(Exception ex)
            {
                if (ex == null) return;
                switch (_Progress)
                {
                    case null: break;
                    case IProgress<Exception> pex: pex.Report(ex); break;
                    case IProgress<(System.Diagnostics.TraceEventType level, string msg)> pex: pex.Report((System.Diagnostics.TraceEventType.Error, ex.Message)); break;
                    case IProgress<string> ptxt: ptxt.Report(ex.Message); break;
                }
            }

            public __DirectoryScanStateSlice Next(int index, int count)
            {
                var ss = this._Start + this._Count * (index + 0) / count;
                var cc = this._Start + this._Count * (index + 1) / count - ss;

                return new __DirectoryScanStateSlice(ss, cc, this._Progress, this._Token);
            }            
        }
    }
}
