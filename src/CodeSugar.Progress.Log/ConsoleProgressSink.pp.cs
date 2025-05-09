// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

#nullable disable

using _LOGLEVEL = System.Diagnostics.TraceEventType;

using _LOGPROGRESS4 = System.IProgress<(System.Diagnostics.TraceEventType level, System.Exception ex, string msg, object[] args)>;
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
        #if DEBUG
        private const _LOGLEVEL _DefaultLogLevel = _LOGLEVEL.Verbose;
        #else
        private const _LOGLEVEL _DefaultLogLevel = _LOGLEVEL.Warning;
        #endif

        [return: NotNull]
        public static _LOGPROGRESS1 GetProgressToConsoleLogger(this Type targetType, (_LOGLEVEL infoLevel, _LOGLEVEL errorLevel) options)
        {
            if (targetType == null) return _NullProgressSink<string>.Instance;

            return new _PlainTextConsoleProgressSink(targetType, options.infoLevel, options.errorLevel);
        }

        [return: NotNull]
        public static _LOGPROGRESS1 GetProgressToConsoleLogger(this Type targetType, _LOGLEVEL infoLevel = _DefaultLogLevel, _LOGLEVEL? errorLevel = null)
        {
            if (targetType == null) return _NullProgressSink<string>.Instance;

            return new _PlainTextConsoleProgressSink(targetType, infoLevel, errorLevel);
        }

        private class _PlainTextConsoleProgressSink : _LOGPROGRESS0, _LOGPROGRESS1, _LOGPROGRESS2, _LOGPROGRESS3, _LOGPROGRESS4, _LOGPROGRESSEX
        {
            #region lifecycle

            public _PlainTextConsoleProgressSink(Type type, _LOGLEVEL minInfoLevel, _LOGLEVEL? minErrorLevel)
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
            private readonly _LOGLEVEL _MinInfoLevel;
            private readonly _LOGLEVEL? _MinErrorLevel;

            #endregion

            #region API

            public void Report(Exception ex)
            {
                _WriteToConsole(_LOGLEVEL.Error, _FormatMessage(ex));
            }

            public void Report((_LOGLEVEL level, Exception ex, string msg, object[] args) value)
            {
                if (value.level > _MinInfoLevel) return;
                _WriteToConsole(value.level, FormatMessage((value.ex, value.msg, value.args)));
            }

            public void Report((_LOGLEVEL level, Exception ex, string msg) value)
            {
                if (value.level > _MinInfoLevel) return;
                _WriteToConsole(value.level, FormatMessage((value.ex, value.msg)));
            }
            
            public void Report((_LOGLEVEL level, string msg) value)
            {
                if (value.level > _MinInfoLevel) return;
                _WriteToConsole(value.level, value.msg);
            }

            public void Report(int value)
            {
                if (_LOGLEVEL.Verbose > _MinInfoLevel) return;
                _WriteToConsole(_LOGLEVEL.Verbose, value.ToString());
            }

            public void Report(string value)
            {
                if (_LOGLEVEL.Verbose > _MinInfoLevel) return;
                _WriteToConsole(_LOGLEVEL.Verbose, value);
            }

            #endregion

            #region actual writing

            private static ConsoleColor _FromLevel(_LOGLEVEL lvl)
            {
                switch (lvl)
                {
                    case _LOGLEVEL.Verbose: return ConsoleColor.Gray;
                    case _LOGLEVEL.Information: return ConsoleColor.White;
                    case _LOGLEVEL.Warning: return ConsoleColor.Yellow;
                    case _LOGLEVEL.Error: return ConsoleColor.Red;
                    case _LOGLEVEL.Critical: return ConsoleColor.Magenta;
                    default: return ConsoleColor.White;
                }
            }

            private void _WriteToConsole(_LOGLEVEL level, string msg)
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
                    if (level == _LOGLEVEL.Critical) sink.Flush();
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
