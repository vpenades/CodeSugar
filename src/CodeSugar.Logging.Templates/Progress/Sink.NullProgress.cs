using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarLoggingExtensions
    {
        private class _NullProgressSink<T> : IProgress<T>
        {
            public static readonly _NullProgressSink<T> Instance = new _NullProgressSink<T>();

            public void Report(T value) { }
        }
    }
}
