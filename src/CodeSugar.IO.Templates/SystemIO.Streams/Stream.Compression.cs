using System;
using System.Text;
using System.IO;

#nullable disable

using __STREAM = System.IO.Stream;

using __STREAMFUNC = System.Func<System.IO.FileMode, System.IO.Stream>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        public static __STREAMFUNC Inflate(this __STREAMFUNC readFunc)
        {
            if (readFunc == null) return null;

            __STREAM _inflate(System.IO.FileMode mode)
            {
                var s = readFunc.Invoke(mode);
                GuardReadable(s);
                return new System.IO.Compression.DeflateStream(s, System.IO.Compression.CompressionMode.Decompress, false);
            }

            return _inflate;
        }

        public static __STREAMFUNC Deflate(this __STREAMFUNC writeFunc, System.IO.Compression.CompressionLevel level = System.IO.Compression.CompressionLevel.Optimal)
        {
            if (writeFunc == null) return null;

            __STREAM _deflate(System.IO.FileMode mode)
            {
                var s = writeFunc.Invoke(mode);
                GuardWriteable(s);
                return new System.IO.Compression.DeflateStream(s, level, false);
            }

            return _deflate;
        }

        public static __STREAMFUNC GzipInflate(this __STREAMFUNC readFunc)
        {
            if (readFunc == null) return null;

            __STREAM _inflate(System.IO.FileMode mode)
            {
                var s = readFunc.Invoke(mode);
                GuardReadable(s);
                return new System.IO.Compression.GZipStream(s, System.IO.Compression.CompressionMode.Decompress, false);
            }

            return _inflate;
        }

        public static __STREAMFUNC GZipDeflate(this __STREAMFUNC writeFunc, System.IO.Compression.CompressionLevel level = System.IO.Compression.CompressionLevel.Optimal)
        {
            if (writeFunc == null) return null;

            __STREAM _deflate(System.IO.FileMode mode)
            {
                var s = writeFunc.Invoke(mode);
                GuardWriteable(s);
                return new System.IO.Compression.GZipStream(s, level, false);
            }

            return _deflate;
        }

        public static __STREAMFUNC BrotliInflate(this __STREAMFUNC readFunc)
        {
            if (readFunc == null) return null;

            __STREAM _inflate(System.IO.FileMode mode)
            {
                var s = readFunc.Invoke(mode);
                GuardReadable(s);
                return new System.IO.Compression.BrotliStream(s, System.IO.Compression.CompressionMode.Decompress, false);
            }

            return _inflate;
        }

        public static __STREAMFUNC BrotliDeflate(this __STREAMFUNC readFunc, System.IO.Compression.CompressionLevel level = System.IO.Compression.CompressionLevel.Optimal)
        {
            if (readFunc == null) return null;

            __STREAM _deflate(System.IO.FileMode mode)
            {
                var s = readFunc.Invoke(mode);
                GuardWriteable(s);
                return new System.IO.Compression.BrotliStream(s, level, false);
            }

            return _deflate;
        }
    }
}
