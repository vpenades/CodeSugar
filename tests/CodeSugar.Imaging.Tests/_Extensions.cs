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
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ArraySegment<T> segment) where T : unmanaged { return segment; }
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array) where T : unmanaged { return array; }
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this Span<T> span) where T:unmanaged { return span; }

        public static IEnumerable<T> Randomize<T>(this IReadOnlyList<T> collection)
        {
            var indices = Enumerable.Range(0, collection.Count).ToList();
            var rnd = new Random();

            while(indices.Count > 0)
            {
                var idx = rnd.Next(indices.Count);
                yield return collection[indices[idx]];

                indices[idx] = indices[indices.Count - 1];
                indices.RemoveAt(indices.Count - 1);
            }
            
        }
    }
}

#pragma warning restore CS8632
