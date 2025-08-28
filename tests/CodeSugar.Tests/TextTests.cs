using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace CodeSugar
{
    internal class TextTests
    {
        [Test]
        public void TestTextSplit()
        {
            "abcd".TrySplitAfterFirst(out var p1, "b", out var p2);
            Assert.That(p1, Is.EqualTo("a"));
            Assert.That(p2, Is.EqualTo("cd"));

            "abcdabc".TrySplitAfterLast(out p1, "b", out p2);
            Assert.That(p1, Is.EqualTo("abcda"));
            Assert.That(p2, Is.EqualTo("c"));

            "abcdabc".TrySplitAfterFirst(out p1, "b", out p2, "a", out var p3);
            Assert.That(p1, Is.EqualTo("a"));
            Assert.That(p2, Is.EqualTo("cd"));
            Assert.That(p3, Is.EqualTo("bc"));

            "a cd  bc".TrySplitAfterFirst(out p1, null, out p2, null, out p3);
            Assert.That(p1, Is.EqualTo("a"));
            Assert.That(p2, Is.EqualTo("cd"));
            Assert.That(p3, Is.EqualTo("bc"));

            "a b\r\n c".TrySplitAfterLast(out p1, null, out p2);
            Assert.That(p1, Is.EqualTo("a b"));
            Assert.That(p2, Is.EqualTo("c"));
        }

        [Test]
        public void TestTokenize()
        {
            Func<char, char> blocks = (char c) => c == '{' ? '}' : default;

            var split = CodeSugarForText.Tokenize("  abc { a b c } 123 ", null, blocks).ToArray();
            Assert.That(split, Is.EqualTo(new[] { "abc", " a b c ", "123" }));

            split = CodeSugarForText.Tokenize("{abc } {} { abc}", null, blocks).ToArray();
            Assert.That(split, Is.EqualTo(new[] { "abc ", string.Empty, " abc" }));

            Assert.That(CodeSugarForText.Tokenize("abc 123"), Is.EqualTo(new[] { "abc", "123" }));

            Assert.That(CodeSugarForText.Tokenize("    abc     123    "), Is.EqualTo(new[] { "abc", "123" }));

            Assert.That(CodeSugarForText.Tokenize("abc"), Is.EqualTo(new[] { "abc" }));

            Assert.That(CodeSugarForText.Tokenize("-a --b -c:hello -d:{hello world} -e",null, blocks), Is.EqualTo(new[] { "-a","--b","-c:hello","-d:hello world","-e" }));
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase("abc")]
        [TestCase("abc ")]
        [TestCase("abc \"\" 123")]
        [TestCase("   abc   123 ")]
        [TestCase(" \"abc\"\"123\"\"abc\" \"\"\"123\" ")]
        [TestCase("   abc   \" 123   \" ")]
        [TestCase("-a --b -c:hello -d:\"hello world\" -e")]
        [TestCase(" -d:\"hello\"xyz 123 \"\"  555  a\"\"b ")]
        public void TestTokenizeCommandLine(string textLine)
        {
            var refSplit = System.CommandLine.Parsing.CommandLineParser.SplitCommandLine(textLine).ToArray();

            var impSplit = textLine.Tokenize(null, c=> c== '"' ? '"' : default).ToArray();

            Assert.That(impSplit, Is.EqualTo(refSplit));
        }

    }
}
