using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarNumericsExtensions
    {
        public static void DivideSpanBy(this Span<float> dividend, float divisor)
        {
            #if __REFERENCES_SYSTEMNUMERICSTENSORS
            System.Numerics.Tensors.TensorPrimitives.Divide(dividend, divisor, dividend);
            #else
            for(int i=0; i < dividend.Length; ++i)
            {
                dividend[i] /= divisor;
            }
            #endif
        }

        public static void MultiplySpanBy(this Span<float> dst, float scalar)
        {
            #if __REFERENCES_SYSTEMNUMERICSTENSORS
            System.Numerics.Tensors.TensorPrimitives.Multiply(dst, scalar, dst);
            #else
            for(int i=0; i < dst.Length; ++i)
            {
                dst[i] *= scalar;
            }
            #endif
        }

        public static void AddSpanTo(this ReadOnlySpan<float> src, Span<float> dst)
        {
            #if __REFERENCES_SYSTEMNUMERICSTENSORS
            System.Numerics.Tensors.TensorPrimitives.Add(src, dst, dst);
            #else
            for(int i=0; i < dst.Length; ++i)
            {
                dst[i] += src[i];
            }
            #endif
        }
        

        public static float EuclideanDistanceTo<TVector1,TVector2>(this TVector1 a, TVector2 b)
            where TVector1: IReadOnlyList<float>
            where TVector2: IReadOnlyList<float>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            System.Diagnostics.Debug.Assert(a.Count == b.Count, "Both vectors must have the same size");

            #if __REFERENCES_SYSTEMNUMERICSTENSORS
            if (_TryGetReadOnlySpan<TVector1, float>(a,out var aa) && _TryGetReadOnlySpan<TVector2, float>(b, out var bb))
            {
                return EuclideanDistanceTo(aa, bb);
            }
            #endif

            if (a.Count != b.Count) throw new ArgumentOutOfRangeException(nameof(b), "Both vectors must have the same size");
            double result = 0;
            for (int i = 0; i < a.Count; i++)
            {
                var x = a[i] - b[i];
                result += x * x;
            }

            return (float)Math.Sqrt(result);
        }

        public static float EuclideanDistanceTo(this Span<float> a, ReadOnlySpan<float> b)
        {
            return EuclideanDistanceTo((ReadOnlySpan<float>)a, (ReadOnlySpan<float>)b);
        }

        public static float EuclideanDistanceTo(this ReadOnlySpan<float> a, ReadOnlySpan<float> b)
        {
            #if __REFERENCES_SYSTEMNUMERICSTENSORS
            return System.Numerics.Tensors.TensorPrimitives.Distance(a, b);
            #else

            if (a.Length != b.Length) throw new ArgumentOutOfRangeException(nameof(b), "Both vectors must have the same size");
            double result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                var x = a[i] - b[i];
                result += x * x;
            }

            return (float)Math.Sqrt(result);

            #endif
        }
    }
}