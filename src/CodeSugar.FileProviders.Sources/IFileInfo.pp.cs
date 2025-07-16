// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.FileProviders;

#nullable disable

using _XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using _XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;
using _XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;


#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForFileProviders
    {
        #region constants

        private static readonly _XINFO __NULLFILE = new Microsoft.Extensions.FileProviders.NotFoundFileInfo("NULL");

        private static readonly _XPROVIDER __NULLPROVIDER = new Microsoft.Extensions.FileProviders.NullFileProvider();

        #endregion

        #region wrappers

        [return: NotNull]
        public static _XINFO ToIFileInfo(this _XDIRECTORY dir, string name)
        {
            switch (dir)
            {
                case null: return new NotFoundFileInfo(name);
                case _XINFO xinfo:
                    if (!string.IsNullOrWhiteSpace(name) && name != xinfo.Name) throw new ArgumentException($"Name mismatch. Expected {xinfo.Name} but was {name}");
                    return xinfo;
                default:
                    if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
                    return new _DirectoryFile(name, dir);
            }
        }

        [return: NotNull]
        public static _XINFO ToIFileInfo([NotNull] this Func<System.IO.Stream> reader, [NotNull] string name, long len, DateTimeOffset lastWrite)
        {
            return new _GenericFileInfo(name, len, lastWrite, reader);
        }

        [return: NotNull]
        public static _XINFO ToIFileInfo([NotNull] this Func<string, System.IO.Stream> reader, [NotNull] string name, long len, DateTimeOffset lastWrite)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            return new _GenericFileInfo(name, len, lastWrite, ()=> reader.Invoke(name));
        }        

        [return: NotNull]
        public static _XINFO ToIFileInfo([NotNull] this Func<System.IO.Stream> reader, [NotNull] string name, long len)
        {
            return new _GenericFileInfo(name, len, DateTime.Today, reader);
        }

        [return: NotNull]
        public static _XINFO ToIFileInfo([NotNull] this Func<string, System.IO.Stream> reader, [NotNull] string name, long len)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            return new _GenericFileInfo(name, len, DateTime.Today, () => reader.Invoke(name));
        }

        [return: NotNull]
        public static _XINFO ToIFileInfo(this ArraySegment<byte> data, [NotNull] string name, DateTimeOffset lastWrite)
        {
            return new _GenericFileInfo(name, data.Count, lastWrite, () => _ToMemoryStream(data));
        }

        [return: NotNull]
        public static _XINFO ToIFileInfo(this ArraySegment<byte> data, [NotNull] string name)
        {
            return new _GenericFileInfo(name, data.Count, DateTime.Today, () => _ToMemoryStream(data));
        }

        #endregion

        #region stream functions

        [return: NotNull]
        public static Func<System.IO.Stream> GetReadStreamFunction([NotNull] this _XINFO xinfo)
        {
            GuardNotNull(xinfo);
            if (xinfo.IsDirectory) throw new ArgumentException("directories don't have a stream", nameof(xinfo));

            return xinfo.CreateReadStream;
        }

        [return: NotNull]
        public static Func<System.IO.Stream> GetWriteStreamFunction([NotNull] this _XINFO xinfo)
        {
            GuardNotNull(xinfo);
            if (xinfo.IsDirectory) throw new ArgumentException("directories don't have a stream", nameof(xinfo));

            if (!string.IsNullOrWhiteSpace(xinfo.PhysicalPath))
            {
                return new System.IO.FileInfo(xinfo.PhysicalPath).Create;
            }

            if (xinfo is IServiceProvider srv) // lambdas
            {
                // this one has the advantage of updating the internal FileInfo's state. (.Exists and .Length)
                if (srv.GetService(typeof(Func<System.IO.FileInfo>)) is System.IO.FileInfo finfo)
                {
                    return finfo.Create;
                }

                // file system writer
                if (srv.GetService(typeof(Func<FileMode, System.IO.Stream>)) is Func<FileMode, System.IO.Stream> lambda0)
                {
                    return () => lambda0.Invoke(FileMode.Create);
                }                

                // poor man's writer
                if (srv.GetService(typeof(Action<ArraySegment<Byte>>)) is Action<ArraySegment<Byte>> lambda1)
                {
                    return ()=> new _ObservableMemoryStream(lambda1);
                }                
            }

            return ()=> null;
        }

        #endregion

        #region nested types

        [System.Diagnostics.DebuggerDisplay("{Name} {Length}")]
        class _GenericFileInfo : _XINFO
        {
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

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly Func<System.IO.Stream> _Reader;            

            public Stream CreateReadStream() { return _Reader.Invoke(); }

            public bool Exists => true;

            public long Length { get; }

            public string PhysicalPath => null;

            public string Name { get; }

            public DateTimeOffset LastModified { get; }

            public bool IsDirectory => false;
        }

        private sealed class _DirectoryFile : _XINFO, _XDIRECTORY
        {
            #region lifecycle
            public _DirectoryFile(string name, _XDIRECTORY source)
            {
                _Source = source;
                Name = name;
            }

            #endregion

            #region data

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            private readonly _XDIRECTORY _Source;

            #endregion

            #region properties

            public bool Exists => true;

            public long Length => -1;

            public string PhysicalPath => null;

            public string Name { get; }

            public DateTimeOffset LastModified => DateTime.Today;

            public bool IsDirectory => true;

            #endregion

            #region API

            public IEnumerator<_XINFO> GetEnumerator()
            {
                return _Source.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _Source.GetEnumerator();
            }

            public Stream CreateReadStream()
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion
    }
}
