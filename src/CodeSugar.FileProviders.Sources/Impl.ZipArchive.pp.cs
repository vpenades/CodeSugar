// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using System.Linq;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

using XFILE = Microsoft.Extensions.FileProviders.IFileInfo;
using XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;
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
        public static XFILE ToIFileInfo(this System.IO.Compression.ZipArchiveEntry entry)
        {
            return entry == null
                ? __NULLFILE
                : new _ZipArchiveFile(entry);
        }

        [return: NotNull]
        public static XPROVIDER ToIFileProvider(this System.IO.Compression.ZipArchive zip)
        {
            return zip == null
                ? __NULLPROVIDER
                : new _ZipArchive(zip);
        }

        #endregion

        #region nested types

        [System.Diagnostics.DebuggerDisplay("{Entry.FullName} {Length}")]
        private readonly struct _ZipArchiveFile : XFILE, IServiceProvider
        {
            #region lifecycle

            public _ZipArchiveFile(System.IO.Compression.ZipArchiveEntry entry)
            {
                System.Diagnostics.Debug.Assert(entry != null);
                Entry = entry;
            }

            #endregion

            #region data

            public System.IO.Compression.ZipArchiveEntry Entry { get; }

            #endregion

            #region properties

            public bool Exists => Entry != null;

            public long Length => Entry.Length;

            public string PhysicalPath => null;

            public string Name
            {
                get
                {
                    // https://github.com/dotnet/runtime/issues/1571#issuecomment-531148486

                    var name = Entry.FullName;
                    var idx = name.LastIndexOfAny(_ZipArchiveDirectory._ZipDirSeparators);
                    return idx < 0 ? name : name.Substring(idx + 1);
                }
            }

            public DateTimeOffset LastModified => Entry.LastWriteTime;

            public bool IsDirectory => false;

            #endregion

            #region API

            public Stream CreateReadStream()
            {
                if (Entry.Archive == null) throw new ObjectDisposedException(nameof(Entry));
                if (Entry.Archive.Mode == System.IO.Compression.ZipArchiveMode.Create) throw new System.IO.IOException("read is not supported");

                return Entry.Open();
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(MATCHCASING)) return MATCHCASING.CaseSensitive;
                if (serviceType == typeof(StringComparison)) return StringComparison.Ordinal;
                if (serviceType == typeof(System.IO.Compression.ZipArchiveEntry)) return Entry;
                if (serviceType == typeof(System.IO.Compression.ZipArchive)) return Entry.Archive;
                return null;
            }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("{_Path}")]
        private readonly struct _ZipArchiveDirectory : XFILE, IDirectoryContents
        {
            #region lifecycle
            public _ZipArchiveDirectory(_ZipArchive zip, string path)
            {                
                path = _SanitizedPath(path);

                System.Diagnostics.Debug.Assert(path == string.Empty || !path.StartsWith(ZipDirectorySeparator));
                System.Diagnostics.Debug.Assert(path == string.Empty || path.EndsWith(ZipDirectorySeparator));

                _Path = path.TrimEnd(ZipDirectorySeparator);

                _Zip = zip;
            }

            #endregion

            #region data

            private readonly _ZipArchive _Zip;
            private readonly string _Path;

            public static readonly char ZipDirectorySeparator = '/';
            public static readonly char ZipAltDirectorySeparator = '\\';

            public static readonly char[] _ZipDirSeparators = new char[] { ZipDirectorySeparator, ZipAltDirectorySeparator };

            #endregion

            #region properties            

            public bool Exists => true;

            public long Length => -1;

            public string PhysicalPath => null;

            public string Name
            {
                get
                {
                    var idx = _Path.LastIndexOfAny(_ZipDirSeparators);
                    return idx < 0 ? _Path : _Path.Substring(idx + 1);
                }
            }

            public DateTimeOffset LastModified => _Zip.Entries[0].LastWriteTime;            

            public bool IsDirectory => true;

            #endregion

            #region API

            public Stream CreateReadStream() { throw new NotSupportedException(); }

            IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

            public IEnumerator<XFILE> GetEnumerator()
            {
                var thisPath = _Path;
                if (thisPath.Length > 0) thisPath += ZipDirectorySeparator;
                return _EnumerateContents(_Zip, thisPath).GetEnumerator();
            }

            private static IEnumerable<XFILE> _EnumerateContents(_ZipArchive zip, string dirPath)
            {
                System.Diagnostics.Debug.Assert(dirPath == string.Empty || !dirPath.StartsWith(ZipDirectorySeparator));
                System.Diagnostics.Debug.Assert(dirPath == string.Empty || dirPath.EndsWith(ZipDirectorySeparator));

                HashSet<string> dirPaths = null;

                foreach (var entry in zip.Entries)
                {
                    var entryPath = _SanitizedPath(entry.FullName);                    

                    // check it's in the cone
                    if (!entryPath.StartsWith(dirPath)) continue;

                    // check if it's a file name entry
                    var entryName = entryPath.Substring(dirPath.Length);
                    if (!(entryName.Contains(ZipDirectorySeparator) || entryName.Contains(ZipAltDirectorySeparator)))
                    {                        
                        yield return new _ZipArchiveFile(entry);
                        continue;
                    }

                    // check if we can return a directory from a partial path
                    var idx = entryPath.IndexOf(ZipDirectorySeparator, dirPath.Length + 1);
                    if (idx <= 0) continue;
                    entryPath = entryPath.Substring(0, idx + 1);

                    // prevent returning the same directory more than once.
                    dirPaths ??= new HashSet<string>();
                    if (dirPaths.Contains(entryPath)) continue;
                    dirPaths.Add(entryPath);

                    // directory
                    yield return new _ZipArchiveDirectory(zip, entryPath);
                }
            }

            private static string _SanitizedPath(string path)
            {
                // in mac && linux both DirectorySeparatorChar and AltDirectorySeparatorChar are equal, so we cannot use them

                return path.Replace(ZipAltDirectorySeparator, ZipDirectorySeparator);
            }

            #endregion
        }        

        private readonly struct _ZipArchive : XPROVIDER
        {
            #region lifecycle
            public _ZipArchive(System.IO.Compression.ZipArchive archive)
            {
                _Archive = archive;
            }

            #endregion

            #region data

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly System.IO.Compression.ZipArchive _Archive;

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            public System.Collections.ObjectModel.ReadOnlyCollection<System.IO.Compression.ZipArchiveEntry> Entries => _Archive.Entries;

            #endregion

            #region API

            public IDirectoryContents GetDirectoryContents(string subpath)
            {
                return GetFileInfo(subpath) as IDirectoryContents
                    ?? NotFoundDirectoryContents.Singleton;
            }

            public XFILE GetFileInfo(string subpath)
            {
                subpath ??= string.Empty;

                // try with file:
                var entry = _Archive.GetEntry(subpath);
                if (entry != null) return new _ZipArchiveFile(entry);

                // try with directory:
                if (subpath.Length > 0 && !subpath.EndsWith(_ZipArchiveDirectory.ZipDirectorySeparator)) subpath += _ZipArchiveDirectory.ZipDirectorySeparator;
                var dir = new _ZipArchiveDirectory(this, subpath);

                return dir.Any()
                    ? dir
                    : (XFILE)new NotFoundFileInfo(subpath);
            }

            public IChangeToken Watch(string filter)
            {
                throw new NotImplementedException();
            }            

            #endregion
        }

        #endregion
    }
}
