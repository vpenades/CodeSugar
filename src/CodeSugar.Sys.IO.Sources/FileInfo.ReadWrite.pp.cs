﻿// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using _FINFO = System.IO.FileInfo;
using _BYTESSEGMENT = System.ArraySegment<byte>;

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
        public static IReadOnlyList<string> ReadAllLines(this _FINFO finfo, Encoding encoding = null)
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
        public static string ReadAllText(this _FINFO finfo, Encoding encoding = null)
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
        public static void WriteAllLines(this _FINFO finfo, Encoding encoding, params string[] lines)
        {
            WriteAllLines(finfo, lines.AsEnumerable(), encoding);
        }

        /// <summary>
		/// Writes all the text lines to a given file.
		/// Equivalent to <see cref="System.IO.File.WriteAllLines(string, IEnumerable{string})"/>
		/// </summary>
		public static void WriteAllLines(this _FINFO finfo, IEnumerable<string> lines, Encoding encoding = null)
        {
            GuardNotNull(finfo);

            EnsureDirectoryExists(finfo.Directory);

            using(var s = finfo.Create())
            {
                s.WriteAllLines(lines, encoding);
            }

            System.Diagnostics.Debug.Assert(finfo.CachedExists());
        }

		/// <summary>
		/// Writes all the text to a given file.
		/// Equivalent to <see cref="System.IO.File.WriteAllText(string, string, Encoding)"/>
		/// </summary>
		public static void WriteAllText(this _FINFO finfo, string text, Encoding encoding = null)
        {
            GuardNotNull(finfo);

            EnsureDirectoryExists(finfo.Directory);

            using(var s = finfo.Create())
            {
                s.WriteAllText(text, encoding);
            }

            System.Diagnostics.Debug.Assert(finfo.CachedExists());
        }

        /// <summary>
        /// Reads all the bytes from a given file.
        /// Equivalent to <see cref="System.IO.File.ReadAllBytes(string)"/>
        /// </summary>        
        public static _BYTESSEGMENT ReadAllBytes(this _FINFO finfo)
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
        public static void WriteAllBytes(this _FINFO finfo, IReadOnlyList<Byte> bytes)
        {
			GuardNotNull(finfo);

            EnsureDirectoryExists(finfo.Directory);

            using(var s = finfo.Create())
            {
                s.WriteAllBytes(bytes);
            }

            System.Diagnostics.Debug.Assert(finfo.CachedExists());
        }        
    }
}