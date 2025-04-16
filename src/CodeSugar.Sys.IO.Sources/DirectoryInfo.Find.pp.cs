// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable disable

using _FINFO = System.IO.FileInfo;
using _FILEFILTER = System.Predicate<System.IO.FileInfo>;
using _DINFO = System.IO.DirectoryInfo;
using _DIRECTORYFILTER = System.Predicate<System.IO.DirectoryInfo>;
using _SEARCHOPTION = System.IO.SearchOption;
using _PPROGRESS = System.IProgress<int>;
using _CTOKEN = System.Threading.CancellationToken;


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

        public static async Task<IReadOnlyList<_DINFO>> FindAllDirectoriesAsync(this _DINFO directoryInfo, _DIRECTORYFILTER resultFilter, _SEARCHOPTION option, _CTOKEN ctoken, _PPROGRESS percentProgress = null)
        {
            return await FindAllDirectoriesAsync(directoryInfo, resultFilter, d=> option == _SEARCHOPTION.AllDirectories, ctoken, percentProgress).ConfigureAwait(false);
        }
        public static async Task<IReadOnlyList<_DINFO>> FindAllDirectoriesAsync(this _DINFO directoryInfo, _DIRECTORYFILTER resultFilter, _DIRECTORYFILTER subdirFilter, _CTOKEN ctoken, _PPROGRESS percentProgress = null)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return Array.Empty<_DINFO>();
            if (resultFilter == null) throw new ArgumentNullException(nameof(resultFilter));
            if (subdirFilter == null) throw new ArgumentNullException(nameof(subdirFilter));

            var result = new List<_DINFO>();

            var context = new __DirectoryScanStateSlice(100, percentProgress, ctoken);

            await _findAsync(directoryInfo, context).ConfigureAwait(false);

            context.ReportCompleted();

            return result;

            async Task _findAsync(_DINFO dinfo, __DirectoryScanStateSlice slice)
            {
                if (dinfo == null || !dinfo.Exists) return;                

                await slice.UpdateAsync(dinfo).ConfigureAwait(false);

                List<_DINFO> subdirs = null;

                try
                {
                    foreach (var info in dinfo.EnumerateDirectories())
                    {
                        if (resultFilter(info)) result.Add(info);
                        if (subdirFilter(info)) { subdirs ??= new List<_DINFO>(); subdirs.Add(info); }
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


        public static async Task<_DINFO> FindFirstDirectoryAsync(this _DINFO directoryInfo, _DIRECTORYFILTER resultFilter, _SEARCHOPTION option, _CTOKEN ctoken, _PPROGRESS percentProgress = null)
        {
            return await FindFirstDirectoryAsync(directoryInfo, resultFilter, d => option == _SEARCHOPTION.AllDirectories, ctoken, percentProgress).ConfigureAwait(false);
        }
        public static async Task<_DINFO> FindFirstDirectoryAsync(this _DINFO directoryInfo, _DIRECTORYFILTER resultFilter, _DIRECTORYFILTER subdirFilter, _CTOKEN ctoken, _PPROGRESS percentProgress = null)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return null;
            if (resultFilter == null) throw new ArgumentNullException(nameof(resultFilter));
            if (subdirFilter == null) throw new ArgumentNullException(nameof(subdirFilter));

            var context = new __DirectoryScanStateSlice(100, percentProgress, ctoken);

            var result = await _findAsync(directoryInfo, context).ConfigureAwait(false);

            context.ReportCompleted();

            return result;

            async Task<_DINFO> _findAsync(_DINFO dinfo, __DirectoryScanStateSlice slice)
            {
                if (dinfo == null || !dinfo.Exists) return null;                

                await slice.UpdateAsync(dinfo).ConfigureAwait(false);

                List<_DINFO> subdirs = null;

                try
                {
                    foreach (var info in dinfo.EnumerateDirectories())
                    {
                        if (resultFilter(info)) return info;
                        if (subdirFilter(info)) { subdirs ??= new List<_DINFO>(); subdirs.Add(info); }
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


        public static async Task<IReadOnlyList<_FINFO>> FindAllFilesAsync(this _DINFO directoryInfo, _FILEFILTER resultFilter, _SEARCHOPTION option, _CTOKEN ctoken, _PPROGRESS percentProgress = null)
        {
            return await FindAllFilesAsync(directoryInfo, resultFilter, d => option == _SEARCHOPTION.AllDirectories, ctoken, percentProgress).ConfigureAwait(false);
        }
        public static async Task<IReadOnlyList<_FINFO>> FindAllFilesAsync(this _DINFO directoryInfo, _FILEFILTER resultFilter, _DIRECTORYFILTER subdirFilter, _CTOKEN ctoken, _PPROGRESS percentProgress = null)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return Array.Empty<_FINFO>();
            if (resultFilter == null) throw new ArgumentNullException(nameof(resultFilter));
            if (subdirFilter == null) throw new ArgumentNullException(nameof(subdirFilter));            

            var result = new List<_FINFO>();

            var context = new __DirectoryScanStateSlice(100, percentProgress, ctoken);

            await _findAsync(directoryInfo, context).ConfigureAwait(false);

            context.ReportCompleted();

            return result;

            async Task _findAsync(_DINFO dinfo, __DirectoryScanStateSlice slice)
            {
                if (dinfo == null || !dinfo.Exists) return;                

                await slice.UpdateAsync(dinfo).ConfigureAwait(false);

                List<_DINFO> subdirs = null;

                try
                {
                    foreach (var info in dinfo.EnumerateFileSystemInfos())
                    {
                        if (info is _FINFO finfo && resultFilter(finfo)) result.Add(finfo);
                        else if (info is _DINFO subd && subdirFilter(subd)) { subdirs ??= new List<_DINFO>(); subdirs.Add(subd); }
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
                

        public static async Task<_FINFO> FindFirstFileAsync(this _DINFO directoryInfo, _FILEFILTER resultFilter, _SEARCHOPTION option, _CTOKEN ctoken, _PPROGRESS percentProgress = null)
        {
            return await FindFirstFileAsync(directoryInfo, resultFilter, d => option == _SEARCHOPTION.AllDirectories, ctoken, percentProgress).ConfigureAwait(false);
        }
        public static async Task<_FINFO> FindFirstFileAsync(this _DINFO directoryInfo, _FILEFILTER resultFilter, _DIRECTORYFILTER subdirFilter, _CTOKEN ctoken, _PPROGRESS percentProgress = null)
        {
            if (directoryInfo == null || !directoryInfo.Exists) return null;
            if (resultFilter == null) throw new ArgumentNullException(nameof(resultFilter));
            if (subdirFilter == null) throw new ArgumentNullException(nameof(subdirFilter));

            var context = new __DirectoryScanStateSlice(100, percentProgress, ctoken);

            var result = await _findAsync(directoryInfo, context).ConfigureAwait(false);

            context.ReportCompleted();

            return result;

            async Task<_FINFO> _findAsync(_DINFO dinfo, __DirectoryScanStateSlice slice)
            {
                if (dinfo == null || !dinfo.Exists) return null;                

                await slice.UpdateAsync(dinfo).ConfigureAwait(false);

                List<_DINFO> subdirs = null;

                try
                {
                    foreach (var info in dinfo.EnumerateFileSystemInfos())
                    {
                        if (info is _FINFO finfo && resultFilter(finfo)) return finfo;
                        else if (info is _DINFO subd && subdirFilter(subd)) { subdirs ??= new List<_DINFO>(); subdirs.Add(subd); }
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
            public __DirectoryScanStateSlice(int count, _PPROGRESS progress, _CTOKEN token)
            {
                _Start = 0;
                _Count = count;
                _Progress = progress;
                _Token = token;
            }

            private __DirectoryScanStateSlice(int start, int count, _PPROGRESS progress, _CTOKEN token)
            {
                _Start = start;
                _Count = count;
                _Progress = progress;
                _Token = token;
            }

            private readonly _PPROGRESS _Progress;
            private readonly _CTOKEN _Token;

            private readonly int _Start;
            private readonly int _Count;

            public async Task UpdateAsync(_DINFO dinfo)
            {
                _Token.ThrowIfCancellationRequested();
                _ReportProgress(_Progress, _Start, dinfo.FullName);

                await Task.Yield();
            }

            public void ReportCompleted()
            {
                _ReportProgress(_Progress, 100, string.Empty);
            }

            private static void _ReportProgress(_PPROGRESS progress, int percent, string msg)
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
