using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#nullable disable

using __MATCHCASING = System.IO.MatchCasing;

using System.Linq;
using System.ComponentModel;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        #region helpers                

        /// <summary>
        /// Used to determine if a given path is in the directory defined by <see cref="FullPath"/>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Diagnostics.DebuggerDisplay("{_PathSelector,nq}")]
        internal readonly struct _PathSlicer
        {
            #region lifecycle

            public _PathSlicer(string fullDirPath, __MATCHCASING casing)
            {
                FullPath = _SanitizePath(fullDirPath ?? string.Empty).TrimEnd('/');
                Casing = GetStringComparison(casing);
            }

            public _PathSlicer(string fullDirPath, StringComparison casing)
            {
                FullPath = _SanitizePath(fullDirPath ?? string.Empty).TrimEnd('/');
                Casing = casing;
            }            

            #endregion

            #region data

            /// <summary>
            /// Represents a full directory path.
            /// </summary>
            public string FullPath { get; }
            public StringComparison Casing { get; }

            #endregion

            #region API

            /// <summary>
            /// Given a collection of full paths, it returns
            /// </summary>
            /// <param name="paths"></param>
            /// <returns></returns>
            public IEnumerable<(string name, bool isDirectory)> Filter(IEnumerable<string> paths)
            {
                HashSet<string> dirs = null;

                foreach (var itemKey in paths)
                {
                    var xkey = itemKey;                    

                    if (!_TryTrimBasePath(ref xkey)) continue;

                    var idx = xkey.IndexOf('/');
                    if (idx >= 0)
                    {
                        dirs ??= new HashSet<string>();
                        dirs.Add(xkey.Substring(0, idx));
                    }
                    else yield return (xkey, false);
                }

                if (dirs != null)
                {
                    foreach (var d in dirs) yield return (d, true);
                }
            }

            public bool Contains(string itemKey)
            {
                if (!_TryTrimBasePath(ref itemKey)) return false;

                return !itemKey.TrimEnd('/').Contains('/');
            }

            private bool _TryTrimBasePath(ref string itemKey)
            {
                itemKey = _SanitizePath(itemKey);

                if (!itemKey.StartsWith(FullPath, Casing)) return false;

                itemKey = itemKey
                    .Substring(FullPath.Length)
                    .TrimStart('/');

                if (itemKey == "/") itemKey = string.Empty;

                return !string.IsNullOrEmpty(itemKey);
            }

            private static string _SanitizePath(string path)
            {
                return path
                    .Replace('\\', '/')
                    .TrimStart('/');
            }

            #endregion
        }

        #endregion
    }
}
