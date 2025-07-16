using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSugar
{
    internal static class _Extensions
    {
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ArraySegment<T> segment) where T : unmanaged { return segment; }
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array) where T : unmanaged { return array; }
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this Span<T> span) where T:unmanaged { return span; }


        public static System.IO.DirectoryInfo FindDirectoryTree(this System.IO.DirectoryInfo initial, params string[] path)
        {
            while(initial != null)
            {
                var probePath = initial.DefineDirectoryInfo(path);
                if (System.IO.Directory.Exists(probePath.FullName)) return probePath;

                initial = initial.Parent;
            }

            return null;
        }
    }
}
