// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using _IOPATH = System.IO.Path;
using _FINFO = System.IO.FileInfo;
using _DINFO = System.IO.DirectoryInfo;
using _SINFO = System.IO.FileSystemInfo;
using _MATCHCASING = System.IO.MatchCasing;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarForSystemIO
    {
        /// <summary>
        /// ensures that the path uses Path.DirectorySeparatorChar and it does not end with a path separator.
        /// </summary>
        /// <returns>
        /// a normalized path that is suited to be used for path string comparison.
        /// </returns>
        public static string GetNormalizedFullName(this _SINFO finfo)
        {
            GuardNotNull(finfo);

            return finfo
                .FullName
                .Replace(_IOPATH.AltDirectorySeparatorChar, _IOPATH.DirectorySeparatorChar)
                .TrimEnd(_IOPATH.DirectorySeparatorChar);
        }

        /// <summary>
        /// Gets the relative path from <paramref name="baseDir"/> to reach <paramref name="finfo"/>
        /// </summary>
        /// <param name="finfo"></param>
        /// <param name="baseDir"></param>
        /// <returns>A relative path.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetPathRelativeTo(this _FINFO finfo, _DINFO baseDir)
        {
            GuardNotNull(finfo);
            GuardNotNull(baseDir);

            // TODO: check for different drives

            var path = finfo.FullName;            

            while(baseDir != null)
            {
                if (baseDir.IsParentOf(finfo)) return path.Substring(baseDir.FullName.Length).TrimStart(_DirectorySeparators);
                baseDir = baseDir.Parent;
            }

            throw new ArgumentException("invalid path", nameof(baseDir));
        }        

        public static bool EndsWith(this _FINFO finfo, string tail)        
        {
            GuardNotNull(finfo);

            return PathEndsWith(_MATCHCASING.PlatformDefault, finfo.FullName, tail);
        }

        /// <summary>
        /// Checks whether <paramref name="xinfo"/> is contained within the children of <paramref name="baseDir"/>.
        /// </summary>
        /// <param name="baseDir">The base directory</param>
        /// <param name="xinfo">A <see cref="_FINFO"/> or <see cref="_DINFO"/></param>
        /// <returns>true if <paramref name="xinfo"/> is a child.</returns>
        public static bool IsParentOf(this _DINFO baseDir, _SINFO xinfo) // TODO: maybe we could use System.IO.SearchOption to define first or all levels
        {
            GuardNotNull(baseDir);
            GuardNotNull(xinfo);            

            var basePath = baseDir.FullName;

            if (!IsDirectorySeparatorChar(basePath[basePath.Length-1])) basePath += _IOPATH.DirectorySeparatorChar;

            return xinfo.FullNameStartsWith(basePath);
        }

        /// <summary>
		/// Gets the hash code of <paramref name="x"/>.FullName,
        /// using platform file system casing rules.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int GetFullNameHashCode(this _SINFO x)
        {
            return _FileSystemInfoComparer<_SINFO>.Default.GetHashCode(x);
        }

        /// <summary>
        /// Checks whether <paramref name="a"/> and <paramref name="b"/> have the same <see cref="_SINFO.FullName"/>,
        /// using platform file system casing rules.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>True if they have an equivalent <see cref="_SINFO.FullName"/></returns>
        public static bool FullNameEquals(this _SINFO a, _SINFO b)
        {
            return _FileSystemInfoComparer<_SINFO>.Default.Equals(a, b);
        }		

        /// <summary>
        /// Checks whether <paramref name="a"/> and <paramref name="path"/> have the same <see cref="_SINFO.FullName"/>,
        /// using platform file system casing rules.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="bPath"></param>
        /// <returns>True if they have an equivalent <see cref="_SINFO.FullName"/></returns>
        public static bool FullNameEquals(this _SINFO a, string bPath)
        {
            if (a == null && bPath == null) return true;
            if (a == null) return false;

            return ArePathsEqual(_MATCHCASING.PlatformDefault, a.FullName, bPath);
        }

        /// <summary>
        /// Checks whether <paramref name="a"/>.FullName starts with <paramref name="path"/>,
        /// using platform file system casing rules.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FullNameStartsWith(this _SINFO a, string path)
        {
            return PathStartsWith(_MATCHCASING.PlatformDefault, a?.FullName, path);
        }

        /// <summary>
        /// Checks whether <paramref name="a"/>.FullName ends with <paramref name="path"/>,
        /// using platform file system casing rules.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FullNameEndsWith(this _SINFO a, string path)
        {
            return PathEndsWith(_MATCHCASING.PlatformDefault, a?.FullName, path);
        }

        #region Linq

        public static IEnumerable<T> FileSystemDistinct<T>(this IEnumerable<T> files)
            where T : _SINFO
        {
            return files.Distinct(MatchCasing.PlatformDefault.GetFullNameComparer<T>());
        }



        public static Dictionary<TKey,TValue> FileSystemToDictionary<TSource,TKey,TValue>(this IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, Func<TSource,TValue> valSelector)
            where TKey: _SINFO
        {
            return collection.ToDictionary(keySelector, valSelector, MatchCasing.PlatformDefault.GetFullNameComparer<TKey>());
        }

        [Obsolete("Use MatchCasing.PlatformDefault.GetFullNameComparer<T>()", true)]
		public static IEqualityComparer<T> GetFullNameComparer<T>()
            where T:_SINFO
        {
            return _FileSystemInfoComparer<T>.GetInstance(FileSystemStringComparison);
        }
        

		/// <summary>
		/// Gets a <see cref="IEqualityComparer{T}"/> specialises in comparing <see cref="_SINFO.FullName"/>
		/// </summary>		
		public static IEqualityComparer<T> GetFullNameComparer<T>(this MatchCasing casing)
            where T:_SINFO
        {
            return _FileSystemInfoComparer<T>.GetInstance(casing);
        }

        #endregion

        #region nested types

        private sealed class _FileSystemInfoComparer<T> : IEqualityComparer<T>
        where T : _SINFO
        {
            private static IEqualityComparer<T>[] _Comparers;

            public static IEqualityComparer<T> GetInstance(_MATCHCASING casing)
            {
                switch(casing)
                {
                    case _MATCHCASING.CaseInsensitive: return GetInstance(StringComparison.OrdinalIgnoreCase);
                    case _MATCHCASING.CaseSensitive: return GetInstance(StringComparison.Ordinal);
                    case _MATCHCASING.PlatformDefault: return GetInstance(FileSystemStringComparison);
                    default: throw new ArgumentOutOfRangeException(nameof(casing), casing.ToString());
                }
            }

            public static IEqualityComparer<T> GetInstance(StringComparison comparison)
            {
                if (_Comparers == null)
                {
                    #if NET6_0_OR_GREATER
                    var values = Enum.GetValues<StringComparison>();
                    #else
                    var values = (StringComparison[])Enum.GetValues(typeof(StringComparison));
                    #endif

                    var len = values.Max() + 1;

				    _Comparers = new IEqualityComparer<T>[(int)len];

                    foreach(var idx in values)
                    {
                        _Comparers[(int)idx] = new _FileSystemInfoComparer<T>(idx);
				    }
                }

                return _Comparers[(int)comparison];
            }

			public static IEqualityComparer<T> Default { get; } = GetInstance(FileSystemStringComparison);

			private _FileSystemInfoComparer(StringComparison comparison)
            {
                _Comparison = comparison;
			}

            private readonly StringComparison _Comparison;			

            public bool Equals(T x, T y)
            {
                if (Object.ReferenceEquals(x, y)) return true;
                if (x == null) return false;
                if (y == null) return false;

                var apath = GetNormalizedFullName(x);
                var bpath = GetNormalizedFullName(y);              

                return string.Equals(apath, bpath, _Comparison);
            }

            public int GetHashCode(T obj)
            {
                return obj == null ? 0 : GetNormalizedFullName(obj).GetHashCode(_Comparison);
            }
        }

#endregion
    }
}
