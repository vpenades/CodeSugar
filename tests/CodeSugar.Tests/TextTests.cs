using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSugar
{
    internal class TextTests
    {
        [Test]
        public async Task TestTextSplit()
        {
            "abcd".TrySplitAfterFirst(out var p1, "b", out var p2);
            await Assert.That(p1).IsEqualTo("a");
            await Assert.That(p2).IsEqualTo("cd");

            "abcdabc".TrySplitAfterLast(out p1, "b", out p2);
            await Assert.That(p1).IsEqualTo("abcda");
            await Assert.That(p2).IsEqualTo("c");

            "abcdabc".TrySplitAfterFirst(out p1, "b", out p2, "a", out var p3);
            await Assert.That(p1).IsEqualTo("a");
            await Assert.That(p2).IsEqualTo("cd");
            await Assert.That(p3).IsEqualTo("bc");

            "a cd  bc".TrySplitAfterFirst(out p1, null, out p2, null, out p3);
            await Assert.That(p1).IsEqualTo("a");
            await Assert.That(p2).IsEqualTo("cd");
            await Assert.That(p3).IsEqualTo("bc");

            "a b\r\n c".TrySplitAfterLast(out p1, null, out p2);
            await Assert.That(p1).IsEqualTo("a b");
            await Assert.That(p2).IsEqualTo("c");
        }

        [Test]
        public async Task TestTokenize()
        {
            Func<char, char> blocks = (char c) => c == '{' ? '}' : default;

            var split = CodeSugarForText.Tokenize("  abc { a b c } 123 ", null, blocks).ToArray();
            await Assert.That(split).IsSequenceEqualTo(new[] { "abc", " a b c ", "123" });

            split = CodeSugarForText.Tokenize("{abc } {} { abc}", null, blocks).ToArray();
            await Assert.That(split).IsSequenceEqualTo(new[] { "abc ", string.Empty, " abc" });

            await Assert.That(CodeSugarForText.Tokenize("abc 123")).IsSequenceEqualTo(new[] { "abc", "123" });

            await Assert.That(CodeSugarForText.Tokenize("    abc     123    ")).IsSequenceEqualTo(new[] { "abc", "123" });

            await Assert.That(CodeSugarForText.Tokenize("abc")).IsSequenceEqualTo(new[] { "abc" });

            await Assert.That(CodeSugarForText.Tokenize("-a --b -c:hello -d:{hello world} -e",null, blocks)).IsSequenceEqualTo(new[] { "-a","--b","-c:hello","-d:hello world","-e" });
        }

        #if NET8_0_OR_GREATER

        [Test]
        [Arguments("")]
        [Arguments("  ")]
        [Arguments("abc")]
        [Arguments("abc ")]
        [Arguments("abc \"\" 123")]
        [Arguments("   abc   123 ")]
        [Arguments(" \"abc\"\"123\"\"abc\" \"\"\"123\" ")]
        [Arguments("   abc   \" 123   \" ")]
        [Arguments("-a --b -c:hello -d:\"hello world\" -e")]
        [Arguments(" -d:\"hello\"xyz 123 \"\"  555  a\"\"b ")]
        public async Task TestTokenizeCommandLine(string textLine)
        {
            var refSplit = System.CommandLine.Parsing.CommandLineParser.SplitCommandLine(textLine).ToArray();

            var impSplit = textLine.Tokenize(null, c=> c== '"' ? '"' : default).ToArray();

            await Assert.That(impSplit).IsSequenceEqualTo(refSplit);
        }

        #endif

    }
}