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
        public static IEnumerable<float> Enumerate(this Matrix3x2 matrix)
        {
            yield return matrix.M11;
            yield return matrix.M12;
            yield return matrix.M21;
            yield return matrix.M22;
            yield return matrix.M31;
            yield return matrix.M32;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static IEnumerable<float> Enumerate(this Matrix4x4 matrix)
        {
            yield return matrix.M11;
            yield return matrix.M12;
            yield return matrix.M13;
            yield return matrix.M14;

            yield return matrix.M21;
            yield return matrix.M22;
            yield return matrix.M23;
            yield return matrix.M24;

            yield return matrix.M31;
            yield return matrix.M32;
            yield return matrix.M33;
            yield return matrix.M34;

            yield return matrix.M41;
            yield return matrix.M42;
            yield return matrix.M43;
            yield return matrix.M44;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static IEnumerable<Matrix3x2> SelectMatrix3x2(this IEnumerable<float> source)
        {
            using (var ctor = source.GetEnumerator())
            {
                while (ctor.MoveNext())
                {
                    var m11 = ctor.Current;
                    var m12 = ctor.__RequireNext();
                    var m21 = ctor.__RequireNext();
                    var m22 = ctor.__RequireNext();
                    var m31 = ctor.__RequireNext();
                    var m32 = ctor.__RequireNext();

                    yield return new Matrix3x2(m11, m12, m21, m22, m31, m32);
                }
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static IEnumerable<Matrix4x4> SelectMatrix4x4(this IEnumerable<float> source)
        {
            using (var ctor = source.GetEnumerator())
            {
                while (ctor.MoveNext())
                {
                    var m11 = ctor.Current;
                    var m12 = ctor.__RequireNext();
                    var m13 = ctor.__RequireNext();
                    var m14 = ctor.__RequireNext();
                    var m21 = ctor.__RequireNext();
                    var m22 = ctor.__RequireNext();
                    var m23 = ctor.__RequireNext();
                    var m24 = ctor.__RequireNext();
                    var m31 = ctor.__RequireNext();
                    var m32 = ctor.__RequireNext();
                    var m33 = ctor.__RequireNext();
                    var m34 = ctor.__RequireNext();
                    var m41 = ctor.__RequireNext();
                    var m42 = ctor.__RequireNext();
                    var m43 = ctor.__RequireNext();
                    var m44 = ctor.__RequireNext();

                    yield return new Matrix4x4(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
                }
            }
        }
    }
}
