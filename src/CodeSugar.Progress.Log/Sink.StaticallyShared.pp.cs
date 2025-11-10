// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using __LOGPROGRESS1 = System.IProgress<string>;

#nullable disable

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
        /// <summary>
        /// we wrap the logger in a private type to ensure only us can access the typed pool
        /// </summary>
        private struct _PrivateContainer
        {
            public __LOGPROGRESS1 Logger { get; set; }
        }

        private static readonly System.Buffers.ArrayPool<_PrivateContainer> _Pool = System.Buffers.ArrayPool<_PrivateContainer>.Shared;

        public static void SetSharedLogger(this AppDomain appDomain, __LOGPROGRESS1 logger)
        {
            var rent = _Pool.Rent(1);
            rent[0].Logger = logger;
            _Pool.Return(rent, false);
        }

        [return: NotNull]
        public static __LOGPROGRESS1 GetSharedOrTraceLogger(this Type targetType)
        {
            if (targetType == null) return _NullProgressSink<string>.Instance;

            return TryGetSharedLogger(targetType, out var sink)
                ? sink
                : GetProgressToTraceLogger(targetType);
        }

        [return: NotNull]
        public static __LOGPROGRESS1 GetSharedLogger(this Type targetType, __LOGPROGRESS1 deflogger = null)
        {
            if (targetType == null) return _NullProgressSink<string>.Instance;

            return TryGetSharedLogger(targetType, out var sink)
                ? sink
                : deflogger ?? _NullProgressSink<string>.Instance;
        }

        internal static bool TryGetSharedLogger(this Type type, out __LOGPROGRESS1 logger)
        {
            var rent = _Pool.Rent(1);
            logger = rent[0].Logger;
            _Pool.Return(rent, false);

            return logger != null;            
        }


    }
}
