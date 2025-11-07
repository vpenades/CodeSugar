// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

// TODO: Add StackTrace

#nullable disable

using __LOGLEVEL = System.Diagnostics.TraceEventType;

using __LOGPROGRESS4 = System.IProgress<(System.Type declaringType, System.Diagnostics.TraceEventType level, System.Object value, System.String callerName)>;
using __LOGPROGRESS3 = System.IProgress<(System.Diagnostics.TraceEventType level, System.Object value, System.String callerName)>;
using __LOGPROGRESS2 = System.IProgress<(System.Diagnostics.TraceEventType level, string msg)>;
using __LOGPROGRESS1 = System.IProgress<string>;
using __LOGPROGRESS0 = System.IProgress<int>;

using __LOGPROGRESSEX = System.IProgress<System.Exception>;

using __CALLERMEMBERNAME = System.Runtime.CompilerServices.CallerMemberNameAttribute;


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
            if (TryGetService<__LOGPROGRESS1>(srvProvider, out var msgPrg) && msgPrg is IServiceProvider srvProvider2) return GetProgressOrNull<T>(srvProvider2);
            if (TryGetService<__LOGPROGRESS0>(srvProvider, out var intPrg) && intPrg is IServiceProvider srvProvider3) return GetProgressOrNull<T>(srvProvider3);

            // self fallback
            return srvProvider as IProgress<T>;
        }        

        public static __LOGPROGRESS1 DefaultToConsole(this __LOGPROGRESS1 progress) { return progress ?? _ConsoleProgressSink.Instance; }

        public static __LOGPROGRESS1 DefaultToTrace(this __LOGPROGRESS1 progress) { return progress ?? _TraceProgressSink.Instance; }

        #region IProgress<int>

        /// <summary>Tries to log a debug message as long as the self progress implement a compatible logging sink.</summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug(this __LOGPROGRESS0 progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
        {
            LogVerbose(progress as __LOGPROGRESS1, msg, callerName);
        }

        /// <summary>Tries to log a verbose message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogVerbose(this __LOGPROGRESS0 progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
        {
            LogVerbose(progress as __LOGPROGRESS1, msg, callerName);
        }

        /// <summary>Tries to log a message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogInfo(this __LOGPROGRESS0 progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
        {
            LogInfo(progress as __LOGPROGRESS1, msg, callerName);
        }

        /// <summary>Tries to log a warning message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogWarn(this __LOGPROGRESS0 progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
        {
            LogWarn(progress as __LOGPROGRESS1, msg, callerName);
        }

        /// <summary>Tries to log an error message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogError(this __LOGPROGRESS0 progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
        {
            LogError(progress as __LOGPROGRESS1, msg, callerName);
        }

        /// <summary>Tries to log a critical message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogCritical(this __LOGPROGRESS0 progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
        {
            LogCritical(progress as __LOGPROGRESS1, msg, callerName);
        }

        /// <summary>Tries to log an exception as long as the self progress implement a compatible logging sink.</summary>
        public static void LogCritical(this __LOGPROGRESS0 progress, Exception ex, [__CALLERMEMBERNAME] string callerName = null)
        {
            LogException(progress as __LOGPROGRESS1, ex, callerName);
        }

        #endregion

        #region IProgress<(LOGTYPE, MSG)>

        /// <summary>Tries to log a debug message as long as the self progress implement a compatible logging sink.</summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug(this __LOGPROGRESS2 progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
        {
            LogVerbose(progress, msg, callerName);
        }

        /// <summary>Tries to log a verbose message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogVerbose(this __LOGPROGRESS2 progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
        {
            _Log(progress, __LOGLEVEL.Verbose, msg, callerName);
        }        

        /// <summary>Tries to log a message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogInfo(this __LOGPROGRESS2 progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
        {
            _Log(progress, __LOGLEVEL.Information, msg, callerName);
        }

        /// <summary>Tries to log a warning message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogWarn(this __LOGPROGRESS2 progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
        {
            _Log(progress, __LOGLEVEL.Warning, msg, callerName);
        }

        /// <summary>Tries to log an error message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogError(this __LOGPROGRESS2 progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
        {
            _Log(progress, __LOGLEVEL.Error, msg, callerName);
        }

        /// <summary>Tries to log a critical message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogCritical(this __LOGPROGRESS2 progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
        {
            _Log(progress, __LOGLEVEL.Critical, msg, callerName);
        }

        /// <summary>Tries to log an exception as long as the self progress implement a compatible logging sink.</summary>
        public static void LogCritical(this __LOGPROGRESS2 progress, Exception ex, [__CALLERMEMBERNAME] string callerName = null)
        {
            if (progress is __LOGPROGRESSEX pex) { pex.Report(ex); return; }

            _Log(progress, __LOGLEVEL.Critical, ex.Message, callerName);
        }

        private static void _Log(__LOGPROGRESS2 progress, __LOGLEVEL lvl, string msg, string callerName)
        {
            switch (progress)
            {
                case null: break;
                case __LOGPROGRESS3 logger: logger.Report((lvl, msg, callerName)); break;
                case __LOGPROGRESS2 logger: logger.Report((lvl, _CombineCallerAndMessage(callerName, msg))); break;
            }
        }

        #endregion

        #region IProgress<T>

        /// <summary>Tries to log a debug message as long as the self progress implement a compatible logging sink.</summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug<T>(this T progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
            where T: __LOGPROGRESS1
        {
            LogVerbose(progress, msg, callerName);
        }

        /// <summary>Tries to log a verbose message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogVerbose<T>(this T progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
            where T: __LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
                case __LOGPROGRESS3 logger: logger.Report((__LOGLEVEL.Verbose, msg, callerName)); break;
                case __LOGPROGRESS2 logger: logger.Report((__LOGLEVEL.Verbose, _CombineCallerAndMessage(callerName, msg))); break;
                default: progress.Report(_CombineCallerAndMessage(callerName, msg)); break;
            }
        }

        /// <summary>Tries to log a message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogInfo<T>(this T progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
            where T: __LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
                case __LOGPROGRESS3 logger: logger.Report((__LOGLEVEL.Information, msg, callerName)); break;
                case __LOGPROGRESS2 logger: logger.Report((__LOGLEVEL.Information, _CombineCallerAndMessage(callerName, msg))); break;
                default: progress.Report(_CombineCallerAndMessage(callerName, msg)); break;
            }
        }

        /// <summary>Tries to log a warning message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogWarn<T>(this T progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
            where T: __LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
                case __LOGPROGRESS3 logger: logger.Report((__LOGLEVEL.Warning, msg, callerName)); break;
                case __LOGPROGRESS2 logger: logger.Report((__LOGLEVEL.Warning, _CombineCallerAndMessage(callerName, msg))); break;
                default: progress.Report(LOGWARNPREFIX + " " + _CombineCallerAndMessage(callerName, msg)); break;
            }
        }

        /// <summary>Tries to log an error message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogError<T>(this T progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
            where T: __LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
                case __LOGPROGRESS3 logger: logger.Report((__LOGLEVEL.Error, msg, callerName)); break;
                case __LOGPROGRESS2 logger: logger.Report((__LOGLEVEL.Error, _CombineCallerAndMessage(callerName, msg))); break;
                default: progress.Report(LOGERRORPREFIX + " " + _CombineCallerAndMessage(callerName, msg)); break;
            }
        }

        /// <summary>Tries to log a critical message as long as the self progress implement a compatible logging sink.</summary>
        public static void LogCritical<T>(this T progress, string msg, [__CALLERMEMBERNAME] string callerName = null)
            where T: __LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
                case __LOGPROGRESS3 logger: logger.Report((__LOGLEVEL.Critical, msg, callerName)); break;
                case __LOGPROGRESS2 logger: logger.Report((__LOGLEVEL.Critical, _CombineCallerAndMessage(callerName, msg))); break;
                default: progress.Report(LOGCRITICALPREFIX + " " + _CombineCallerAndMessage(callerName, msg)); break;
            }
        }        

        /// <summary>Tries to log an exception as long as the self progress implement a compatible logging sink.</summary>
        public static void LogException<T>(this T progress, Exception ex, [__CALLERMEMBERNAME] string callerName = null)
            where T: __LOGPROGRESS1
        {
            switch(progress)
            {
                case null: break;
                case __LOGPROGRESSEX logger: logger.Report(ex); break;
                default: LogError<T>(progress, ex.Message, callerName); break;
            }
        }

        #endregion             

        #region nested types

        private readonly struct _ConsoleProgressSink : __LOGPROGRESS0, __LOGPROGRESS1, __LOGPROGRESS2, __LOGPROGRESS3, __LOGPROGRESSEX
        {
            public static readonly _ConsoleProgressSink Instance = new _ConsoleProgressSink();

            public void Report(int value) { Console.WriteLine(value); }
            public void Report(string value) { if (value != null) Console.WriteLine(value); }
            public void Report((__LOGLEVEL level, string msg) value)
            {
                var msg = _CombineLevelAndMessage(value.level, value.msg);
                if (msg == null) return;

                var cc = System.Console.ForegroundColor;
                System.Console.ForegroundColor = _FromLevel(value.level);
                Console.WriteLine(msg);
                System.Console.ForegroundColor = cc;
            }

            public void Report((__LOGLEVEL level, object value, string callerName) value)
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

            private static ConsoleColor _FromLevel(__LOGLEVEL lvl)
            {
                switch(lvl)
                {
                    case __LOGLEVEL.Verbose: return ConsoleColor.Gray;
                    case __LOGLEVEL.Information: return ConsoleColor.White;
                    case __LOGLEVEL.Warning: return ConsoleColor.Yellow;
                    case __LOGLEVEL.Error: return ConsoleColor.Red;
                    case __LOGLEVEL.Critical: return ConsoleColor.Magenta;
                    default: return ConsoleColor.White;
                }
            }
        }

        private readonly struct _TraceProgressSink : __LOGPROGRESS0, __LOGPROGRESS1, __LOGPROGRESS2, __LOGPROGRESS3, __LOGPROGRESSEX
        {
            public static readonly _TraceProgressSink Instance = new _TraceProgressSink();

            public void Report(int value) { System.Diagnostics.Trace.WriteLine(value); }
            public void Report(string value) { if (value != null) System.Diagnostics.Trace.WriteLine(value); }            
            public void Report((__LOGLEVEL level, string msg) value)
            {
                var msg = _CombineLevelAndMessage(value.level, value.msg);
                if (msg == null) return;

                switch (value.level)
                {
                    case __LOGLEVEL.Verbose: System.Diagnostics.Trace.WriteLine(msg); break;
                    default: System.Diagnostics.Debug.WriteLine(msg); break;
                }
            }
            public void Report((__LOGLEVEL level, object value, string callerName) value)
            {
                if (value.value == null) return;
                var msg = value.ToString();

                msg = _CombineLevelAndMessage(value.level, msg);
                msg = _CombineCallerAndMessage(value.callerName, msg);

                switch (value.level)
                {
                    case __LOGLEVEL.Verbose: System.Diagnostics.Trace.WriteLine(msg); break;
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

        private static string _CombineLevelAndMessage(__LOGLEVEL level, string msg)
        {            
            if (msg == null) return null;

            switch (level)
            {
                case __LOGLEVEL.Verbose:
                case __LOGLEVEL.Information:
                    return msg;

                case __LOGLEVEL.Warning: return LOGWARNPREFIX + msg;
                case __LOGLEVEL.Error: return LOGERRORPREFIX + msg;
                case __LOGLEVEL.Critical: return LOGCRITICALPREFIX + msg;
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
