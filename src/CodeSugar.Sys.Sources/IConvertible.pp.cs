// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using __RTINTEROPSVCS = System.Runtime.InteropServices;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarForSystem
    {
        public static string ToStringInvariant<T>(T value)
            where T:IConvertible
        {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }        

        public static T ConvertToInvariant<T>(this System.IConvertible value)
            where T:IConvertible
        {
            switch(value)
            {
                case null: throw new System.ArgumentNullException(nameof(value));
                case T same: return same;
                default: return (T)System.Convert.ChangeType(value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
            }
            
        }
    }
}
