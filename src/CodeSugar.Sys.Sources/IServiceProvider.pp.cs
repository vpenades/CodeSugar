// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using _RTINTEROPSVCS = System.Runtime.InteropServices;

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
        [Obsolete("Use GetServiceOrDefault instead", true)]
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetServiceOrDefault<T>();
        }

        /// <summary>
        /// Gets the given service object from a <see cref="IServiceProvider"/>, or <paramref name="defval"/> if the service does not exist.
        /// </summary>
        public static T GetServiceOrDefault<T>(this IServiceProvider serviceProvider, T defval = default)
        {
            return TryGetService<T>(serviceProvider, out var service)
                ? service
                : defval;
        }

        /// <summary>
        /// Tries to get a service object from a <see cref="IServiceProvider"/>
        /// </summary>
        public static bool TryGetService<T>(this IServiceProvider serviceProvider, out T service)
        {
            service = default;

            if (serviceProvider == null) return false;

            #if !DEBUG
            try {
            #endif

                var result = serviceProvider.GetService(typeof(T));

                if (result is T value)
                {
                    service = value;
                    return true;
                }

            #if !DEBUG
            } catch { }
            #endif
            
            return false;
        }
    }
}
