using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using TUnit.Assertions.Conditions;
using TUnit.Assertions.Core;

using TUnit.Assertions.Enums;

#pragma warning disable CS8632

namespace CodeSugar
{

    internal static class _Extensions
    {
        public static string ReadAllText(this System.IO.FileInfo finfo)
        {
            return System.IO.File.ReadAllText(finfo.FullName);
        }
       
        public static System.IO.DirectoryInfo? FindDirectoryTree(this System.IO.DirectoryInfo? initial, params string[] paths)
        {
            while(initial != null)
            {
                var path = System.IO.Path.Combine(paths);

                if (initial != null) path = System.IO.Path.Combine(initial.FullName, path);

                var probePath = new System.IO.DirectoryInfo(path);

                if (probePath.Exists) return probePath;

                initial = initial.Parent;
            }

            return null;
        }
    }
}

#pragma warning restore CS8632
