// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Text;
using System.IO;
using System.Diagnostics;

#nullable disable

using __STREAM = System.IO.Stream;
using __MEMSTREAM = System.IO.MemoryStream;
using __LAZYHASHALGORYTHM = System.Lazy<System.Security.Cryptography.HashAlgorithm>;

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
        private static __LAZYHASHALGORYTHM _Sha512Engine = new __LAZYHASHALGORYTHM(System.Security.Cryptography.SHA512.Create);
        private static __LAZYHASHALGORYTHM _Sha384Engine = new __LAZYHASHALGORYTHM(System.Security.Cryptography.SHA384.Create);
        private static __LAZYHASHALGORYTHM _Sha256Engine = new __LAZYHASHALGORYTHM(System.Security.Cryptography.SHA256.Create);
        private static __LAZYHASHALGORYTHM _Md5Engine = new __LAZYHASHALGORYTHM(System.Security.Cryptography.MD5.Create);

        private static System.Security.Cryptography.HashAlgorithm __GetHashAlgorythmBySize(int byteSize)
        {
            switch(byteSize)
            {
                case 16: return _Md5Engine.Value;
                case 32: return _Sha256Engine.Value;
                case 48: return _Sha384Engine.Value;
                case 64: return _Sha512Engine.Value;
                default: throw new ArgumentOutOfRangeException(nameof(byteSize));
            }
        }

        public static void ComputeHashes(this Func<__STREAM> streamFunc, params Byte[][] result)
        {
            using(var s = streamFunc()) { ComputeHashes(s, result); }
        }

        /// <summary>
        /// Computes multiple hashes from the contents of the given stream.
        /// </summary>
        /// <remarks>
        /// The hashing algorythm is selected based in the length of the imput byte array.
        /// </remarks>
        public static void ComputeHashes(this __STREAM stream, params Byte[][] result)
        {
            GuardReadable(stream);

            if (!stream.CanSeek && result.Length > 1) throw new ArgumentException("seekable stream required", nameof(stream));

            var position = stream.CanSeek
                ? stream.Position
                : -1;

            foreach(var r in result)
            {
                var algo = __GetHashAlgorythmBySize(r.Length);

                if (position >= 0) stream.Position = position;

                _ComputeHash(stream, algo).CopyTo(r,0);                
            }
        }

        public static Byte[] ComputeSha512(this Func<__STREAM> streamFunc)
        {
            using (var s = streamFunc()) { return ComputeSha512(s); }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA512"/> on the contents of the given stream.
        /// </summary>
        public static Byte[] ComputeSha512(this __STREAM stream)
        {
            return _ComputeHash(stream, _Sha512Engine.Value);
        }

        public static Byte[] ComputeSha384(this Func<__STREAM> streamFunc)
        {
            using (var s = streamFunc()) { return ComputeSha384(s); }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA384"/> on the contents of the given stream.
        /// </summary>
        public static Byte[] ComputeSha384(this __STREAM stream)
        {
            return _ComputeHash(stream, _Sha384Engine.Value);
        }

        public static Byte[] ComputeSha256(this Func<__STREAM> streamFunc)
        {
            using (var s = streamFunc()) { return ComputeSha256(s); }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA256"/> on the contents of the given stream.
        /// </summary>
        public static Byte[] ComputeSha256(this __STREAM stream)
        {
            return _ComputeHash(stream, _Sha256Engine.Value);
        }

        public static Byte[] ComputeMd5(this Func<__STREAM> streamFunc)
        {
            using (var s = streamFunc()) { return ComputeMd5(s); }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.MD5"/> on the contents of the given stream.
        /// </summary>
        public static Byte[] ComputeMd5(this __STREAM stream)
        {
            return _ComputeHash(stream, _Md5Engine.Value);
        }        

        private static Byte[] _ComputeHash(__STREAM stream, System.Security.Cryptography.HashAlgorithm engine)
        {
            GuardReadable(stream);

            if (stream is __MEMSTREAM memStream)
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