// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using __FINFO = System.IO.FileInfo;
using __BYTESSEGMENT = System.ArraySegment<byte>;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarForSystemIO
    {
        public static void ThrowNotFound(this __FINFO dinfo, Exception innerException = null)
        {
            GuardNotNull(dinfo);
            if (innerException == null) throw new System.IO.FileNotFoundException(dinfo.FullName);
            else throw new System.IO.FileNotFoundException(dinfo.FullName, innerException);
        }

        /// <summary>
        /// Checks if the file exists, refreshing the state beforehand
        /// </summary>
        /// <param name="file">the file to check</param>
        /// <returns>true if it exists</returns>
        public static bool RefreshedExists(this __FINFO file)
        {
            // https://github.com/dotnet/corefx/pull/40677
            // https://github.com/dotnet/runtime/issues/31425
            // https://github.com/dotnet/runtime/issues/117709

            if (file == null) return false;            
            file.Refresh();
            return CachedExists(file);            
        }

        /// <summary>
        /// Checks if the file exists, using the internal cached value
        /// </summary>
        /// <param name="file">the file to check</param>
        /// <returns>true if it exists</returns>
        public static bool CachedExists(this __FINFO file)
        {
            // https://github.com/dotnet/corefx/pull/40677
            // https://github.com/dotnet/runtime/issues/31425
            // https://github.com/dotnet/runtime/issues/117709

            if (file == null) return false;
            #pragma warning disable            
            return file.Exists;
            #pragma warning restore
        }        

        /// <summary>
        /// Checks if the file exists, bypassing internal cache
        /// </summary>
        /// <param name="file">the file to check</param>
        /// <returns>true if it exists</returns>
        public static bool PhysicallyExists(this __FINFO file)
        {
            if (file == null) return false;
            return System.IO.File.Exists(file.FullName);
        }

        /// <summary>
        /// Gets the file length, refreshing the state beforehand
        /// </summary>
        /// <param name="file">the file to check</param>
        /// <returns>the length of the file</returns>
        public static long RefreshedLength(this __FINFO file)
        {
            // https://github.com/dotnet/corefx/pull/40677
            // https://github.com/dotnet/runtime/issues/31425
            // https://github.com/dotnet/runtime/issues/117709

            if (file == null) return 0;
            file.Refresh();
            return CachedLength(file);
        }

        /// <summary>
        /// Gets the file length, using the internal cached value
        /// </summary>
        /// <param name="file">the file to check</param>
        /// <returns>the length of the file</returns>
        public static long CachedLength(this __FINFO file)
        {
            // https://github.com/dotnet/corefx/pull/40677
            // https://github.com/dotnet/runtime/issues/31425
            // https://github.com/dotnet/runtime/issues/117709

            if (file == null) return 0;
            #pragma warning disable            
            return file.Length;
            #pragma warning restore
        }       
    }
}
