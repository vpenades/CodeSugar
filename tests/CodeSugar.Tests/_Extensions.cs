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

        /// <summary>
        /// Extension method for IsEquivalentToAssertion.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Collection equivalency uses structural comparison for complex objects, which requires reflection and is not compatible with AOT")]
        public static IsEquivalentToAssertion<TCollection, TItem> IsSequenceEqualTo<TCollection, TItem>(this IAssertionSource<TCollection> source, System.Collections.Generic.IEnumerable<TItem> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
            where TCollection : System.Collections.Generic.IEnumerable<TItem>
        {
            source.Context.ExpressionBuilder.Append(".IsEquivalentTo(");
            var added = false;
            if (expectedExpression != null)
            {
                source.Context.ExpressionBuilder.Append(added ? ", " : "");
                source.Context.ExpressionBuilder.Append(expectedExpression);
                added = true;
            }            
            source.Context.ExpressionBuilder.Append(")");
            return new IsEquivalentToAssertion<TCollection, TItem>(source.Context, expected, CollectionOrdering.Matching);
        }


        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ArraySegment<T> segment) where T : unmanaged { return segment; }
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array) where T : unmanaged { return array; }
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this Span<T> span) where T:unmanaged { return span; }


        public static System.IO.DirectoryInfo? FindDirectoryTree(this System.IO.DirectoryInfo? initial, params string[] path)
        {
            while(initial != null)
            {
                var probePath = initial.DefineDirectoryInfo(path);
                if (System.IO.Directory.Exists(probePath.FullName)) return probePath;

                initial = initial.Parent;
            }

            return null;
        }

        public static string ToText<T>(this IEnumerable<T> collection)
        {
            return collection.Aggregate(string.Empty, (a, b) => a + b + ", ");
        }

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
