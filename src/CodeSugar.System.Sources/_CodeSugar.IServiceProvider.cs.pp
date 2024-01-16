// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

using RTINTEROPSVCS = System.Runtime.InteropServices;

#nullable disable

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
        /// <summary>
        /// Gets the given service from a service provider, or default if the service does not exist.
        /// </summary>
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) return default;

            var result = serviceProvider.GetService(typeof(T));

            return result is T value
                ? value
                : default;
        }

        /// <summary>
        /// Tries to get a service from a service provider
        /// </summary>
        public static bool TryGetService<T>(this IServiceProvider serviceProvider, out T service)
        {
            service = default;

            if (serviceProvider == null) return false;

            var result = serviceProvider.GetService(typeof(T));

            if (result is T value)
            {
                service = value;
                return true;
            }            
            
            return false;
        }
    }
}
