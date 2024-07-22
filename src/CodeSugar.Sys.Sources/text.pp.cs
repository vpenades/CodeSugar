// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

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
        public static IEnumerable<string> TokenizeCommandLineArguments(string commandLineSentence)
        {
            return TokenizeCommandLineArguments(commandLineSentence,null, c => c == '"' ? '"' : default);
        }

        private static IEnumerable<string> TokenizeCommandLineArguments(string commandLineSentence, Predicate<char> separator = null, Func<char, char> openBlock = null)
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

                if (accum.Length == 0 && separator(c)) continue;

                if (endBlock != default && c == endBlock) { c = default; continue; }

                var end = openBlock(c);
                if (end != default) { c = end; continue; }

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
