using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSugar
{
    internal class LoggingTests
    {
        [Explicit]
        [Test]
        public void RegisterLoggerToAppDomain()
        {
            System.AppDomain.CurrentDomain.RedirectCrashLoggingToConsole();
        }

        [Test]
        public async Task TestSharedlogger()
        {
            await Assert.That(typeof(LoggingTests).TryGetSharedLogger(out var logger)).IsFalse();

            var sink = new Progress<string>(msg => { });

            System.AppDomain.CurrentDomain.SetSharedLogger(sink);

            await Assert.That(typeof(LoggingTests).TryGetSharedLogger(out logger)).IsTrue();

            await Assert.That(logger).IsEqualTo(sink);
        }

        [Explicit]
        [Test]
        public async Task TestLogToFile()
        {
            var path = System.IO.Path.Combine(AppContext.BaseDirectory, "CrashLog1.txt");

            using (var logContext = System.AppDomain.CurrentDomain.RedirectConsoleOutputToFile(path))
            {
                var log = typeof(LoggingTests).GetProgressToConsoleLogger();
                log.LogInformation("Hello world!");
            }

            var finfo = new System.IO.FileInfo(path);

            await Assert.That(finfo.Exists).IsTrue();

            var text = finfo.GetReadStreamFunction().ReadAllText();

            await Assert.That(text.Contains("Hello world!")).IsTrue();
        }
    }
}