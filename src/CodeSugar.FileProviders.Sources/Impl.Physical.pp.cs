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

using FINFO = System.IO.FileInfo;
using DINFO = System.IO.DirectoryInfo;
using XFILE = Microsoft.Extensions.FileProviders.IFileInfo;
using MATCHCASING = System.IO.MatchCasing;


#nullable disable

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
        public static XFILE ToIFileInfo(this System.IO.FileSystemInfo xinfo)
        {
            switch (xinfo)
            {
                case null: return __NULLFILE;
                case DINFO d: return new _BasicPhysicalDirectory(d);
                case FINFO f: return new _BasicPhysicalFile(f);                
                default: throw new NotImplementedException();
            }
        }

        [return: NotNull]
        public static XFILE ToIFileInfo(this FINFO finfo)
        {
            return finfo == null
                ? __NULLFILE
                : new _BasicPhysicalFile(finfo);
        }

        [return: NotNull]
        public static XFILE ToIFileInfo(this DINFO dinfo)
        {
            return dinfo == null
                ? __NULLFILE
                : new _BasicPhysicalDirectory(dinfo);
        }

        public static bool TryGetFileInfo(this XFILE xinfo, out FINFO finfo)
        {
            finfo = null;
            if (xinfo == null) return false;
            if (xinfo.IsDirectory) return false;

            switch(xinfo)
            {
                case _BasicPhysicalFile f: finfo = f.Info; return true;
                case IServiceProvider s:
                    finfo = s.GetService(typeof(FINFO)) as FINFO;
                    if (finfo != null) return true;
                    else break;
            }

            if (string.IsNullOrWhiteSpace(xinfo.PhysicalPath)) return false;            

            try
            {
                finfo = new FINFO(xinfo.PhysicalPath);
                return true;
            }
            catch { return false; }
        }

        public static bool TryGetDirectoryInfo(this XFILE xinfo, out DINFO dinfo)
        {
            dinfo = null;
            if (xinfo == null) return false;
            if (!xinfo.IsDirectory) return false;

            switch (xinfo)
            {
                case _BasicPhysicalDirectory d: dinfo = d.Info; return true;
                case IServiceProvider s:
                    dinfo = s.GetService(typeof(DINFO)) as DINFO;
                    if (dinfo != null) return true;
                    else break;
            }

            if (string.IsNullOrWhiteSpace(xinfo.PhysicalPath)) return false;            

            try
            {
                dinfo = new DINFO(xinfo.PhysicalPath);
                return true;
            }
            catch { return false; }
        }

        #endregion

        #region nested types

        [System.Diagnostics.DebuggerDisplay("{PhysicalPath}")]
        private readonly struct _BasicPhysicalFile : XFILE , IServiceProvider
        {
            #region constructor
            public _BasicPhysicalFile(FINFO finfo) { Info = finfo; }

            #endregion

            #region properties

            public FINFO Info { get; }

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
                if (serviceType == typeof(FINFO)) return Info;
                if (serviceType == typeof(MATCHCASING)) return FileSystemPathCasing;
                if (serviceType == typeof(StringComparison)) return FileSystemPathComparison;                
                if (serviceType == typeof(Action<ArraySegment<Byte>>)) return (Action<ArraySegment<Byte>>) _WriteBytes;

                return null;
            }

            private void _WriteBytes(ArraySegment<Byte> bytes)
            {
                using(var s = Info.OpenWrite())
                {
                    if (bytes.Count > 0) s.Write(bytes.Array,bytes.Offset,bytes.Count);
                }
            }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("{PhysicalPath}")]
        private readonly struct _BasicPhysicalDirectory : XFILE, IDirectoryContents , IServiceProvider
        {
            #region constructor

            public _BasicPhysicalDirectory(DINFO dinfo) { Info = dinfo; }

            #endregion

            #region properties

            public DINFO Info { get; }

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
            public IEnumerator<XFILE> GetEnumerator()
            {
                return Info
                    .EnumerateFileSystemInfos()
                    .Select(ToIFileInfo)
                    .GetEnumerator();
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(System.IO.MatchCasing)) return FileSystemPathComparison;
                if (serviceType == typeof(DINFO)) return Info;

                return null;
            }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("{Info.FullPath}")]
        private readonly struct _BasicPhysicalDirectoryContents : IDirectoryContents, IServiceProvider
        {
            #region constructor

            public _BasicPhysicalDirectoryContents(DINFO dinfo) { Info = dinfo; }

            #endregion

            #region properties

            public DINFO Info { get; }

            public bool Exists => Info?.Exists ?? false;            

            #endregion

            #region API

            public Stream CreateReadStream() { throw new NotSupportedException(); }
            IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
            public IEnumerator<XFILE> GetEnumerator()
            {
                return Info
                    .EnumerateFileSystemInfos()
                    .Select(ToIFileInfo)
                    .GetEnumerator();
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(MATCHCASING)) return FileSystemPathCasing;
                if (serviceType == typeof(StringComparison)) return FileSystemPathComparison;
                if (serviceType == typeof(DINFO)) return Info;

                return null;
            }

            #endregion
        }

        #endregion
    }
}
