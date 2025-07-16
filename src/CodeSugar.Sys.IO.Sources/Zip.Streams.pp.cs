// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

#nullable disable

using _ZIPENTRY = System.IO.Compression.ZipArchiveEntry;
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
        /// extracts the stream of the entry and copies it to a MemoryStream
        /// </summary>
        /// <remarks>
        /// by default a zip entry returns a compressed stream that does not support Seek operations.
        /// </remarks>
        public static System.IO.MemoryStream ToMemoryStream(this _ZIPENTRY entry)
        {
            GuardReadable(entry);            

            var m = new System.IO.MemoryStream();

            using(var s = entry.Open())
            {                
                s.CopyTo(m);
            }

            m.Position = 0;

            return m;
        }


        public static void CopyToFile(this _ZIPENTRY entry, System.IO.FileInfo dst)
        {
            GuardReadable(entry);
            GuardNotNull(dst);

            using(var dstS = dst.Create())
            {
                using(var srcS = entry.Open())
                {
                    srcS.CopyTo(dstS);
                }
            }

            System.Diagnostics.Debug.Assert(dst.CachedExists());
        }

        public static void CopyFromFile(this _ZIPENTRY entry, System.IO.FileInfo src)
        {            
            GuardWriteable(entry);
            GuardExists(src);

            using(var srcS = src.OpenRead())
            {
                using(var dstS = entry.Open())
                {
                    srcS.CopyTo(dstS);
                }
            }
        }

        public static string ReadAllText(this _ZIPENTRY entry)
        {
            GuardReadable(entry);

            using(var s = entry.Open())
            {
                return s.ReadAllText();
            }
        }

        public static void WriteAllText(this _ZIPENTRY entry, string text)
        {
            GuardWriteable(entry);

            using (var s = entry.Open())
            {
                s.WriteAllText(text);
            }
        }

        public static _BYTESSEGMENT ReadAllBytes(this _ZIPENTRY entry)
        {
            GuardReadable(entry);

            using (var s = entry.Open())
            {
                return s.ReadAllBytes();
            }
        }        

        public static void WriteAllBytes(this _ZIPENTRY entry, IReadOnlyList<Byte> bytes)
        {
            GuardWriteable(entry);

            using (var s = entry.Open())
            {
                s.WriteAllBytes(bytes);
            }
        }

        public static Byte[] ComputeSha512(this _ZIPENTRY entry)
        {
            GuardReadable(entry);
            using(var s = entry.Open())
            {
                return s.ComputeSha512();
            }
        }

        public static Byte[] ComputeSha384(this _ZIPENTRY entry)
        {
            GuardReadable(entry);
            using(var s = entry.Open())
            {
                return s.ComputeSha384();
            }
        }

        public static Byte[] ComputeSha256(this _ZIPENTRY entry)
        {
            GuardReadable(entry);
            using(var s = entry.Open())
            {
                return s.ComputeSha256();
            }
        }

        public static Byte[] ComputeMd5(this _ZIPENTRY entry)
        {
            GuardReadable(entry);
            using(var s = entry.Open())
            {
                return s.ComputeMd5();
            }
        }
    }
}