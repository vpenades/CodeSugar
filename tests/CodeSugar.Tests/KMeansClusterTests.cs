using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace CodeSugar
{
    internal class KMeansClusterTests
    {
        [Test]
        public void TestClustering()
        {
            var vectors = CreateSimpleVectors().ToList().Randomize().ToList();

            var groups = vectors.GroupByFarthestKMeans(3, item => item);

            foreach (var group in groups)
            {
                TestContext.Out.WriteLine($"Key: {group.Key.ToText()}");

                foreach(var val in group)
                {
                    TestContext.Out.WriteLine("     " + val.ToText());
                }

            }            
        }        
        static IEnumerable<float[]> CreateSimpleVectors()
        {
            foreach(var k in new float[] {1, 4, 8 })
            {
                for(int i=0; i < 10; ++i)
                {
                    var v = new float[10];
                    v.AsSpan().Fill(k);
                    v[9] += (float)i / 100;

                    yield return v;
                }                
            }
        }

        static IEnumerable<float[]> CreateRandomVectors()
        {
            var rnd = new Random();

            for (int i = 0; i < 40; ++i)
            {
                var v = new float[10];
                
                for(int j=0; j<v.Length; ++j)
                {
                    v[j] = rnd.NextSingle();
                }                

                yield return v;
            }
        }

    }
}
