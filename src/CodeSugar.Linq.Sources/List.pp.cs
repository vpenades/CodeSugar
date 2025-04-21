// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

#nullable disable

#if !NETSTANDARD
using _UNSAFE = System.Runtime.CompilerServices.Unsafe;
#endif

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
        #region indexing

        public static int IndexOf<TCollection, T>(this TCollection collection, Predicate<T> predicate)
            where TCollection : IReadOnlyList<T>
        {
            switch (collection)
            {
                case null: return -1;

                case T[] array:
                    return Array.FindIndex(array, predicate);

                case ArraySegment<T> segment:
                    if (segment.Count == 0) return -1;
                    var idx = Array.FindIndex(segment.Array, segment.Offset, segment.Count, predicate);
                    return idx < 0 ? -1 : idx - segment.Offset;

                case List<T> list:
                    return list.FindIndex(predicate);
            }

            // fallback            

            for (int i = 0; i < collection.Count; ++i)
            {
                if (predicate(collection[i])) return i;
            }

            return -1;
        }

        public static int IndexOf<TCollection, T>(this TCollection collection, T value, System.Collections.Generic.EqualityComparer<T> comparer = null)
            where TCollection : IReadOnlyList<T>
        {
            switch (collection)
            {
                case null: return -1;                

                case T[] array:
                    return comparer == null
                        ? Array.IndexOf(array, value)
                        : Array.FindIndex(array, item => comparer.Equals(item, value));

                case ArraySegment<T> segment:
                    if (segment.Count == 0) return -1;
                    var idx = comparer == null
                        ? Array.IndexOf(segment.Array, value, segment.Offset, segment.Count)
                        : Array.FindIndex(segment.Array, segment.Offset, segment.Count, item => comparer.Equals(item, value));
                    return idx < 0 ? -1 : idx - segment.Offset;

                case List<T> list:
                    return comparer == null
                        ? list.IndexOf(value)
                        : list.FindIndex(item => comparer.Equals(item, value));
            }

            // fallback

            comparer ??= System.Collections.Generic.EqualityComparer<T>.Default;

            for (int i = 0; i < collection.Count; ++i)
            {
                if (comparer.Equals(collection[i], value)) return i;
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AtLoop<T>(this T[] collection, int idx)
        {
            var len = collection.Length;
            idx = idx >= 0 ? idx % len : len - (-idx - 1) % len - 1;
            return collection[idx];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AtLoop<T>(this ArraySegment<T> collection, int idx)
        {
            var len = collection.Count;
            idx = idx >= 0 ? idx % len : len - (-idx - 1) % len - 1;
            return collection[idx];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AtLoop<T>(this Span<T> collection, int idx)
        {
            var len = collection.Length;
            idx = idx >= 0 ? idx % len : len - (-idx - 1) % len - 1;
            return collection[idx];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AtLoop<T>(this ReadOnlySpan<T> collection, int idx)
        {
            var len = collection.Length;
            idx = idx >= 0 ? idx % len : len - (-idx - 1) % len - 1;
            return collection[idx];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AtLoop<T>(this List<T> collection, int idx) // we cannot use IList<T> because it conflicts with IReadOnlyList<T>
        {
            var len = collection.Count;
            idx = idx >= 0 ? idx % len : len - (-idx - 1) % len - 1;
            return collection[idx];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AtLoop<T>(this IReadOnlyList<T> collection, int idx)
        {
            var len = collection.Count;
            idx = idx >= 0 ? idx % len : len - (-idx - 1) % len - 1;
            return collection[idx];
        }

        #endregion

        #region casting

        public static bool TryCastToSpan<TCollection, T>(this TCollection collection, out ReadOnlySpan<T> span)
            where TCollection: IReadOnlyList<T>
        {
            switch(collection)
            {
                case null: span = default; return false;
                case T[] array: span = array; return true;                
                case ArraySegment<T> segment: span = segment; return true;
                case List<T> list:
                    #if NET6_0_OR_GREATER
                    span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
                    return true;
                    #else
                    break;
                    #endif
            }

            span = default;
            return false;
        }

        #endregion

        #region transfer

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
            if (src == null) return;

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

        #endregion

        #region arrays

        public static IReadOnlyList<T> Slice<T>(this IReadOnlyList<T> list, int offset)
        {
            return Slice<T>(list, offset, list.Count - offset);
        }
        public static IReadOnlyList<T> Slice<T>(this IReadOnlyList<T> list, int offset, int count)
        {
            switch (list)
            {
                case null: return Array.Empty<T>();
                case T[] array: return new ArraySegment<T>(array, offset, count);
                case ArraySegment<T> segment: return segment.Slice(offset, count);
                case List<T> xlist: return new _SliceReadOnlyList<T, List<T>>(xlist, offset, count);

                case _SliceReadOnlyList<T, List<T>> slice: return slice.Slice(offset, count);
                case _SliceReadOnlyList<T, IReadOnlyList<T>> slice: return slice.Slice(offset, count);
                    
                default: return new _SliceReadOnlyList<T, IReadOnlyList<T>>(list, offset, count);
            }
        }

        #endregion

        #region Linq

        public static IList<TResult> SelectList<TSource, TResult>(this IList<TSource> collection, Func<TSource, TResult> getter, Func<TResult, TSource> setter)
        {
            if (getter == null) throw new ArgumentNullException(nameof(getter));
            if (setter == null) throw new ArgumentNullException(nameof(setter));

            switch (collection)
            {
                case null: return Array.Empty<TResult>();
                case TSource[] array: return new _SelectWriteableList<TSource, TResult, TSource[]>(array, getter, setter);
                case ArraySegment<TSource> segment: return new _SelectWriteableList<TSource, TResult, ArraySegment<TSource>>(segment, getter, setter);
                case List<TSource> list: return new _SelectWriteableList<TSource, TResult, List<TSource>>(list, getter, setter);
                default: return new _SelectWriteableList<TSource, TResult, IList<TSource>>(collection, getter, setter);
            }
        }

        #if NET9_0_OR_GREATER
        [System.Runtime.CompilerServices.OverloadResolutionPriority(1)]
        #endif
        public static IReadOnlyList<TResult> SelectList<TSource, TResult>(this IReadOnlyList<TSource> collection, Func<TSource, TResult> getter)
        {            
            if (getter == null) throw new ArgumentNullException(nameof(getter));            

            switch (collection)
            {
                case null: return Array.Empty<TResult>();
                case TSource[] array: return new _SelectReadOnlyList<TSource, TResult, TSource[]>(array, getter);
                case ArraySegment<TSource> segment: return new _SelectReadOnlyList<TSource, TResult, ArraySegment<TSource>>(segment, getter);
                case List<TSource> list: return new _SelectReadOnlyList<TSource, TResult, List<TSource>>(list, getter);
                default: return new _SelectReadOnlyList<TSource, TResult, IReadOnlyList<TSource>>(collection, getter);
            }            
        }

        public static IReadOnlyCollection<TResult> SelectCollection<TSource, TResult>(this IReadOnlyCollection<TSource> collection, Func<TSource, TResult> selector)
        {            
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            if (collection == null) return Array.Empty<TResult>();

            return new _SelectCollection<TSource, TResult>(collection, selector);
        }

        #endregion

        #region nested types

        /// <summary>
        /// Helper class for <see cref="Slice{T}(IReadOnlyList{T}, int, int)"/>
        /// </summary>        
        private readonly struct _SliceReadOnlyList<T, TList> : IReadOnlyList<T>
            where TList : IReadOnlyList<T>
        {
            #region constructor

            public _SliceReadOnlyList(TList list, int offset, int count)
            {
                if (list == null) throw new ArgumentNullException(nameof(list));
                if ((uint)offset > (uint)list.Count || (uint)count > (uint)(list.Count - offset))
                {
                    if (offset < 0 || offset >= list.Count) throw new ArgumentOutOfRangeException(nameof(offset));
                    throw new ArgumentOutOfRangeException(nameof(count));
                }

                _List = list;
                _Offset = offset;
                _Count = count;
            }

            #endregion

            #region data

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly TList _List;
            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly int _Offset;
            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly int _Count;

            #endregion

            #region API

            public T this[int index]
            {
                get
                {
                    index -= _Offset;
                    if (index < 0 || index >= _Count) throw new ArgumentOutOfRangeException(nameof(index));
                    return _List[index];
                }
            }

            public int Count => _Count;

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < _Count; ++i) yield return _List[i + _Offset];
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                for (int i = 0; i < _Count; ++i) yield return _List[i + _Offset];
            }

            public _SliceReadOnlyList<T, TList> Slice(int offset, int count)
            {
                return new _SliceReadOnlyList<T, TList>(_List, _Offset + offset, count);
            }

            #endregion
        }

        /// <summary>
        /// Helper class for <see cref="SelectList{TSource, TResult}(IReadOnlyList{TSource}, Func{TSource, TResult})"/>
        /// </summary>        
        private readonly struct _SelectReadOnlyList<TSource, TResult, TList> : IReadOnlyList<TResult>
            where TList : IReadOnlyList<TSource>
        {
            #region constructor
            public _SelectReadOnlyList(TList list, Func<TSource, TResult> getter)
            {
                _List = list;
                _Getter = getter;
            }
            #endregion

            #region data

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            private readonly TList _List;
            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly Func<TSource, TResult> _Getter;
            #endregion

            #region API

            public TResult this[int index] => _Getter(_List[index]);

            public int Count => _List.Count;

            public IEnumerator<TResult> GetEnumerator()
            {
                foreach (var item in _List) yield return _Getter(item);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                foreach (var item in _List) yield return _Getter(item);
            }

            #endregion
        }

        /// <summary>
        /// Helper class for <see cref="SelectList{TSource, TResult}(IList{TSource}, Func{TSource, TResult}, Func{TResult, TSource})"/>
        /// </summary>
        private readonly struct _SelectWriteableList<TSource, TResult, TList> : IList<TResult>, IReadOnlyList<TResult>
            where TList : IList<TSource>
        {
            #region constructor
            public _SelectWriteableList(TList list, Func<TSource, TResult> getter, Func<TResult, TSource> setter)
            {
                _List = list;
                _Getter = getter;
                _Setter = setter;
            }
            #endregion

            #region data

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            private readonly TList _List;
            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly Func<TSource, TResult> _Getter;
            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly Func<TResult, TSource> _Setter;
            #endregion

            #region API

            public TResult this[int index]
            {
                get => _Getter(_List[index]);
                set => _List[index] = _Setter(value);
            }

            public bool IsReadOnly => false;

            public int Count => _List.Count;

            public IEnumerator<TResult> GetEnumerator()
            {
                foreach (var item in _List) yield return _Getter(item);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                foreach (var item in _List) yield return _Getter(item);
            }

            public int IndexOf(TResult item)
            {
                var src = _Setter(item);
                return _List.IndexOf(src);
            }

            public bool Contains(TResult item) { return IndexOf(item) >= 0; }

            public void Insert(int index, TResult item)
            {
                var src = _Setter(item);
                _List.Insert(index, src);
            }

            public void RemoveAt(int index)
            {
                _List.RemoveAt(index);
            }

            public bool Remove(TResult item)
            {
                var src = _Setter(item);
                return _List.Remove(src);
            }

            public void Add(TResult item)
            {
                var src = _Setter(item);
                _List.Add(src);
            }

            public void Clear()
            {
                _List.Clear();
            }

            public void CopyTo(TResult[] array, int arrayIndex)
            {
                if (array == null) throw new ArgumentNullException(nameof(array));
                if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

                var count = Math.Min(_List.Count, array.Length - arrayIndex);

                for (int i = 0; i < count; ++i)
                {
                    array[i + arrayIndex] = this[i];
                }
            }

            #endregion
        }

        /// <summary>
        /// helper class for <see cref="SelectCollection{TSource, TResult}(IReadOnlyCollection{TSource}, Func{TSource, TResult})"/>
        /// </summary>        
        private readonly struct _SelectCollection<TSource, TResult> : IReadOnlyCollection<TResult>
        {
            #region constructor
            public _SelectCollection(IReadOnlyCollection<TSource> list, Func<TSource, TResult> selector)
            {
                _List = list;
                _Selector = selector;
            }
            #endregion

            #region data

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            private readonly IReadOnlyCollection<TSource> _List;
            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly Func<TSource, TResult> _Selector;

            #endregion

            #region API

            public int Count => _List.Count;

            public IEnumerator<TResult> GetEnumerator()
            {
                foreach (var item in _List) yield return _Selector(item);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                foreach (var item in _List) yield return _Selector(item);
            }

            #endregion
        }

        #endregion
    }
}
