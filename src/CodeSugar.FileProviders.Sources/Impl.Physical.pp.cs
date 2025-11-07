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

using __FINFO = System.IO.FileInfo;
using __DINFO = System.IO.DirectoryInfo;
using __XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using __MATCHCASING = System.IO.MatchCasing;

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
        public static __XINFO ToIFileInfo(this System.IO.FileSystemInfo xinfo)
        {
            switch (xinfo)
            {
                case null: return __NULLFILE;
                case __DINFO d: return new _BasicPhysicalDirectory(d);
                case __FINFO f: return new _BasicPhysicalFile(f);                
                default: throw new NotImplementedException();
            }
        }

        [return: NotNull]
        public static __XINFO ToIFileInfo(this __FINFO finfo)
        {
            return finfo == null
                ? __NULLFILE
                : new _BasicPhysicalFile(finfo);
        }

        [return: NotNull]
        public static __XINFO ToIFileInfo(this __DINFO dinfo)
        {
            return dinfo == null
                ? __NULLFILE
                : new _BasicPhysicalDirectory(dinfo);
        }

        public static bool TryGetFileInfo(this __XINFO xinfo, out __FINFO finfo)
        {
            finfo = null;
            if (xinfo == null) return false;
            if (xinfo.IsDirectory) return false;

            switch(xinfo)
            {
                case _BasicPhysicalFile f: finfo = f.Info; return true;
                case IServiceProvider s:
                    finfo = s.GetService(typeof(__FINFO)) as __FINFO;
                    if (finfo != null) return true;
                    else break;
            }

            if (string.IsNullOrWhiteSpace(xinfo.PhysicalPath)) return false;            

            try
            {
                finfo = new __FINFO(xinfo.PhysicalPath);
                return true;
            }
            catch { return false; }
        }

        public static bool TryGetDirectoryInfo(this __XINFO xinfo, out __DINFO dinfo)
        {
            dinfo = null;
            if (xinfo == null) return false;
            if (!xinfo.IsDirectory) return false;

            switch (xinfo)
            {
                case _BasicPhysicalDirectory d: dinfo = d.Info; return true;
                case IServiceProvider s:
                    dinfo = s.GetService(typeof(__DINFO)) as __DINFO;
                    if (dinfo != null) return true;
                    else break;
            }

            if (string.IsNullOrWhiteSpace(xinfo.PhysicalPath)) return false;            

            try
            {
                dinfo = new __DINFO(xinfo.PhysicalPath);
                return true;
            }
            catch { return false; }
        }

        #endregion

        #region nested types

        [System.Diagnostics.DebuggerDisplay("{PhysicalPath}")]
        private readonly struct _BasicPhysicalFile : __XINFO , IServiceProvider
        {
            #region constructor
            public _BasicPhysicalFile(__FINFO finfo) { Info = finfo; }

            #endregion

            #region properties

            public __FINFO Info { get; }

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
                if (serviceType == typeof(__FINFO)) return Info;
                if (serviceType == typeof(__MATCHCASING)) return FileSystemPathCasing;
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
        private readonly struct _BasicPhysicalDirectory : __XINFO, IDirectoryContents , IServiceProvider
        {
            #region constructor

            public _BasicPhysicalDirectory(__DINFO dinfo) { Info = dinfo; }

            #endregion

            #region properties

            public __DINFO Info { get; }

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
            public IEnumerator<__XINFO> GetEnumerator()
            {
                return Info
                    .EnumerateFileSystemInfos()
                    .Select(ToIFileInfo)
                    .GetEnumerator();
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(__MATCHCASING)) return FileSystemPathComparison;
                if (serviceType == typeof(__DINFO)) return Info;

                return null;
            }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("{Info.FullPath}")]
        private readonly struct _BasicPhysicalDirectoryContents : IDirectoryContents, IServiceProvider
        {
            #region constructor

            public _BasicPhysicalDirectoryContents(__DINFO dinfo) { Info = dinfo; }

            #endregion

            #region properties

            public __DINFO Info { get; }

            public bool Exists => Info?.Exists ?? false;            

            #endregion

            #region API

            public Stream CreateReadStream() { throw new NotSupportedException(); }
            IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
            public IEnumerator<__XINFO> GetEnumerator()
            {
                return Info
                    .EnumerateFileSystemInfos()
                    .Select(ToIFileInfo)
                    .GetEnumerator();
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(__MATCHCASING)) return FileSystemPathCasing;
                if (serviceType == typeof(StringComparison)) return FileSystemPathComparison;
                if (serviceType == typeof(__DINFO)) return Info;

                return null;
            }

            #endregion
        }

        #endregion
    }
}
