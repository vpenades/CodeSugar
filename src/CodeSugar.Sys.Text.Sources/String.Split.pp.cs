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
    partial class CodeSugarForText
    {
        /// <summary>
        /// Tries to split <paramref name="source"/> in three parts by the first appearence of <paramref name="separator1"/> and the next <paramref name="separator2"/>
        /// </summary>
        /// <param name="source">The string to split.</param>
        /// <param name="part1">The first part.</param>
        /// <param name="separator1">The first separator.</param>
        /// <param name="part2">The second part.</param>
        /// <param name="separator2">The next separator.</param>
        /// <param name="part3">The third part.</param>
        /// <param name="separator3">The next separator.</param>
        /// <param name="part4">The fourth part.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>True if the splitting was successful.</returns>
        public static bool TrySplitAfterFirst(this string source, out string part1, string separator1, out string part2, string separator2, out string part3, string separator3, out string part4, StringComparison comparisonType = StringComparison.Ordinal)
        {
            part1 = null;
            part2 = null;
            part3 = null;
            part4 = null;
            if (source == null) return false;

            if (!TrySplitAfterFirst(source, out part1, separator1, out var next1, comparisonType)) return false;
            if (!TrySplitAfterFirst(next1, out part2, separator2, out var next2, comparisonType)) return false;
            if (!TrySplitAfterFirst(next2, out part3, separator3, out part4, comparisonType)) return false;
            return true;
        }

        /// <summary>
        /// Tries to split <paramref name="source"/> in three parts by the last appearence of <paramref name="separator1"/> and the next <paramref name="separator2"/>
        /// </summary>
        /// <param name="source">The string to split.</param>
        /// <param name="part1">The first part.</param>
        /// <param name="separator1">The first separator.</param>
        /// <param name="part2">The second part.</param>
        /// <param name="separator2">The next separator.</param>
        /// <param name="part3">The third part.</param>
        /// <param name="separator3">The next separator.</param>
        /// <param name="part4">The fourth part.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>True if the splitting was successful.</returns>
        public static bool TrySplitAfterLast(this string source, out string part1, string separator1, out string part2, string separator2, out string part3, string separator3, out string part4, StringComparison comparisonType = StringComparison.Ordinal)
        {
            part1 = null;
            part2 = null;
            part3 = null;
            part4 = null;
            if (source == null) return false;

            if (!TrySplitAfterLast(source, out var prev1, separator3, out part4, comparisonType)) return false;
            if (!TrySplitAfterLast(prev1, out var prev2, separator2, out part3, comparisonType)) return false;
            if (!TrySplitAfterLast(prev2, out part1, separator1, out part2, comparisonType)) return false;
            return true;
        }

        /// <summary>
        /// Tries to split <paramref name="source"/> in three parts by the first appearence of <paramref name="separator1"/> and the next <paramref name="separator2"/>
        /// </summary>
        /// <param name="source">The string to split.</param>
        /// <param name="part1">The first part.</param>
        /// <param name="separator1">The first separator.</param>
        /// <param name="part2">The second part.</param>
        /// <param name="separator2">The next separator.</param>
        /// <param name="part3">The third part.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>True if the splitting was successful.</returns>
        public static bool TrySplitAfterFirst(this string source, out string part1, string separator1, out string part2, string separator2, out string part3, StringComparison comparisonType = StringComparison.Ordinal)
        {
            part1 = null;
            part2 = null;
            part3 = null;
            if (source == null) return false;

            if (!TrySplitAfterFirst(source, out part1, separator1, out var next, comparisonType)) return false;
            if (!TrySplitAfterFirst(next, out part2, separator2, out part3, comparisonType)) return false;
            return true;
        }

        /// <summary>
        /// Tries to split <paramref name="source"/> in three parts by the last appearence of <paramref name="separator1"/> and the next <paramref name="separator2"/>
        /// </summary>
        /// <param name="source">The string to split.</param>
        /// <param name="part1">The first part.</param>
        /// <param name="separator1">The first separator.</param>
        /// <param name="part2">The second part.</param>
        /// <param name="separator2">The next separator.</param>
        /// <param name="part3">The third part.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>True if the splitting was successful.</returns>
        public static bool TrySplitAfterLast(this string source, out string part1, string separator1, out string part2, string separator2, out string part3, StringComparison comparisonType = StringComparison.Ordinal)
        {
            part1 = null;
            part2 = null;
            part3 = null;
            if (source == null) return false;

            if (!TrySplitAfterLast(source, out var prev, separator2, out part3, comparisonType)) return false;
            if (!TrySplitAfterLast(prev, out part1, separator1, out part2, comparisonType)) return false;
            return true;
        }

        /// <summary>
        /// Tries to split <paramref name="source"/> in two parts by the first appearence of <paramref name="separator"/>
        /// </summary>
        /// <param name="source">The string to split.</param>
        /// <param name="part1">The first part.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="part2">The second part.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>True if the splitting was successful.</returns>
        public static bool TrySplitAfterFirst(this string source, out string part1, string separator, out string part2, StringComparison comparisonType = StringComparison.Ordinal)
        {
            part1 = null;
            part2 = null;
            if (string.IsNullOrEmpty(source)) return false;

            var (idx, len) = __FindFirstWord(source, separator, comparisonType);            

            if (idx < 0) return false;            

            __SplitTwoParts(source, idx, len, out part1, out part2);
            return true;
        }

        /// <summary>
        /// Tries to split <paramref name="source"/> in two parts by the last appearence of <paramref name="separator"/>
        /// </summary>
        /// <param name="source">The string to split.</param>
        /// <param name="part1">The first part.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="part2">The second part.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>True if the splitting was successful.</returns>
        public static bool TrySplitAfterLast(this string source, out string part1, string separator, out string part2, StringComparison comparisonType = StringComparison.Ordinal)
        {
            part1 = null;
            part2 = null;
            if (string.IsNullOrEmpty(source)) return false;

            var (idx,len) = __FindLastWord(source, separator, comparisonType);

            if (idx < 0) return false;
            
            __SplitTwoParts(source, idx, len, out part1, out part2);
            return true;
        }

        private static (int idx, int count) __FindFirstWord(string source, string wordOrNull, StringComparison comparisonType)
        {
            if (wordOrNull != null) return (source.IndexOf(wordOrNull, comparisonType), wordOrNull.Length);

            // search for whitespace sequence

            for (int i=0; i < source.Length; ++i)
            {
                if (char.IsWhiteSpace(source[i]))
                {
                    for(int j=i+1;  j < source.Length; ++j)
                    {
                        if (!char.IsWhiteSpace(source[j])) return (i, j - i);
                    }
                }
            }

            return (-1, 0);
        }

        private static (int idx, int count) __FindLastWord(string source, string wordOrNull, StringComparison comparisonType)
        {
            if (wordOrNull != null) return (source.LastIndexOf(wordOrNull, comparisonType), wordOrNull.Length);

            // search for whitespace sequence

            for (int i = source.Length-1; i >= 0; --i)
            {
                if (char.IsWhiteSpace(source[i]))
                {
                    for (int j = i - 1; j >= 0; --j)
                    {
                        if (!char.IsWhiteSpace(source[j])) return (j+1, i-j);
                    }
                }
            }

            return (-1, 0);
        }

        private static void __SplitTwoParts(string source, int separatorOffset, int separatorLength, out string part1, out string part2)
        {
            System.Diagnostics.Debug.Assert(separatorOffset >= 0 && separatorOffset < source.Length);
            System.Diagnostics.Debug.Assert(separatorLength >= 0 && separatorLength < source.Length - separatorOffset);

            part1 = separatorOffset > 0 ? source.Substring(0, separatorOffset) : string.Empty;
            separatorOffset += separatorLength;
            part2 = separatorOffset < source.Length ? source.Substring(separatorOffset) : string.Empty;            
        }
    }
}
