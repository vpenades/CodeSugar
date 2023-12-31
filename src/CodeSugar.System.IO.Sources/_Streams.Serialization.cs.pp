// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

#nullable disable

using STREAM = System.IO.Stream;
using BYTESSEGMENT = System.ArraySegment<byte>;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarIO
    {
        /// <summary>
        /// Reads bytes from the stream until the end of the stream or until the destination buffer is full.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="bytes">The destination buffer.</param>
        /// <returns>true if the bytes have successfully read, or false if EOF</returns>
        public static bool TryReadBytes(this STREAM stream, Span<Byte> bytes)
        {
            if (stream == null) return false;

            var bbb = bytes;

            while (bbb.Length > 0)
            {
                var l = stream.Read(bbb);
                if (l <= 0) return false;

                bbb = bbb.Slice(l);
            }

            return true;
        }

        public static (T0, T1) ReadValues<T0,T1>(this STREAM stream, bool streamIsBigEndian = false)
            where T0:unmanaged
            where T1:unmanaged
        {
            var v0 = ReadValue<T0>(stream, streamIsBigEndian);
            var v1 = ReadValue<T1>(stream, streamIsBigEndian);
            return (v0,v1);
        }

        public static (T0, T1, T2) ReadValues<T0,T1,T2>(this STREAM stream, bool streamIsBigEndian = false)
            where T0:unmanaged
            where T1:unmanaged
            where T2:unmanaged
        {
            var v0 = ReadValue<T0>(stream, streamIsBigEndian);
            var v1 = ReadValue<T1>(stream, streamIsBigEndian);
            var v2 = ReadValue<T2>(stream, streamIsBigEndian);
            return (v0,v1,v2);
        }

        public static (T0, T1, T2, T3) ReadValues<T0,T1,T2, T3>(this STREAM stream, bool streamIsBigEndian = false)
            where T0:unmanaged
            where T1:unmanaged
            where T2:unmanaged
            where T3:unmanaged
        {
            var v0 = ReadValue<T0>(stream, streamIsBigEndian);
            var v1 = ReadValue<T1>(stream, streamIsBigEndian);
            var v2 = ReadValue<T2>(stream, streamIsBigEndian);
            var v3 = ReadValue<T3>(stream, streamIsBigEndian);
            return (v0,v1,v2,v3);
        }

        /// <summary>
        /// Reads a struct value from the stream.
        /// </summary>
        /// <typeparam name="T">A simple struct or a native type.</typeparam>
        /// <param name="stream">The source stream</param>
        /// <param name="streamIsBigEndian">Indicates whether the values stored in the stream are big endian.</param>
        /// <returns>the read value.</returns>
        /// <exception cref="System.IO.EndOfStreamException"></exception>
        public static T ReadValue<T>(this STREAM stream, bool streamIsBigEndian = false)
            where T:unmanaged
        {
            // special cases

            if (typeof(System.Runtime.CompilerServices.ITuple).IsAssignableFrom(typeof(T)))
            {
			    throw new ArgumentException("Unsupported. Use ReadValues instead.", nameof(T));
		    }  

            if (typeof(T) == typeof(TimeSpan))
            {
                var val = ReadValue<long>(stream, streamIsBigEndian);
                return (T)(Object)new TimeSpan(val);
            }            

            if (typeof(T) == typeof(DateTime))
            {
                var val = ReadValue<long>(stream, streamIsBigEndian);
                return (T)(Object)DateTime.FromBinary(val);
            }

            if (typeof(T) == typeof(DateTimeOffset))
            {
                var t = ReadValue<DateTime>(stream, streamIsBigEndian);
                var o = ReadValue<TimeSpan>(stream, streamIsBigEndian);
                return (T)(Object)new DateTimeOffset(t, o);
            }

            _CheckUnmanagedTypeIsSerializable<T>(streamIsBigEndian);

            Span<T> span = stackalloc T[1];            
            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes<T>(span);

            if (!TryReadBytes(stream, buff)) throw new System.IO.EndOfStreamException();

            if (streamIsBigEndian != System.BitConverter.IsLittleEndian) buff.Reverse();            

            return span[0];
        }

        public static void WriteValues<T0,T1>(this STREAM stream, T0 value0, T1 value1, bool streamIsBigEndian = false)
            where T0:unmanaged
            where T1:unmanaged            
        {
            WriteValue<T0>(stream, value0, streamIsBigEndian);
            WriteValue<T1>(stream, value1, streamIsBigEndian);            
        }

        public static void WriteValues<T0,T1,T2>(this STREAM stream, T0 value0, T1 value1, T2 value2, bool streamIsBigEndian = false)
            where T0:unmanaged
            where T1:unmanaged
            where T2:unmanaged            
        {
            WriteValue<T0>(stream, value0, streamIsBigEndian);
            WriteValue<T1>(stream, value1, streamIsBigEndian);
            WriteValue<T2>(stream, value2, streamIsBigEndian);            
        }

        public static void WriteValues<T0,T1,T2,T3>(this STREAM stream, T0 value0, T1 value1, T2 value2, T3 value3, bool streamIsBigEndian = false)
            where T0:unmanaged
            where T1:unmanaged
            where T2:unmanaged
            where T3:unmanaged
        {
            WriteValue<T0>(stream, value0, streamIsBigEndian);
            WriteValue<T1>(stream, value1, streamIsBigEndian);
            WriteValue<T2>(stream, value2, streamIsBigEndian);
            WriteValue<T3>(stream, value3, streamIsBigEndian);
        }

		/// <summary>
		/// Writes a struct value to the stream.
		/// </summary>
		/// <typeparam name="T">A simple struct or a native type.</typeparam>
		/// <param name="stream">The target stream.</param>
		/// <param name="value">The value to write.</param>
		/// <param name="streamIsBigEndian">Indicates whether the values stored in the stream are big endian.</param>
		public static void WriteValue<T>(this STREAM stream, T value, bool streamIsBigEndian = false)
            where T : unmanaged
        {
        switch (value) // special cases
            {
                case System.Runtime.CompilerServices.ITuple tuple: throw new ArgumentException("Unsupported. Use WriteValues instead.", nameof(value));
				case TimeSpan ts: WriteValue(stream, ts.Ticks, streamIsBigEndian); return;
                case DateTime dt: WriteValue(stream, dt.ToBinary(), streamIsBigEndian); return;
                case DateTimeOffset dto:
                    WriteValue(stream, dto.DateTime, streamIsBigEndian);
                    WriteValue(stream, dto.Offset, streamIsBigEndian);
                    return;                
            }

            _CheckUnmanagedTypeIsSerializable<T>(streamIsBigEndian);            

            Span<T> span = stackalloc T[1];            
            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes<T>(span);

            span[0] = value;

            if (streamIsBigEndian != System.BitConverter.IsLittleEndian) buff.Reverse();            

            stream.Write(buff);
        }

        private static void _CheckUnmanagedTypeIsSerializable<T>(bool isBigEndian) where T : unmanaged
        {
            if (BitConverter.IsLittleEndian && !isBigEndian) return;

            // under these circumstances, these types would be wrongly serialized because the element order would also be reversed.

            if (Type.GetTypeCode(typeof(T)) == TypeCode.Empty) throw new NotImplementedException($"Composite values not supported on Big Endian");
        }

        /// <summary>
        /// Packs a <see cref="UInt64"/> value.
        /// </summary>
        /// <param name="stream">The target stream.</param>
        /// <param name="uValue">The value to pack.</param>
        /// <remarks>
        /// Stores the sign in the lowest bit to allow the same encoding strength on positive and negative values.
        /// </remarks>
        public static void WriteSigned64Packed(this STREAM stream, long value)
        {       
            var uval = (ulong)value;

            if (value >= 0)
            {                
                uval <<= 1;                
            }
            else
            {                
                uval = ~uval;
                uval <<= 1;
                uval |= 1;                
            }   
            
            WriteUnsigned64Packed(stream, uval);
        }

        /// <summary>
        /// Packs a <see cref="UInt64"/> value.
        /// </summary>
        /// <param name="stream">The target stream.</param>
        /// <param name="uValue">The value to pack.</param>
        /// <remarks>
        /// This is equivalent to <see cref="BinaryWriter.Write7BitEncodedInt64(long)"/>
        /// </remarks>
        public static void WriteUnsigned64Packed(this STREAM stream, ulong uValue)
        {
            // Write out an int 7 bits at a time. The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            //
            // Using the constants 0x7F and ~0x7F below offers smaller
            // codegen than using the constant 0x80.

            Span<byte> buff = stackalloc byte[10];

            int idx = 0;

            while (uValue > 0x7Fu)
            {
                buff[idx++] =(byte)((uint)uValue | ~0x7Fu);
                uValue >>= 7;
            }

            buff[idx++] =(byte)uValue;

            buff = buff.Slice(0, idx);

            stream.Write(buff);
        }


        /// <summary>
        /// Unpacks a <see cref="Int64"/> value.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <returns>The unpacked value</returns>
        /// <remarks>
        /// Stores the sign in the lowest bit to allow the same encoding strength on positive and negative values.
        /// </remarks>        
        public static long ReadSigned64Packed(this STREAM stream)
        {
            var uval = ReadUnsigned64Packed(stream);
            var neg = (uval & 1) != 0;
            uval >>= 1;
            if (neg) uval = ~uval;
            return (long)uval;
        }

        /// <summary>
        /// Unpacks a <see cref="UInt64"/> value.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <returns>The unpacked value</returns>
        /// <remarks>
        /// This is equivalent to <see cref="BinaryReader.Read7BitEncodedInt64"/>
        /// </remarks>        
        public static ulong ReadUnsigned64Packed(this STREAM stream)
        {
            ulong result = 0;
            int readValue;
            byte byteReadJustNow;

            // Read the integer 7 bits at a time. The high bit
            // of the byte when on means to continue reading more bytes.
            //
            // There are two failure cases: we've read more than 10 bytes,
            // or the tenth byte is about to cause integer overflow.
            // This means that we can read the first 9 bytes without
            // worrying about integer overflow.

            const int MaxBytesWithoutOverflow = 9;
            for (int shift = 0; shift < MaxBytesWithoutOverflow * 7; shift += 7)
            {
                // ReadByte handles end of stream cases for us.
                readValue = stream.ReadByte(); if (readValue < 0) throw new System.IO.EndOfStreamException();
                byteReadJustNow = (byte)readValue;
                result |= (byteReadJustNow & 0x7Ful) << shift;                

                if (byteReadJustNow <= 0x7Fu)
                {
                    return result; // early exit
                }
            }

            // Read the 10th byte. Since we already read 63 bits,
            // the value of this byte must fit within 1 bit (64 - 63),
            // and it must not have the high bit set.

            readValue = stream.ReadByte(); if (readValue < 0) throw new System.IO.EndOfStreamException();
            byteReadJustNow = (byte)readValue;
            if (byteReadJustNow > 0b_1u)
            {
                throw new FormatException("invalid encoding");
            }

            result |= (ulong)byteReadJustNow << (MaxBytesWithoutOverflow * 7);
            return result;
        }

        /// <summary>
        /// Writes a string.
        /// </summary>
        /// <param name="stream">The target stream.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <remarks>
        /// This is equivalent to <see cref="BinaryWriter.Write(string)"/>
        /// </remarks>
        public static void WriteString(this STREAM stream, string text, System.Text.Encoding encoding = null)
        {
            if (encoding == null) encoding = System.Text.Encoding.UTF8;

            if (text == null) text = string.Empty;

            var chars = text.ToCharArray();

            var blen = encoding.GetByteCount(chars);
            
            if (blen < 256)
            {
                Span<byte> span = stackalloc byte[blen];
                var xlen = encoding.GetBytes(chars, span);
                span = span.Slice(0, xlen);
                WriteUnsigned64Packed(stream, (ulong)span.Length);
                stream.Write(span);
                return;
            }            
            
            var buf = encoding.GetBytes(chars);
            WriteUnsigned64Packed(stream, (ulong)buf.Length);
            stream.Write(buf,0, buf.Length);
        }

        /// <summary>
        /// Read a string.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>The string being read.</returns>
        /// <remarks>
        /// This is equivalent to <see cref="BinaryReader.ReadString"/>
        /// </remarks>
        public static string ReadString(this STREAM stream, System.Text.Encoding encoding = null)
        {
            if (encoding == null) encoding = System.Text.Encoding.UTF8;

            // Length of the string in bytes, not chars
            var blen = ReadUnsigned64Packed(stream);
            if (blen > int.MaxValue) throw new IOException("invalid string length");
            if (blen == 0) return string.Empty;

            if (blen < 256)
            {
                Span<byte> buf = stackalloc byte[(int)blen];
                if (!TryReadBytes(stream, buf)) throw new System.IO.EndOfStreamException();

                var s = encoding.GetString(buf);

                return string.IsInterned(s) ?? s;
            }
            else
            {
                var buf = new byte[(int)blen];

                if (!TryReadBytes(stream, buf)) throw new System.IO.EndOfStreamException();

                var s = encoding.GetString(buf);

                return string.IsInterned(s) ?? s;
            }            
        }
    }
}