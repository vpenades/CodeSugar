// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using __FINFO = System.IO.FileInfo;

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
        /// Checks if the given file has the given hash.
        /// </summary>
        /// <param name="finfo">The file to check</param>
        /// <param name="hash">The hash value</param>
        /// <returns>true if the hash matches</returns>
        /// <remarks>
        /// The hash algorythm is selected based in the hash size.
        /// </remarks>
        public static bool CheckHash(this __FINFO finfo, string hash)
        {
            byte[] bytes = __DecodeBase64OrHex(hash);
            return CheckHash(finfo, bytes);
        }

        private static Byte[] __DecodeBase64OrHex(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(value);

            byte[] bytes;

            try
            {
                if (value.EndsWith("=")) // decode as base64
                {
                    return System.Convert.FromBase64String(value);                    
                }
            }
            catch { }

            if (value.Length % 2 != 0) throw new ArgumentException("incorrect hash size for hex", nameof(value));

            bytes = new byte[value.Length / 2];
            for (int i = 0; i < value.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);
            }

            return bytes;
        }

        /// <summary>
        /// Checks if the given file has the given hash.
        /// </summary>
        /// <param name="finfo">The file to check</param>
        /// <param name="hash">The hash value</param>
        /// <returns>true if the hash matches</returns>
        /// <remarks>
        /// The hash algorythm is selected based in the hash size.
        /// </remarks>
        public static bool CheckHash(this __FINFO finfo, byte[] hash)
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
        /// Computes multiple hashes from the contents of the given file.
        /// </summary>
        /// <remarks>
        /// The hashing algorythm is selected based in the length of the imput byte array.
        /// </remarks>
        public static void ComputeHashes(this __FINFO finfo, params Byte[][] result)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                ComputeHashes(s, result);
            }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA512"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeSha512(this __FINFO finfo)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                return ComputeSha512(s);
            }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA384"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeSha384(this __FINFO finfo)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                return ComputeSha384(s);
            }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA256"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeSha256(this __FINFO finfo)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                return ComputeSha256(s);
            }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.MD5"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeMd5(this __FINFO finfo)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                return ComputeMd5(s);
            }
        }
    }
}