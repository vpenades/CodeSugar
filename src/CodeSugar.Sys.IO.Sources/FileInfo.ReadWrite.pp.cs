// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using FILE = System.IO.FileInfo;
using BYTESSEGMENT = System.ArraySegment<byte>;

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
        /// <summary>
        /// Reads all the text lines from the given file.
        /// Equivalent to <see cref="System.IO.File.ReadAllLines(string, Encoding)"/>
        /// </summary>        
        public static IReadOnlyList<string> ReadAllLines(this FILE finfo, Encoding encoding = null)
        {
			GuardExists(finfo);

            using(var s = finfo.OpenRead())
            {
                return s.ReadAllLines(encoding);
            }
        }

        /// <summary>
        /// Reads all the text from the given file.
        /// Equivalent to <see cref="System.IO.File.ReadAllText(string, Encoding)"/>
        /// </summary>        
        public static string ReadAllText(this FILE finfo, Encoding encoding = null)
        {
			GuardExists(finfo);

            using(var s = finfo.OpenRead())
            {
                return s.ReadAllText(encoding);
            }
        }

        /// <summary>
		/// Writes all the text lines to a given file.
		/// Equivalent to <see cref="System.IO.File.WriteAllLines(string, IEnumerable{string})"/>
		/// </summary>
        public static void WriteAllLines(this FILE finfo, Encoding encoding, params string[] lines)
        {
            WriteAllLines(finfo, lines.AsEnumerable(), encoding);
        }

        /// <summary>
		/// Writes all the text lines to a given file.
		/// Equivalent to <see cref="System.IO.File.WriteAllLines(string, IEnumerable{string})"/>
		/// </summary>
		public static void WriteAllLines(this FILE finfo, IEnumerable<string> lines, Encoding encoding = null)
        {
            GuardNotNull(finfo);

            EnsureDirectoryExists(finfo.Directory);

            using(var s = finfo.Create())
            {
                s.WriteAllLines(lines, encoding);
            }

            finfo.Refresh();
        }

		/// <summary>
		/// Writes all the text to a given file.
		/// Equivalent to <see cref="System.IO.File.WriteAllText(string, string, Encoding)"/>
		/// </summary>
		public static void WriteAllText(this FILE finfo, string text, Encoding encoding = null)
        {
            GuardNotNull(finfo);

            EnsureDirectoryExists(finfo.Directory);

            using(var s = finfo.Create())
            {
                s.WriteAllText(text, encoding);
            }

            finfo.Refresh();
        }

        /// <summary>
        /// Reads all the bytes from a given file.
        /// Equivalent to <see cref="System.IO.File.ReadAllBytes(string)"/>
        /// </summary>        
        public static BYTESSEGMENT ReadAllBytes(this FILE finfo)
        {
			GuardExists(finfo);

            using(var s = finfo.OpenRead())
            {
                return s.ReadAllBytes();
            }
        }

        /// <summary>
        /// Writes all the bytes to a given file.
        /// Equivalent to <see cref="System.IO.File.WriteAllBytes(string, byte[])"/>
        /// </summary>        
        public static void WriteAllBytes(this FILE finfo, IReadOnlyList<Byte> bytes)
        {
			GuardNotNull(finfo);

            EnsureDirectoryExists(finfo.Directory);

            using(var s = finfo.Create())
            {
                s.WriteAllBytes(bytes);
            }

            finfo.Refresh();
        }        
    }
}