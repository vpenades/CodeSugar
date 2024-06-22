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
    }
}
