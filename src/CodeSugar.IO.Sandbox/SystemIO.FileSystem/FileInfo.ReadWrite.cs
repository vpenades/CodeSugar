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

using System.Diagnostics.CodeAnalysis;

using static System.Net.Mime.MediaTypeNames;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions    
    {
        [return: NotNull]
        public static Func<System.IO.Stream> GetReadStreamFunction([NotNull] this __FINFO finfo)
        {
            GuardExists(finfo);
            return finfo.OpenRead;
        }

        [return: NotNull]
        public static Func<System.IO.Stream> GetWriteStreamFunction([NotNull] this __FINFO finfo, bool syncFile = true)
        {
            GuardNotNull(finfo);

            EnsureDirectoryExists(finfo.Directory);

            if (!syncFile) return finfo.Create;

            System.IO.Stream openWrite()
            {                
                return finfo.Create().WithDisposeObserver(() => finfo.Refresh());
            }

            return openWrite;
        }

        /// <summary>
        /// Reads all the text lines from the given file.
        /// Equivalent to <see cref="System.IO.File.ReadAllLines(string, Encoding)"/>
        /// </summary>
        [Obsolete("Use GetReadStreamFunction().ReadAllLines()")]
        public static IReadOnlyList<string> ReadAllLines([DisallowNull] this __FINFO finfo, Encoding encoding = null)
        {
            return GetReadStreamFunction(finfo).ReadAllLines(encoding);
        }

        /// <summary>
        /// Reads all the text from the given file.
        /// Equivalent to <see cref="System.IO.File.ReadAllText(string, Encoding)"/>
        /// </summary>
        [Obsolete("Use GetReadStreamFunction().ReadAllText()")]
        public static string ReadAllText([DisallowNull] this __FINFO finfo, Encoding encoding = null)
        {
            return GetReadStreamFunction(finfo).ReadAllText(encoding);
        }

        /// <summary>
		/// Writes all the text lines to a given file.
		/// Equivalent to <see cref="System.IO.File.WriteAllLines(string, IEnumerable{string})"/>
		/// </summary>
        [Obsolete("Use GetWriteStreamFunction().WriteAllLines()")]
        public static void WriteAllLines([DisallowNull] this __FINFO finfo, Encoding encoding, params string[] lines)
        {
            GetWriteStreamFunction(finfo).WriteAllLines(lines, encoding);
        }

        /// <summary>
		/// Writes all the text lines to a given file.
		/// Equivalent to <see cref="System.IO.File.WriteAllLines(string, IEnumerable{string})"/>
		/// </summary>
        [Obsolete("Use GetWriteStreamFunction().WriteAllLines()")]
        public static void WriteAllLines([DisallowNull] this __FINFO finfo, [DisallowNull] IEnumerable<string> lines, Encoding encoding = null)
        {
            GetWriteStreamFunction(finfo).WriteAllLines(lines, encoding);
        }

        /// <summary>
        /// Writes all the text to a given file.
        /// Equivalent to <see cref="System.IO.File.WriteAllText(string, string, Encoding)"/>
        /// </summary>
        [Obsolete("Use GetWriteStreamFunction().WriteAllText()")]
        public static void WriteAllText([DisallowNull] this __FINFO finfo, string text, Encoding encoding = null)
        {
            GetWriteStreamFunction(finfo, true).WriteAllText(text, encoding);
        }

        /// <summary>
        /// Reads all the bytes from a given file.
        /// Equivalent to <see cref="System.IO.File.ReadAllBytes(string)"/>
        /// </summary>        
        [Obsolete("Use GetReadStreamFunction().ReadAllBytes()")]
        public static __BYTESSEGMENT ReadAllBytes([DisallowNull] this __FINFO finfo)
        {
            return GetReadStreamFunction(finfo).ReadAllBytes();
        }

        /// <summary>
        /// Writes all the bytes to a given file.
        /// Equivalent to <see cref="System.IO.File.WriteAllBytes(string, byte[])"/>
        /// </summary>        
        [Obsolete("Use GetWriteStreamFunction().WriteAllBytes()")]
        public static void WriteAllBytes([DisallowNull] this __FINFO finfo, IReadOnlyList<Byte> bytes)
        {
            GetWriteStreamFunction(finfo, true).WriteAllBytes(bytes);
        }        
    }
}