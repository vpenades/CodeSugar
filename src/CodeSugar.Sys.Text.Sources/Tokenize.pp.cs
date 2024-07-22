// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

using RTINTEROPSVCS = System.Runtime.InteropServices;

#nullable disable

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
        // collision with Microsoft.VisualStudio.TestPlatform.Utilities.StringExtensions.Tokenize

        public static IEnumerable<string> Tokenize(this string commandLineSentence, Predicate<char> separator = null, Func<char, char> openBlock = null)
        {
            commandLineSentence = commandLineSentence?.Trim();

            if (string.IsNullOrWhiteSpace(commandLineSentence)) yield break;

            separator ??= char.IsWhiteSpace;
            openBlock ??= c => default;

            var accum = new StringBuilder();
            char endBlock = default;

            for (int i = 0; i < commandLineSentence.Length; ++i)
            {
                var c = commandLineSentence[i];

                // block mode
                if (endBlock != default)
                {
                    if (c == endBlock) { endBlock = default; continue; } // exit block mode

                    accum.Append(c);
                    continue;
                }

                // check block start
                var end = openBlock(c);
                if (end != default) { endBlock = end; continue; }



                // check whitespaces before token
                if (accum.Length == 0 && separator(c)) continue;                

                // check end token
                if (separator(c) && endBlock == default)
                {
                    if (accum.Length > 0)
                    {
                        yield return accum.ToString();
                        accum.Clear();
                    }
                    continue;
                }

                accum.Append(c);
            }

            if (accum.Length > 0) yield return accum.ToString();
        }
    }
}
