// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;


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
        public static Dictionary<TKey,TValue> ToDicionary<TKey,TValue>(this IEnumerable<KeyValuePair<TKey,TValue>> kvPairs)
        {
            return kvPairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dst, IEnumerable<KeyValuePair<TKey, TValue>> src)
        {
            foreach (var kvp in src)
            {
                dst.Add(kvp.Key, kvp.Value);
            }
        }

    }
}
