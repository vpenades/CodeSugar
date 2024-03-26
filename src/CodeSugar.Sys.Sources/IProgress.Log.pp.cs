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
using LOGPROGRESS0 = System.IProgress<string>;

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

        #region IProgress<int>

        /// <summary>Tries to log a debug message as long as the self progress implement a compatible logging sink.</summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug(this IProgress<int> progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogVerbose(progress as IProgress<string>, msg, callerName);
        }

        /// <summary>Tries to log a verbose message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogVerbose(this IProgress<int> progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogVerbose(progress as IProgress<string>, msg, callerName);
        }

        /// <summary>Tries to log a message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogInfo(this IProgress<int> progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogInfo(progress as IProgress<string>, msg, callerName);
        }

        /// <summary>Tries to log a warning message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogWarn(this IProgress<int> progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogWarn(progress as IProgress<string>, msg, callerName);
        }

        /// <summary>Tries to log an error message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogError(this IProgress<int> progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogError(progress as IProgress<string>, msg, callerName);
        }

        /// <summary>Tries to log a critical message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogCritical(this IProgress<int> progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogCritical(progress as IProgress<string>, msg, callerName);
        }

        /// <summary>Tries to log an exception as long as the self progress implement a compatible logging sink.</summary>
        public static void LogCritical(this IProgress<int> progress, Exception ex, [CALLERMEMBERNAME] string callerName = null)
        {
            LogException(progress as IProgress<string>, ex, callerName);
        }

        #endregion

        #region IProgress<string>

        /// <summary>Tries to log a debug message as long as the self progress implement a compatible logging sink.</summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug<T>(this T progress, string msg, [CALLERMEMBERNAME] string callerName = null)
            where T: LOGPROGRESS1
        {
            LogVerbose<T>(progress, msg, callerName);
        }

        /// <summary>Tries to log a verbose message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogVerbose<T>(this T progress, string msg, [CALLERMEMBERNAME] string callerName = null)
            where T: LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
                case LOGPROGRESS3 logger: logger.Report((LOGTYPE.Verbose, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGTYPE.Verbose, _CombineCallerAndMessage(callerName, msg))); break;
                default: progress.Report(_CombineCallerAndMessage(callerName, msg)); break;
            }
        }

        /// <summary>Tries to log a message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogInfo<T>(this T progress, string msg, [CALLERMEMBERNAME] string callerName = null)
            where T: LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
                case LOGPROGRESS3 logger: logger.Report((LOGTYPE.Information, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGTYPE.Information, _CombineCallerAndMessage(callerName, msg))); break;
                default: progress.Report(_CombineCallerAndMessage(callerName, msg)); break;
            }
        }

        /// <summary>Tries to log a warning message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogWarn<T>(this T progress, string msg, [CALLERMEMBERNAME] string callerName = null)
            where T: LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
                case LOGPROGRESS3 logger: logger.Report((LOGTYPE.Warning, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGTYPE.Warning, _CombineCallerAndMessage(callerName, msg))); break;
                default: progress.Report(LOGWARNPREFIX + " " + _CombineCallerAndMessage(callerName, msg)); break;
            }
        }

        /// <summary>Tries to log an error message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogError<T>(this T progress, string msg, [CALLERMEMBERNAME] string callerName = null)
            where T: LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
                case LOGPROGRESS3 logger: logger.Report((LOGTYPE.Error, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGTYPE.Error, _CombineCallerAndMessage(callerName, msg))); break;
                default: progress.Report(LOGERRORPREFIX + " " + _CombineCallerAndMessage(callerName, msg)); break;
            }
        }

        /// <summary>Tries to log a critical message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogCritical<T>(this T progress, string msg, [CALLERMEMBERNAME] string callerName = null)
            where T: LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
                case LOGPROGRESS3 logger: logger.Report((LOGTYPE.Critical, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGTYPE.Critical, _CombineCallerAndMessage(callerName, msg))); break;
                default: progress.Report(LOGCRITICALPREFIX + " " + _CombineCallerAndMessage(callerName, msg)); break;
            }
        }        

        /// <summary>Tries to log an exception as long as the self progress implement a compatible logging sink.</summary>
        public static void LogException<T>(this T progress, Exception ex, [CALLERMEMBERNAME] string callerName = null)
            where T: LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
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

        #endregion
    }
}
