// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Collections;

#nullable disable

using _XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using _XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;


#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForFileProviders        
    {
        #region diagnostics

        #if !NET

        /// <summary>
        /// Checks whether a <see cref="_XINFO"/> is not null.
        /// </summary>        
        /// <exception cref="ArgumentNullException"></exception>
        public static void GuardNotNull(this _XINFO info, string name = null)
        {
            if (info == null) throw new ArgumentNullException(name ?? nameof(info));
        }

        /// <summary>
        /// Checks whether a <see cref="_XINFO"/> exists in the file system.
        /// </summary>        
        /// <exception cref="ArgumentNullException"></exception>
        public static void GuardExists(this _XINFO info, string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);
            if (!info.Exists) throw new ArgumentException($"'{info.PhysicalPath ?? info.Name}' does not exist.", name ?? nameof(info));
        }

        /// <summary>
        /// Checks whether a <see cref="_XDIRECTORY"/> is not null.
        /// </summary>        
        /// <exception cref="ArgumentNullException"></exception>
        public static void GuardNotNull(this _XDIRECTORY info, string name = null)
        {
            if (info == null) throw new ArgumentNullException(name ?? nameof(info));
        }

        /// <summary>
        /// Checks whether a <see cref="_XDIRECTORY"/> exists in the file system.
        /// </summary>        
        /// <exception cref="ArgumentNullException"></exception>
        public static void GuardExists(this _XDIRECTORY info, string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);
            if (!info.Exists) throw new ArgumentException($"does not exist.", name ?? nameof(info));
        }

        #else

		/// <summary>
		/// Checks whether a <see cref="_XINFO"/> is not null.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardNotNull(this _XINFO info, [CallerArgumentExpression("info")] string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);            
        }

		/// <summary>
		/// Checks whether a <see cref="_XINFO"/> exists in the file system.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardExists(this _XINFO info, [CallerArgumentExpression("info")] string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);
            if (!info.Exists) throw new ArgumentException($"'{info.PhysicalPath ?? info.Name}' does not exist.", name);
        }

        /// <summary>
		/// Checks whether a <see cref="_XDIRECTORY"/> is not null.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardNotNull(this _XDIRECTORY info, [CallerArgumentExpression("info")] string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);            
        }

		/// <summary>
		/// Checks whether a <see cref="_XDIRECTORY"/> exists in the file system.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardExists(this _XDIRECTORY info, [CallerArgumentExpression("info")] string name = null)
        {
            if (info == null) throw new ArgumentNullException(name);
            if (!info.Exists) throw new ArgumentException($"does not exist.", name);
        }

        #endif

        #endregion
    }
}
