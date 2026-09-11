using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using __RTINTEROPSVCS = System.Runtime.InteropServices;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarForSystem
    {
        public static string ToInvariantString<T>(this T value) where T: IConvertible
        {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string InvariantFormat(this string format, object arg0)
        {
            return format is null ? null : string.Format(System.Globalization.CultureInfo.InvariantCulture, format, arg0);
        }

        public static string InvariantFormat(this string format, object arg0, object arg1)
        {
            return format is null ? null : string.Format(System.Globalization.CultureInfo.InvariantCulture, format, arg0, arg1);
        }

        public static string InvariantFormat(this string format, object arg0, object arg1, object arg2)
        {
            return format is null ? null : string.Format(System.Globalization.CultureInfo.InvariantCulture, format, arg0, arg1, arg2);
        }        

        public static string InvariantFormat(this string format, params object[] args)
        {
            return format is null ? null : string.Format(System.Globalization.CultureInfo.InvariantCulture, format, args);
        }

        public static bool OrdinalEqual(this string text, string other)
        {
            return string.Equals(text, other, StringComparison.Ordinal);
        }

        public static int GetOrdinalHashCode(this string text)
        {
            return text == null ? 0 : text.GetHashCode(StringComparison.Ordinal);
        }

        public static int OrdinalIndexOf(this string text, string searchValue)
        {
            return text is null ? -1 : text.IndexOf(searchValue, StringComparison.Ordinal);
        }

        public static int OrdinalIndexOf(this string text, string searchValue, int startIndex)
        {
            return text is null ? -1 : text.IndexOf(searchValue, startIndex, StringComparison.Ordinal);
        }

        public static int OrdinalIndexOf(this string text, string searchValue, int startIndex, int count)
        {
            return text is null ? -1 : text.IndexOf(searchValue, startIndex, count, StringComparison.Ordinal);
        }

        public static bool OrdinalContains(this string text, char value)
        {
            return text is null ? false : text.Contains(value, StringComparison.Ordinal);
        }

        public static bool OrdinalContains(this string text, string value)
        {
            return text is null ? false : text.Contains(value, StringComparison.Ordinal);
        }

        public static void OrdinalReplace(this string text, string oldString, string newString)
        {            
            text?.Replace(oldString, newString, StringComparison.Ordinal);
        }

        public static void InvariantReplace(this string text, string oldString, string newString, bool ignoreCase)
        {
            text?.Replace(oldString, newString, ignoreCase, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
