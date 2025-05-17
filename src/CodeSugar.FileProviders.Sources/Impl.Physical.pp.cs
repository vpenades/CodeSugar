// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.FileProviders;

#nullable disable

using _FINFO = System.IO.FileInfo;
using _DINFO = System.IO.DirectoryInfo;
using _XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using _MATCHCASING = System.IO.MatchCasing;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{    
    internal static partial class CodeSugarForFileProviders    
    {
        #region API        

        [return: NotNull]
        public static _XINFO ToIFileInfo(this System.IO.FileSystemInfo xinfo)
        {
            switch (xinfo)
            {
                case null: return __NULLFILE;
                case _DINFO d: return new _BasicPhysicalDirectory(d);
                case _FINFO f: return new _BasicPhysicalFile(f);                
                default: throw new NotImplementedException();
            }
        }

        [return: NotNull]
        public static _XINFO ToIFileInfo(this _FINFO finfo)
        {
            return finfo == null
                ? __NULLFILE
                : new _BasicPhysicalFile(finfo);
        }

        [return: NotNull]
        public static _XINFO ToIFileInfo(this _DINFO dinfo)
        {
            return dinfo == null
                ? __NULLFILE
                : new _BasicPhysicalDirectory(dinfo);
        }

        public static bool TryGetFileInfo(this _XINFO xinfo, out _FINFO finfo)
        {
            finfo = null;
            if (xinfo == null) return false;
            if (xinfo.IsDirectory) return false;

            switch(xinfo)
            {
                case _BasicPhysicalFile f: finfo = f.Info; return true;
                case IServiceProvider s:
                    finfo = s.GetService(typeof(_FINFO)) as _FINFO;
                    if (finfo != null) return true;
                    else break;
            }

            if (string.IsNullOrWhiteSpace(xinfo.PhysicalPath)) return false;            

            try
            {
                finfo = new _FINFO(xinfo.PhysicalPath);
                return true;
            }
            catch { return false; }
        }

        public static bool TryGetDirectoryInfo(this _XINFO xinfo, out _DINFO dinfo)
        {
            dinfo = null;
            if (xinfo == null) return false;
            if (!xinfo.IsDirectory) return false;

            switch (xinfo)
            {
                case _BasicPhysicalDirectory d: dinfo = d.Info; return true;
                case IServiceProvider s:
                    dinfo = s.GetService(typeof(_DINFO)) as _DINFO;
                    if (dinfo != null) return true;
                    else break;
            }

            if (string.IsNullOrWhiteSpace(xinfo.PhysicalPath)) return false;            

            try
            {
                dinfo = new _DINFO(xinfo.PhysicalPath);
                return true;
            }
            catch { return false; }
        }

        #endregion

        #region nested types

        [System.Diagnostics.DebuggerDisplay("{PhysicalPath}")]
        private readonly struct _BasicPhysicalFile : _XINFO , IServiceProvider
        {
            #region constructor
            public _BasicPhysicalFile(_FINFO finfo) { Info = finfo; }

            #endregion

            #region properties

            public _FINFO Info { get; }

            public bool Exists => Info?.Exists ?? false;

            public long Length => Info?.Length ?? 0;

            public string PhysicalPath => Info?.FullName;

            public string Name => Info?.Name;

            public DateTimeOffset LastModified => Info.LastWriteTime;

            public bool IsDirectory => false;

            #endregion

            #region API

            public Stream CreateReadStream() { return Info.OpenRead(); }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(_FINFO)) return Info;
                if (serviceType == typeof(_MATCHCASING)) return FileSystemPathCasing;
                if (serviceType == typeof(StringComparison)) return FileSystemPathComparison;                
                if (serviceType == typeof(Action<ArraySegment<Byte>>)) return (Action<ArraySegment<Byte>>) _WriteBytes;

                return null;
            }

            private void _WriteBytes(ArraySegment<Byte> bytes)
            {
                using(var s = Info.Create())
                {
                    if (bytes.Count > 0) s.Write(bytes.Array,bytes.Offset,bytes.Count);
                }
            }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("{PhysicalPath}")]
        private readonly struct _BasicPhysicalDirectory : _XINFO, IDirectoryContents , IServiceProvider
        {
            #region constructor

            public _BasicPhysicalDirectory(_DINFO dinfo) { Info = dinfo; }

            #endregion

            #region properties

            public _DINFO Info { get; }

            public bool Exists => Info?.Exists ?? false;

            public long Length => 0;

            public string PhysicalPath => Info?.FullName;

            public string Name => Info?.Name;

            public DateTimeOffset LastModified => Info.LastWriteTime;

            public bool IsDirectory => true;

            #endregion

            #region API

            public Stream CreateReadStream() { throw new NotSupportedException(); }
            IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
            public IEnumerator<_XINFO> GetEnumerator()
            {
                return Info
                    .EnumerateFileSystemInfos()
                    .Select(ToIFileInfo)
                    .GetEnumerator();
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(System.IO.MatchCasing)) return FileSystemPathComparison;
                if (serviceType == typeof(_DINFO)) return Info;

                return null;
            }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("{Info.FullPath}")]
        private readonly struct _BasicPhysicalDirectoryContents : IDirectoryContents, IServiceProvider
        {
            #region constructor

            public _BasicPhysicalDirectoryContents(_DINFO dinfo) { Info = dinfo; }

            #endregion

            #region properties

            public _DINFO Info { get; }

            public bool Exists => Info?.Exists ?? false;            

            #endregion

            #region API

            public Stream CreateReadStream() { throw new NotSupportedException(); }
            IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
            public IEnumerator<_XINFO> GetEnumerator()
            {
                return Info
                    .EnumerateFileSystemInfos()
                    .Select(ToIFileInfo)
                    .GetEnumerator();
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(_MATCHCASING)) return FileSystemPathCasing;
                if (serviceType == typeof(StringComparison)) return FileSystemPathComparison;
                if (serviceType == typeof(_DINFO)) return Info;

                return null;
            }

            #endregion
        }

        #endregion
    }
}
