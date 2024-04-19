// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using FILE = System.IO.FileInfo;

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
        public static bool CheckHash(this FILE finfo, string hash)
        {
            if (string.IsNullOrWhiteSpace(hash)) throw new ArgumentNullException(hash);

            byte[] bytes;

            try
            {
                if (hash.EndsWith("=")) // decode as base64
                {
                    bytes = System.Convert.FromBase64String(hash);
                    return CheckHash(finfo, bytes);
                }
            }
            catch { }

            if (hash.Length % 2 != 0) throw new ArgumentException("incorrect hash size for hex", nameof(hash));

            bytes = new byte[hash.Length / 2];
            for (int i = 0; i < hash.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hash.Substring(i, 2), 16);
            }
            return CheckHash(finfo, bytes);


            throw new ArgumentException("invalid hash", nameof(hash));
        }

        public static bool CheckHash(this FILE finfo, byte[] hash)
        {
            if (hash == null || hash.Length == 0) throw new ArgumentNullException(nameof(hash));

            byte[] fileHash = null;

            switch(hash.Length)
            {
                case 16: fileHash = ComputeMd5(finfo); break;
                case 32: fileHash = ComputeSha256(finfo); break;
                case 48: fileHash = ComputeSha384(finfo); break;
                case 64: fileHash = ComputeSha512(finfo); break;
                default: throw new ArgumentException($"invalid hash size {hash.Length}");
            }

            return hash.AsSpan().SequenceEqual(fileHash);
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA512"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeSha512(this FILE finfo)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                return s.ComputeSha512();
            }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA384"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeSha384(this FILE finfo)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                return s.ComputeSha384();
            }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA256"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeSha256(this FILE finfo)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                return s.ComputeSha256();
            }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.MD5"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeMd5(this FILE finfo)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                return s.ComputeMd5();
            }
        }
    }
}