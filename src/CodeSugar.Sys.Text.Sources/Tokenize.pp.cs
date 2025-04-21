// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using _RTINTEROPSVCS = System.Runtime.InteropServices;

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
            var accumIsBlock = false;
            char endBlock = default;            

            for (int i = 0; i < commandLineSentence.Length; ++i)
            {
                var c = commandLineSentence[i];

                // is block state?
                if (endBlock != default)
                {
                    // exit block state
                    if (c == endBlock) { endBlock = default; accumIsBlock = true; continue; }

                    accum.Append(c);
                    continue;
                }

                // check block start
                var end = openBlock(c);
                if (end != default)
                {
                    if (accumIsBlock)
                    {
                        yield return accum.ToString();
                        accum.Clear();
                        accumIsBlock = false;
                    }

                    endBlock = end; continue;
                }

                // non separator char
                if (!separator(c)) { accum.Append(c); continue; }                

                // break
                if (accum.Length > 0 || accumIsBlock)
                {                    
                    yield return accum.ToString();
                    accum.Clear();
                    accumIsBlock = false;
                }
            }

            if (accum.Length > 0) yield return accum.ToString();
        }        
    }
}
