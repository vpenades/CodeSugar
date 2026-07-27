using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarLinqExtensions
    {
        public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> collection, Func<TSource, TKey> sortKey, System.ComponentModel.ListSortDirection direction)
        {
            switch (direction)
            {
                case System.ComponentModel.ListSortDirection.Ascending: return collection.OrderBy(sortKey);
                case System.ComponentModel.ListSortDirection.Descending: return collection.OrderByDescending(sortKey);
                default: throw new ArgumentException("unsupported", nameof(direction));
            }
        }

        public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> collection, Func<TSource, TKey> sortKey, IComparer<TKey> comparer, System.ComponentModel.ListSortDirection direction)
        {
            switch (direction)
            {
                case System.ComponentModel.ListSortDirection.Ascending: return collection.OrderBy(sortKey, comparer);
                case System.ComponentModel.ListSortDirection.Descending: return collection.OrderByDescending(sortKey, comparer);
                default: throw new ArgumentException("unsupported", nameof(direction));
            }
        }
    }
}
