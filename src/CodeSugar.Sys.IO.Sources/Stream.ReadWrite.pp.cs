﻿// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

#nullable disable

using _STREAM = System.IO.Stream;
using _BYTESSEGMENT = System.ArraySegment<byte>;

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
        #region diagnostics

        #if !NET

        public static void GuardReadable(this _STREAM stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException("Can't read from strean", nameof(stream));
        }

        public static void GuardWriteable(this _STREAM stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite) throw new ArgumentException("Can't read from strean", nameof(stream));
        }

        public static void GuardSeekable(this _STREAM stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanSeek) throw new ArgumentException("Can't seek strean", nameof(stream));
        }

        #else

        public static void GuardReadable(this _STREAM stream, [CallerArgumentExpression("stream")] string name = null)
        {
            if (stream == null) throw new ArgumentNullException(name);
            if (!stream.CanRead) throw new ArgumentException("Can't read from strean", name);
        }

        public static void GuardWriteable(this _STREAM stream, [CallerArgumentExpression("stream")] string name = null)
        {
            if (stream == null) throw new ArgumentNullException(name);
            if (!stream.CanWrite) throw new ArgumentException("Can't read from strean", name);
        }

        public static void GuardSeekable(this _STREAM stream, [CallerArgumentExpression("stream")] string name = null)
        {
            if (stream == null) throw new ArgumentNullException(name);
            if (!stream.CanSeek) throw new ArgumentException("Can't seek strean", name);
        }

        #endif

        #endregion

        #region seeking

        public static bool TrySetPosition(this _STREAM stream, long position)
        {
            if (stream == null) return false;
            if (!stream.CanSeek) return false;
            if (position < 0) return false;

            try
            {
                stream.Position = position;
                return true;
            }
            catch (ObjectDisposedException) { }
            catch (System.IO.IOException) { }
            catch (System.NotSupportedException) { }

            return false;
        }

        #endregion

        #region text extensions

        #if NETSTANDARD || NETFRAMEWORK
        private static readonly Encoding UTF8NoBOM = new UTF8Encoding(false);
        #endif

        public static async Task<IReadOnlyList<string>> ReadAllLinesAsync(this Func<Task<_STREAM>> openStream, CancellationToken ctoken, Encoding encoding = null)
        {
            using (var s = await openStream().ConfigureAwait(true))
            {
                return await ReadAllLinesAsync(s, ctoken, encoding).ConfigureAwait(true);
            }
        }

        public static async Task<IReadOnlyList<string>> ReadAllLinesAsync(this _STREAM stream, CancellationToken ctoken, Encoding encoding = null)
        {
            using (var sr = CreateTextReader(stream, true, encoding))
            {
                string line;
                var lines = new List<string>();

                while ((line = await sr.ReadLineAsync().ConfigureAwait(true)) != null)
                {
                    lines.Add(line);
                }

                return lines;
            }
        }

        public static IReadOnlyList<string> ReadAllLines(this Func<_STREAM> openStream, Encoding encoding = null)
        {
            using (var s = openStream())
            {
                return ReadAllLines(s, encoding);
            }
        }

        public static IReadOnlyList<string> ReadAllLines(this _STREAM stream, Encoding encoding = null)
        {
            using(var sr = CreateTextReader(stream, true, encoding))
            {
                string line;
                var lines = new List<string>();
                
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }

                return lines;
            }
        }

        public static void WriteAllLines(this _STREAM stream, Encoding encoding, params string[] lines)
        {
            WriteAllLines(stream, lines.AsEnumerable(), encoding);
        }

        public static void WriteAllLines(this _STREAM stream, IEnumerable<string> lines, Encoding encoding = null)
        {
            using(var sw = CreateTextWriter(stream, true, encoding))
            {
                foreach(var line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="StreamWriter"/> from the given <see cref="_STREAM"/>
        /// </summary>
        public static StreamWriter CreateTextWriter(this _STREAM stream, bool leaveStreamOpen = true, Encoding encoding = null)
        {
            GuardWriteable(stream);

            #if NETSTANDARD || NETFRAMEWORK
            // NetStd implementation matching net6
            return new StreamWriter(stream, encoding ?? UTF8NoBOM, 1024, leaveStreamOpen);
            #else
            return new StreamWriter(stream, encoding: encoding, leaveOpen: leaveStreamOpen);
            #endif
        }

		/// <summary>
		/// Creates a <see cref="StreamReader"/> from the given <see cref="_STREAM"/>
		/// </summary>
		public static StreamReader CreateTextReader(this _STREAM stream, bool leaveStreamOpen = true, Encoding encoding = null)
        {
            GuardReadable(stream);

            #if NETSTANDARD || NETFRAMEWORK
            // NetStd implementation matching net6
            return new StreamReader(stream, encoding ?? Encoding.UTF8, true, 1024, leaveStreamOpen);
            #else
            return new StreamReader(stream, encoding: encoding, leaveOpen: leaveStreamOpen);
            #endif    
        }

		/// <summary>
		/// writes all the text from the given stream.
		/// Equivalent to <see cref="System.IO.File.WriteAllText(string, string?, Encoding)"/>
		/// </summary>   
		public static void WriteAllText(this _STREAM stream, string contents, Encoding encoding = null)
        {
            GuardWriteable(stream);

            if (contents == null) contents = string.Empty;

            using (var ss = CreateTextWriter(stream, true, encoding))
            {
                ss.Write(contents);            
            }
        }

        

        public static string ReadAllText(this Func<Stream> openStream, Encoding encoding = null)
        {
            using (var s = openStream())
            {
                return ReadAllText(s, encoding);
            }
        }

        /// <summary>
        /// Reads all the text from the given stream.
        /// Equivalent to <see cref="System.IO.File.ReadAllText(string, Encoding)"/>
        /// </summary>   
        public static string ReadAllText(this _STREAM stream, Encoding encoding = null)
        {
            GuardReadable(stream);

            using (var sr = CreateTextReader(stream, true, encoding))
            {
                return sr.ReadToEnd();
            }
        }

        public static async Task<string> ReadAllTextAsync(this Func<Task<_STREAM>> openStream, CancellationToken ctoken, Encoding encoding = null)
        {
            using (var s = await openStream().ConfigureAwait(true))
            {
                return await ReadAllTextAsync(s, ctoken, encoding).ConfigureAwait(true);
            }
        }

        public static async Task<string> ReadAllTextAsync(this _STREAM stream, CancellationToken ctoken, Encoding encoding = null)
        {
            GuardReadable(stream);

            using (var sr = CreateTextReader(stream, true, encoding))
            {
                return await sr.ReadToEndAsync().ConfigureAwait(true);
            }
        }

        #endregion

        #region binary extensions

		/// <summary>
		/// Creates a <see cref="BinaryWriter"/> from the given <see cref="_STREAM"/>
		/// </summary>
		public static BinaryWriter CreateBinaryWriter(this _STREAM stream, bool leaveStreamOpen = true, Encoding encoding = null)
        {
            GuardWriteable(stream);

            if (encoding == null) encoding = Encoding.UTF8;

            return new System.IO.BinaryWriter(stream, encoding, leaveStreamOpen);
        }

		/// <summary>
		/// Creates a <see cref="BinaryReader"/> from the given <see cref="_STREAM"/>
		/// </summary>
		public static BinaryReader CreateBinaryReader(this _STREAM stream, bool leaveStreamOpen = true, Encoding encoding = null)
        {
            GuardReadable(stream);

            if (encoding == null) encoding = Encoding.UTF8;

            return new System.IO.BinaryReader(stream, encoding, leaveStreamOpen);
        }

		/// <summary>
		/// Writes all the bytes to the given stream.
		/// Equivalent to <see cref="System.IO.File.WriteAllBytes(string, byte[])"/>
		/// </summary>   
		public static void WriteAllBytes(this _STREAM stream, IReadOnlyList<Byte> bytes)
        {
            GuardWriteable(stream);

            if (bytes.Count == 0) return;

            switch(bytes)
            {
                case Byte[] array: stream.Write(array, 0, array.Length); break;
                case _BYTESSEGMENT segment: stream.Write(segment.Array, segment.Offset, segment.Count); break;                    
                default:                    
                    var buf = new Byte[8192];
                    var pos = 0;
                    while(pos < buf.Length)
                    {
                        var len = Math.Min(buf.Length, bytes.Count - pos);
                        for (int i = 0; i < len; ++i) buf[i] = bytes[pos + i];
                        stream.Write(buf, 0, len);
                        pos += buf.Length;
                    }
                    break;

            }
        }

		/// <summary>
		/// Writes all the bytes to the given stream.
		/// Equivalent to <see cref="System.IO.File.WriteAllBytesAsync(string, byte[], CancellationToken)"/>
		/// </summary>  
		public static async Task WriteAllBytesAsync(this _STREAM stream, IReadOnlyList<Byte> bytes, CancellationToken ctoken)
        {
            GuardWriteable(stream);

            if (bytes.Count == 0) return;

            switch(bytes)
            {
                case Byte[] array: await stream.WriteAsync(array, 0, array.Length, ctoken).ConfigureAwait(true); break;
                case _BYTESSEGMENT segment: await stream.WriteAsync(segment.Array, segment.Offset, segment.Count, ctoken).ConfigureAwait(true); break;

                default:                    
                    var buf = new Byte[8192];
                    var pos = 0;
                    while(pos < buf.Length)
                    {
                        var len = Math.Min(buf.Length, bytes.Count - pos);
                        for (int i = 0; i < len; ++i) buf[i] = bytes[pos + i];
                        await stream.WriteAsync(buf, 0, len, ctoken).ConfigureAwait(true);
                        pos += buf.Length;
                    }
                    break;
            }
        }

        public static async Task<_BYTESSEGMENT> ReadAllBytesAsync(this Func<Task<_STREAM>> openStream, CancellationToken ctoken)
        {
            using (var s = await openStream().ConfigureAwait(true))
            {
                return await s.ReadAllBytesAsync(ctoken).ConfigureAwait(true);
            }
        }

        public static _BYTESSEGMENT ReadAllBytes(this Func<_STREAM> openStream)
        {
            using (var s = openStream())
            {
                return s.ReadAllBytes();
            }
        }

		/// <summary>
		/// Reads all the bytes from the given stream.
		/// Equivalent to <see cref="System.IO.File.ReadAllBytes(string)"/>
		/// </summary>
		public static _BYTESSEGMENT ReadAllBytes(this _STREAM stream)
        {
            GuardReadable(stream);

            // fast path for MemoryStream

            if (stream is MemoryStream memStream)
            {
                if (memStream.TryGetBuffer(out var buffer))
                {
                    buffer = buffer.Slice((int)memStream.Position);
                    return new _BYTESSEGMENT(buffer.ToArray()); // ReadAllBytes always return a copy;
                }
            }

            // taken from Net6's ReadAllBytes

            long fileLength = stream.CanSeek ? stream.Length : 0;

            if (fileLength > int.MaxValue)
            {
                throw new IOException("File too long");
            }

            if (fileLength == 0)
            {
                // Some file systems (e.g. procfs on Linux) return 0 for length even when there's content;
                // also there is non-seekable file stream.
                // Thus we need to assume 0 doesn't mean empty.

                using (var m = new System.IO.MemoryStream())
                {
                    stream.CopyTo(m);
                    return m.TryGetBuffer(out var buffer)
                        ? buffer
                        : new _BYTESSEGMENT(m.ToArray());
                }
            }

            int index = 0;
            int count = (int)fileLength;
            byte[] bytes = new byte[count];
            while (count > 0)
            {
                int n = stream.Read(bytes, index, count);
                if (n == 0) throw new System.IO.EndOfStreamException();

                index += n;
                count -= n;
            }
            return new _BYTESSEGMENT(bytes);
        }

		/// <summary>
		/// Reads all the bytes from the given stream.
		/// Equivalent to <see cref="System.IO.File.ReadAllBytesAsync(string, CancellationToken)"/>
		/// </summary>
		public static async Task<_BYTESSEGMENT> ReadAllBytesAsync(this _STREAM stream, CancellationToken ctoken)
        {
            GuardReadable(stream);

            // fast path for MemoryStream

            if (stream is MemoryStream memStream)
            {
                if (memStream.TryGetBuffer(out var buffer))
                {
                    buffer = buffer.Slice((int)memStream.Position);
                    return new _BYTESSEGMENT(buffer.ToArray()); // ReadAllBytesAsync always return a copy;
                }
            }

            // taken from Net6's ReadAllBytes

            long fileLength = stream.CanSeek ? stream.Length : 0;

            if (fileLength > int.MaxValue)
            {
                throw new IOException("File too long");
            }

            if (fileLength == 0)
            {
                // Some file systems (e.g. procfs on Linux) return 0 for length even when there's content;
                // also there is non-seekable file stream.
                // Thus we need to assume 0 doesn't mean empty.

                using (var m = new System.IO.MemoryStream())
                {
                    await stream.CopyToAsync(m, ctoken).ConfigureAwait(true);
                    return m.TryGetBuffer(out var buffer)
                        ? buffer
                        : m.ToArray();
                }
            }

            int index = 0;
            int count = (int)fileLength;
            byte[] bytes = new byte[count];
            while (count > 0)
            {
                var memory = new Memory<Byte>(bytes, index, count);

                int n = await stream.ReadAsync(memory, ctoken).ConfigureAwait(true);
                if (n == 0) throw new System.IO.EndOfStreamException();

                index += n;
                count -= n;
            }
            return new _BYTESSEGMENT(bytes);
        }

        #endregion        
    }
}