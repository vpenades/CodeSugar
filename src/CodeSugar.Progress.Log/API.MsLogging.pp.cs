// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using _LOGLEVEL = System.Diagnostics.TraceEventType;
using _LOGGER = System.IProgress<string>;

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
        public static IDisposable BeginScope(_LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Start, message, args);
            return new _EndScore(logger);
        }

        private sealed class _EndScore : IDisposable
        {
            public _EndScore(_LOGGER logger)
            {
                _Logger = logger;
            }

            private _LOGGER _Logger;            

            public void Dispose()
            {
                var l = System.Threading.Interlocked.Exchange(ref _Logger, null);
                l?.ReportLog(_LOGLEVEL.Stop, null);
            }
        }

        public static void LogDebug(this _LOGGER logger, Exception exception, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Verbose, exception, message, args);
        }
        
        public static void LogDebug(this _LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Verbose, message, args);
        }
        
        public static void LogTrace(this _LOGGER logger, Exception exception, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Verbose, exception, message, args);
        }
        
        public static void LogTrace(this _LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Verbose, message, args);
        }
        
        public static void LogInformation(this _LOGGER logger, Exception exception, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Information, exception, message, args);
        }
        
        public static void LogInformation(this _LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Information, message, args);
        }
        
        public static void LogWarning(this _LOGGER logger, Exception exception, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Warning, exception, message, args);
        }
        
        public static void LogWarning(this _LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Warning, message, args);
        }
        
        public static void LogError(this _LOGGER logger, Exception exception, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Error, exception, message, args);
        }
        
        public static void LogError(this _LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Error, message, args);
        }
        
        public static void LogCritical(this _LOGGER logger, Exception exception, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Critical, exception, message, args);
        }
        
        public static void LogCritical(this _LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(_LOGLEVEL.Critical, message, args);
        }
    }
}