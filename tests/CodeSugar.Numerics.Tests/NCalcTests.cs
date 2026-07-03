using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSugar
{
    internal class NCalcTests
    {
        [Test]
        public async Task EvaluateExpressions()
        {
            var expr = new NCalc.Expression("5+7");
            var result = await expr.EvaluateAsync<int>();
            await Assert.That(result).IsEqualTo(12);

            var arr = new NCalc.Expression("(1,2)");
            await Assert.That(arr.EvaluateEx<(int, int)>).IsEqualTo((1,2));
            await Assert.That(arr.EvaluateEx<(float, float)>).IsEqualTo((1f, 2f));
            await Assert.That(arr.EvaluateEx<System.Numerics.Vector2>).IsEqualTo(new System.Numerics.Vector2(1f, 2f));
        }
    }
}
