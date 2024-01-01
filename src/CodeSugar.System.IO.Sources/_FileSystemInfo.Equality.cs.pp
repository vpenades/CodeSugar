// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using FILE = System.IO.FileInfo;
using DIRECTORY = System.IO.DirectoryInfo;
using SYSTEMENTRY = System.IO.FileSystemInfo;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarIO
    {
        /// <summary>
        /// ensures that the path uses Path.DirectorySeparatorChar and it does not end with a path separator.
        /// </summary>
        /// <returns>
        /// a normalized path that is suited to be used for path string comparison.
        /// </returns>
        public static string GetNormalizedFullName(this System.IO.FileSystemInfo finfo)
        {
            GuardNotNull(finfo);
            return GetNormalizedPath(finfo.FullName);
        }

        /// <summary>
        /// Gets the relative path from <paramref name="baseDir"/> to reach <paramref name="finfo"/>
        /// </summary>
        /// <param name="finfo"></param>
        /// <param name="baseDir"></param>
        /// <returns>A relative path.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetRelativePath(this FILE finfo, DIRECTORY baseDir)
        {
            GuardNotNull(finfo);
            GuardNotNull(baseDir);

            // TODO: check for different drives

            var path = finfo.FullName;            

            while(baseDir != null)
            {
                if (baseDir.ContainsFileOrDirectory(finfo)) return path.Substring(baseDir.FullName.Length).TrimStart(_DirectorySeparators);
                baseDir = baseDir.Parent;
            }

            throw new ArgumentException("invalid path", nameof(baseDir));
        }        

        /// <summary>
        /// Checks whether <paramref name="xinfo"/> is contained within the children of <paramref name="baseDir"/>.
        /// </summary>
        /// <param name="baseDir">The base directory</param>
        /// <param name="xinfo">A <see cref="FILE"/> or <see cref="DIRECTORY"/></param>
        /// <returns>true if <paramref name="xinfo"/> is a child.</returns>
        public static bool ContainsFileOrDirectory(this DIRECTORY baseDir, SYSTEMENTRY xinfo)
        {
            GuardNotNull(baseDir);
            GuardNotNull(xinfo);

            return xinfo.FullNameStartsWith(baseDir.FullName);
        }

		/// <summary>
		/// Checks whether <paramref name="a"/> and <paramref name="b"/> have the same <see cref="SYSTEMENTRY.FullName"/>,
		/// using case insensitive rules.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns>True if they have an equivalent <see cref="SYSTEMENTRY.FullName"/></returns>
		public static bool FullNameEquals(this SYSTEMENTRY a, SYSTEMENTRY b)
        {
            return _FileSystemInfoComparer<SYSTEMENTRY>.Default.Equals(a, b);
        }

		/// <summary>
		/// Gets the hash code of <paramref name="x"/>.FullName, using case insensitive rules.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int GetFullNameHashCode(this SYSTEMENTRY x)
        {
            return _FileSystemInfoComparer<SYSTEMENTRY>.Default.GetHashCode(x);
		}

		/// <summary>
		/// Checks whether <paramref name="a"/> and <paramref name="path"/> have the same <see cref="SYSTEMENTRY.FullName"/>,
		/// using case insensitive rules.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="bPath"></param>
		/// <returns>True if they have an equivalent <see cref="SYSTEMENTRY.FullName"/></returns>
		public static bool FullNameEquals(this SYSTEMENTRY a, string bPath)
        {            
            if (a == null) return false;            

            var aPath = GetNormalizedFullName(a);

            bPath = bPath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

            return string.Equals(aPath, bPath, FileSystemStringComparison);
        }

		/// <summary>
		/// Checks whether <paramref name="a"/>.FullName starts with <paramref name="path"/>,
		/// using case insensitive rules.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool FullNameStartsWith(this SYSTEMENTRY a, string path)
        {
            if (a == null) return false;

            var aPath = GetNormalizedFullName(a);
            path = path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

            return aPath.StartsWith(path, FileSystemStringComparison);
        }

        /// <summary>
		/// Gets a <see cref="IEqualityComparer{T}"/> specialises in comparing <see cref="SYSTEMENTRY.FullName"/>
		/// </summary>		
		public static IEqualityComparer<T> GetFullNameComparer<T>()
            where T:SYSTEMENTRY
        {
            return _FileSystemInfoComparer<T>.GetInstance(FileSystemStringComparison);
        }
        

		/// <summary>
		/// Gets a <see cref="IEqualityComparer{T}"/> specialises in comparing <see cref="SYSTEMENTRY.FullName"/>
		/// </summary>		
		public static IEqualityComparer<T> GetFullNameComparer<T>(this StringComparison comparison)
            where T:SYSTEMENTRY
        {
            return _FileSystemInfoComparer<T>.GetInstance(comparison);
        }

        private sealed class _FileSystemInfoComparer<T> : IEqualityComparer<T>
        where T : SYSTEMENTRY
        {
            private static IEqualityComparer<T>[] _Comparers;			

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
    }
}
