using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarLinqExtensions
    {
        /// <summary>
        /// Performs an in-place ascending sort of the items.
        /// </summary>
        /// <remarks>
        /// <see cref="List{T}"/> has a <see cref="List{T}.Sort()"/> method, but <see cref="IList{T}"/> interface does not.
        /// </remarks>
        /// <typeparam name="TList"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ilist"></param>
        /// <param name="selector"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void SortAscending<TList, TValue, TProperty>(this TList ilist, Func<TValue, TProperty> selector)
            where TList : IList<TValue>
            where TProperty : IComparable
        {
            var comparer = new _SortAscending<TValue, TProperty>(selector);

            switch (ilist)
            {
                case TValue[] array: Array.Sort(array, comparer); return;
                case List<TValue> list: list.Sort(comparer); return;
                default:
                    var tmp = new TValue[ilist.Count];
                    ilist.CopyTo(tmp, 0);
                    Array.Sort(tmp, comparer);
                    ilist.Clear();
                    ilist.AddRange(tmp);
                    return;
            }
        }        

        /// <summary>
        /// Helper class for <see cref="SortAscending{TList, TValue, TProperty}(TList, Func{TValue, TProperty})"/>
        /// </summary>        
        private readonly struct _SortAscending<TItem, TProperty> : IComparer<TItem>
            where TProperty : IComparable
        {
            public _SortAscending(Func<TItem, TProperty> selector)
            {
                _Selector = selector;
            }

            private readonly Func<TItem, TProperty> _Selector;

            public int Compare(TItem x, TItem y)
            {
                var xx = _Selector(x);
                var yy = _Selector(y);

                return xx.CompareTo(yy);
            }
        }
    }
}