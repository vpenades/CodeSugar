// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

// TODO: Add StackTrace

#nullable disable

using LOGTYPE = System.Diagnostics.TraceEventType;

using LOGPROGRESS4 = System.IProgress<(System.Type declaringType, System.Diagnostics.TraceEventType level, System.Object value, System.String callerName)>;
using LOGPROGRESS3 = System.IProgress<(System.Diagnostics.TraceEventType level, System.Object value, System.String callerName)>;
using LOGPROGRESS2 = System.IProgress<(System.Diagnostics.TraceEventType level, string msg)>;
using LOGPROGRESS1 = System.IProgress<string>;
using LOGPROGRESSEX = System.IProgress<System.Exception>;

using CALLERMEMBERNAME = System.Runtime.CompilerServices.CallerMemberNameAttribute;


#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System
#else
namespace $rootnamespace$
#endif
{    
    partial class CodeSugarForSystem
    {
        public const string LOGVERBOSEPREFIX = "VERBOSE:";
        public const string LOGINFOPREFIX = "INFO:";
        public const string LOGWARNPREFIX = "WARNING:";
        public const string LOGERRORPREFIX = "ERROR:";
        public const string LOGCRITICALPREFIX = "CRITICAL:";

        // TODO: Do not use SWITCH in older LANGVERSIONS

        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug<T>(this IProgress<T> progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            // https://stackoverflow.com/questions/17893827/how-to-get-type-name-of-a-callermember
            // considered horribly expensive.
            // var method = new System.Diagnostics.StackTrace(1, false).GetFrame(1).GetMethod();            
            // _DeclaringType = method.DeclaringType;

            LogVerbose<T>(progress, msg, callerName);
        }

        public static void LogVerbose<T>(this IProgress<T> progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            switch(progress)
            {
                case LOGPROGRESS3 logger: logger.Report((LOGTYPE.Verbose, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGTYPE.Verbose, _CombineCallerAndMessage(callerName, msg))); break;
                case LOGPROGRESS1 logger: logger.Report(_CombineCallerAndMessage(callerName, msg)); break;
            }
        }

        public static void LogInfo<T>(this IProgress<T> progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            switch(progress)
            {
                case LOGPROGRESS3 logger: logger.Report((LOGTYPE.Information, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGTYPE.Information, _CombineCallerAndMessage(callerName, msg))); break;
                case LOGPROGRESS1 logger: logger.Report(_CombineCallerAndMessage(callerName, msg)); break;
            }
        }

        public static void LogWarn<T>(this IProgress<T> progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            switch(progress)
            {
                case LOGPROGRESS3 logger: logger.Report((LOGTYPE.Warning, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGTYPE.Warning, _CombineCallerAndMessage(callerName, msg))); break;
                case LOGPROGRESS1 logger: logger.Report(LOGWARNPREFIX + " " + _CombineCallerAndMessage(callerName, msg)); break;
            }
        }

        public static void LogError<T>(this IProgress<T> progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            switch(progress)
            {
                case LOGPROGRESS3 logger: logger.Report((LOGTYPE.Error, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGTYPE.Error, _CombineCallerAndMessage(callerName, msg))); break;
                case LOGPROGRESS1 logger: logger.Report(LOGERRORPREFIX + " " + _CombineCallerAndMessage(callerName, msg)); break;
            }
        }

        public static void LogCritical<T>(this IProgress<T> progress, string msg, [CALLERMEMBERNAME] string callerName = null)        
        {
            switch(progress)
            {
                case LOGPROGRESS3 logger: logger.Report((LOGTYPE.Critical, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGTYPE.Critical, _CombineCallerAndMessage(callerName, msg))); break;
                case LOGPROGRESS1 logger: logger.Report(LOGCRITICALPREFIX + " " + _CombineCallerAndMessage(callerName, msg)); break;
            }
        }        

        public static void LogException<T>(this IProgress<T> progress, Exception ex, [CALLERMEMBERNAME] string callerName = null)        
        {
            switch(progress)
            {
                case LOGPROGRESSEX logger: logger.Report(ex); break;
                default: LogError<T>(progress, ex.Message, callerName); break;
            }
        }  

        private static string _CombineCallerAndMessage(string callerName, string msg)
        {
            msg ??= String.Empty;

            if (string.IsNullOrWhiteSpace(callerName)) return msg;

            return callerName + ": " + msg;
        }
    }
}
