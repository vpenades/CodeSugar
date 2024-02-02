using System;
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CodeSugar.Tests
{
    using CODESUGAR = CodeSugarForSystem;

    public class SystemTests
        : IProgress<string>
        , IProgress<int>
        , IProgress<(TraceEventType level, object value, string callerName)>
    {
        // https://stackoverflow.com/questions/17893827/how-to-get-type-name-of-a-callermember
        // considered horribly expensive.
        // var method = new System.Diagnostics.StackTrace(1, false).GetFrame(1).GetMethod();            
        // _DeclaringType = method.DeclaringType;

        public void Report((TraceEventType level, object value, string callerName) value)
        {
            var msg = $"{value.level}, {value.callerName}: {value.value}";            
            TestContext.Progress.WriteLine(msg);
        }

        public void Report(string value)
        {
            value ??= string.Empty;
            TestContext.Progress.WriteLine(value);
        }

        public void Report(int value)
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TestInterfaceAccess()
        {
            this.LogVerbose("");
            (this as IProgress<int>).LogVerbose("");

            this.LogWarn("warning");
            this.LogWarn<IProgress<string>>("warning");
        }
    }
}
