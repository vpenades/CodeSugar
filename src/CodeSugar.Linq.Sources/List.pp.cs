// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;


#if !NETSTANDARD
using UNSAFE = System.Runtime.CompilerServices.Unsafe;
#endif

using METHODOPTIONS = System.Runtime.CompilerServices.MethodImplOptions;

#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.Collections.Generic
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarForLinq
    {
        public static void AddRange<T>(this IList<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items) collection.Add(item);
        }

        public static void AddRange<T>(this IList<T> collection, ReadOnlySpan<T> items)
        {
            foreach (var item in items) collection.Add(item);
        }

        public static void CopyTo<T>(this IReadOnlyList<T> src, Span<T> dst)
        {
            CopyTo(src, 0, dst);
        }

        public static void CopyTo<T>(this IReadOnlyList<T> src, int srcIndex, Span<T> dst)
        {
            switch(src)
            {
                #if NET6_0_OR_GREATER
                case List<T> list:
                    {
                        var srcSpan = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
                        srcSpan = srcSpan.Slice(srcIndex, Math.Min(list.Count - srcIndex, dst.Length));
                        srcSpan.CopyTo(dst);
                        break;
                    }
                #endif

                case ArraySegment<T> arraySegment:
                    {
                        var srcSpan = arraySegment.AsSpan();
                        srcSpan = srcSpan.Slice(srcIndex, Math.Min(arraySegment.Count - srcIndex, dst.Length));
                        srcSpan.CopyTo(dst);
                        break;
                    }

                default:
                    {
                        var len = Math.Min(src.Count - srcIndex, dst.Length);
                        for (int i = 0; i < len; i++) { dst[i] = src[i+srcIndex]; }
                        break;
                    }
            }            
        }



        public static IReadOnlyList<TResult> SelectList<TSource, TResult>(this IReadOnlyList<TSource> collection, Func<TSource, TResult> selector)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return new _SelectList<TSource, TResult>(collection, selector);
        }

        public static IReadOnlyCollection<TResult> SelectCollection<TSource, TResult>(this IReadOnlyCollection<TSource> collection, Func<TSource, TResult> selector)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return new _SelectCollection<TSource, TResult>(collection, selector);
        }
        
        private readonly struct _SelectList<TSource, TResult> : IReadOnlyList<TResult>
        {
            public _SelectList(IReadOnlyList<TSource> list, Func<TSource, TResult> selector)
            {
                _List = list;
                _Selector = selector;
            }

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            private readonly IReadOnlyList<TSource> _List;
            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly Func<TSource, TResult> _Selector;

            public TResult this[int index] => _Selector(_List[index]);

            public int Count => _List.Count;

            public IEnumerator<TResult> GetEnumerator()
            {
                foreach (var item in _List) yield return _Selector(item);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                foreach (var item in _List) yield return _Selector(item);
            }
        }

        private readonly struct _SelectCollection<TSource, TResult> : IReadOnlyCollection<TResult>
        {
            public _SelectCollection(IReadOnlyCollection<TSource> list, Func<TSource, TResult> selector)
            {
                _List = list;
                _Selector = selector;
            }

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            private readonly IReadOnlyCollection<TSource> _List;
            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly Func<TSource, TResult> _Selector;

            public int Count => _List.Count;

            public IEnumerator<TResult> GetEnumerator()
            {
                foreach (var item in _List) yield return _Selector(item);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                foreach (var item in _List) yield return _Selector(item);
            }
        }
    }
}
