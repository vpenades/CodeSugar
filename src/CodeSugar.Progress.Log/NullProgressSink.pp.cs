// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using _LOGLEVEL = System.Diagnostics.TraceEventType;

using _LOGPROGRESS4 = System.IProgress<(System.Diagnostics.TraceEventType level, System.Exception ex, string msg, object[] extparams)>;
using _LOGPROGRESS3 = System.IProgress<(System.Diagnostics.TraceEventType level, System.Exception ex, string msg)>;
using _LOGPROGRESS2 = System.IProgress<(System.Diagnostics.TraceEventType level, string msg)>;
using _LOGPROGRESS1 = System.IProgress<string>;
using _LOGPROGRESS0 = System.IProgress<int>;
using _LOGPROGRESSEX = System.IProgress<System.Exception>;

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
