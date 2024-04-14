// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Text;
using System.IO;

#nullable disable

using STREAM = System.IO.Stream;
using MEMSTREAM = System.IO.MemoryStream;
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
            return _ComputeHash(stream, _Sha512Engine.Value);
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA384"/> on the contents of the given stream.
        /// </summary>
        public static Byte[] ComputeSha384(this STREAM stream)
        {
            return _ComputeHash(stream, _Sha384Engine.Value);
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA256"/> on the contents of the given stream.
        /// </summary>
        public static Byte[] ComputeSha256(this STREAM stream)
        {
            return _ComputeHash(stream, _Sha256Engine.Value);
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.MD5"/> on the contents of the given stream.
        /// </summary>
        public static Byte[] ComputeMd5(this STREAM stream)
        {
            return _ComputeHash(stream, _Md5Engine.Value);
        }        

        private static Byte[] _ComputeHash(STREAM stream, System.Security.Cryptography.HashAlgorithm engine)
        {
            GuardReadable(stream);

            if (stream is MEMSTREAM memStream)
            {
                if (memStream.TryGetBuffer(out var buff))
                {
                    buff = buff.Slice((int)memStream.Position);
                    return engine.ComputeHash(buff.Array, buff.Offset, buff.Count);
                }
            }

            var position = stream.CanSeek ? stream.Position : -1;

            var value = engine.ComputeHash(stream);

            // try restore stream's position
            try { if (position >= 0) stream.Position = position; } catch { }

            return value;
        }
    }
}