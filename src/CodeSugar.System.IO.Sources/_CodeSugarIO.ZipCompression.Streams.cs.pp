// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

#nullable disable

using ZIPENTRY = System.IO.Compression.ZipArchiveEntry;
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
        public static string ReadAllText(this ZIPENTRY entry)
        {
            GuardReadable(entry);

            using(var s = entry.Open())
            {
                return s.ReadAllText();
            }
        }

        public static void WriteAllText(this ZIPENTRY entry, string text)
        {
            GuardWriteable(entry);

            using (var s = entry.Open())
            {
                s.WriteAllText(text);
            }
        }

        public static BYTESSEGMENT ReadAllBytes(this ZIPENTRY entry)
        {
            GuardReadable(entry);

            using (var s = entry.Open())
            {
                return s.ReadAllBytes();
            }
        }

        public static void WriteAllBytes(this ZIPENTRY entry, IReadOnlyList<Byte> bytes)
        {
            GuardWriteable(entry);

            using (var s = entry.Open())
            {
                s.WriteAllBytes(bytes);
            }
        }

        public static Byte[] ComputeSha512(this ZIPENTRY entry)
        {
            GuardReadable(entry);
            using(var s = entry.Open())
            {
                return s.ComputeSha512();
            }
        }

        public static Byte[] ComputeSha384(this ZIPENTRY entry)
        {
            GuardReadable(entry);
            using(var s = entry.Open())
            {
                return s.ComputeSha384();
            }
        }

        public static Byte[] ComputeSha256(this ZIPENTRY entry)
        {
            GuardReadable(entry);
            using(var s = entry.Open())
            {
                return s.ComputeSha256();
            }
        }

        public static Byte[] ComputeMd5(this ZIPENTRY entry)
        {
            GuardReadable(entry);
            using(var s = entry.Open())
            {
                return s.ComputeMd5();
            }
        }
    }
}