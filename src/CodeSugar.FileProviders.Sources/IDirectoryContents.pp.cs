// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System.Collections;

using Microsoft.Extensions.FileProviders;

#nullable disable

using __XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using __XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;
using __MATCHCASING = System.IO.MatchCasing;



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
        #region API

        /// <summary>
        /// Joins a collection of <see cref="__XINFO"/> as a <see cref="__XDIRECTORY"/>.
        /// </summary>
        /// <param name="files">the collection of entries to join</param>
        /// <returns>A single <see cref="__XDIRECTORY"/></returns>
        /// <remarks>
        /// This will create a <see cref="__XDIRECTORY"/> even if the entries are not in the same logical path.
        /// </remarks>
        [return: NotNull]
        public static __XDIRECTORY ToIDirectoryContents(this IEnumerable<__XINFO> files)
        {
            switch(files)
            {
                case null: return NotFoundDirectoryContents.Singleton;                
                default:
                    var items = files.ToList();
                    return new _DirectoryCollectionGroup(items);
            }            
        }

        [return: NotNull]
        public static __XDIRECTORY ToIDirectoryContents<T>(this IEnumerable<T> files, Func<T, string> fullPathFunc, string pathSelector, __MATCHCASING casing)
            where T: __XINFO
        {
            switch (files)
            {
                case null: return NotFoundDirectoryContents.Singleton;
                default: return new _DirectoryCollectionSlice<T>(files, fullPathFunc, pathSelector, casing);
            }
        }

        [return: NotNull]
        public static __XDIRECTORY ToIDirectoryContents<T>(this IEnumerable<T> files, string name, Func<T, string> fullPathFunc, string pathSelector, __MATCHCASING casing)
            where T : __XINFO
        {
            switch (files)
            {
                case null: return NotFoundDirectoryContents.Singleton;
                default: return new _NamedDirectoryCollectionSlice<T>(name, files, fullPathFunc, pathSelector, casing);
            }
        }

        #endregion

        #region nested types        

        /// <summary>
        /// Represents a simple grouping of a collection of <see cref="__XINFO"/> entries
        /// </summary>
        /// <remarks>
        /// The elements listed by this collection do not neccesarily require to be children of the same logical parent.
        /// </remarks>
        private class _DirectoryCollectionGroup : __XDIRECTORY
        {
            #region lifecycle
            public _DirectoryCollectionGroup(IReadOnlyList<__XINFO> entries)
            {
                _Entries = entries;
            }

            #endregion

            #region data

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            private readonly IReadOnlyList<__XINFO> _Entries;

            #endregion

            #region API

            public bool Exists => true;

            public IEnumerator<__XINFO> GetEnumerator()
            {
                return _Entries.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _Entries.GetEnumerator();
            }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("{Name,nq}")]
        private class _DirectoryCollectionSlice<T> : __XDIRECTORY, __XINFO
            where T: __XINFO
        {
            #region lifecycle
            public _DirectoryCollectionSlice(IEnumerable<T> flattenedEntries, Func<T, string> fullPathFunc, string pathSelector, __MATCHCASING casing)
            {
                if (string.IsNullOrEmpty(pathSelector)) pathSelector = "/";

                _Entries = flattenedEntries;
                _FullPathFunc = fullPathFunc;
                _Path = new _PathSlicer(pathSelector, casing);
            }

            protected _DirectoryCollectionSlice(IEnumerable<T> flattenedEntries, Func<T, string> fullPathFunc, string pathSelector, StringComparison casing)
            {
                if (string.IsNullOrEmpty(pathSelector)) pathSelector = "/";

                _Entries = flattenedEntries;
                _FullPathFunc = fullPathFunc;
                _Path = new _PathSlicer(pathSelector, casing);
            }

            #endregion

            #region data

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            private readonly IEnumerable<T> _Entries;

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly Func<T, string> _FullPathFunc;

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly _PathSlicer _Path;

            private Dictionary<T, __XINFO> _DirectoryCache;

            #endregion

            #region API

            public bool Exists => true;

            public IEnumerator<__XINFO> GetEnumerator()
            {
                return _Entries
                    .Where(entry => _Path.Contains(_FullPathFunc(entry)))
                    .Select(_SanitizeDirectory)
                    .GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private __XINFO _SanitizeDirectory(T xentry)
            {
                if (!xentry.IsDirectory || xentry is __XDIRECTORY) return xentry;

                // in case we find a directory entry, but it does not implement __XDIRECTORY we wrap it with it.

                _DirectoryCache ??= new Dictionary<T, __XINFO>();

                if (_DirectoryCache.TryGetValue(xentry, out var xdir)) return xdir;

                var selector = _FullPathFunc(xentry);

                xdir = new _WrappedDirectoryCollectionSlice<T>(xentry, _Entries, _FullPathFunc, selector, _Path.Casing);

                _DirectoryCache[xentry] = xdir;

                return xdir;
            }            

            public long Length => 0;

            public virtual string PhysicalPath => null;

            public virtual string Name => System.IO.Path.GetFileName(_Path.FullPath);

            public virtual DateTimeOffset LastModified => DateTime.Today;

            public bool IsDirectory => true;

            Stream __XINFO.CreateReadStream() { throw new NotSupportedException(); }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("{Name,nq}/")]
        private sealed class _NamedDirectoryCollectionSlice<T> : _DirectoryCollectionSlice<T>, __XINFO
            where T : __XINFO
        {
            #region lifecycle
            public _NamedDirectoryCollectionSlice(string name, IEnumerable<T> files, Func<T, string> fullPathFunc, string pathSelector, __MATCHCASING casing)
                : base(files, fullPathFunc, pathSelector, casing)
            {
                this.Name = name;
            }

            #endregion            

            #region properties            
            public override string Name { get; }

            #endregion            
        }

        /// <summary>
        /// This is used in cases where we find a <see cref="__XINFO"/>
        /// representing a dictionary, but missing <see cref="__XDIRECTORY"/> implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [System.Diagnostics.DebuggerDisplay("{Name,nq}/")]
        private sealed class _WrappedDirectoryCollectionSlice<T> : _DirectoryCollectionSlice<T>
            where T : __XINFO
        {
            #region lifecycle            

            public _WrappedDirectoryCollectionSlice(T dir, IEnumerable<T> files, Func<T, string> fullPathFunc, string pathSelector, StringComparison casing)
                : base(files, fullPathFunc, pathSelector, casing)
            {
                System.Diagnostics.Debug.Assert(dir.IsDirectory, "must be a directory");

                _DirInfo = dir;
            }

            #endregion

            #region data

            private readonly T _DirInfo;

            #endregion

            #region properties            

            public override string PhysicalPath => _DirInfo.PhysicalPath;

            public override string Name => _DirInfo.Name;

            public override DateTimeOffset LastModified => _DirInfo.LastModified;

            #endregion        
        }        

        #endregion
    }
}
