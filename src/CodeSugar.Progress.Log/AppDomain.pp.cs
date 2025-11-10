// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Data.Common;
using System.Security.AccessControl;
using System.Runtime.Versioning;

#nullable disable

using __LOGLEVEL = System.Diagnostics.TraceEventType;

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
        public static IDisposable RedirectConsoleToFile(this AppDomain appDomain, string filePath)
        {
            var writer = _AppendToLog(filePath);
            if (writer == null) return null;

            // Set the StreamWriter as the Console output and error
            Console.SetOut(writer);
            Console.SetError(writer);

            return writer;
        }

        public static IDisposable RedirectConsoleOutputToFile(this AppDomain appDomain, string filePath)
        {
            var writer = _AppendToLog(filePath);
            if (writer == null) return null;

            // Set the StreamWriter as the Console output
            Console.SetOut(writer);

            return writer;
        }        

        public static IDisposable RedirectConsoleErrorToFile(this AppDomain appDomain, string filePath)
        {
            var writer = _AppendToLog(filePath);
            if (writer == null) return null;

            // Set the StreamWriter as the Console output
            Console.SetError(writer);

            return writer;
        }

        private static StreamWriter _AppendToLog(string filePath)
        {
            // do multiple attempts in case the file is taken by another process.

            for(int i=0; i < 10; ++i) 
            {
                try
                {
                    filePath = i == 0
                        ? _GetAbolutePath(filePath, ".log")
                        : _GetAbolutePath(filePath, $".{i}.log");

                    // Create a StreamWriter to append log events.
                    return new StreamWriter(filePath, true, Encoding.UTF8);
                }
                catch { }
            }

            return null;            
        }

        public static void RedirectCrashLoggingToConsole(this AppDomain appDomain)
        {
            static IProgress<string> _func(Type type)
            {
                return GetProgressToConsoleLogger(type);
            }            

            RedirectCrashLoggingToFactory(appDomain, _func);
        }

        public static void RedirectCrashLoggingToFactory(this AppDomain appDomain, Func<string, IProgress<string>> logFactory)
        {
            var logger = logFactory.Invoke(typeof(AppDomain).Name);

            appDomain.UnhandledException += (sender, e) => logger.ReportLog(__LOGLEVEL.Critical, _FormatMessage(e));
            TaskScheduler.UnobservedTaskException += (sender, e) => logger.ReportLog(__LOGLEVEL.Critical, _FormatMessage(e.Exception));
        }

        public static void RedirectCrashLoggingToFactory(this AppDomain appDomain, Func<Type, IProgress<string>> logFactory)
        {
            var logger = logFactory.Invoke(typeof(AppDomain));

            appDomain.UnhandledException += (sender, e) => logger.ReportLog(__LOGLEVEL.Critical, _FormatMessage(e));
            TaskScheduler.UnobservedTaskException += (sender, e) => logger.ReportLog(__LOGLEVEL.Critical, _FormatMessage(e.Exception));
        }

        public static void RedirectCrashLoggingToFile(this AppDomain appDomain, string filePath)
        {
            filePath = _GetAbolutePath(filePath, ".crash.log");

            var dirPath = System.IO.Path.GetDirectoryName(filePath);
            System.IO.Directory.CreateDirectory(dirPath);

            appDomain.UnhandledException += (sender, e) => _WriteCrashDump(filePath, _FormatMessage(e));
            TaskScheduler.UnobservedTaskException += (sender, e) => _WriteCrashDump(filePath, _FormatMessage(e.Exception));
        }

        private static string _FormatMessage(UnhandledExceptionEventArgs args)
        {
            try
            {
                if (args?.ExceptionObject is Exception ex) return _FormatMessage(ex);
                if (args.ExceptionObject != null) return args.ExceptionObject.ToString();
                return "Unknown exception";
            }
            catch(Exception ex)
            {
                return "Failed to format exception:" + ex.Message + "\r\n" + ex.StackTrace;
            }
        }

        private static void _WriteCrashDump(string fileName, string report)
        {
            try { System.IO.File.AppendAllText(fileName, report); }
            catch { }
        }

        private static string _GetAbolutePath(string fileNameOrPath, string extension)
        {            
            #if NET
            if (string.IsNullOrWhiteSpace(fileNameOrPath) && !string.IsNullOrWhiteSpace(Environment.ProcessPath))
            {
                fileNameOrPath = System.IO.Path.GetFileName(Environment.ProcessPath);
                fileNameOrPath = System.IO.Path.ChangeExtension(fileNameOrPath, extension);
            }
            #endif

            if (string.IsNullOrWhiteSpace(fileNameOrPath)) throw new NullReferenceException(fileNameOrPath);

            if (System.IO.Path.IsPathFullyQualified(fileNameOrPath)) return fileNameOrPath;

            #if NET
            if (OperatingSystem.IsWindows()) // on windows, we can try write the log in the same directory as the executable if we have write access
            {
                var directoryInfo = new DirectoryInfo(AppContext.BaseDirectory);

                if (_HasWriteAccessToDirectory(directoryInfo))
                {
                    return Path.Combine(AppContext.BaseDirectory, fileNameOrPath);
                }
            }
            #endif

            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrashDumps");
            
            return System.IO.Path.Combine(path, fileNameOrPath);
        }

        #if NET
        [SupportedOSPlatform("windows")]        
        private static bool _HasWriteAccessToDirectory(System.IO.DirectoryInfo dir)
        {
            // Check existence first
            if (!dir.Exists) return false;

            // Compose a temporary file path
            var testFilePath = Path.Combine(dir.FullName, Guid.NewGuid().ToString() + ".tmp");

            try
            {
                // Try to create and write to a file in that directory
                using (var fs = File.Create(testFilePath, 1, FileOptions.DeleteOnClose))
                {
                    fs.WriteByte(0); // Try writing a single byte
                }

                // If no exception occurred, we have write access
                return true;
            }
            catch
            {
                // If any exception occurs, we don't have permission
                return false;
            }
            finally
            {
                // Attempt to clean up, just in case
                try { if (File.Exists(testFilePath)) File.Delete(testFilePath); } catch { }
            }
        }
        #endif
    }
}
