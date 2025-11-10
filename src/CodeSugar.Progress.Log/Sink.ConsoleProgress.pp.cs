// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
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
        #if DEBUG
        private const __LOGLEVEL _DefaultLogLevel = __LOGLEVEL.Verbose;
        #else
        private const __LOGLEVEL _DefaultLogLevel = __LOGLEVEL.Warning;
        #endif

        [return: NotNull]
        public static __LOGPROGRESS1 GetProgressToConsoleLogger(this Type targetType, (__LOGLEVEL infoLevel, __LOGLEVEL errorLevel) options)
        {
            if (targetType == null) return _NullProgressSink<string>.Instance;

            return new _PlainTextConsoleProgressSink(targetType, options.infoLevel, options.errorLevel);
        }

        [return: NotNull]
        public static __LOGPROGRESS1 GetProgressToConsoleLogger(this Type targetType, __LOGLEVEL infoLevel = _DefaultLogLevel, __LOGLEVEL? errorLevel = null)
        {
            if (targetType == null) return _NullProgressSink<string>.Instance;

            return new _PlainTextConsoleProgressSink(targetType, infoLevel, errorLevel);
        }

        private class _PlainTextConsoleProgressSink : __LOGPROGRESS0, __LOGPROGRESS1, __LOGPROGRESS2, __LOGPROGRESS3, __LOGPROGRESS4, __LOGPROGRESSEX
        {
            #region lifecycle

            public _PlainTextConsoleProgressSink(Type type, __LOGLEVEL minInfoLevel, __LOGLEVEL? minErrorLevel)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                if (minErrorLevel.HasValue && minErrorLevel.Value > minErrorLevel) throw new ArgumentOutOfRangeException(nameof(minErrorLevel), $"must be less or equal than {minInfoLevel}");

                _type = type;
                _MinInfoLevel = minInfoLevel;
                _MinErrorLevel = minErrorLevel;
            }

            #endregion

            #region data

            private readonly Type _type;
            private readonly __LOGLEVEL _MinInfoLevel;
            private readonly __LOGLEVEL? _MinErrorLevel;

            #endregion

            #region API

            public void Report(Exception ex)
            {
                _WriteToConsole(__LOGLEVEL.Error, _FormatMessage(ex));
            }

            public void Report((__LOGLEVEL level, Exception ex, string msg, object[] args) value)
            {
                if (value.level > _MinInfoLevel) return;
                _WriteToConsole(value.level, FormatMessage((value.ex, value.msg, value.args)));
            }

            public void Report((__LOGLEVEL level, Exception ex, string msg) value)
            {
                if (value.level > _MinInfoLevel) return;
                _WriteToConsole(value.level, FormatMessage((value.ex, value.msg)));
            }
            
            public void Report((__LOGLEVEL level, string msg) value)
            {
                if (value.level > _MinInfoLevel) return;
                _WriteToConsole(value.level, value.msg);
            }

            public void Report(int value)
            {
                if (__LOGLEVEL.Verbose > _MinInfoLevel) return;
                _WriteToConsole(__LOGLEVEL.Verbose, value.ToString());
            }

            public void Report(string value)
            {
                if (__LOGLEVEL.Verbose > _MinInfoLevel) return;
                _WriteToConsole(__LOGLEVEL.Verbose, value);
            }

            #endregion

            #region actual writing

            private static ConsoleColor _FromLevel(__LOGLEVEL lvl)
            {
                switch (lvl)
                {
                    case __LOGLEVEL.Verbose: return ConsoleColor.Gray;
                    case __LOGLEVEL.Information: return ConsoleColor.White;
                    case __LOGLEVEL.Warning: return ConsoleColor.Yellow;
                    case __LOGLEVEL.Error: return ConsoleColor.Red;
                    case __LOGLEVEL.Critical: return ConsoleColor.Magenta;
                    default: return ConsoleColor.White;
                }
            }

            private void _WriteToConsole(__LOGLEVEL level, string msg)
            {
                if (msg == null) return;

                var sink = Console.Out;
                var isRedirected = System.Console.IsOutputRedirected;

                if (_MinErrorLevel.HasValue && level <= _MinErrorLevel.Value)
                {
                    sink = Console.Error;
                    isRedirected = System.Console.IsErrorRedirected;
                }

                if (isRedirected)
                {
                    msg = _FormatAsLog4net(level, _type.Name, msg);
                    sink.WriteLine(msg);
                    if (level == __LOGLEVEL.Critical) sink.Flush();
                }
                else
                {
                    msg = FormatMessage((level, msg));

                    var cc = System.Console.ForegroundColor;
                    System.Console.ForegroundColor = _FromLevel(level);
                    sink.WriteLine(msg);
                    System.Console.ForegroundColor = cc;
                }                    
            }

            #endregion
        }
    }
}
