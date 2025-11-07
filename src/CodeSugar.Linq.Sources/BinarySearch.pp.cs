// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

#nullable disable

#if !NETSTANDARD
using __UNSAFE = System.Runtime.CompilerServices.Unsafe;
#endif

using __METHODOPTIONS = System.Runtime.CompilerServices.MethodImplOptions;

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
        public static int BinarySearch<T>(IReadOnlyList<T> collection, T pivot)
            where T : IComparable
        {
            return BinarySearch(collection, item => item, pivot);
        }        

        public static int BinarySearch<T, TComparable>(IReadOnlyList<T> collection, Func<T, TComparable> selector, TComparable pivot)
            where TComparable : IComparable
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (selector == null) throw new ArgumentNullException(nameof(collection));
            if (pivot == null) throw new ArgumentNullException(nameof(pivot));

            int lo = 0;
            int hi = collection.Count - 1;

            // If length == 0, hi == -1, and loop will not be entered

            while (lo <= hi)
            {
                // PERF: `lo` or `hi` will never be negative inside the loop,
                //       so computing median using uints is safe since we know
                //       `length <= int.MaxValue`, and indices are >= 0
                //       and thus cannot overflow an uint.
                //       Saves one subtraction per loop compared to
                //       `int i = lo + ((hi - lo) >> 1);`

                int i = (int)(((uint)hi + (uint)lo) >> 1);

                int c = pivot.CompareTo(selector(collection[i]));

                if (c == 0) return i;
                else if (c > 0) lo = i + 1;
                else hi = i - 1;
            }

            // If none found, then a negative number that is the bitwise complement
            // of the index of the next element that is larger than or, if there is
            // no larger element, the bitwise complement of `length`, which
            // is `lo` at this point.
            return ~lo;
        }

        public static int BinarySearch<T>(IReadOnlyList<T> collection, T pivot, IComparer<T> comparer)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));            
            if (pivot == null) throw new ArgumentNullException(nameof(pivot));
            if (comparer == null) throw new ArgumentNullException(nameof(collection));

            int lo = 0;
            int hi = collection.Count - 1;

            // If length == 0, hi == -1, and loop will not be entered

            while (lo <= hi)
            {
                // PERF: `lo` or `hi` will never be negative inside the loop,
                //       so computing median using uints is safe since we know
                //       `length <= int.MaxValue`, and indices are >= 0
                //       and thus cannot overflow an uint.
                //       Saves one subtraction per loop compared to
                //       `int i = lo + ((hi - lo) >> 1);`

                int i = (int)(((uint)hi + (uint)lo) >> 1);

                int c = comparer.Compare(pivot, collection[i]);

                if (c == 0) return i;
                else if (c > 0) lo = i + 1;
                else hi = i - 1;
            }

            // If none found, then a negative number that is the bitwise complement
            // of the index of the next element that is larger than or, if there is
            // no larger element, the bitwise complement of `length`, which
            // is `lo` at this point.
            return ~lo;
        }
    }
}
