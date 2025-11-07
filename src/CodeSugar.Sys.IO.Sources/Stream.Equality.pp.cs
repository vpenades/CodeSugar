// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Text;
using System.IO;
using System.Xml.Serialization;

#nullable disable

using __FINFO = System.IO.FileInfo;
using __STREAM = System.IO.Stream;
using __MEMSTREAM = System.IO.MemoryStream;
using __BYTESEGMENT = System.ArraySegment<byte>;

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
        private const int DEFAULTEQUALITYCOMPAREBUFFERLENGTH = 1024 * 1024 * 256; // 256 mb

        public static bool StreamEquals(this __FINFO a, __FINFO b, Func<long, __MEMSTREAM> memStreamFactory = null, int bufferSize = DEFAULTEQUALITYCOMPAREBUFFERLENGTH)
        {
            GuardExists(a);
            GuardExists(b);

            if (a.RefreshedLength() != b.RefreshedLength()) return false;

            if (Object.ReferenceEquals(a, b)) return true; // both files are the same

            return StreamEquals(a.OpenRead, b.OpenRead, memStreamFactory, bufferSize);
        }

        public static bool StreamEquals(this __FINFO a, Func<__STREAM> b, Func<long, __MEMSTREAM> memStreamFactory = null, int bufferSize = DEFAULTEQUALITYCOMPAREBUFFERLENGTH)
        {
            GuardExists(a);
            return StreamEquals(a.OpenRead, b, memStreamFactory, bufferSize);
        }

        public static bool StreamEquals(this Func<__STREAM> a, Func<__STREAM> b, Func<long, __MEMSTREAM> memStreamFactory = null, int bufferSize = DEFAULTEQUALITYCOMPAREBUFFERLENGTH)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));            

            using (var x = a.Invoke())
            {
                using (var y = b.Invoke())
                {
                    return _StreamEqualsCore(x, y, false, memStreamFactory, bufferSize);
                }
            }
        }

        public static bool StreamEquals(this __STREAM x, __STREAM y, Func<long, __MEMSTREAM> memStreamFactory = null, int bufferSize = DEFAULTEQUALITYCOMPAREBUFFERLENGTH)
        {
            return _StreamEqualsCore(x, y, true, memStreamFactory, bufferSize);
        }

        private static bool _StreamEqualsCore(__STREAM x, __STREAM y, bool restorePositions, Func<long, __MEMSTREAM> memStreamFactory, int largeBufferSize)
        {
            if (largeBufferSize <= 0) throw new ArgumentOutOfRangeException(nameof(largeBufferSize));
            
            if (object.ReferenceEquals(x, y)) return true; // both streams are the same.

            GuardReadable(x);
            GuardReadable(y);

            // keep positions so we can restore them back later
            var xPosition = x.CanSeek ? x.Position : -1;
            var yPosition = y.CanSeek ? y.Position : -1;            

            try
            {                
                if (x is __MEMSTREAM || y is __MEMSTREAM) // if at least one stream is in memory, we can use a small buffer.
                {
                    largeBufferSize = 65536;
                }
                else // Large buffers are only needed if both streams are read from a physical drive
                {
                    __MEMSTREAM defMemStreamFactory(long len)
                    {
                        if (len > largeBufferSize) return null;
                        if (len >= int.MaxValue) return null;
                        return new __MEMSTREAM((int)len);
                    }

                    memStreamFactory ??= defMemStreamFactory;

                    bool result;
                    if (_TryCheckEqualityUsingMemoryStream(x, y, memStreamFactory, out result)) return result;
                    if (_TryCheckEqualityUsingMemoryStream(y, x, memStreamFactory, out result)) return result;
                }                

                __BYTESEGMENT xbuff = new byte[largeBufferSize];
                __BYTESEGMENT ybuff = new byte[largeBufferSize];

                while (true)
                {
                    var xlen = _StreamEqualityReadBytes(x, xbuff);
                    var ylen = _StreamEqualityReadBytes(y, ybuff);

                    if (xlen != ylen) return false;// if number of bytes read mismatch, files have different length

                    if (xlen <= 0) return true; // both EOF reached, streams are equal.

                    var xslice = xbuff.Slice(0, xlen);
                    var yslice = ybuff.Slice(0, xlen);

                    if (!xslice.AsSpan().SequenceEqual(yslice)) return false;
                }
            }
            finally
            {
                if (restorePositions)
                {
                    // try restore stream's positions
                    try { if (xPosition >= 0) x.Position = xPosition; } catch { }
                    try { if (yPosition >= 0) y.Position = yPosition; } catch { }
                }                
            }            
        }

        private static bool _TryCheckEqualityUsingMemoryStream(__STREAM x, __STREAM y, Func<long, __MEMSTREAM> memStreamFactory, out bool result)
        {
            result = false;

            try
            {
                var xLen = x.CanSeek ? x.Length : long.MaxValue;
                var yLen = y.CanSeek ? y.Length : long.MaxValue;

                // streams reporting length 0 may be misleading
                if (xLen == 0) xLen = long.MaxValue;
                if (yLen == 0) yLen = long.MaxValue;

                if (xLen > yLen) return false;

                using (var xm = memStreamFactory(xLen))
                {
                    if (xm == null) return false;                    
                    System.Diagnostics.Debug.Assert(xm.Position == 0);

                    x.CopyTo(xm);
                    xm.Position = 0;
                    result = _StreamEqualsCore(xm, y, false, memStreamFactory, 65536);
                    return true;
                }
            }
            catch {  return false; }
        }

        /// <summary>
        /// Reads bytes from the stream until the end of the stream or until the destination buffer is full.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="bytes">The destination buffer.</param>
        /// <returns>the number of bytes read.</returns>
        private static int _StreamEqualityReadBytes(__STREAM stream, __BYTESEGMENT bytes)
        {
            var bbb = bytes;

            while (bbb.Count > 0)
            {
                var l = stream.Read(bbb.Array, bbb.Offset, bbb.Count);
                if (l <= 0) return bytes.Count - bbb.Count;

                bbb = bbb.Slice(l);
            }

            return bytes.Count;
        }
    }
}
