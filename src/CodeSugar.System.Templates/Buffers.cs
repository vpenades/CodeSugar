using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarForSystem
    {
        #if !NETSTANDARD2_0
        public static ArraySegment<T> RentSegment<T>(this System.Buffers.ArrayPool<T> pool, int length)
        {
            var array = pool.Rent(length);
            return array == null
                ? default
                : new ArraySegment<T>(array, 0, length);
        }

        public static void ReturnSegment<T>(this System.Buffers.ArrayPool<T> pool, ArraySegment<T> segment)
        {
            pool.Return(segment.Array);
        }
        #endif
    }
}
