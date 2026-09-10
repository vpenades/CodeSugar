using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarLinqExtensions
    {
        public static bool TryGetNext<T>(this IEnumerator<T> ctor, out T current)
        {
            if (ctor == null) throw new ArgumentNullException(nameof(ctor));
            if (ctor.MoveNext()) { current = ctor.Current; return true; }
            else { current = default; return false; }
        }

        public static T GetNext<T>(this IEnumerator<T> ctor)
        {
            if (ctor == null) throw new ArgumentNullException(nameof(ctor));
            if (ctor.MoveNext()) return ctor.Current;
            throw new ArgumentException("unexpected end of collection", nameof(ctor));
        }

    }
}
