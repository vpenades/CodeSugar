using System;
using System.Text;
using System.IO;

#nullable disable

using __STREAM = System.IO.Stream;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        public static Func<__STREAM> Inflate(this Func<__STREAM> readFunc)
        {
            if (readFunc == null) return null;

            __STREAM _inflate()
            {
                var s = readFunc.Invoke();
                GuardReadable(s);
                return new System.IO.Compression.DeflateStream(s, System.IO.Compression.CompressionMode.Decompress, false);
            }

            return _inflate;
        }

        public static Func<__STREAM> Deflate(this Func<__STREAM> writeFunc, System.IO.Compression.CompressionLevel level = System.IO.Compression.CompressionLevel.Optimal)
        {
            if (writeFunc == null) return null;

            __STREAM _deflate()
            {
                var s = writeFunc.Invoke();
                GuardWriteable(s);
                return new System.IO.Compression.DeflateStream(s, level, false);
            }

            return _deflate;
        }

        public static Func<__STREAM> GzipInflate(this Func<__STREAM> readFunc)
        {
            if (readFunc == null) return null;

            __STREAM _inflate()
            {
                var s = readFunc.Invoke();
                GuardReadable(s);
                return new System.IO.Compression.GZipStream(s, System.IO.Compression.CompressionMode.Decompress, false);
            }

            return _inflate;
        }

        public static Func<__STREAM> GZipDeflate(this Func<__STREAM> writeFunc, System.IO.Compression.CompressionLevel level = System.IO.Compression.CompressionLevel.Optimal)
        {
            if (writeFunc == null) return null;

            __STREAM _deflate()
            {
                var s = writeFunc.Invoke();
                GuardWriteable(s);
                return new System.IO.Compression.GZipStream(s, level, false);
            }

            return _deflate;
        }

        public static Func<__STREAM> BrotliInflate(this Func<__STREAM> readFunc)
        {
            if (readFunc == null) return null;

            __STREAM _inflate()
            {
                var s = readFunc.Invoke();
                GuardReadable(s);
                return new System.IO.Compression.BrotliStream(s, System.IO.Compression.CompressionMode.Decompress, false);
            }

            return _inflate;
        }

        public static Func<__STREAM> BrotliDeflate(this Func<__STREAM> readFunc, System.IO.Compression.CompressionLevel level = System.IO.Compression.CompressionLevel.Optimal)
        {
            if (readFunc == null) return null;

            __STREAM _deflate()
            {
                var s = readFunc.Invoke();
                GuardWriteable(s);
                return new System.IO.Compression.BrotliStream(s, level, false);
            }

            return _deflate;
        }
    }
}
