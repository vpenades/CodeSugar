using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Collections;

using Microsoft.Extensions.FileProviders;

#nullable disable

using __XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using __XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;
using __XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;
using __MATCHCASING = System.IO.MatchCasing;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        #region API

        [return: NotNull]
        public static __XDIRECTORY GetDirectoryContents(this __XINFO xfile)
        {
            return TryGetDirectoryContents(xfile, out var xdir)
                ? xdir
                : Microsoft.Extensions.FileProviders.NotFoundDirectoryContents.Singleton;
        }

        /// <summary>
        /// If <paramref name="xfile"/> is a directory, it tries to return its <paramref name="xdir"/>
        /// </summary>
        /// <param name="xfile">the input file</param>
        /// <param name="xdir">the output directory contents</param>
        /// <returns>true on success</returns>
        public static bool TryGetDirectoryContents([DisallowNull] this __XINFO xfile, out __XDIRECTORY xdir)
        {
            // notice that we're not handling .Exist here, because if the file does not exist,
            // the returned IDirectoryInfo returned should also have the .Exist to false

            xdir = null;
            if (xfile == null || !xfile.IsDirectory) return false;

            switch (xfile)
            {
                // IFileInfo implements IDirectoryContents on Microsoft.Extensions.FileProviders.Physical.9.0.0 and up.
                case __XDIRECTORY asDC:
                    xdir = asDC;
                    return true;

                // it may be odd for a IFileInfo to also implement IFileProvider, but it may happen in the wild.
                case __XPROVIDER asFP:
                    xdir = asFP.GetDirectoryContents(string.Empty);
                    return true;

                // easter egg: some implementations may choose to expose the IDirectoryContents as a service.
                case System.IServiceProvider asSrv:
                    xdir = asSrv.GetService(typeof(__XDIRECTORY)) as __XDIRECTORY;
                    if (xdir != null) return true;
                    break;
            }

            // fallback for physical IFileInfo directories not implementing IDirectoryContents
            // (Microsoft.Extensions.FileProviders.Physical.8.0.0 and below)
            if (IsPhysical(xfile) && System.IO.Path.IsPathFullyQualified(xfile.PhysicalPath))
            {
                System.Diagnostics.Debug.Assert(xfile.IsDirectory);
                var dinfo = new DirectoryInfo(xfile.PhysicalPath);
                xdir = ToIFileInfo(dinfo) as __XDIRECTORY;
            }

            return xdir != null;
        }

        /// <summary>
        /// Joins a collection of <see cref="__XINFO"/> as a <see cref="__XDIRECTORY"/>.
        /// </summary>
        /// <param name="files">the collection of entries to join</param>
        /// <returns>A single <see cref="__XDIRECTORY"/></returns>
        /// <remarks>
        /// This will create a <see cref="__XDIRECTORY"/> even if the entries are not in the same logical path.
        /// </remarks>
        [return: NotNull]
        public static __XDIRECTORY ToIDirectoryContents([DisallowNull] this IEnumerable<__XINFO> files)
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
        public static __XDIRECTORY ToIDirectoryContents<T>([DisallowNull] this IEnumerable<T> files, Func<T, string> fullPathFunc, string pathSelector, __MATCHCASING casing)
            where T: __XINFO
        {
            switch (files)
            {
                case null: return NotFoundDirectoryContents.Singleton;
                default: return new _DirectoryCollectionSlice<T>(files, fullPathFunc, pathSelector, casing);
            }
        }

        [return: NotNull]
        public static __XDIRECTORY ToIDirectoryContents<T>([DisallowNull] this IEnumerable<T> files, string name, Func<T, string> fullPathFunc, string pathSelector, __MATCHCASING casing)
            where T : __XINFO
        {
            switch (files)
            {
                case null: return NotFoundDirectoryContents.Singleton;
                default: return new _NamedDirectoryCollectionSlice<T>(name, files, fullPathFunc, pathSelector, casing);
            }
        }

        #endregion

        #region creation functions
        
        public static bool TryCreateFile([NotNull] this __XDIRECTORY xdir, string fileName, [NotNullWhen(true)] out __XINFO xinfo)
        {
            GuardNotNull(xdir);

            if (TryGetPhysicalPath(xdir, out var physicalPath))
            {
                var path = System.IO.Path.Combine(physicalPath, fileName);
                var finfo = new FileInfo(path);
                xinfo = new _BasicPhysicalFile(finfo);
                return true;
            }
            
            xinfo = null;
            return false;            
        }

        public static bool TryCreateDirectory([NotNull] this __XDIRECTORY xdir, string fileName, [NotNullWhen(true)] out __XINFO xinfo)
        {
            GuardNotNull(xdir);

            if (TryGetPhysicalPath(xdir, out var physicalPath))
            {
                var path = System.IO.Path.Combine(physicalPath, fileName);
                var dinfo = new DirectoryInfo(path);
                xinfo = new _BasicPhysicalDirectory(dinfo);
                return true;
            }

            xinfo = null;
            return false;
        }

        public static bool TryGetPhysicalPath([NotNull] this __XDIRECTORY xdir, [NotNullWhen(true)] out string physicalPath)
        {
            GuardNotNull(xdir);

            physicalPath = null;

            if (xdir is IGrouping<__XINFO, __XINFO> grouping)
            {
                physicalPath ??= grouping.Key.PhysicalPath;
            }

            if (xdir is _BasicPhysicalDirectory internalPhysicalDir)
            {
                physicalPath ??= internalPhysicalDir.Info.FullName;
            }

            if (xdir is __XINFO xinfo)
            {
                physicalPath ??= xinfo.PhysicalPath;
            }

            return !string.IsNullOrWhiteSpace(physicalPath);
        }

        #endregion

        #region nested types        

        /// <summary>
        /// Represents a flat grouping of a collection of <see cref="__XINFO"/> entries
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

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Collapsed)]
            private readonly IEnumerable<T> _Entries;

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly Func<T, string> _FullPathFunc;

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly _PathSlicer _Path;

            private ConcurrentDictionary<T, __XINFO> _DirectoryCache;

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

                // in case we find an entry representing a directory, but it does not implement __XDIRECTORY we wrap it with it.                

                __XINFO createFrom(T entry)
                {
                    var selector = _FullPathFunc(xentry);
                    return new _WrappedDirectoryCollectionSlice<T>(xentry, _Entries, _FullPathFunc, selector, _Path.Casing);
                }

                _DirectoryCache ??= new ConcurrentDictionary<T, __XINFO>();

                return _DirectoryCache.GetOrAdd(xentry, createFrom);                
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
        private sealed class _WrappedDirectoryCollectionSlice<T>
            : _DirectoryCollectionSlice<T>
            , IGrouping<__XINFO,__XINFO>
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

            /// <summary>
            /// Parent
            /// </summary>
            __XINFO IGrouping<__XINFO, __XINFO>.Key => _DirInfo;

            public override string PhysicalPath => _DirInfo.PhysicalPath;

            public override string Name => _DirInfo.Name;

            public override DateTimeOffset LastModified => _DirInfo.LastModified;

            #endregion        
        }        

        #endregion
    }
}
