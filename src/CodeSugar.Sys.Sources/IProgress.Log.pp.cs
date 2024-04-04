// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

// TODO: Add StackTrace

#nullable disable

using LOGLEVEL = System.Diagnostics.TraceEventType;

using LOGPROGRESS4 = System.IProgress<(System.Type declaringType, System.Diagnostics.TraceEventType level, System.Object value, System.String callerName)>;
using LOGPROGRESS3 = System.IProgress<(System.Diagnostics.TraceEventType level, System.Object value, System.String callerName)>;
using LOGPROGRESS2 = System.IProgress<(System.Diagnostics.TraceEventType level, string msg)>;
using LOGPROGRESS1 = System.IProgress<string>;
using LOGPROGRESS0 = System.IProgress<int>;

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

        public static IProgress<T> GetProgressOrConsoleSink<T>(this IServiceProvider srvProvider)
        {
            return GetProgressOrNull<T>(srvProvider)
                ?? _ConsoleProgressSink.Instance as IProgress<T>
                ?? _NullProgressSink<T>.Instance;
        }

        /// <summary>
        /// Tries to get the underlaying progress sink.
        /// </summary>
        /// <typeparam name="T">The <see cref="IProgress{T}" type./></typeparam>
        /// <param name="srvProvider"></param>
        /// <returns></returns>
        public static IProgress<T> GetProgressOrNull<T>(this IServiceProvider srvProvider)
        {
            if (TryGetService<IProgress<T>>(srvProvider, out var prg)) return prg;

            // dig into default progress loggers
            if (TryGetService<LOGPROGRESS1>(srvProvider, out var msgPrg) && msgPrg is IServiceProvider srvProvider2) return GetProgressOrNull<T>(srvProvider2);
            if (TryGetService<LOGPROGRESS0>(srvProvider, out var intPrg) && intPrg is IServiceProvider srvProvider3) return GetProgressOrNull<T>(srvProvider3);

            // self fallback
            return srvProvider as IProgress<T>;
        }        

        public static LOGPROGRESS1 DefaultToConsole(this LOGPROGRESS1 progress) { return progress ?? _ConsoleProgressSink.Instance; }

        public static LOGPROGRESS1 DefaultToTrace(this LOGPROGRESS1 progress) { return progress ?? _TraceProgressSink.Instance; }

        #region IProgress<int>

        /// <summary>Tries to log a debug message as long as the self progress implement a compatible logging sink.</summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug(this LOGPROGRESS0 progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogVerbose(progress as LOGPROGRESS1, msg, callerName);
        }

        /// <summary>Tries to log a verbose message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogVerbose(this LOGPROGRESS0 progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogVerbose(progress as LOGPROGRESS1, msg, callerName);
        }

        /// <summary>Tries to log a message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogInfo(this LOGPROGRESS0 progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogInfo(progress as LOGPROGRESS1, msg, callerName);
        }

        /// <summary>Tries to log a warning message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogWarn(this LOGPROGRESS0 progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogWarn(progress as LOGPROGRESS1, msg, callerName);
        }

        /// <summary>Tries to log an error message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogError(this LOGPROGRESS0 progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogError(progress as LOGPROGRESS1, msg, callerName);
        }

        /// <summary>Tries to log a critical message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogCritical(this LOGPROGRESS0 progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogCritical(progress as LOGPROGRESS1, msg, callerName);
        }

        /// <summary>Tries to log an exception as long as the self progress implement a compatible logging sink.</summary>
        public static void LogCritical(this LOGPROGRESS0 progress, Exception ex, [CALLERMEMBERNAME] string callerName = null)
        {
            LogException(progress as LOGPROGRESS1, ex, callerName);
        }

        #endregion

        #region IProgress<(LOGTYPE, MSG)>

        /// <summary>Tries to log a debug message as long as the self progress implement a compatible logging sink.</summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug(this LOGPROGRESS2 progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            LogVerbose(progress, msg, callerName);
        }

        /// <summary>Tries to log a verbose message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogVerbose(this LOGPROGRESS2 progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            _Log(progress, LOGLEVEL.Verbose, msg, callerName);
        }        

        /// <summary>Tries to log a message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogInfo(this LOGPROGRESS2 progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            _Log(progress, LOGLEVEL.Information, msg, callerName);
        }

        /// <summary>Tries to log a warning message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogWarn(this LOGPROGRESS2 progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            _Log(progress, LOGLEVEL.Warning, msg, callerName);
        }

        /// <summary>Tries to log an error message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogError(this LOGPROGRESS2 progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            _Log(progress, LOGLEVEL.Error, msg, callerName);
        }

        /// <summary>Tries to log a critical message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogCritical(this LOGPROGRESS2 progress, string msg, [CALLERMEMBERNAME] string callerName = null)
        {
            _Log(progress, LOGLEVEL.Critical, msg, callerName);
        }

        /// <summary>Tries to log an exception as long as the self progress implement a compatible logging sink.</summary>
        public static void LogCritical(this LOGPROGRESS2 progress, Exception ex, [CALLERMEMBERNAME] string callerName = null)
        {
            if (progress is LOGPROGRESSEX pex) { pex.Report(ex); return; }

            _Log(progress, LOGLEVEL.Critical, ex.Message, callerName);
        }

        private static void _Log(LOGPROGRESS2 progress, LOGLEVEL lvl, string msg, string callerName)
        {
            switch (progress)
            {
                case null: break;
                case LOGPROGRESS3 logger: logger.Report((lvl, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((lvl, _CombineCallerAndMessage(callerName, msg))); break;
            }
        }

        #endregion

        #region IProgress<T>

        /// <summary>Tries to log a debug message as long as the self progress implement a compatible logging sink.</summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug<T>(this T progress, string msg, [CALLERMEMBERNAME] string callerName = null)
            where T: LOGPROGRESS1
        {
            LogVerbose(progress, msg, callerName);
        }

        /// <summary>Tries to log a verbose message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogVerbose<T>(this T progress, string msg, [CALLERMEMBERNAME] string callerName = null)
            where T: LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
                case LOGPROGRESS3 logger: logger.Report((LOGLEVEL.Verbose, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGLEVEL.Verbose, _CombineCallerAndMessage(callerName, msg))); break;
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
                case LOGPROGRESS3 logger: logger.Report((LOGLEVEL.Information, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGLEVEL.Information, _CombineCallerAndMessage(callerName, msg))); break;
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
                case LOGPROGRESS3 logger: logger.Report((LOGLEVEL.Warning, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGLEVEL.Warning, _CombineCallerAndMessage(callerName, msg))); break;
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
                case LOGPROGRESS3 logger: logger.Report((LOGLEVEL.Error, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGLEVEL.Error, _CombineCallerAndMessage(callerName, msg))); break;
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
                case LOGPROGRESS3 logger: logger.Report((LOGLEVEL.Critical, msg, callerName)); break;
                case LOGPROGRESS2 logger: logger.Report((LOGLEVEL.Critical, _CombineCallerAndMessage(callerName, msg))); break;
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

        #endregion             

        #region nested types

        private readonly struct _ConsoleProgressSink : LOGPROGRESS0, LOGPROGRESS1, LOGPROGRESS2, LOGPROGRESS3, LOGPROGRESSEX
        {
            public static readonly _ConsoleProgressSink Instance = new _ConsoleProgressSink();

            public void Report(int value) { Console.WriteLine(value); }
            public void Report(string value) { if (value != null) Console.WriteLine(value); }
            public void Report((LOGLEVEL level, string msg) value)
            {
                var msg = _CombineLevelAndMessage(value.level, value.msg);
                if (msg == null) return;

                var cc = System.Console.ForegroundColor;
                System.Console.ForegroundColor = _FromLevel(value.level);
                Console.WriteLine(msg);
                System.Console.ForegroundColor = cc;
            }

            public void Report((LOGLEVEL level, object value, string callerName) value)
            {
                if (value.value == null) return;
                var msg = value.ToString();

                msg = _CombineLevelAndMessage(value.level, msg);
                msg = _CombineCallerAndMessage(value.callerName, msg);

                var cc = System.Console.ForegroundColor;
                System.Console.ForegroundColor = _FromLevel(value.level);
                Report(msg);
                System.Console.ForegroundColor = cc;
            }

            public void Report(Exception value)
            {
                if (value == null) return;

                var cc = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(value.GetType().Name);
                System.Console.WriteLine(value.Message);
                System.Console.WriteLine(value.StackTrace);
                System.Console.ForegroundColor = cc;
            }

            private static ConsoleColor _FromLevel(LOGLEVEL lvl)
            {
                switch(lvl)
                {
                    case LOGLEVEL.Verbose: return ConsoleColor.Gray;
                    case LOGLEVEL.Information: return ConsoleColor.White;
                    case LOGLEVEL.Warning: return ConsoleColor.Yellow;
                    case LOGLEVEL.Error: return ConsoleColor.Red;
                    case LOGLEVEL.Critical: return ConsoleColor.Magenta;
                    default: return ConsoleColor.White;
                }
            }
        }

        private readonly struct _TraceProgressSink : LOGPROGRESS0, LOGPROGRESS1, LOGPROGRESS2, LOGPROGRESS3, LOGPROGRESSEX
        {
            public static readonly _TraceProgressSink Instance = new _TraceProgressSink();

            public void Report(int value) { System.Diagnostics.Trace.WriteLine(value); }
            public void Report(string value) { if (value != null) System.Diagnostics.Trace.WriteLine(value); }            
            public void Report((LOGLEVEL level, string msg) value)
            {
                var msg = _CombineLevelAndMessage(value.level, value.msg);
                if (msg == null) return;

                switch (value.level)
                {
                    case LOGLEVEL.Verbose: System.Diagnostics.Trace.WriteLine(msg); break;
                    default: System.Diagnostics.Debug.WriteLine(msg); break;
                }
            }
            public void Report((LOGLEVEL level, object value, string callerName) value)
            {
                if (value.value == null) return;
                var msg = value.ToString();

                msg = _CombineLevelAndMessage(value.level, msg);
                msg = _CombineCallerAndMessage(value.callerName, msg);

                switch (value.level)
                {
                    case LOGLEVEL.Verbose: System.Diagnostics.Trace.WriteLine(msg); break;
                    default: System.Diagnostics.Debug.WriteLine(msg); break;
                }
            }

            public void Report(Exception value)
            {
                if (value == null) return;
                System.Diagnostics.Debug.WriteLine(value.GetType().Name);
                System.Diagnostics.Debug.WriteLine(value.Message);
                System.Diagnostics.Debug.WriteLine(value.StackTrace);
            }
        }

        private readonly struct _NullProgressSink<T> : IProgress<T>
        {
            public static readonly _NullProgressSink<T> Instance = new _NullProgressSink<T>();

            public void Report(T value) { }
        }

        #endregion

        #region helpers

        private static string _CombineLevelAndMessage(LOGLEVEL level, string msg)
        {            
            if (msg == null) return null;

            switch (level)
            {
                case LOGLEVEL.Verbose:
                case LOGLEVEL.Information:
                    return msg;

                case LOGLEVEL.Warning: return LOGWARNPREFIX + msg;
                case LOGLEVEL.Error: return LOGERRORPREFIX + msg;
                case LOGLEVEL.Critical: return LOGCRITICALPREFIX + msg;
            }

            return msg;
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
