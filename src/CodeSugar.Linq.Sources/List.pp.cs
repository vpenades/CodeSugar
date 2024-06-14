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
        public static IReadOnlyList<TResult> ListSelect<TSource, TResult>(this IReadOnlyList<TSource> collection, Func<TSource, TResult> selector)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return new _ListSelect<TSource, TResult>(collection, selector);
        }

        public static IReadOnlyCollection<TResult> CollectionSelect<TSource, TResult>(this IReadOnlyCollection<TSource> collection, Func<TSource, TResult> selector)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return new _CollectionSelect<TSource, TResult>(collection, selector);
        }
        
        private readonly struct _ListSelect<TSource, TResult> : IReadOnlyList<TResult>
        {
            public _ListSelect(IReadOnlyList<TSource> list, Func<TSource, TResult> selector)
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

        private readonly struct _CollectionSelect<TSource, TResult> : IReadOnlyCollection<TResult>
        {
            public _CollectionSelect(IReadOnlyCollection<TSource> list, Func<TSource, TResult> selector)
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
