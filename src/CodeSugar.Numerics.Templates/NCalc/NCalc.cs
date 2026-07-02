using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.InteropServices;
using System.Text;
using System.Buffers;
using System.Reflection;
using System.Transactions;
using System.Threading;
using System.Threading.Tasks;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarNumericsExtensions
    {
        public static T Evaluate<T>(this NCalc.Expression expression)
        {
            return _CastNCalcValue<T>(expression.Evaluate());
        }

        public static async Task<T> EvaluateAsync<T>(this NCalc.Expression expression, CancellationToken token = default)
        {
            var r = await expression.EvaluateAsync(token).ConfigureAwait(false);
            return _CastNCalcValue<T>(r);
        }


        private static T _CastNCalcValue<T>(object result)
        {
            switch (result)
            {
                case null: return default;
                case T exact: return exact;
                case IConvertible cvt when typeof(IConvertible).IsAssignableFrom(typeof(T)): return _CastNCalcConvertible<T>(cvt);
                case IReadOnlyList<object> array: return _CastNCalcArray<T>(array);                
            }

            throw new InvalidOperationException($"Can't cast {result} to {typeof(T).Name}");
        }

        private static T _CastNCalcConvertible<T>(IConvertible cvt)
        {
            var t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;

            return Convert.ChangeType(cvt, t, System.Globalization.CultureInfo.InvariantCulture) is T converted
                ? converted
                : throw new FormatException($"Can't cast {cvt} to {typeof(T).Name}");
        }

        private static T _CastNCalcArray<T>(IReadOnlyList<Object> collection)
        {
            if (collection is T exact) return exact;

            if (typeof(T) == typeof(List<int>))
            {
                return (T)(object)new List<int>(collection.Select(item => _CastNCalcValue<int>(item)));
            }

            if (typeof(T) == typeof(List<float>))
            {
                return (T)(object)new List<float>(collection.Select(item => _CastNCalcValue<float>(item)));
            }

            if (typeof(T) == typeof(IReadOnlyList<int>))
            {
                var result = new int[collection.Count];
                for (int i = 0; i < result.Length; ++i)
                {
                    result[i] = _CastNCalcValue<int>(collection[i]);
                }

                return (T)(object)result;
            }            

            if (typeof(T) == typeof(IReadOnlyList<float>))
            {
                var result = new float[collection.Count];
                for (int i = 0; i < result.Length; ++i)
                {
                    result[i] = _CastNCalcValue<float>(collection[i]);
                }

                return (T)(object)result;
            }

            if (typeof(T) == typeof((int, int)))
            {
                System.Diagnostics.Debug.Assert(collection.Count == 2, "Expected an array of 2 items");
                var maxidx = collection.Count - 1;
                var x = _CastNCalcValue<int>(collection[Math.Min(0, maxidx)]);
                var y = _CastNCalcValue<int>(collection[Math.Min(1, maxidx)]);
                return (T)(object)(x, y);
            }

            if (typeof(T) == typeof((int, int, int)))
            {
                System.Diagnostics.Debug.Assert(collection.Count == 3, "Expected an array of 3 items");
                var maxidx = collection.Count - 1;
                var x = _CastNCalcValue<int>(collection[Math.Min(0, maxidx)]);
                var y = _CastNCalcValue<int>(collection[Math.Min(1, maxidx)]);
                var z = _CastNCalcValue<int>(collection[Math.Min(2, maxidx)]);
                return (T)(object)(x, y, z);
            }

            if (typeof(T) == typeof((int, int, int, int)))
            {
                System.Diagnostics.Debug.Assert(collection.Count == 4, "Expected an array of 4 items");
                var maxidx = collection.Count - 1;
                var x = _CastNCalcValue<int>(collection[Math.Min(0, maxidx)]);
                var y = _CastNCalcValue<int>(collection[Math.Min(1, maxidx)]);
                var z = _CastNCalcValue<int>(collection[Math.Min(2, maxidx)]);
                var w = _CastNCalcValue<int>(collection[Math.Min(3, maxidx)]);
                return (T)(object)(x, y, z, w);
            }

            if (typeof(T) == typeof((float, float)))
            {
                System.Diagnostics.Debug.Assert(collection.Count == 2, "Expected an array of 2 items");
                var maxidx = collection.Count - 1;
                var x = _CastNCalcValue<float>(collection[Math.Min(0, maxidx)]);
                var y = _CastNCalcValue<float>(collection[Math.Min(1, maxidx)]);
                return (T)(object)(x, y);
            }

            if (typeof(T) == typeof((float, float, float)))
            {
                System.Diagnostics.Debug.Assert(collection.Count == 3, "Expected an array of 3 items");
                var maxidx = collection.Count - 1;
                var x = _CastNCalcValue<float>(collection[Math.Min(0, maxidx)]);
                var y = _CastNCalcValue<float>(collection[Math.Min(1, maxidx)]);
                var z = _CastNCalcValue<float>(collection[Math.Min(2, maxidx)]);
                return (T)(object)(x, y, z);
            }

            if (typeof(T) == typeof((float, float, float, float)))
            {
                System.Diagnostics.Debug.Assert(collection.Count == 4, "Expected an array of 4 items");
                var maxidx = collection.Count - 1;
                var x = _CastNCalcValue<float>(collection[Math.Min(0, maxidx)]);
                var y = _CastNCalcValue<float>(collection[Math.Min(1, maxidx)]);
                var z = _CastNCalcValue<float>(collection[Math.Min(2, maxidx)]);
                var w = _CastNCalcValue<float>(collection[Math.Min(3, maxidx)]);
                return (T)(object)(x, y, z, w);
            }

            if (typeof(T) == typeof(Vector2))
            {
                System.Diagnostics.Debug.Assert(collection.Count == 2, "Expected an array of 2 items");
                var maxidx = collection.Count - 1;
                var x = _CastNCalcValue<float>(collection[Math.Min(0, maxidx)]);
                var y = _CastNCalcValue<float>(collection[Math.Min(1, maxidx)]);
                return (T)(object)new Vector2(x, y);
            }

            if (typeof(T) == typeof(Vector3))
            {
                System.Diagnostics.Debug.Assert(collection.Count == 3, "Expected an array of 3 items");
                var maxidx = collection.Count - 1;
                var x = _CastNCalcValue<float>(collection[Math.Min(0, maxidx)]);
                var y = _CastNCalcValue<float>(collection[Math.Min(1, maxidx)]);
                var z = _CastNCalcValue<float>(collection[Math.Min(2, maxidx)]);
                return (T)(object)new Vector3(x, y, z);
            }

            if (typeof(T) == typeof(Vector4))
            {
                System.Diagnostics.Debug.Assert(collection.Count == 4, "Expected an array of 4 items");
                var maxidx = collection.Count - 1;
                var x = _CastNCalcValue<float>(collection[Math.Min(0, maxidx)]);
                var y = _CastNCalcValue<float>(collection[Math.Min(1, maxidx)]);
                var z = _CastNCalcValue<float>(collection[Math.Min(2, maxidx)]);
                var w = _CastNCalcValue<float>(collection[Math.Min(3, maxidx)]);
                return (T)(object)new Vector4(x, y, z, w);
            }

            if (typeof(T) == typeof(Quaternion))
            {
                System.Diagnostics.Debug.Assert(collection.Count == 4, "Expected an array of 4 items");
                var maxidx = collection.Count - 1;
                var x = _CastNCalcValue<float>(collection[Math.Min(0, maxidx)]);
                var y = _CastNCalcValue<float>(collection[Math.Min(1, maxidx)]);
                var z = _CastNCalcValue<float>(collection[Math.Min(2, maxidx)]);
                var w = _CastNCalcValue<float>(collection[Math.Min(3, maxidx)]);
                return (T)(object)new Quaternion(x, y, z, w);
            }

            throw new NotImplementedException($"Cant convert float[] to {typeof(T).Name}");
        }

    }
}
