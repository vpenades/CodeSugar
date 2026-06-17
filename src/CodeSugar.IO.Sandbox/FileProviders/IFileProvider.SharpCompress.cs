// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#if __REFERENCES_SHARPCOMPRESS

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

using SharpCompress.Archives;
using SharpCompress.Readers;

#nullable disable

using __FINFO = System.IO.FileInfo;
using __XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using __MATCHCASING = System.IO.MatchCasing;

using __PROGRESS = System.IProgress<int>;
using __LOGGER = System.IProgress<string>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        #region API

        public static bool TryLoadSharpCompressArchive([DisallowNull] this __FINFO finfo, [AllowNull] SharpCompress.Readers.ReaderOptions options, [AllowNull] __LOGGER logger, [NotNullWhen(true)] out IFileProvider provider)
        {
            provider = _SharpCompressProvider.TryCreate(finfo, options, logger);
            return provider != null;
        }
        
        public static async Task<IFileProvider> TryLoadSharpCompressArchiveAsync([DisallowNull] this __FINFO finfo, [AllowNull] SharpCompress.Readers.ReaderOptions options, [AllowNull] __LOGGER logger)
        {
            return await _SharpCompressProvider.TryCreateAsync(finfo, options, logger);
        }

        #endregion

        #region nested types

        /// <summary>
        /// Represents a <see cref="IFileProvider"/> over a <see cref="SharpCompress.Archives.IArchive"/>
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("_SharpCompressProvider {_ArchiveName,nq}")]
        sealed class _SharpCompressProvider : IFileProvider, IServiceProvider
        {
            #region lifecycle        

            public object GetService(Type serviceType)
            {
                if (typeof(__MATCHCASING) == serviceType) return __MATCHCASING.CaseSensitive;
                return null;
            }            

            public static _SharpCompressProvider TryCreate([DisallowNull] __FINFO finfo, SharpCompress.Readers.ReaderOptions options, __LOGGER logger)
            {
                if (finfo == null || !finfo.Exists) return null;

                try
                {
                    var entries = _SharpCompressEntry.ReadEntries(finfo, options, logger);

                    return new _SharpCompressProvider(entries, finfo.FullName);
                }                
                catch (SharpCompress.Common.ArchiveException ex) { _LogError(logger, ex); } // this happens when the archive has errors
                catch (SharpCompress.Common.CryptographicException ex) { _LogError(logger, ex); }
                catch (System.ArgumentNullException ex) { _LogError(logger, ex); }
                catch (System.InvalidOperationException ex) { _LogError(logger, ex); }
                catch (System.IndexOutOfRangeException ex) { _LogError(logger, ex); } // in multizip files, this happens when the 2nd file is missing

                return null;
            }

            public static async Task<_SharpCompressProvider> TryCreateAsync([DisallowNull] __FINFO finfo, SharpCompress.Readers.ReaderOptions options, __LOGGER logger)
            {
                if (finfo == null || !finfo.Exists) return null;

                try
                {
                    var entries = await _SharpCompressEntry.ReadEntriesAsync(finfo, options, logger);

                    return new _SharpCompressProvider(entries, finfo.FullName);
                }
                catch (SharpCompress.Common.ArchiveException ex) { _LogError(logger, ex); } // this happens when the archive has errors
                catch (SharpCompress.Common.CryptographicException ex) { _LogError(logger, ex); }
                catch (System.ArgumentNullException ex) { _LogError(logger, ex); }
                catch (System.InvalidOperationException ex) { _LogError(logger, ex); }
                catch (System.IndexOutOfRangeException ex) { _LogError(logger, ex); } // in multizip files, this happens when the 2nd file is missing

                return null;
            }

            private static void _LogError(__LOGGER logger, Exception ex)
            {
                // logger?.LogInformation(finfo.FullName);
                // logger?.LogCritical(ex, ex.Message);
            }

            private _SharpCompressProvider(IEnumerable<_SharpCompressEntry> entries, string archName)
            {
                #if NET10_0_OR_GREATER
                var comparer = StringComparer.Create(System.Globalization.CultureInfo.InvariantCulture, System.Globalization.CompareOptions.NumericOrdering);
                #else
                var comparer = StringComparer.Create(System.Globalization.CultureInfo.InvariantCulture, System.Globalization.CompareOptions.OrdinalIgnoreCase);
                #endif

                _ArchiveName = archName;

                _Files = entries
                    .OrderBy(item => item.Name, comparer)
                    .ToList();
            }

            #endregion

            #region data

            private readonly string _ArchiveName;
            private readonly IReadOnlyList<_SharpCompressEntry> _Files;

            #endregion

            #region properties

            public IReadOnlyList<__XINFO> Files => _Files;

            #endregion

            #region API        

            public async Task PreloadAsync(__XINFO file)
            {
                if (file is _SharpCompressRandomEntry entry)
                {
                    await entry.PreloadAsync();
                }
            }            

            #endregion

            #region fileprovider API

            public __XINFO GetFileInfo(string subpath)
            {
                subpath = subpath.Replace('\\', '/');

                var xfile = _Files.FirstOrDefault(item => item.Key.Replace('\\', '/') == subpath);
                if (xfile is __XINFO xinfo) return xinfo;

                var name = System.IO.Path.GetFileName(subpath.TrimEnd('/').TrimEnd('\\'));

                return GetDirectoryContents(subpath).ToIFileInfo(name);
            }

            public IDirectoryContents GetDirectoryContents(string subpath)
            {
                return _Files.ToIDirectoryContents(entry => entry.Key, subpath, MatchCasing.CaseSensitive);
            }

            IChangeToken IFileProvider.Watch(string filter)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        /// <summary>
        /// Represents a file entry from a <see cref="SharpCompress"/> archive.
        /// </summary>
        /// <remarks>
        /// Derived classes are:<br/>
        /// <see cref="_SharpCompressDirectory"/><br/>
        /// <see cref="_SharpCompressRandomEntry"/><br/>
        /// <see cref="_SharpCompressCachedEntry"/><br/>
        /// </remarks>
        abstract class _SharpCompressEntry : __XINFO
        {
            #region lifecycle

            public static IReadOnlyList<_SharpCompressEntry> EmptyArray { get; } = Array.Empty<_SharpCompressEntry>();

            public static IReadOnlyList<_SharpCompressEntry> ReadEntries(__FINFO finfo, SharpCompress.Readers.ReaderOptions options, __LOGGER logger)
            {
                if (finfo == null || !finfo.Exists) return EmptyArray;

                using var arch = SharpCompress.Archives.ArchiveFactory.OpenArchive(finfo.FullName, options);                
                if (arch == null) throw new InvalidOperationException("Could not open archive");

                if (arch.IsEncrypted && string.IsNullOrEmpty(options?.Password)) throw new ArgumentNullException("Password required", nameof(options));

                return arch.IsSolid
                    ? _SharpCompressCachedEntry.ReadEntries(finfo, arch)
                    : _SharpCompressRandomEntry.ReadEntries(finfo, options, arch);
            }

            public static async Task<IReadOnlyList<_SharpCompressEntry>> ReadEntriesAsync(__FINFO finfo, SharpCompress.Readers.ReaderOptions options, __LOGGER logger)
            {
                if (finfo == null || !finfo.Exists) return EmptyArray;
                
                await using var arch = await SharpCompress.Archives.ArchiveFactory.OpenAsyncArchive(finfo.FullName, options);
                if (arch == null) throw new InvalidOperationException("Could not open archive");

                if (await arch.IsEncryptedAsync() && string.IsNullOrEmpty(options?.Password)) throw new ArgumentNullException("Password required", nameof(options));

                return await arch.IsSolidAsync()
                    ? await _SharpCompressCachedEntry.ReadEntriesAsync(finfo, arch)
                    : await _SharpCompressRandomEntry.ReadEntriesAsync(finfo, options, arch);
            }

            protected _SharpCompressEntry(string key, bool isDirectory)
            {
                this.Key = key;
                this.IsDirectory = isDirectory;
            }

            #endregion

            #region data

            public string Key { get; }

            public bool IsDirectory { get; }

            #endregion

            #region properties

            public string Name => System.IO.Path.GetFileName(Key.TrimEnd('/', '\\'));
            public bool Exists => true;
            public long Length { get; protected set; }
            public DateTimeOffset LastModified { get; private set; }

            string __XINFO.PhysicalPath => null;

            #endregion

            #region API

            protected void SetLastModifiedTime(DateTime? entryTime)
            {
                // https://github.com/Azure/azure-webjobs-sdk/issues/2957
                // https://github.com/Azure/azure-webjobs-sdk/issues/2959
                var date = entryTime ?? DateTime.Now;
                if (date == DateTime.MinValue) date = DateTime.Now; // for some reason, DateTime.Min cannot be converted to DateTimeOffset

                LastModified = date;
            }

            public bool IsInDirectory(string dirPath)
            {
                if (!dirPath.EndsWith("/")) dirPath += "/";
                dirPath = dirPath.Replace("\\", "/");

                var key = Key.Replace("\\", "/");

                if (!key.StartsWith(dirPath)) return false;

                key = key.Substring(dirPath.Length);

                if (key.Contains('/')) return false;

                return true;
            }            

            public virtual Stream CreateReadStream() { throw new NotImplementedException(); }            

            #endregion
        }

        /// <summary>
        /// Represents a directory within the archive.
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("_SharpCompressDummyDir {Name}")]
        sealed class _SharpCompressDirectory : _SharpCompressEntry , __XINFO
        {
            public _SharpCompressDirectory(string key) : base(key, true) { }                        
        }

        /// <summary>
        /// Archive context for <see cref="_SharpCompressRandomEntry"/>
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("_SharpCompressRandomArchive {_Archive.FullName}")]
        sealed class _SharpCompressRandomArchive
        {
            #region lifecycle

            public _SharpCompressRandomArchive(__FINFO archive, ReaderOptions options, bool useCache)
            {
                _Archive = archive;
                _Options = options;
                
                _UseCache = useCache;
            }

            #endregion

            #region data

            private readonly __FINFO _Archive;

            private readonly SharpCompress.Readers.ReaderOptions _Options;

            private readonly Object _Mutex = new object();

            private readonly bool _UseCache;

            #endregion

            #region API

            public System.IO.MemoryStream CreateMemoryStream(string key)
            {
                lock (_Mutex)
                {
                    using (var arch = SharpCompress.Archives.ArchiveFactory.OpenArchive(_Archive.FullName, _Options))
                    {
                        var entry = arch.Entries.FirstOrDefault(item => item.Key == key);
                        if (entry == null) return null;

                        var m = new MemoryStream();

                        using (var s = entry.OpenEntryStream())
                        {
                            s.CopyTo(m);
                        }                        

                        m.Position = 0;
                        return m;
                    }
                }
            }

            public System.IO.Stream CreateDirectStream(string key)
            {
                var arch = SharpCompress.Archives.ArchiveFactory.OpenArchive(_Archive.FullName, _Options);
                
                var entry = arch.Entries.FirstOrDefault(item => item.Key == key);
                var stream = entry?.OpenEntryStream();
                if (stream == null)
                {
                    arch.Dispose();
                    return null;
                }

                return stream.WithDisposeObserver(() => arch.Dispose());
            }

            #endregion
        }

        /// <summary>
        /// Represents an entry of an closed archive.
        /// </summary>
        /// <remarks>
        /// It also has weak content cache support (may be counterproductive if we know the entry is read only once)
        /// </remarks>
        [System.Diagnostics.DebuggerDisplay("_SharpCompressRandomEntry {Name}")]
        sealed class _SharpCompressRandomEntry : _SharpCompressEntry, IEquatable<_SharpCompressRandomEntry>
        {
            #region lifecycle                

            internal static IReadOnlyList<_SharpCompressEntry> ReadEntries(__FINFO finfo, SharpCompress.Readers.ReaderOptions options, IArchive arch)
            {
                var context = new _SharpCompressRandomArchive(finfo, options, true);
                var entries = new List<_SharpCompressEntry>();

                foreach (var entry in arch.Entries)
                {
                    entries.Add(_ProcessEntry(context, entry));                    
                }                

                return entries;
            }

            internal static async Task<IReadOnlyList<_SharpCompressEntry>> ReadEntriesAsync(__FINFO finfo, SharpCompress.Readers.ReaderOptions options, IAsyncArchive arch)
            {
                var context = new _SharpCompressRandomArchive(finfo, options, true);
                var entries = new List<_SharpCompressEntry>();

                await foreach (var entry in arch.EntriesAsync)
                {
                    entries.Add(_ProcessEntry(context, entry));                    
                }                

                return entries;
            }

            private static _SharpCompressEntry _ProcessEntry(_SharpCompressRandomArchive context, IArchiveEntry entry)
            {
                return entry.IsDirectory
                    ? (_SharpCompressEntry)new _SharpCompressDirectory(entry.Key!)
                    : new _SharpCompressRandomEntry(context, entry);
            }

            private _SharpCompressRandomEntry(_SharpCompressRandomArchive context, IArchiveEntry entry)
                : base(entry.Key!, entry.IsDirectory)
            {
                _Context = context;
                Length = entry.Size;
                SetLastModifiedTime(entry.LastModifiedTime);
            }

            #endregion

            #region data
            private readonly _SharpCompressRandomArchive _Context;

            private const int _CacheMaxSize = int.MaxValue / 2;

            private WeakReference<Byte[]> _WeakContent;

            public override int GetHashCode()
            {
                var kh = Key.GetHashCode(StringComparison.Ordinal);
                return HashCode.Combine(kh, _Context);
            }

            public override bool Equals(object obj)
            {
                return obj is _SharpCompressRandomEntry other && Equals(other);
            }

            public bool Equals(_SharpCompressRandomEntry other)
            {
                if (other == null) return false;
                if (string.Equals(this.Key, other.Key, StringComparison.Ordinal)) return false;
                if (this._Context != other._Context) return false;
                return true;
            }

            #endregion

            #region API            

            public async Task PreloadAsync()
            {
                await Task.Run(Preload);
            }

            public void Preload()
            {
                using (var tmp = CreateReadStream()) { }
            }

            internal void PreloadFrom(SharpCompress.Readers.IReader reader)
            {
                using var m = _Context.CreateMemoryStream(Key);

                _WeakContent = new WeakReference<byte[]>(m.ToArray());
            }

            public override Stream CreateReadStream()
            {
                if (this.Length >= _CacheMaxSize)
                {
                    return _Context.CreateDirectStream(Key);
                }

                if (_WeakContent != null && _WeakContent.TryGetTarget(out var strongContent))
                {
                    return new MemoryStream(strongContent, false);
                }

                _WeakContent = null;

                using var m = _Context.CreateMemoryStream(Key);

                _WeakContent = new WeakReference<byte[]>(m.ToArray());

                m.Position = 0;
                return m;
            }

            #endregion
        }

        /// <summary>
        /// An archive entry that hard caches the content of the archive in memory
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("_SharpCompressCachedEntry {Name}")]
        sealed class _SharpCompressCachedEntry : _SharpCompressEntry, IEquatable<_SharpCompressCachedEntry>
        {
            #region lifecycle                

            internal static IReadOnlyList<_SharpCompressEntry> ReadEntries(__FINFO finfo, IArchive arch)
            {
                var entries = new List<_SharpCompressEntry>();

                using (var reader = arch.ExtractAllEntries())
                {
                    var dirs = new HashSet<string>();

                    while (reader.MoveToNextEntry())
                    {
                        _HandleDirectoryFound(entries, reader.Entry.Key, reader.Entry.IsDirectory, dirs);

                        if (!reader.Entry.IsDirectory)
                        {
                            var entry = CreateFromCurrent(reader);
                            if (entry != null) entries.Add(entry);
                        }
                    }
                }                

                return entries;
            }

            internal static async Task<IReadOnlyList<_SharpCompressEntry>> ReadEntriesAsync(__FINFO finfo, IAsyncArchive arch)
            {
                var entries = new List<_SharpCompressEntry>();

                await using var reader = await arch.ExtractAllEntriesAsync();

                var dirs = new HashSet<string>();

                while (await reader.MoveToNextEntryAsync())
                {
                    _HandleDirectoryFound(entries, reader.Entry.Key, reader.Entry.IsDirectory, dirs);

                    if (!reader.Entry.IsDirectory)
                    {
                        var entry = await CreateFromCurrentAsync(reader);
                        if (entry != null) entries.Add(entry);
                    }
                }                

                return entries;
            }

            private static void _HandleDirectoryFound(List<_SharpCompressEntry> entries, string key, bool isDir, HashSet<string> dirs)
            {
                var dir = key.Replace('\\', '/').TrimEnd('/');
                if (!isDir) dir = System.IO.Path.GetDirectoryName(dir);

                if (!string.IsNullOrEmpty(dir) && !dirs.Contains(dir))
                {
                    dirs.Add(dir);                    
                    entries.Add(new _SharpCompressDirectory(dir));
                }
            }

            private static _SharpCompressCachedEntry CreateFromCurrent(SharpCompress.Readers.IReader reader)
            {
                if (reader?.Entry == null) return null;
                if (reader.Entry.Size == 0) return null;

                System.IO.MemoryStream m = null;

                using (var s = reader.OpenEntryStream())
                {
                    if (s == null) return null;
                    m = new System.IO.MemoryStream();
                    s.CopyTo(m);
                }

                if (!m.TryGetBuffer(out var data)) data = m.ToArray();

                m.Dispose();

                return new _SharpCompressCachedEntry(reader, data);
            }

            private static async Task<_SharpCompressCachedEntry> CreateFromCurrentAsync(SharpCompress.Readers.IAsyncReader reader)
            {
                if (reader?.Entry == null) return null;
                if (reader.Entry.Size == 0) return null;

                System.IO.MemoryStream m = null;

                using (var s = await reader.OpenEntryStreamAsync())
                {
                    if (s == null) return null;
                    m = new System.IO.MemoryStream();
                    s.CopyTo(m);
                }

                if (!m.TryGetBuffer(out var data)) data = m.ToArray();

                m.Dispose();

                return new _SharpCompressCachedEntry(reader, data);
            }

            private _SharpCompressCachedEntry(SharpCompress.Readers.IReader reader, ArraySegment<byte> data)
                : base(reader.Entry.Key!, reader.Entry.IsDirectory)
            {
                Length = reader.Entry.Size;
                SetLastModifiedTime(reader.Entry.LastModifiedTime);

                _Content = data;
            }

            private _SharpCompressCachedEntry(SharpCompress.Readers.IAsyncReader reader, ArraySegment<byte> data)
                : base(reader.Entry.Key!, reader.Entry.IsDirectory)
            {
                Length = reader.Entry.Size;
                SetLastModifiedTime(reader.Entry.LastModifiedTime);

                _Content = data;
            }

            #endregion

            #region data            

            private readonly ArraySegment<byte> _Content;

            public override int GetHashCode()
            {
                return Key.GetHashCode(StringComparison.Ordinal);
            }

            public override bool Equals(object obj)
            {
                return obj is _SharpCompressCachedEntry other && Equals(other);
            }

            public bool Equals(_SharpCompressCachedEntry other)
            {
                if (other == null) return false;
                if (string.Equals(this.Key, other.Key, StringComparison.Ordinal)) return false;
                return this._Content.SequenceEqual(other._Content);
            }

            #endregion

            #region API            

            public override Stream CreateReadStream()
            {
                return new MemoryStream(_Content.Array, _Content.Offset, _Content.Count, false);
            }

            #endregion
        }

        #endregion
    }
}

#endif
