using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.FileProviders;

#nullable disable

using __XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using __XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;
using __XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;


namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        #region constants

        private static readonly __XINFO __NULLFILE = new Microsoft.Extensions.FileProviders.NotFoundFileInfo("NULL");

        private static readonly __XPROVIDER __NULLPROVIDER = new Microsoft.Extensions.FileProviders.NullFileProvider();

        #endregion

        #region wrappers

        [return: NotNull]
        public static __XINFO ToIFileInfo(this __XDIRECTORY dir, string fallbackName)
        {
            switch (dir)
            {
                case null: return new NotFoundFileInfo(fallbackName);
                case __XINFO xinfo: return xinfo;
                default:
                    if (string.IsNullOrWhiteSpace(fallbackName)) throw new ArgumentNullException(nameof(fallbackName));
                    return new _GenericDirectoryInfo(fallbackName, dir);
            }
        }

        [return: NotNull]
        public static __XINFO ToIFileInfo([NotNull] this Func<System.IO.Stream> reader, [NotNull] string name, long len, DateTimeOffset lastWrite)
        {
            return new _GenericFileInfo(name, len, lastWrite, reader);
        }

        [return: NotNull]
        public static __XINFO ToIFileInfo([NotNull] this Func<string, System.IO.Stream> reader, [NotNull] string name, long len, DateTimeOffset lastWrite)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            return new _GenericFileInfo(name, len, lastWrite, ()=> reader.Invoke(name));
        }        

        [return: NotNull]
        public static __XINFO ToIFileInfo([NotNull] this Func<System.IO.Stream> reader, [NotNull] string name, long len)
        {
            return new _GenericFileInfo(name, len, DateTime.Today, reader);
        }

        [return: NotNull]
        public static __XINFO ToIFileInfo([NotNull] this Func<string, System.IO.Stream> reader, [NotNull] string name, long len)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            return new _GenericFileInfo(name, len, DateTime.Today, () => reader.Invoke(name));
        }

        [return: NotNull]
        public static __XINFO ToIFileInfo(this ArraySegment<byte> data, [NotNull] string name, DateTimeOffset lastWrite)
        {
            return new _GenericFileInfo(name, data.Count, lastWrite, () => _ToMemoryStream(data));
        }

        [return: NotNull]
        public static __XINFO ToIFileInfo(this ArraySegment<byte> data, [NotNull] string name)
        {
            return new _GenericFileInfo(name, data.Count, DateTime.Today, () => _ToMemoryStream(data));
        }

        #endregion

        #region stream functions

        [return: NotNull]
        public static Func<System.IO.Stream> GetReadStreamFunction([NotNull] this __XINFO xinfo)
        {
            GuardNotNull(xinfo);
            if (xinfo.IsDirectory) throw new ArgumentException("directories don't have a stream", nameof(xinfo));

            return xinfo.CreateReadStream;
        }

        [return: NotNull]
        public static Func<System.IO.Stream> GetWriteStreamFunction([NotNull] this __XINFO xinfo)
        {
            GuardNotNull(xinfo);
            if (xinfo.IsDirectory) throw new ArgumentException("directories don't have a stream", nameof(xinfo));

            // if we can get the internal FileInfo, use it over PhysicalPath because we can update
            // it after writing, which will also update the public properties of the IFileInfo.
            if (TryGetInternalFileInfo(xinfo, out var finfo)) 
            {
                return GetWriteStreamFunction(finfo, true); // true to update after finished writing
            }

            // use the physical path.
            if (!string.IsNullOrWhiteSpace(xinfo.PhysicalPath))
            {
                finfo = new System.IO.FileInfo(xinfo.PhysicalPath);
                return GetWriteStreamFunction(finfo, false); // false because there's nothing to update
            }

            // try get the open stream lambdas
            if (xinfo is IServiceProvider srv2)
            {
                // file system writer
                if (srv2.GetService(typeof(Func<FileMode, System.IO.Stream>)) is Func<FileMode, System.IO.Stream> lambda0)
                {
                    return () => lambda0.Invoke(FileMode.Create);
                }                

                // WriteAllBytes
                if (srv2.GetService(typeof(Action<ArraySegment<Byte>>)) is Action<ArraySegment<Byte>> lambda1)
                {
                    return ()=> new _ObservableMemoryStream(lambda1);
                }                
            }

            return ()=> null;
        }

        #endregion

        #region nested types

        [System.Diagnostics.DebuggerDisplay("{GenericFileInfo} {Name} {Length}")]
        sealed class _GenericFileInfo : __XINFO
        {
            #region lifecycle
            public _GenericFileInfo(string name, long length, DateTimeOffset lastModified, Func<Stream> reader)
            {
                if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
                if (name.Contains(System.IO.Path.AltDirectorySeparatorChar)) throw new ArgumentException(nameof(name));

                if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
                if (reader == null) throw new ArgumentNullException(nameof(reader));

                Name = name;                
                Length = length;                
                LastModified = lastModified;
                _Reader = reader;
            }
            #endregion

            #region data

            public string Name { get; }
            public long Length { get; } // ToDo this should be another lambda.
            public DateTimeOffset LastModified { get; }

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly Func<System.IO.Stream> _Reader;

            #endregion

            #region properties

            public bool Exists => true;

            public string PhysicalPath => null;

            public bool IsDirectory => false;

            #endregion

            #region API

            public Stream CreateReadStream() { return _Reader.Invoke(); }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("GenericDirectoryInfo {Name,nq}")]
        sealed class _GenericDirectoryInfo : __XINFO, __XDIRECTORY
        {
            #region lifecycle
            public _GenericDirectoryInfo(string name, __XDIRECTORY source)
            {
                _Source = source;
                Name = name;
            }

            #endregion

            #region data

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            private readonly __XDIRECTORY _Source;

            #endregion

            #region properties

            public bool Exists => true;

            long __XINFO.Length => -1;

            string __XINFO.PhysicalPath => null;

            public string Name { get; }

            public DateTimeOffset LastModified => DateTime.Today;

            public bool IsDirectory => true;

            #endregion

            #region API

            public IEnumerator<__XINFO> GetEnumerator()
            {
                return _Source.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _Source.GetEnumerator();
            }

            Stream __XINFO.CreateReadStream()
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion
    }
}
