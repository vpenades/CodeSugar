using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarNumericsExtensions
    {
        private static bool _IsDefaultOrEmpty<TCollection,TElement>(TCollection collection)
            where TCollection: IReadOnlyList<TElement>
        {
            #if NET
            if (collection is System.Collections.Immutable.ImmutableArray<TElement> immutable && immutable.IsDefault) return true;
            #endif

            return collection == null || collection.Count == 0;
        }

        private static bool _TryGetReadOnlySpan<TCollection, TElement>(TCollection collection, out ReadOnlySpan<TElement> span)
            where TCollection : IReadOnlyList<TElement>
            where TElement : unmanaged
        {
            switch (collection)
            {
                case TElement[] array: span = array; return true;
                case ArraySegment<TElement> array: span = array; return true;

                #if NET8_0_OR_GREATER

                case System.Collections.Immutable.ImmutableArray<TElement> array: span = array.AsSpan(); return true;

                case List<TElement> list:
                    {
                        span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
                        return true;
                    }

                #endif

                default: span = default; return false;
            }
        }
    }
}
