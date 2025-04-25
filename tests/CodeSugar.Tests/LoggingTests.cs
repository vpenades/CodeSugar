using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace CodeSugar
{
    internal class LoggingTests
    {
        [Test]
        public void TestLogToFile()
        {
            var path = System.IO.Path.Combine(NUnit.Framework.TestContext.CurrentContext.WorkDirectory, "CrashLog1.txt");

            using (var logContext = System.AppDomain.CurrentDomain.RedirectConsoleOutputToFile(path))
            {
                var log = typeof(LoggingTests).GetProgressToConsoleLogger();
                log.LogInformation("Hello world!");
            }

            var finfo = new System.IO.FileInfo(path);

            Assert.That(finfo.Exists);

            var text = finfo.ReadAllText();

            Assert.That(text.Contains("Hello world!"));
        }
    }
}
