using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NET
using System.Collections.Immutable;
#endif

#nullable disable

using __KMEANSPUBLICKEY = System.Collections.Generic.IReadOnlyList<float>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarNumericsExtensions
    {
        // check: https://github.com/facebookresearch/faiss and https://github.com/criteo/autofaiss
        // https://www.nuget.org/packages?q=faiss
        // https://github.com/wlou/HNSW.Net
        // Annoy.NET is a C# port of the Annoy library (Approximate Nearest Neighbors Oh Yeah) originally written in C++. It is designed for efficient similarity search and retrieval.

        /// <summary>
        /// Groups the data based on random seeds.
        /// </summary>
        /// <param name="srcData">The data to group</param>
        /// <param name="k">number of groups to create</param>
        /// <param name="vfunc">Embedded vector evaluator function</param>
        /// <param name="progress">progress reporting</param>
        /// <returns>A collection of <see cref="_KMeansCluster{T}"/> where the key is the item representing the group.</returns>
        public static IGrouping<__KMEANSPUBLICKEY, TValue>[] GroupByRandomKMeans<TValue>(this IReadOnlyList<TValue> srcData, int k, Func<TValue, __KMEANSPUBLICKEY> vfunc, IProgress<int> progress = null)
        {
            progress ??= new Progress<int>(x => { });

            return _KMeansCluster<TValue>.ClusterBuilder.GroupByRandomKMeans(srcData, k, item => new _KMeansKey(vfunc(item)), progress);
        }

        /// <summary>
        /// Groups the data based by first identifying the farthest seeds.
        /// </summary>
        /// <param name="srcData">The data to group</param>
        /// <param name="k">number of groups to create</param>
        /// <param name="vfunc">Embedded vector evaluator function</param>
        /// <param name="progress">progress reporting</param>
        /// <returns>A collection of <see cref="_KMeansCluster{T}"/> where the key is the item representing the group.</returns>
        public static IGrouping<__KMEANSPUBLICKEY, T>[] GroupByFarthestKMeans<T>(this IReadOnlyList<T> srcData, int k, Func<T, __KMEANSPUBLICKEY> vfunc, IProgress<int> progress = null)
        {
            progress ??= new Progress<int>(x => { });

            return _KMeansCluster < T > .ClusterBuilder.GroupByFarthestKMeans(srcData, k, item => new _KMeansKey(vfunc(item)), progress);
        }        

        

        private readonly struct _KMeansKey : IEquatable<_KMeansKey>
        {
            public static _KMeansKey Mean(IEnumerable<_KMeansKey> items)
            {
                float[] accum = null;
                int count = 0;

                foreach(var item in items)
                {
                    if (item.IsEmpty) continue;
                    accum ??= new float[item.PublicKey.Count];

                    item.AddTo(accum);
                    ++count;
                }

                System.Numerics.Tensors.TensorPrimitives.Divide(accum, count, accum);

                return new _KMeansKey(accum);
            }

            public _KMeansKey(__KMEANSPUBLICKEY key)
            {            
                _Key = key.ToArray();
            }
           
            private readonly float[] _Key;
            public bool IsEmpty => _Key == null || _Key.Length == 0;
            public __KMEANSPUBLICKEY PublicKey => _Key;           

            public override int GetHashCode()
            {
                if (IsEmpty) return 0;
                return _Key.Aggregate(0, (accum, value) => accum * 17 + value.GetHashCode());
            }

            public bool Equals(_KMeansKey other)
            {
                if (this.IsEmpty && other.IsEmpty) return true;
                if (this.IsEmpty) return false;
                if (other.IsEmpty) return false;

                return _Key.AsSpan().SequenceEqual(other._Key.AsSpan());
            }

            public float DistanceTo(_KMeansKey other)
            {
                return _Key.EuclideanDistanceTo(other._Key);
            }

            public void AddTo(Span<float> dst)
            {
                System.Numerics.Tensors.TensorPrimitives.Add(dst, _Key.AsSpan(), dst);
            }            
        }

        private class _KMeansCluster<TValue> : IGrouping<__KMEANSPUBLICKEY, TValue>
        {
            #region lifecycle            

            private _KMeansCluster(_KMeansKey key, IReadOnlyList<TValue> values)
            {
                _Key = key;
                _Values = values;
            }

            #endregion

            #region data

            private _KMeansKey _Key;
            private readonly IReadOnlyList<TValue> _Values;

            #endregion

            #region API

            public __KMEANSPUBLICKEY Key => _Key.PublicKey;
            
            public IEnumerator<TValue> GetEnumerator() { return _Values.AsEnumerable().GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return _Values.GetEnumerator(); }

            #endregion

            #region nested types

            [System.Diagnostics.DebuggerDisplay("{IsFinished} {_Values.Count}")]
            internal class ClusterBuilder
            {
                #region lifecycle

                public static _KMeansCluster<TValue>[] GroupByRandomKMeans(IReadOnlyList<TValue> srcData, int k, Func<TValue, _KMeansKey> vfunc, IProgress<int> progress)
                {
                    k = Math.Min(srcData.Count, k);

                    // take k seeds randomly

                    var random = new Random();

                    var clusters = srcData
                        .OrderBy(x => random.Next())
                        .Take(k)
                        .Select(item => new ClusterBuilder(item, vfunc))
                        .ToArray();

                    if (clusters.Length <= 2) return default;

                    return _Refine(srcData, clusters, progress)
                        .Select(item => item.ToCluster())
                        .ToArray();
                }

                public static _KMeansCluster<TValue>[] GroupByFarthestKMeans(IReadOnlyList<TValue> srcData, int k, Func<TValue, _KMeansKey> vfunc, IProgress<int> progress)
                {
                    k = Math.Min(srcData.Count, k);

                    // take k seeds that are as far away from each other as possible

                    var clusters = _FindFarthest(srcData, k, vfunc)
                        .Select(item => new ClusterBuilder(item, vfunc))
                        .ToArray();

                    if (clusters.Length <= 2) return default;

                    return _Refine(srcData, clusters, progress)
                        .Select(item => item.ToCluster())
                        .ToArray();
                }


                private static TValue[] _FindFarthest(IReadOnlyList<TValue> items, int k, Func<TValue, _KMeansKey> vfunc)
                {
                    var remaining = items.ToList();
                    var result = new List<TValue>();

                    var item = remaining.Last();
                    remaining.Remove(item);
                    result.Add(item);

                    float distanceToCurrent(TValue item)
                    {
                        var itemV = vfunc(item);

                        float minDist = float.MaxValue;
                        foreach (var r in result)
                        {
                            var rV = vfunc(r);
                            var d = itemV.DistanceTo(rV);
                            minDist = Math.Min(d, minDist);
                        }
                        return minDist;
                    }

                    while (result.Count < k) // theoretically, instead of continuing until result.Count < k, we could continue until a certain minimum distance is met
                    {
                        float maxDist = 0;
                        item = default;

                        foreach (var r in remaining)
                        {
                            var d = distanceToCurrent(r);
                            if (d > maxDist) { maxDist = d; item = r; }
                        }

                        remaining.Remove(item);
                        result.Add(item);
                    }

                    return result.ToArray();

                }

                private static ClusterBuilder[] _Refine(IReadOnlyList<TValue> srcData, ClusterBuilder[] clusters, IProgress<int> progress)
                {
                    // KMeans works by establishing a collection of initial centroids, which can be randomly chosen
                    // then it iteratively fills the clusters with the closest items, and re-calculates the centroids
                    // on each step, so in the next interation the clusters are filled with slightly different items
                    // the iteration process ends when we detect the centroids of all the clusters have stopped moving.

                    int finishedCount = 0;

                    while (true) // iterate until clusters don't change anymore
                    {
                        // calculate progress based on how many clusters have already converged
                        finishedCount = Math.Max(finishedCount, clusters.Count(item => item.IsFinished));
                        progress?.Report(finishedCount * 100 / clusters.Length);

                        // reset keys and clear the clusters internal list
                        foreach (var cluster in clusters) cluster.ResetKey();

                        void _addItem(TValue point)
                        {
                            float dist = float.MaxValue;
                            int idx = 0;

                            for (int i = 0; i < clusters.Length; ++i)
                            {
                                var d = clusters[i].GetDistanceTo(point);
                                if (d < dist) { idx = i; dist = d; }
                            }

                            clusters[idx].Add(point);
                        }

                        // System.Threading.Tasks.Parallel.ForEach(srcData, _addItem);
                        foreach (var point in srcData) { _addItem(point); }

                        if (clusters.All(c => c.IsFinished)) break;
                    }

                    return clusters;
                }

                private ClusterBuilder(TValue value, Func<TValue, _KMeansKey> vfunc)
                {
                    _Key = vfunc(value);
                    _EmbeddedVectorEval = vfunc;
                }

                #endregion

                #region data

                private readonly List<TValue> _Values = new List<TValue>();
                private readonly Func<TValue, _KMeansKey> _EmbeddedVectorEval;

                private _KMeansKey _OldKey;
                private _KMeansKey _Key;

                #endregion

                #region API

                public _KMeansKey Key
                {
                    get
                    {
                        lock (_Values)
                        {

                            if (!_Key.IsEmpty) return _Key;
                            if (_Values.Count == 0) return _OldKey;

                            _Key = _KMeansKey.Mean(_Values.Select(_EmbeddedVectorEval));

                            return _Key;
                        }
                    }
                }

                private void ResetKey()
                {
                    _OldKey = _Key;
                    _Key = default;
                    _Values.Clear();
                }

                private void Add(TValue value)
                {
                    lock (_Values)
                    {
                        _Values.Add(value);
                    }
                }                

                private bool IsFinished
                {
                    get
                    {
                        if (_OldKey.IsEmpty) return false;
                        if (Key.IsEmpty) return false;
                        return Key.Equals(_OldKey);
                    }
                }

                private float GetDistanceTo(TValue value)
                {
                    var vvv = _EmbeddedVectorEval(value);
                    return Key.DistanceTo(vvv);
                }

                public _KMeansCluster<TValue> ToCluster()
                {
                    return new _KMeansCluster<TValue>(Key, _Values);
                }

                #endregion
            }            

            #endregion
        }
    }    
}
