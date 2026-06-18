using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

#nullable disable

using __ZIPENTRY = System.IO.Compression.ZipArchiveEntry;
using __BYTESSEGMENT = System.ArraySegment<byte>;

using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions    
    {
        [return: NotNull]
        public static Func<System.IO.Stream> GetReadStreamFunction([NotNull] this __ZIPENTRY entry)
        {
            GuardReadable(entry);
            return entry.Open;
        }

        [return: NotNull]
        public static Func<System.IO.Stream> GetWriteStreamFunction([NotNull] this __ZIPENTRY entry)
        {
            GuardWriteable(entry);
            return entry.Open;
        }        

        /// <summary>
        /// extracts the stream of the entry and copies it to a MemoryStream
        /// </summary>
        /// <remarks>
        /// by default a zip entry returns a compressed stream that does not support Seek operations.
        /// </remarks>
        [Obsolete("Use GetReadStreamFunction().ToMemoryStream()")]
        public static System.IO.MemoryStream ToMemoryStream(this __ZIPENTRY entry)
        {
            return GetReadStreamFunction(entry).ToMemoryStream();
        }


        public static void CopyToFile(this __ZIPENTRY entry, System.IO.FileInfo dst)
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

        public static void CopyFromFile(this __ZIPENTRY entry, System.IO.FileInfo src)
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

        [Obsolete("Use GetReadStreamFunction().ReadAllText()", true)]
        public static string ReadAllText(this __ZIPENTRY entry)
        {
            return GetReadStreamFunction(entry).ReadAllText();
        }

        [Obsolete("Use GetWriteStreamFunction().WriteAllText()", true)]
        public static void WriteAllText(this __ZIPENTRY entry, string text)
        {
            GetWriteStreamFunction(entry).WriteAllText(text);
        }

        [Obsolete("Use GetReadStreamFunction().ReadAllBytes()", true)]
        public static __BYTESSEGMENT ReadAllBytes(this __ZIPENTRY entry)
        {
            return GetReadStreamFunction(entry).ReadAllBytes();
        }

        [Obsolete("Use GetWriteStreamFunction().WriteAllBytes()", true)]
        public static void WriteAllBytes(this __ZIPENTRY entry, IReadOnlyList<Byte> bytes)
        {
            GetWriteStreamFunction(entry).WriteAllBytes(bytes);
        }

        [Obsolete("Use GetReadStreamFunction().ComputeSha512()", true)]
        public static Byte[] ComputeSha512(this __ZIPENTRY entry)
        {
            return GetReadStreamFunction(entry).ComputeSha512();
        }

        [Obsolete("Use GetReadStreamFunction().ComputeSha384()", true)]
        public static Byte[] ComputeSha384(this __ZIPENTRY entry)
        {
            return GetReadStreamFunction(entry).ComputeSha384();
        }

        [Obsolete("Use GetReadStreamFunction().ComputeSha256()", true)]
        public static Byte[] ComputeSha256(this __ZIPENTRY entry)
        {
            return GetReadStreamFunction(entry).ComputeSha256();
        }

        [Obsolete("Use GetReadStreamFunction().ComputeMd5()", true)]
        public static Byte[] ComputeMd5(this __ZIPENTRY entry)
        {
            return GetReadStreamFunction(entry).ComputeMd5();
        }
    }
}