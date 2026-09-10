using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

        private static float __RequireNext(this IEnumerator<float> ctor)
        {
            if (!ctor.MoveNext()) throw new ArgumentException("unexpected end of collection");
            return ctor.Current;
        }

        private static string __ToBase64String(System.Collections.Generic.IEnumerable<float> srcElements, Span<byte> dstSpan)
        {
            __ToBase64Span(srcElements, dstSpan);

            return System.Convert.ToBase64String(dstSpan);
        }

        private static void __ToBase64Span(System.Collections.Generic.IEnumerable<float> srcElements, Span<byte> dstSpan)
        {
            var t = dstSpan;
            foreach (var element in srcElements)
            {
                if (t.Length < 4) break;
                #if NET
                System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(t, element);
                #else
                __WriteSingleLittleEndian(t, element);
                #endif
                t = t.Slice(4);
            }
        }

        #if !NET
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void __WriteSingleLittleEndian(Span<byte> destination, float value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                int tmp = __ReverseEndianness(BitConverter.SingleToInt32Bits(value));
                MemoryMarshal.Write(destination, ref tmp);
            }
            else
            {
                MemoryMarshal.Write(destination, ref value);
            }
        }

        private static T __ReverseEndianness<T>(T value) where T:unmanaged
        {
            Span<byte> tbytes = stackalloc byte[Unsafe.SizeOf<T>()];
            var tval = System.Runtime.InteropServices.MemoryMarshal.Cast<byte, T>(tbytes);

            tval[0] = value;
            tbytes.Reverse();
            return tval[0];            
        }        

        #endif
    }
}
