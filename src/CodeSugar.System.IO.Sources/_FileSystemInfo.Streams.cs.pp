// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
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
        /// Reads all the text lines from the given file.
        /// Equivalent to <see cref="System.IO.File.ReadAllLines(string, Encoding)"/>
        /// </summary>        
        public static IReadOnlyList<string> ReadAllLines(this FileInfo finfo, Encoding encoding = null)
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
        public static string ReadAllText(this FileInfo finfo, Encoding encoding = null)
        {
			GuardExists(finfo);

            using(var s = finfo.OpenRead())
            {
                return s.ReadAllText(encoding);
            }
        }

        public static void WriteAllLines(this FileInfo finfo, Encoding encoding, params string[] lines)
        {
            WriteAllLines(finfo, lines.AsEnumerable(), encoding);
        }

        /// <summary>
		/// Writes all the text lines to a given file.
		/// Equivalent to <see cref="System.IO.File.WriteAllLines(string, IEnumerabke<string>, Encoding)(string, Encoding)"/>
		/// </summary>
		public static void WriteAllLines(this FileInfo finfo, IEnumerable<string> lines, Encoding encoding = null)
        {
            GuardNotNull(finfo);

            using(var s = finfo.OpenWrite())
            {
                s.WriteAllLines(lines, encoding);
            }

            finfo.Refresh();
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

        public static void WriteShortcutUri(this FileInfo finfo, Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            if (uri.IsFile)
            {
                WriteAllLines(finfo, null,
                    "[{000214A0-0000-0000-C000-000000000046}]",
                    "Prop3=19,11",
                    "[InternetShortcut]",
                    "IDList=",
                    $"URL={uri.AbsoluteUri}",
                    "IconIndex=1",
                    "IconFile=" + uri.LocalPath.Replace('\\', '/')
                    );
            }
            else
            {
                WriteAllLines(finfo, null,
                    "[{000214A0-0000-0000-C000-000000000046}]",
                    "Prop3=19,11",
                    "[InternetShortcut]",
                    "IDList=",
                    $"URL={uri.AbsoluteUri}",
                    "IconIndex=0"
                    );
            }            
        }

        public static Uri ReadShortcutUri(this FileInfo finfo)
        {
            var lines = ReadAllLines(finfo);

            var line = lines.FirstOrDefault(l=> l.StartsWith("URL="));
            if (line == null) return null;
            line = line.Trim();
            if (line.Length < 5) return null;

            line = line.Substring(4); // remove "URL="

            line = line.Trim();

            return Uri.TryCreate(line, UriKind.Absolute, out Uri uri) ? uri : null;        
        }
    }
}