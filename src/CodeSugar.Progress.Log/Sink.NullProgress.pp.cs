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
    partial class CodeSugarForLogging
    {
        private class _NullProgressSink<T> : IProgress<T>
        {
            public static readonly _NullProgressSink<T> Instance = new _NullProgressSink<T>();

            public void Report(T value) { }
        }
    }
}
