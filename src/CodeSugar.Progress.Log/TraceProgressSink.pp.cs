// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using _LOGLEVEL = System.Diagnostics.TraceEventType;

using _LOGPROGRESS4 = System.IProgress<(System.Diagnostics.TraceEventType level, System.Exception ex, string msg, object[] args)>;
using _LOGPROGRESS3 = System.IProgress<(System.Diagnostics.TraceEventType level, System.Exception ex, string msg)>;
using _LOGPROGRESS2 = System.IProgress<(System.Diagnostics.TraceEventType level, string msg)>;
using _LOGPROGRESS1 = System.IProgress<string>;
using _LOGPROGRESS0 = System.IProgress<int>;
using _LOGPROGRESSEX = System.IProgress<System.Exception>;
using System.Diagnostics.CodeAnalysis;

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
        [return: NotNull]
        public static _LOGPROGRESS1 GetProgressToTraceLogger(this Type targetType)
        {
            if (targetType == null) return _NullProgressSink<string>.Instance;

            return new _ProgressToTraceLogger(targetType);
        }

        private class _ProgressToTraceLogger : _LOGPROGRESS0, _LOGPROGRESS1, _LOGPROGRESS2, _LOGPROGRESS3, _LOGPROGRESS4, _LOGPROGRESSEX
        {
            #region lifecycle

            public _ProgressToTraceLogger(Type type)
            {
                _type = type;
            }

            #endregion

            #region data

            private readonly Type _type;

            #endregion

            #region API

            public void Report((_LOGLEVEL level, Exception ex, string msg, object[] args) value)
            {
                _WriteToTrace(value.level, FormatMessage((value.ex, value.msg, value.args)));
            }

            public void Report((_LOGLEVEL level, Exception ex, string msg) value)
            {
                _WriteToTrace(value.level, FormatMessage((value.ex, value.msg)));
            }

            public void Report(Exception ex)
            {
                _WriteToTrace(_LOGLEVEL.Error, _FormatMessage(ex));
            }

            public void Report((_LOGLEVEL level, string msg) value)
            {
                _WriteToTrace(value.level, value.msg);
            }

            public void Report(int value)
            {
                _WriteToTrace(_LOGLEVEL.Verbose, value.ToString());
            }

            public void Report(string value)
            {
                _WriteToTrace(_LOGLEVEL.Verbose, value);
            }

            #endregion

            #region actual writing            

            private void _WriteToTrace(_LOGLEVEL level, string msg)
            {
                if (msg == null) return;

                msg = (level, msg).FormatMessage();
                System.Diagnostics.Trace.WriteLine(msg);
            }

            #endregion
        }
    }
}
