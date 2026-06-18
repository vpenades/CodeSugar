using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using TUnit.Assertions.Conditions;
using TUnit.Assertions.Core;
using TUnit.Assertions.Enums;

namespace CodeSugar
{
    internal static class _TUnitExtensions
    {

        



        /// <summary>
        /// Extension method for IsEquivalentToAssertion.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Collection equivalency uses structural comparison for complex objects, which requires reflection and is not compatible with AOT")]
        public static IsEquivalentToAssertion<TCollection, TItem> IsSequenceEqualTo<TCollection, TItem>(this IAssertionSource<TCollection> source, System.Collections.Generic.IEnumerable<TItem> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
            where TCollection : System.Collections.Generic.IEnumerable<TItem>
        {
            source.Context.ExpressionBuilder.Append(".IsEquivalentTo(");
            var added = false;
            if (expectedExpression != null)
            {
                source.Context.ExpressionBuilder.Append(added ? ", " : "");
                source.Context.ExpressionBuilder.Append(expectedExpression);
                added = true;
            }
            source.Context.ExpressionBuilder.Append(")");
            return new IsEquivalentToAssertion<TCollection, TItem>(source.Context, expected, CollectionOrdering.Matching);
        }
    }
}
