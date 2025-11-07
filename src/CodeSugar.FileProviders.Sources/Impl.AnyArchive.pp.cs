// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using System.Linq;
using System.IO.Compression;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

#nullable disable

using __XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using __XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;
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

        /// <summary>
        /// Creates a lazily opened <see cref="__XPROVIDER"/> from a <see cref="ZipArchive"/>
        /// </summary>
        /// <param name="zipFactory">A lambda that will open a <see cref="ZipArchive"/> only when needed</param>
        /// <returns>A <see cref="__XPROVIDER"/>.</returns>
        [return: NotNull]
        public static __XPROVIDER ToIfileProvider(this Func<ZipArchive> zipFactory)
        {
            if (zipFactory == null) return __NULLPROVIDER;

            List<(string key, long len, DateTimeOffset offs)> toc = null;

            using (var zip = zipFactory.Invoke())
            {
                if (zip == null) return __NULLPROVIDER;

                toc = zip
                    .Entries
                    .Select(item => (item.FullName, item.Length, item.LastWriteTime))
                    .ToList();
            }

            System.IO.Stream _OpenKey(string key)
            {
                var m = new System.IO.MemoryStream();

                using (var zip = zipFactory.Invoke())
                {
                    using (var s = zip.GetEntry(key).Open())
                    {
                        s.CopyTo(m);
                    }
                }

                m.Position = 0;

                return m;
            }

            return ToIFileProvider(toc, _OpenKey);
        }

        /// <summary>
        /// Creates a single <see cref="__XINFO"/>.
        /// </summary>
        /// <param name="entry">a collection of file entries</param>
        /// <param name="openReadFunc">a lamda used to open the stream of a file entry.</param>
        /// <returns>An <see cref="__XINFO"/> representing the data</returns>
        [return: NotNull]
        public static __XINFO ToIFileInfo(this (string key, long len) entry, Func<string, System.IO.Stream> openReadFunc)
        {
            if (string.IsNullOrWhiteSpace(entry.key)) return __NULLFILE;

            var item = new _AnyArchiveEntry(entry.key, entry.len, DateTime.Now, openReadFunc);
                
            return new _AnyArchiveFile(item);
        }

        /// <summary>
        /// Creates a single <see cref="__XINFO"/>.
        /// </summary>
        /// <param name="entry">a collection of file entries</param>
        /// <param name="openReadFunc">a lamda used to open the stream of a file entry.</param>
        /// <returns>An <see cref="__XINFO"/> representing the data</returns>
        [return: NotNull]
        public static __XINFO ToIFileInfo(this (string key, long len, DateTimeOffset dt) entry, Func<string, System.IO.Stream> openReadFunc)
        {
            if (string.IsNullOrWhiteSpace(entry.key)) return __NULLFILE;

            var item = new _AnyArchiveEntry(entry.key, entry.len, entry.dt, openReadFunc);

            return new _AnyArchiveFile(item);
        }

        /// <summary>
        /// Creates a <see cref="__XPROVIDER"/> from a collection of file entries.
        /// </summary>
        /// <param name="entries">A collection of file entries representing a Table of Contents (TOC)</param>
        /// <param name="openReadFunc">a lamda used to open the stream of a file entry.</param>
        /// <returns>A <see cref="__XPROVIDER"/> containing all the entries.</returns>
        [return: NotNull]
        public static __XPROVIDER ToIFileProvider(this IEnumerable<(string key, long len)> entries, Func<string, System.IO.Stream> openReadFunc)
        {
            if (entries == null) return __NULLPROVIDER;

            DateTimeOffset dt = DateTime.Now;

            var items = entries
                .Select(item => new _AnyArchiveEntry(item.key, item.len, dt, openReadFunc))                
                .ToArray();

            if (items.Length == 0) return __NULLPROVIDER;

            return new _AnyArchive(items);
        }

        /// <summary>
        /// Creates a <see cref="__XPROVIDER"/> from a collection of file entries.
        /// </summary>
        /// <param name="entries">A collection of file entries representing a Table of Contents (TOC)</param>
        /// <param name="openReadFunc">a lamda used to open the stream of a file entry.</param>
        /// <returns>A <see cref="__XPROVIDER"/> containing all the entries.</returns>
        [return: NotNull]
        public static __XPROVIDER ToIFileProvider(this IEnumerable<(string key, long len, DateTimeOffset dt)> entries, Func<string, System.IO.Stream> openReadFunc)
        {
            if (entries == null) return __NULLPROVIDER;

            var items = entries
                .Select(item => new _AnyArchiveEntry(item.key, item.len, item.dt, openReadFunc))                
                .ToArray();

            if (items.Length == 0) return __NULLPROVIDER;

            return new _AnyArchive(items);
        }

        #endregion

        #region nested types

        [System.Diagnostics.DebuggerDisplay("{Key} {Length}")]
        private readonly struct _AnyArchiveFile : __XINFO, IServiceProvider
        {
            #region lifecycle           

            public _AnyArchiveFile(_IArchiveEntry entry)
            {
                System.Diagnostics.Debug.Assert(entry != null);
                _Entry = entry;                
            }

            #endregion

            #region data

            private readonly _IArchiveEntry _Entry;            

            #endregion

            #region properties

            public string Key => _Entry.Key;

            public long Length => _Entry.Length;

            public DateTimeOffset LastModified => _Entry.LastModified;

            public bool Exists => !string.IsNullOrWhiteSpace(Key);

            public string PhysicalPath => null;

            public string Name
            {
                get
                {
                    // https://github.com/dotnet/runtime/issues/1571#issuecomment-531148486

                    var name = Key;
                    var idx = name.LastIndexOfAny(_AnyArchiveDirectory._ArchiveDirSeparators);
                    return idx < 0 ? name : name.Substring(idx + 1);
                }
            }

            public bool IsDirectory => false;

            #endregion

            #region API

            public Stream CreateReadStream()
            {
                if (!Exists) return null;

                return _Entry.OpenRead();
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(__MATCHCASING)) return __MATCHCASING.CaseSensitive;
                if (serviceType == typeof(StringComparison)) return StringComparison.Ordinal;                
                return null;
            }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("{_Path}")]
        private readonly struct _AnyArchiveDirectory : __XINFO, IDirectoryContents
        {
            #region lifecycle
            public _AnyArchiveDirectory(_AnyArchive arch, string path)
            {
                path = _SanitizedPath(path);

                System.Diagnostics.Debug.Assert(path == string.Empty || !path.StartsWith(ArchiveDirectorySeparator));
                System.Diagnostics.Debug.Assert(path == string.Empty || path.EndsWith(ArchiveDirectorySeparator));

                _Path = path.TrimEnd(ArchiveDirectorySeparator);

                _Archive = arch;
            }

            #endregion

            #region data

            private readonly _AnyArchive _Archive;
            private readonly string _Path;

            public static readonly char ArchiveDirectorySeparator = '/';
            public static readonly char ArchiveAltDirectorySeparator = '\\';

            public static readonly char[] _ArchiveDirSeparators = new char[] { ArchiveDirectorySeparator, ArchiveAltDirectorySeparator };

            #endregion

            #region properties            

            public bool Exists => true;

            public long Length => -1;

            public string PhysicalPath => null;

            public string Name
            {
                get
                {
                    var idx = _Path.LastIndexOfAny(_ArchiveDirSeparators);
                    return idx < 0 ? _Path : _Path.Substring(idx + 1);
                }
            }

            public DateTimeOffset LastModified => _Archive.Entries.FirstOrDefault().LastModified;

            public bool IsDirectory => true;

            #endregion

            #region API

            public Stream CreateReadStream() { throw new NotSupportedException(); }

            IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

            public IEnumerator<__XINFO> GetEnumerator()
            {
                var thisPath = _Path;
                if (thisPath.Length > 0) thisPath += ArchiveDirectorySeparator;
                return _EnumerateContents(_Archive, thisPath).GetEnumerator();
            }

            private static IEnumerable<__XINFO> _EnumerateContents(_AnyArchive arch, string dirPath)
            {
                System.Diagnostics.Debug.Assert(dirPath == string.Empty || !dirPath.StartsWith(ArchiveDirectorySeparator));
                System.Diagnostics.Debug.Assert(dirPath == string.Empty || dirPath.EndsWith(ArchiveDirectorySeparator));

                HashSet<string> dirPaths = null;

                foreach (var entry in arch.Entries)
                {
                    var entryPath = _SanitizedPath(entry.Key);

                    // check it's in the cone
                    if (!entryPath.StartsWith(dirPath)) continue;

                    // check if it's a file name entry
                    var entryName = entryPath.Substring(dirPath.Length);
                    if (!(entryName.Contains(ArchiveDirectorySeparator) || entryName.Contains(ArchiveAltDirectorySeparator)))
                    {
                        yield return new _AnyArchiveFile(entry);
                        continue;
                    }

                    // check if we can return a directory from a partial path
                    var idx = entryPath.IndexOf(ArchiveDirectorySeparator, dirPath.Length + 1);
                    if (idx <= 0) continue;
                    entryPath = entryPath.Substring(0, idx + 1);

                    // prevent returning the same directory more than once.
                    dirPaths ??= new HashSet<string>();
                    if (dirPaths.Contains(entryPath)) continue;
                    dirPaths.Add(entryPath);

                    // directory
                    yield return new _AnyArchiveDirectory(arch, entryPath);
                }
            }

            private static string _SanitizedPath(string path)
            {
                // in mac && linux both DirectorySeparatorChar and AltDirectorySeparatorChar are equal, so we cannot use them

                return path.Replace(ArchiveAltDirectorySeparator, ArchiveDirectorySeparator);
            }

            #endregion
        }

        private sealed class _AnyArchive : __XPROVIDER
        {
            #region lifecycle
            public _AnyArchive(_AnyArchiveEntry[] entries)
            {
                _TableOfContents = entries;                
            }

            #endregion

            #region data

            // we must keep the full table of contents here to keep the original keys in a flat list
            // because some archives may not have a clear way to use separators.

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly _AnyArchiveEntry[] _TableOfContents;            

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            public IReadOnlyList<_AnyArchiveEntry> Entries => _TableOfContents;

            #endregion

            #region API

            public IDirectoryContents GetDirectoryContents(string subpath)
            {
                return GetFileInfo(subpath) as IDirectoryContents
                    ?? NotFoundDirectoryContents.Singleton;
            }

            public __XINFO GetFileInfo(string subpath)
            {
                subpath ??= string.Empty;

                // try with file:
                var entry = _TableOfContents.FirstOrDefault(item => item.Key == subpath);
                if (entry != null) return new _AnyArchiveFile(entry);

                // try with directory:
                if (subpath.Length > 0 && !subpath.EndsWith(_AnyArchiveDirectory.ArchiveDirectorySeparator))
                {
                    subpath += _AnyArchiveDirectory.ArchiveDirectorySeparator;
                }

                var dir = new _AnyArchiveDirectory(this, subpath);

                return dir.Any()
                    ? dir
                    : (__XINFO)new NotFoundFileInfo(subpath);
            }

            public IChangeToken Watch(string filter)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        private interface _IArchiveEntry
        {
            string Key { get; }
            long Length { get; }
            DateTimeOffset LastModified { get; }
            System.IO.Stream OpenRead();
        }

        [System.Diagnostics.DebuggerDisplay("{Key} {Length} {LastModified}")]
        private sealed class _AnyArchiveEntry : _IArchiveEntry
        {
            #region lifecycle

            public _AnyArchiveEntry(string key, long length, DateTimeOffset lastModified, Func<string, System.IO.Stream> openReadFunc)
            {
                Key = key;
                Length = length;
                LastModified = lastModified;
                _OpenReadFunc = openReadFunc;
            }

            #endregion

            #region data
            public string Key { get; }
            public long Length { get; }
            public DateTimeOffset LastModified { get; }

            private readonly Func<string, System.IO.Stream> _OpenReadFunc;

            #endregion

            #region API

            public System.IO.Stream OpenRead()
            {
                var s = _OpenReadFunc.Invoke(Key);

                #if DEBUG
                if (s is System.IO.MemoryStream ms)
                {
                    System.Diagnostics.Debug.Assert(ms.Length == this.Length);
                }
                #endif

                return s;
            }

            #endregion
        }

        #endregion
    }
}
