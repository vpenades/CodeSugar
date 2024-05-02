// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using PATH = System.IO.Path;
using FILE = System.IO.FileInfo;
using DIRECTORY = System.IO.DirectoryInfo;
using SYSTEMENTRY = System.IO.FileSystemInfo;
using MATCHCASING = System.IO.MatchCasing;

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
        public static string GetNormalizedFullName(this SYSTEMENTRY finfo)
        {
            GuardNotNull(finfo);

            return finfo
                .FullName
                .Replace(PATH.AltDirectorySeparatorChar, PATH.DirectorySeparatorChar)
                .TrimEnd(PATH.DirectorySeparatorChar);
        }

        /// <summary>
        /// Gets the relative path from <paramref name="baseDir"/> to reach <paramref name="finfo"/>
        /// </summary>
        /// <param name="finfo"></param>
        /// <param name="baseDir"></param>
        /// <returns>A relative path.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetPathRelativeTo(this FILE finfo, DIRECTORY baseDir)
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

        public static bool EndsWith(this FILE finfo, string tail)        
        {
            GuardNotNull(finfo);

            return PathEndsWith(MATCHCASING.PlatformDefault, finfo.FullName, tail);
        }

        /// <summary>
        /// Checks whether <paramref name="xinfo"/> is contained within the children of <paramref name="baseDir"/>.
        /// </summary>
        /// <param name="baseDir">The base directory</param>
        /// <param name="xinfo">A <see cref="FILE"/> or <see cref="DIRECTORY"/></param>
        /// <returns>true if <paramref name="xinfo"/> is a child.</returns>
        public static bool IsParentOf(this DIRECTORY baseDir, SYSTEMENTRY xinfo)
        {
            GuardNotNull(baseDir);
            GuardNotNull(xinfo);

            var basePath = baseDir.FullName;

            if (!IsDirectorySeparatorChar(basePath[basePath.Length-1])) basePath += PATH.DirectorySeparatorChar;

            return xinfo.FullNameStartsWith(basePath);
        }

        /// <summary>
		/// Gets the hash code of <paramref name="x"/>.FullName,
        /// using platform file system casing rules.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int GetFullNameHashCode(this SYSTEMENTRY x)
        {
            return _FileSystemInfoComparer<SYSTEMENTRY>.Default.GetHashCode(x);
        }

        /// <summary>
        /// Checks whether <paramref name="a"/> and <paramref name="b"/> have the same <see cref="SYSTEMENTRY.FullName"/>,
        /// using platform file system casing rules.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>True if they have an equivalent <see cref="SYSTEMENTRY.FullName"/></returns>
        public static bool FullNameEquals(this SYSTEMENTRY a, SYSTEMENTRY b)
        {
            return _FileSystemInfoComparer<SYSTEMENTRY>.Default.Equals(a, b);
        }		

        /// <summary>
        /// Checks whether <paramref name="a"/> and <paramref name="path"/> have the same <see cref="SYSTEMENTRY.FullName"/>,
        /// using platform file system casing rules.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="bPath"></param>
        /// <returns>True if they have an equivalent <see cref="SYSTEMENTRY.FullName"/></returns>
        public static bool FullNameEquals(this SYSTEMENTRY a, string bPath)
        {
            if (a == null && bPath == null) return true;
            if (a == null) return false;

            return ArePathsEqual(MATCHCASING.PlatformDefault, a.FullName, bPath);
        }

        /// <summary>
        /// Checks whether <paramref name="a"/>.FullName starts with <paramref name="path"/>,
        /// using platform file system casing rules.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FullNameStartsWith(this SYSTEMENTRY a, string path)
        {
            return PathStartsWith(MATCHCASING.PlatformDefault, a?.FullName, path);
        }

        /// <summary>
        /// Checks whether <paramref name="a"/>.FullName ends with <paramref name="path"/>,
        /// using platform file system casing rules.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FullNameEndsWith(this SYSTEMENTRY a, string path)
        {
            return PathEndsWith(MATCHCASING.PlatformDefault, a?.FullName, path);
        }

        #region Linq

        public static IEnumerable<T> FileSystemDistinct<T>(this IEnumerable<T> files)
            where T : SYSTEMENTRY
        {
            return files.Distinct(MatchCasing.PlatformDefault.GetFullNameComparer<T>());
        }



        public static Dictionary<TKey,TValue> FileSystemToDictionary<TSource,TKey,TValue>(this IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, Func<TSource,TValue> valSelector)
            where TKey: SYSTEMENTRY
        {
            return collection.ToDictionary(keySelector, valSelector, MatchCasing.PlatformDefault.GetFullNameComparer<TKey>());
        }

        [Obsolete("Use MatchCasing.PlatformDefault.GetFullNameComparer<T>()", true)]
		public static IEqualityComparer<T> GetFullNameComparer<T>()
            where T:SYSTEMENTRY
        {
            return _FileSystemInfoComparer<T>.GetInstance(FileSystemStringComparison);
        }
        

		/// <summary>
		/// Gets a <see cref="IEqualityComparer{T}"/> specialises in comparing <see cref="SYSTEMENTRY.FullName"/>
		/// </summary>		
		public static IEqualityComparer<T> GetFullNameComparer<T>(this MatchCasing casing)
            where T:SYSTEMENTRY
        {
            return _FileSystemInfoComparer<T>.GetInstance(casing);
        }

        #endregion

        #region nested types

        private sealed class _FileSystemInfoComparer<T> : IEqualityComparer<T>
        where T : SYSTEMENTRY
        {
            private static IEqualityComparer<T>[] _Comparers;

            public static IEqualityComparer<T> GetInstance(MATCHCASING casing)
            {
                switch(casing)
                {
                    case MATCHCASING.CaseInsensitive: return GetInstance(StringComparison.OrdinalIgnoreCase);
                    case MATCHCASING.CaseSensitive: return GetInstance(StringComparison.Ordinal);
                    case MATCHCASING.PlatformDefault: return GetInstance(FileSystemStringComparison);
                    default: throw new ArgumentOutOfRangeException(casing.ToString(), nameof(casing));
                }
            }

            public static IEqualityComparer<T> GetInstance(StringComparison comparison)
            {
                if (_Comparers == null)
                {
                    var values = (StringComparison[])Enum.GetValues(typeof(StringComparison));
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
