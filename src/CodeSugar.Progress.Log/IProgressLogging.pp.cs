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
        public static void ReportLog(this _LOGPROGRESS0 dst, _LOGLEVEL level, string msg, params object[] args)
        {
            ReportLog(dst, level, null, msg, args);
        }

        public static void ReportLog(this _LOGPROGRESS1 dst, _LOGLEVEL level, string msg, params object[] args)
        {
            ReportLog(dst, level, null, msg, args);
        }

        public static void ReportLog(this _LOGPROGRESS2 dst, _LOGLEVEL level, string msg, params object[] args)
        {
            ReportLog(dst, level, null, msg, args);
        }

        public static void ReportLog(this _LOGPROGRESS3 dst, _LOGLEVEL level, string msg, params object[] args)
        {
            ReportLog(dst, level, null, msg, args);
        }

        public static void ReportLog(this _LOGPROGRESS4 dst, _LOGLEVEL level, string msg, params object[] args)
        {
            dst.Report((level, null, msg, args));
        }        

        public static void ReportLog(this _LOGPROGRESS0 dst, _LOGLEVEL level, System.Exception ex, string msg, params object[] args)
        {
            switch (dst)
            {
                case _LOGPROGRESS4 dst4: ReportLog(dst4, level, ex, msg, args); break;
                case _LOGPROGRESS3 dst3: ReportLog(dst3, level, ex, msg, args); break;
                case _LOGPROGRESS2 dst2: ReportLog(dst2, level, ex, msg, args); break;
                case _LOGPROGRESS1 dst1: ReportLog(dst1, level, ex, msg, args); break;
            }
        }

        public static void ReportLog(this _LOGPROGRESS1 dst, _LOGLEVEL level, System.Exception ex, string msg, params object[] args)
        {
            switch (dst)
            {
                case _LOGPROGRESS4 dst4: ReportLog(dst4, level, ex, msg, args); break;
                case _LOGPROGRESS3 dst3: ReportLog(dst3, level, ex, msg, args); break;
                case _LOGPROGRESS2 dst2: ReportLog(dst2, level, ex, msg, args); break;
                default: dst.Report((level, ex, msg, args).FormatMessage()); break;
            }
        }

        public static void ReportLog(this _LOGPROGRESS2 dst, _LOGLEVEL level, System.Exception ex, string msg, params object[] args)
        {
            switch (dst)
            {
                case _LOGPROGRESS4 dst4: ReportLog(dst4, level, ex, msg, args); break;
                case _LOGPROGRESS3 dst3: ReportLog(dst3, level, ex, msg, args); break;
                default: dst.Report((level, (ex, msg, args).FormatMessage())); break;
            }
        }

        public static void ReportLog(this _LOGPROGRESS3 dst, _LOGLEVEL level, System.Exception ex, string msg, params object[] args)
        {
            switch (dst)
            {
                case _LOGPROGRESS4 dst4: ReportLog(dst4, level, ex, msg, args); break;
                default: dst.Report((level, ex, _FormatMessage(msg, args))); break;
            }
        }

        public static void ReportLog(this _LOGPROGRESS4 dst, _LOGLEVEL level, System.Exception ex, string msg, params object[] args)
        {
            dst.Report((level, ex, msg, args));
        }
    }
}
