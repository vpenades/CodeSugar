using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable


namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        #if !NET6_0_OR_GREATER

        [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
        public sealed class CallerArgumentExpressionAttribute : Attribute
        {
            public CallerArgumentExpressionAttribute(string parameterName)
            {
                ParameterName = parameterName;
            }

            public string ParameterName { get; }
        }

        #endif
    }
}
