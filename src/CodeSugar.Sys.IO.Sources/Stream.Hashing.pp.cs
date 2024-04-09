// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Text;
using System.IO;

#nullable disable

using STREAM = System.IO.Stream;
using LAZYHASHALGORYTHM = System.Lazy<System.Security.Cryptography.HashAlgorithm>;

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
        private static LAZYHASHALGORYTHM _Sha512Engine = new LAZYHASHALGORYTHM(System.Security.Cryptography.SHA512.Create);
        private static LAZYHASHALGORYTHM _Sha384Engine = new LAZYHASHALGORYTHM(System.Security.Cryptography.SHA384.Create);
        private static LAZYHASHALGORYTHM _Sha256Engine = new LAZYHASHALGORYTHM(System.Security.Cryptography.SHA256.Create);
        private static LAZYHASHALGORYTHM _Md5Engine = new LAZYHASHALGORYTHM(System.Security.Cryptography.MD5.Create);

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA512"/> on the contents of the given stream.
        /// </summary>
        public static Byte[] ComputeSha512(this STREAM stream)
        {
            GuardReadable(stream);
            return _Sha512Engine.Value.ComputeHash(stream);
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA384"/> on the contents of the given stream.
        /// </summary>
        public static Byte[] ComputeSha384(this STREAM stream)
        {
            GuardReadable(stream);
            return _Sha384Engine.Value.ComputeHash(stream);
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA256"/> on the contents of the given stream.
        /// </summary>
        public static Byte[] ComputeSha256(this STREAM stream)
        {
            GuardReadable(stream);
            return _Sha256Engine.Value.ComputeHash(stream);
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.MD5"/> on the contents of the given stream.
        /// </summary>
        public static Byte[] ComputeMd5(this STREAM stream)
        {
            GuardReadable(stream);
            return _Md5Engine.Value.ComputeHash(stream);
        }        
    }
}