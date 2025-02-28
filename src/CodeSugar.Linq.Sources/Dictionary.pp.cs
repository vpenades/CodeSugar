// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;

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
        public static Dictionary<TKey,TValue> ToDicionary<TKey,TValue>(this IEnumerable<KeyValuePair<TKey,TValue>> kvPairs)
        {
            return kvPairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dst, IEnumerable<KeyValuePair<TKey, TValue>> src)
        {
            if (dst == null) throw new ArgumentNullException(nameof(dst));
            if (src == null) return;

            foreach (var kvp in src)
            {
                dst.Add(kvp.Key, kvp.Value);
            }
        }

        public static bool DictionaryEquals<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> left, IReadOnlyDictionary<TKey, TValue> right, IEqualityComparer<TValue> valueComparer = null)
        {
            if (object.ReferenceEquals(left, right)) return true;
            if (left == null) return false;
            if (right == null) return false;            

            if (left.Count != right.Count) return false;

            if (left.Count == 0) return true;

            valueComparer ??= EqualityComparer<TValue>.Default;

            var keys = left.Keys.Concat(right.Keys).Distinct();            

            foreach (var key in keys)
            {
                if (!left.TryGetValue(key, out var leftValue)) return false;
                if (!right.TryGetValue(key, out var rightValue)) return false;
                if (!valueComparer.Equals(leftValue , rightValue)) return false;
            }

            return true;
        }

    }
}
