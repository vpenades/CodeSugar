// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using BYTESSEGMENT = System.ArraySegment<byte>;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarIO    
    {
        /// <summary>
        /// Reads all the text from the given file.
        /// Equivalent to <see cref="System.IO.File.ReadAllText(string, Encoding)"/>
        /// </summary>        
        public static string ReadAllText(this FileInfo finfo, Encoding encoding = null)
        {
			GuardExists(finfo);

            using(var s = finfo.OpenRead())
            {
                return s.ReadAllText(encoding);
            }
        }

		/// <summary>
		/// Writes all the text to a given file.
		/// Equivalent to <see cref="System.IO.File.WriteAllText(string, string, Encoding)(string, Encoding)"/>
		/// </summary>
		public static void WriteAllText(this FileInfo finfo, string text, Encoding encoding = null)
        {
            GuardNotNull(finfo);

            using(var s = finfo.OpenWrite())
            {
                s.WriteAllText(text, encoding);
            }

            finfo.Refresh();
        }

        /// <summary>
        /// Reads all the bytes from a given file.
        /// Equivalent to <see cref="System.IO.File.ReadAllBytes(string)"/>
        /// </summary>        
        public static BYTESSEGMENT ReadAllBytes(this FileInfo finfo)
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
        public static void WriteAllBytes(this FileInfo finfo, IReadOnlyList<Byte> bytes)
        {
			GuardNotNull(finfo);

            using(var s = finfo.OpenWrite())
            {
                s.WriteAllBytes(bytes);
            }

            finfo.Refresh();
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA512"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeSha512(this FileInfo finfo)
        {
            GuardExists(finfo);

            using(var s = finfo.OpenRead())
            {
                return s.ComputeSha512();
            }
        }

		/// <summary>
		/// Computes the <see cref="System.Security.Cryptography.SHA384"/> on the contents of the given file.
		/// </summary>
		public static Byte[] ComputeSha384(this FileInfo finfo)
        {
			GuardExists(finfo);

            using(var s = finfo.OpenRead())
            {
                return s.ComputeSha384();
            }
        }

		/// <summary>
		/// Computes the <see cref="System.Security.Cryptography.SHA256"/> on the contents of the given file.
		/// </summary>
		public static Byte[] ComputeSha256(this FileInfo finfo)
        {
			GuardExists(finfo);

            using(var s = finfo.OpenRead())
            {
                return s.ComputeSha256();
            }
        }

		/// <summary>
		/// Computes the <see cref="System.Security.Cryptography.MD5"/> on the contents of the given file.
		/// </summary>
		public static Byte[] ComputeMd5(this FileInfo finfo)
        {
			GuardExists(finfo);

            using(var s = finfo.OpenRead())
            {
                return s.ComputeMd5();
            }
        }
    }
}