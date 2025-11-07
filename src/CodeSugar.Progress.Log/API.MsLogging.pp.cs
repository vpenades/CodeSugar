// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using __LOGLEVEL = System.Diagnostics.TraceEventType;
using __LOGGER = System.IProgress<string>;

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
        public static IDisposable BeginScope(__LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Start, message, args);
            return new _EndScore(logger);
        }

        private sealed class _EndScore : IDisposable
        {
            public _EndScore(__LOGGER logger)
            {
                __LOGGER = logger;
            }

            private __LOGGER __LOGGER;            

            public void Dispose()
            {
                var l = System.Threading.Interlocked.Exchange(ref __LOGGER, null);
                l?.ReportLog(__LOGLEVEL.Stop, null);
            }
        }

        public static void LogDebug(this __LOGGER logger, Exception exception, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Verbose, exception, message, args);
        }
        
        public static void LogDebug(this __LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Verbose, message, args);
        }
        
        public static void LogTrace(this __LOGGER logger, Exception exception, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Verbose, exception, message, args);
        }
        
        public static void LogTrace(this __LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Verbose, message, args);
        }
        
        public static void LogInformation(this __LOGGER logger, Exception exception, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Information, exception, message, args);
        }
        
        public static void LogInformation(this __LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Information, message, args);
        }
        
        public static void LogWarning(this __LOGGER logger, Exception exception, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Warning, exception, message, args);
        }
        
        public static void LogWarning(this __LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Warning, message, args);
        }
        
        public static void LogError(this __LOGGER logger, Exception exception, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Error, exception, message, args);
        }
        
        public static void LogError(this __LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Error, message, args);
        }
        
        public static void LogCritical(this __LOGGER logger, Exception exception, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Critical, exception, message, args);
        }
        
        public static void LogCritical(this __LOGGER logger, string message, params object[] args)
        {
            logger.ReportLog(__LOGLEVEL.Critical, message, args);
        }
    }
}