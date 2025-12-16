// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using __IOPATH = System.IO.Path;
using __FINFO = System.IO.FileInfo;
using __DINFO = System.IO.DirectoryInfo;
using __SINFO = System.IO.FileSystemInfo;
using __MATCHCASING = System.IO.MatchCasing;

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
        public static string GetNormalizedFullName(this __SINFO finfo)
        {
            GuardNotNull(finfo);

            return finfo
                .FullName
                .Replace(__IOPATH.AltDirectorySeparatorChar, __IOPATH.DirectorySeparatorChar)
                .TrimEnd(__IOPATH.DirectorySeparatorChar);
        }

        /// <summary>
        /// Gets the relative path from <paramref name="baseDir"/> to reach <paramref name="finfo"/>
        /// </summary>
        /// <param name="finfo"></param>
        /// <param name="baseDir"></param>
        /// <returns>A relative path.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetPathRelativeTo(this __FINFO finfo, __DINFO baseDir)
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

        public static bool EndsWith(this __FINFO finfo, string tail)        
        {
            GuardNotNull(finfo);

            return PathEndsWith(__MATCHCASING.PlatformDefault, finfo.FullName, tail);
        }

        /// <summary>
        /// Checks whether <paramref name="xinfo"/> is contained within the children of <paramref name="baseDir"/>.
        /// </summary>
        /// <param name="baseDir">The base directory</param>
        /// <param name="xinfo">A <see cref="__FINFO"/> or <see cref="__DINFO"/></param>
        /// <returns>true if <paramref name="xinfo"/> is a child.</returns>
        public static bool IsParentOf(this __DINFO baseDir, __SINFO xinfo) // TODO: maybe we could use System.IO.SearchOption to define first or all levels
        {
            GuardNotNull(baseDir);
            GuardNotNull(xinfo);            

            var basePath = baseDir.FullName;

            if (!IsDirectorySeparatorChar(basePath[basePath.Length-1])) basePath += __IOPATH.DirectorySeparatorChar;

            return xinfo.FullNameStartsWith(basePath);
        }

        /// <summary>
		/// Gets the hash code of <paramref name="x"/>.FullName,
        /// using platform file system casing rules.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int GetFullNameHashCode(this __SINFO x)
        {
            return _FileSystemInfoComparer<__SINFO>.Default.GetHashCode(x);
        }

        /// <summary>
        /// Checks whether <paramref name="a"/> and <paramref name="b"/> have the same <see cref="__SINFO.FullName"/>,
        /// using platform file system casing rules.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>True if they have an equivalent <see cref="__SINFO.FullName"/></returns>
        public static bool FullNameEquals(this __SINFO a, __SINFO b)
        {
            return _FileSystemInfoComparer<__SINFO>.Default.Equals(a, b);
        }		

        /// <summary>
        /// Checks whether <paramref name="a"/> and <paramref name="bPath"/> have the same <see cref="__SINFO.FullName"/>,
        /// using platform file system casing rules.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="bPath"></param>
        /// <returns>True if they have an equivalent <see cref="__SINFO.FullName"/></returns>
        public static bool FullNameEquals(this __SINFO a, string bPath)
        {
            if (a == null && bPath == null) return true;
            if (a == null) return false;

            return ArePathsEqual(__MATCHCASING.PlatformDefault, a.FullName, bPath);
        }

        /// <summary>
        /// Checks whether <paramref name="a"/>.FullName starts with <paramref name="path"/>,
        /// using platform file system casing rules.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FullNameStartsWith(this __SINFO a, string path)
        {
            return PathStartsWith(__MATCHCASING.PlatformDefault, a?.FullName, path);
        }

        /// <summary>
        /// Checks whether <paramref name="a"/>.FullName ends with <paramref name="path"/>,
        /// using platform file system casing rules.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FullNameEndsWith(this __SINFO a, string path)
        {
            return PathEndsWith(__MATCHCASING.PlatformDefault, a?.FullName, path);
        }

        #region Linq

        public static IEnumerable<T> FileSystemDistinct<T>(this IEnumerable<T> files)
            where T : __SINFO
        {
            return files.Distinct(MatchCasing.PlatformDefault.GetFullNameComparer<T>());
        }
        
        public static Dictionary<TKey,TValue> FileSystemToDictionary<TSource,TKey,TValue>(this IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, Func<TSource,TValue> valSelector)
            where TKey: __SINFO
        {
            return collection.ToDictionary(keySelector, valSelector, MatchCasing.PlatformDefault.GetFullNameComparer<TKey>());
        }
        
        /// <summary>
		/// Gets a <see cref="IEqualityComparer{T}"/> specialises in comparing <see cref="__SINFO.FullName"/>
		/// </summary>		
		public static IEqualityComparer<T> GetFullNameComparer<T>(this MatchCasing casing)
            where T:__SINFO
        {
            return _FileSystemInfoComparer<T>.GetInstance(casing);
        }

        #endregion

        #region nested types

        private sealed class _FileSystemInfoComparer<T> : IEqualityComparer<T>
        where T : __SINFO
        {
            private static IEqualityComparer<T>[] _Comparers;

            public static IEqualityComparer<T> GetInstance(__MATCHCASING casing)
            {
                switch(casing)
                {
                    case __MATCHCASING.CaseInsensitive: return GetInstance(StringComparison.OrdinalIgnoreCase);
                    case __MATCHCASING.CaseSensitive: return GetInstance(StringComparison.Ordinal);
                    case __MATCHCASING.PlatformDefault: return GetInstance(FileSystemStringComparison);
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
