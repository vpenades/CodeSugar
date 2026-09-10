using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarNumericsExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static IEnumerable<float> Enumerate(this Vector2 v)
        {
            yield return v.X;
            yield return v.Y;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static IEnumerable<float> Enumerate(this Vector3 v)
        {
            yield return v.X;
            yield return v.Y;
            yield return v.Z;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static IEnumerable<float> Enumerate(this Vector4 v)
        {
            yield return v.X;
            yield return v.Y;
            yield return v.Z;
            yield return v.W;
        }


        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static IEnumerable<Vector2> SelectVector2(this IEnumerable<float> source)
        {
            using(var ctor = source.GetEnumerator())
            {
                while(ctor.MoveNext())
                {
                    var x = ctor.Current;
                    var y = ctor.__RequireNext();
                    yield return new Vector2(x, y);
                }
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static IEnumerable<Vector3> SelectVector3(this IEnumerable<float> source)
        {
            using (var ctor = source.GetEnumerator())
            {
                while (ctor.MoveNext())
                {
                    var x = ctor.Current;
                    var y = ctor.__RequireNext();
                    var z = ctor.__RequireNext();
                    yield return new Vector3(x, y, z);
                }
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static IEnumerable<Vector4> SelectVector4(this IEnumerable<float> source)
        {
            using (var ctor = source.GetEnumerator())
            {
                while (ctor.MoveNext())
                {
                    var x = ctor.Current;
                    var y = ctor.__RequireNext();
                    var z = ctor.__RequireNext();
                    var w = ctor.__RequireNext();
                    yield return new Vector4(x, y, z, w);
                }
            }
        }
    }
}
