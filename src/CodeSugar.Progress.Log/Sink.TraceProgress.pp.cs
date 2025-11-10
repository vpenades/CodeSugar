// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Text;
using System.IO;
using System.Diagnostics.CodeAnalysis;

#nullable disable

using __LOGLEVEL = System.Diagnostics.TraceEventType;

using __LOGPROGRESS4 = System.IProgress<(System.Diagnostics.TraceEventType level, System.Exception ex, string msg, object[] args)>;
using __LOGPROGRESS3 = System.IProgress<(System.Diagnostics.TraceEventType level, System.Exception ex, string msg)>;
using __LOGPROGRESS2 = System.IProgress<(System.Diagnostics.TraceEventType level, string msg)>;
using __LOGPROGRESS1 = System.IProgress<string>;
using __LOGPROGRESS0 = System.IProgress<int>;
using __LOGPROGRESSEX = System.IProgress<System.Exception>;

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
        public static __LOGPROGRESS1 GetProgressToTraceLogger(this Type targetType)
        {
            if (targetType == null) return _NullProgressSink<string>.Instance;

            return new _ProgressToTraceLogger(targetType);
        }

        private class _ProgressToTraceLogger : __LOGPROGRESS0, __LOGPROGRESS1, __LOGPROGRESS2, __LOGPROGRESS3, __LOGPROGRESS4, __LOGPROGRESSEX
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

            public void Report((__LOGLEVEL level, Exception ex, string msg, object[] args) value)
            {
                _WriteToTrace(value.level, FormatMessage((value.ex, value.msg, value.args)));
            }

            public void Report((__LOGLEVEL level, Exception ex, string msg) value)
            {
                _WriteToTrace(value.level, FormatMessage((value.ex, value.msg)));
            }

            public void Report(Exception ex)
            {
                _WriteToTrace(__LOGLEVEL.Error, _FormatMessage(ex));
            }

            public void Report((__LOGLEVEL level, string msg) value)
            {
                _WriteToTrace(value.level, value.msg);
            }

            public void Report(int value)
            {
                _WriteToTrace(__LOGLEVEL.Verbose, value.ToString());
            }

            public void Report(string value)
            {
                _WriteToTrace(__LOGLEVEL.Verbose, value);
            }

            #endregion

            #region actual writing            

            private void _WriteToTrace(__LOGLEVEL level, string msg)
            {
                if (msg == null) return;

                msg = (level, msg).FormatMessage();
                System.Diagnostics.Trace.WriteLine(msg);
            }

            #endregion
        }
    }
}
