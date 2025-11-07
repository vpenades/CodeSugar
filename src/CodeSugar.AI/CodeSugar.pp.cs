// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using __RTINTEROPSVCS = System.Runtime.InteropServices;


#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System
#else
namespace $rootnamespace$
#endif
{
    [global::System.CodeDom.Compiler.GeneratedCode("CodeSugar.CodeGen", "1.0.0.0")]
    internal static partial class CodeSugarForAI
    {
        private static bool IsDefaultOrEmpty<T>(this IReadOnlyCollection<T> collection)
        {
            #if NET
            if (collection is System.Collections.Immutable.ImmutableArray<T> immutable && immutable.IsDefault) return true;
            #endif

            return collection == null || collection.Count == 0;
        }

        private static bool IsDefaultOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }

        private static ReadOnlySpan<T> AsSpan<T>(this IReadOnlyList<T> collection)
        {
            switch(collection)
            {
                case T[] array: return array.AsSpan();
                case ArraySegment<T> segment: return segment.AsSpan();
                default: return collection.ToArray();
            }
        }


        #if NETSTANDARD

        private static IReadOnlyList<T> ToImmutableArray<T>(this T[] array)
        {
            return array;
        }
        private static IReadOnlyList<T> ToImmutableArray<T>(this IReadOnlyList<T> array)
        {
            return array;
        }        
        #else

        private static float DistanceTo(this Span<float> a, ReadOnlySpan<float> b)
        {
            return System.Numerics.Tensors.TensorPrimitives.Distance(a, b);
        }

        private static float DistanceTo(this ReadOnlySpan<float> a, ReadOnlySpan<float> b)
        {
            return System.Numerics.Tensors.TensorPrimitives.Distance(a, b);
        }

        #endif

        private static float DistanceTo(this IReadOnlyList<float> a, IReadOnlyList<float> b)
        {
            switch (a, b)
            {
                case (float[] aa , float[] bb): return System.Numerics.Tensors.TensorPrimitives.Distance(aa.AsSpan(), bb.AsSpan());
                case (ArraySegment<float> aa, float[] bb): return System.Numerics.Tensors.TensorPrimitives.Distance(aa.AsSpan(), bb.AsSpan());
                case (float[] aa, ArraySegment<float> bb): return System.Numerics.Tensors.TensorPrimitives.Distance(aa.AsSpan(), bb.AsSpan());
                case (ArraySegment<float> aa, ArraySegment<float> bb): return System.Numerics.Tensors.TensorPrimitives.Distance(aa.AsSpan(), bb.AsSpan());

                default:
                    if (a.Count != b.Count) throw new ArgumentException(nameof(b));
                    float result = 0;
                    for (int i = 0; i < a.Count; i++)
                    {
                        var x = a[i] - b[i];                        
                        result += x * x;
                    }

                    return MathF.Sqrt(result);
            }

        }



    }
}
