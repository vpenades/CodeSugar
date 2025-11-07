// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using __LOGLEVEL = System.Diagnostics.TraceEventType;
using System.Reflection.Emit;

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
        public const string LOGVERBOSEPREFIX = "VERBOSE:";
        public const string LOGINFOPREFIX = "INFO:";
        public const string LOGWARNPREFIX = "WARNING:";
        public const string LOGERRORPREFIX = "ERROR:";
        public const string LOGCRITICALPREFIX = "CRITICAL:";

        public static string FormatMessage(this (__LOGLEVEL level, string msg) log)
        {
            return _FormatMessage(log.level, log.msg);
        }

        public static string FormatMessage(this (__LOGLEVEL level, System.Exception ex, string msg) log)
        {
            var msg = _CombineLines(log.msg, _FormatMessage(log.ex));
            return _FormatMessage(log.level, msg);
        }

        public static string FormatMessage(this (__LOGLEVEL level, System.Exception ex, string msg, object[] args) log)
        {
            var msg = _FormatMessage(log.msg, log.args);
            msg = _CombineLines(msg, _FormatMessage(log.ex));
            return _FormatMessage(log.level, msg);
        }

        public static string FormatMessage(this (__LOGLEVEL level, string msg, object[] args) log)
        {
            var msg = _FormatMessage(log.msg, log.args);
            return _FormatMessage(log.level, msg);
        }

        public static string FormatMessage(this (System.Exception ex, string msg, object[] args) log)
        {
            var msg = _FormatMessage(log.msg, log.args);

            return _CombineLines(msg, _FormatMessage(log.ex));
        }

        public static string FormatMessage(this (System.Exception ex, string msg) log)
        {
            return _CombineLines(log.msg, _FormatMessage(log.ex));
        }

        private static string _FormatMessage(__LOGLEVEL level, string msg)
        {
            if (string.IsNullOrWhiteSpace(msg)) return level.ToString();
            return $"{level} {msg}";
        }

        private static string _FormatMessage(string msg, object[] args)
        {
            if (string.IsNullOrWhiteSpace(msg)) return null;
            if (args == null || args.Length == 0) return msg;
            return string.Format(msg, args);
        }        

        private static string _FormatMessage(Exception ex)
        {
            if (ex == null) return null;

            var msg = _CombineLines(ex.GetType().Name, ex.Message);
            msg = _CombineLines(msg, ex.StackTrace);

            return msg;
        }

        private static string _CombineLines(string line1, string line2)
        {
            if (line1 == null) return line2;
            if (line2 == null) return line1;
            return line1 + "\r\n" + line2;
        }


        /// <summary>
        /// Formats a log event message into default log4net style format
        /// </summary>
        /// <param name="logLevel">event level</param>
        /// <param name="category">usually the container's class type</param>
        /// <param name="msg">the final message, which is the output of Func&lt;string,Exception,String&gt; </param>
        /// <returns>the formatted message</returns>
        private static string _FormatAsLog4net(__LOGLEVEL logLevel, string category, string msg)
        {
            string lvl;

            switch (logLevel)
            {                
                case __LOGLEVEL.Verbose: lvl = "DEBUG"; break;                
                case __LOGLEVEL.Warning: lvl = "WARN"; break;
                case __LOGLEVEL.Error: lvl = "ERROR"; break;
                case __LOGLEVEL.Critical: lvl = "FATAL"; break;
                case __LOGLEVEL.Information: lvl = "INFO"; break;
                default: return null;
            }

            var now = DateTime.Now;
            msg ??= "NULL";

            msg = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} [{1}] {2} {3} - {4}",
                now.ToString("yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture),
                Environment.CurrentManagedThreadId,
                lvl,
                category,
                msg);

            return msg;
        }
    }
}
