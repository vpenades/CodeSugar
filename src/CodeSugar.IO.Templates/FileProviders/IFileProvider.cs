using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

#nullable disable

using __XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using __XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;
using __XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;
using __MATCHCASING = System.IO.MatchCasing;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        #region API

        [return: NotNull]
        public static __XINFO GetFileInfo(this __XPROVIDER provider, params string[] subpath)
        {
            var path = System.IO.Path.Combine(subpath);
            path = path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            return provider.GetFileInfo(path);
        }


        [return: NotNull]
        public static __XDIRECTORY GetDirectoryContents(this __XPROVIDER provider, params string[] subpath)
        {
            var path = System.IO.Path.Combine(subpath);
            path = path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            return provider.GetDirectoryContents(path);
        }

        [return: NotNull]
        public static __XPROVIDER ToIFileProvider(this __XDIRECTORY dir, __MATCHCASING casing)
        {
            switch (dir)
            {
                case null: return _NullFileProvider;
                case __XPROVIDER provider: return provider; // already a provider
                default: return new _FileProviderOverDirectoryContents(dir, casing);
            }
        }

        [return: NotNull]
        public static __XPROVIDER ToIFileProvider<T>(this IEnumerable<T> flatFiles, Func<T,string> pathSolver, __MATCHCASING casing) where T: __XINFO
        {
            var traits = new _FileProviderPathTraits(casing.GetStringComparer());

            return new _FileProviderOverFlatCollection<T>(flatFiles, pathSolver, traits);
        }

        #endregion

        #region nested types

        private static readonly NullFileProvider _NullFileProvider = new NullFileProvider();

        [System.Diagnostics.DebuggerDisplay("{_Dir}")]
        private sealed class _FileProviderOverDirectoryContents : __XPROVIDER
        {
            #region lifecycle
            public _FileProviderOverDirectoryContents(__XDIRECTORY dir, __MATCHCASING casing)
            {
                _Dir = dir;
                _Casing = casing;
            }

            #endregion

            #region data

            private readonly __XDIRECTORY _Dir;
            private readonly __MATCHCASING _Casing;

            #endregion

            #region API

            public __XINFO GetFileInfo(string subpath)
            {
                return FindEntry(_Dir, _Casing, _GetPathParts(subpath));
            }

            public __XDIRECTORY GetDirectoryContents(string subpath)
            {
                return FindEntry(_Dir, _Casing, _GetPathParts(subpath)) is __XDIRECTORY xdir
                    ? xdir
                    : Microsoft.Extensions.FileProviders.NotFoundDirectoryContents.Singleton;
            }

            private static string[] _GetPathParts(string subpath)
            {
                subpath ??= string.Empty;
                return subpath.Replace('\\', '/').Trim('/').Split('/');
            }

            public IChangeToken Watch(string filter)
            {
                return NullChangeToken.Singleton;
            }

            #endregion
        }



        class _FileProviderPathTraits
        {
            public _FileProviderPathTraits(IEqualityComparer<string> pathComparer)
            {
                PathComparer = pathComparer;
            }

            private readonly char[] _PathSepatators = new[] { '/', '\\' };

            public IEqualityComparer<string> PathComparer { get; }

            public bool ArePathsEqual(string left, string right) => PathComparer.Equals(left, right);

            public string GetFileName(string path)
            {
                path = path.Trim(_PathSepatators);

                var idx = path.LastIndexOfAny(_PathSepatators);
                if (idx < 0) return path;

                return path.Substring(idx + 1);
            }

            public string GetFilePath(string path)
            {
                path = path.Trim(_PathSepatators);

                var idx = path.LastIndexOfAny(_PathSepatators);
                if (idx < 0) return string.Empty;

                return path.Substring(0, idx);
            }

            public bool IsInDirectory(string baseDir, string path)
            {
                if (path.Length < baseDir.Length) return false;
                if (!PathComparer.Equals(baseDir, path.Substring(0, baseDir.Length))) return false;

                path = path.Substring(baseDir.Length);
                if (!StartsWithSeparator(path)) return false;
                path = path.Substring(1);

                if (ContainsSeparator(path)) return false;

                return true;
            }

            public void ValidatePath(string key, string argName)
            {
                if (StartsWithSeparator(key)) throw new ArgumentException("returned paths should not start with separators", argName);
                if (EndsWithSeparator(key)) throw new ArgumentException("returned paths should not start with separators", argName);
            }

            public bool ContainsSeparator(string path)
            {
                #if NET10_0_OR_GREATER
                return path.ContainsAny(_PathSepatators);
                #else
                return _PathSepatators.Any(c => path.Contains(c));
                #endif
            }

            public bool StartsWithSeparator(string path)
            {
                if (string.IsNullOrEmpty(path)) return false;
                return _PathSepatators.Any(c => path[0] == c);
            }

            public bool EndsWithSeparator(string path)
            {
                if (string.IsNullOrEmpty(path)) return false;
                var i = path.Length - 1;
                return _PathSepatators.Any(c => path[i] == c);
            }

        }


        private sealed class _FileProviderOverFlatCollection<T> : __XPROVIDER where T : __XINFO
        {
            public _FileProviderOverFlatCollection(IEnumerable<T> flatEntries, Func<T, string> pathSolver, _FileProviderPathTraits pathTraits)
            {
                _FlatEntries = new Dictionary<string, T>(pathTraits.PathComparer);
                _FolderEntries = new Dictionary<string, _FolderInfo>(pathTraits.PathComparer);

                var directories = new Dictionary<string, __XINFO>();

                foreach (var entry in flatEntries)
                {
                    var key = pathSolver(entry);
                    pathTraits.ValidatePath(key, nameof(pathSolver));

                    _FlatEntries[key] = entry;

                    // track directories

                    if (entry.IsDirectory)
                    {
                        if (entry is __XDIRECTORY)
                        {
                            directories[key] = entry;
                        }
                        else
                        {
                            System.Diagnostics.Debug.Assert(entry is __XDIRECTORY, $"entry {entry.Name} must implement {nameof(__XDIRECTORY)}");
                            directories[key] = null;
                        }
                    }
                    else
                    {
                        var dir = pathTraits.GetFilePath(key);
                        if (!directories.ContainsKey(dir)) directories[dir] = null;
                    }
                }

                // construct missing directory nodes

                var dirNames = directories
                    .Where(item => item.Value == null)
                    .Select(item => item.Key)
                    .OrderByDescending(item => item);

                foreach (var dpath in dirNames)
                {
                    var eee = new List<T>();
                    var fff = new List<_FolderInfo>();

                    foreach (var kve in _FlatEntries)
                    {
                        if (pathTraits.IsInDirectory(dpath, kve.Key)) eee.Add(kve.Value);
                    }

                    foreach (var kve in _FolderEntries)
                    {
                        if (pathTraits.IsInDirectory(dpath, kve.Key)) fff.Add(kve.Value);
                    }

                    var name = pathTraits.GetFileName(dpath);
                    var folder = new _FolderInfo(name, eee, fff);

                    _FolderEntries[dpath] = folder;
                }
            }



            private readonly Dictionary<string, T> _FlatEntries;
            private readonly Dictionary<string, _FolderInfo> _FolderEntries;

            public __XINFO GetFileInfo(string subpath)
            {
                subpath ??= string.Empty;

                if (_FlatEntries.TryGetValue(subpath, out var xinfo) && !xinfo.IsDirectory)
                {
                    return xinfo;
                }

                return new NotFoundFileInfo(subpath);
            }

            public __XDIRECTORY GetDirectoryContents(string subpath)
            {
                subpath ??= string.Empty;

                if (_FlatEntries.TryGetValue(subpath, out var xinfo) && xinfo.IsDirectory)
                {
                    if (xinfo is __XDIRECTORY contents) return contents;
                }

                if (_FolderEntries.TryGetValue(subpath, out var xfolder)) return xfolder;

                return NotFoundDirectoryContents.Singleton;
            }

            public IChangeToken Watch(string filter)
            {
                return Microsoft.Extensions.FileProviders.NullChangeToken.Singleton;
            }

            [System.Diagnostics.DebuggerDisplay("_FolderInfo {Name}")]
            private sealed class _FolderInfo : __XINFO, __XDIRECTORY
            {
                #region lifecycle                

                public _FolderInfo(string name, List<T> entries, List<_FolderInfo> folders)
                {
                    _Entries = entries;
                    _Folders = folders;
                    Name = name;
                }

                #endregion

                #region data

                private readonly List<T> _Entries;
                private readonly List<_FolderInfo> _Folders;

                #endregion

                #region properties

                public Stream CreateReadStream() { throw new NotImplementedException(); }

                public bool Exists => true;

                public long Length => 0;

                public string PhysicalPath => null;

                public string Name { get; }

                public DateTimeOffset LastModified => DateTime.Today;

                public bool IsDirectory => true;

                public IEnumerator<__XINFO> GetEnumerator()
                {
                    return _Entries.OfType<__XINFO>().Concat(_Folders).GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion
            }
        }

#endregion
    }
}
