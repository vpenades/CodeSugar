// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

#nullable disable

using __READABLEBLOCK_SPAM = System.ReadOnlySpan<byte>;
using __WRITEABLEBLOCK_SPAN = System.Span<byte>;

using __READABLEBLOCK_ARRAY = System.ArraySegment<byte>;
using __WRITEABLEBLOCK_ARRAY = System.ArraySegment<byte>;

using __READABLEBLOCK__STREAM = System.IO.Stream;
using __WRITEABLEBLOCK__STREAM = System.IO.Stream;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForSerialization
    {
        #region core - Span

        // it could be great to have an enum Endianness { PlatformSpecific, LittleEndian, BigEndian }
        // so the extension would be: Endianness.LittleEndian.ReadValue(source,out value);        

        /// <summary>
        /// Writes the value to the target, using the given endianness
        /// </summary>
        /// <returns>The pointer to the next position.</returns>
        [return: NotNull]
        public static __WRITEABLEBLOCK_SPAN WriteEndian<T>([NotNull] this __WRITEABLEBLOCK_SPAN target, T value, bool targetIsBigEndian)
            where T : unmanaged
        {
            if (targetIsBigEndian != System.BitConverter.IsLittleEndian) return WritePlatform(target, value);

            Span<T> span = stackalloc T[1];
            span[0] = value;

            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes(span);

            buff.Reverse();
            buff.CopyTo(target);

            return target.Slice(buff.Length);
        }

        /// <summary>
        /// Reads the value from the source, using the given endianness
        /// </summary>
        /// <returns>The pointer to the next position.</returns>
        [return: NotNull]
        public static __READABLEBLOCK_SPAM ReadEndian<T>([NotNull] this __READABLEBLOCK_SPAM source, out T value, bool sourceIsBigEndian)
            where T : unmanaged
        {
            if (sourceIsBigEndian != System.BitConverter.IsLittleEndian) return ReadPlatform(source, out value);

            Span<T> span = stackalloc T[1];

            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes(span);

            source.Slice(0, buff.Length).CopyTo(buff);

            buff.Reverse();

            value = span[0];
            return source.Slice(buff.Length);
        }

        /// <summary>
        /// Writes the value to the target, using the platform endianness
        /// </summary>
        /// <returns>The pointer to the next position.</returns>
        [return: NotNull]
        public static __WRITEABLEBLOCK_SPAN WritePlatform<T>([NotNull] this __WRITEABLEBLOCK_SPAN target, T value)
            where T : unmanaged
        {
            var sequence = System.Runtime.InteropServices.MemoryMarshal.Cast<byte, T>(target);
            sequence[0] = value;

            return target.Slice(__SizeOf<T>.ByteSize);
        }

        /// <summary>
        /// Reads the value from the source, using the platform endianness
        /// </summary>
        /// <returns>The pointer to the next position.</returns>
        [return: NotNull]
        public static __READABLEBLOCK_SPAM ReadPlatform<T>([NotNull] this __READABLEBLOCK_SPAM source, out T value)
            where T : unmanaged
        {
            var sequence = System.Runtime.InteropServices.MemoryMarshal.Cast<byte, T>(source);

            value = sequence[0];

            return source.Slice(__SizeOf<T>.ByteSize);
        }

        [return: NotNull]
        public static __WRITEABLEBLOCK_SPAN WriteBytes([NotNull] this __WRITEABLEBLOCK_SPAN target, ReadOnlySpan<Byte> array)
        {
            array.CopyTo(target);
            return target.Slice(array.Length);
        }

        [return: NotNull]
        public static __READABLEBLOCK_SPAM ReadBytes([NotNull] this __READABLEBLOCK_SPAM source, Span<Byte> array)
        {
            source.Slice(0, array.Length).CopyTo(array);
            return source.Slice(array.Length);
        }

        #endregion

        #region core - Array Segment

        // it could be great to have an enum Endianness { PlatformSpecific, LittleEndian, BigEndian }
        // so the extension would be: Endianness.LittleEndian.ReadValue(source,out value);        

        /// <summary>
        /// Writes the value to the target, using the given endianness
        /// </summary>
        /// <returns>The pointer to the next position.</returns>
        [return: NotNull]
        public static __WRITEABLEBLOCK_ARRAY WriteEndian<T>([NotNull] this __WRITEABLEBLOCK_ARRAY target, T value, bool targetIsBigEndian)
            where T : unmanaged
        {
            if (targetIsBigEndian != System.BitConverter.IsLittleEndian) return WritePlatform(target, value);

            Span<T> span = stackalloc T[1];
            span[0] = value;

            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes(span);

            buff.Reverse();
            buff.CopyTo(target);

            return target.Slice(buff.Length);
        }

        /// <summary>
        /// Reads the value from the source, using the given endianness
        /// </summary>
        /// <returns>The pointer to the next position.</returns>
        [return: NotNull]
        public static __READABLEBLOCK_ARRAY ReadEndian<T>([NotNull] this __READABLEBLOCK_ARRAY source, out T value, bool sourceIsBigEndian)
            where T : unmanaged
        {
            if (sourceIsBigEndian != System.BitConverter.IsLittleEndian) return ReadPlatform(source, out value);

            Span<T> span = stackalloc T[1];

            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes(span);

            source.Slice(0, buff.Length).AsSpan().CopyTo(buff);

            buff.Reverse();

            value = span[0];
            return source.Slice(buff.Length);
        }

        /// <summary>
        /// Writes the value to the target, using the platform endianness
        /// </summary>
        /// <returns>The pointer to the next position.</returns>
        [return: NotNull]
        public static __WRITEABLEBLOCK_ARRAY WritePlatform<T>([NotNull] this __WRITEABLEBLOCK_ARRAY target, T value)
            where T : unmanaged
        {
            var sequence = System.Runtime.InteropServices.MemoryMarshal.Cast<byte, T>(target.AsSpan());
            sequence[0] = value;

            return target.Slice(__SizeOf<T>.ByteSize);
        }

        /// <summary>
        /// Reads the value from the source, using the platform endianness
        /// </summary>
        /// <returns>The pointer to the next position.</returns>
        [return: NotNull]
        public static __READABLEBLOCK_ARRAY ReadPlatform<T>([NotNull] this __READABLEBLOCK_ARRAY source, out T value)
            where T : unmanaged
        {
            var sequence = System.Runtime.InteropServices.MemoryMarshal.Cast<byte, T>(source.AsSpan());

            value = sequence[0];

            return source.Slice(__SizeOf<T>.ByteSize);
        }

        [return: NotNull]
        public static __WRITEABLEBLOCK_ARRAY WriteBytes([NotNull] this __WRITEABLEBLOCK_ARRAY target, ReadOnlySpan<Byte> array)            
        {
            array.CopyTo(target);
            return target.Slice(array.Length);
        }

        [return: NotNull]
        public static __READABLEBLOCK_ARRAY ReadBytes([NotNull] this __READABLEBLOCK_ARRAY source, Span<Byte> array)
        {
            source.Slice(0, array.Length).AsSpan().CopyTo(array);            
            return source.Slice(array.Length);
        }

        #endregion

        #region core - Stream

        /// <summary>
        /// Writes a struct value to the stream.
        /// </summary>
        /// <typeparam name="T">A simple struct or a native type.</typeparam>
        /// <param name="stream">The target stream.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="streamIsBigEndian">Indicates whether the values stored in the stream are big endian.</param>
        [return: NotNull]
        public static __WRITEABLEBLOCK__STREAM WriteEndian<T>([NotNull] this __WRITEABLEBLOCK__STREAM stream, T value, bool streamIsBigEndian)
            where T : unmanaged
        {
            if (streamIsBigEndian != System.BitConverter.IsLittleEndian) return WritePlatform(stream, value);

            Span<T> span = stackalloc T[1];
            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes(span);

            WriteEndian(buff, value, streamIsBigEndian);

            stream.Write(buff);

            return stream;
        }

        /// <summary>
        /// Reads a struct value from the stream.
        /// </summary>
        /// <typeparam name="T">A simple struct or a native type.</typeparam>
        /// <param name="stream">The source stream</param>
        /// <param name="value">value read</param>
        /// <param name="streamIsBigEndian">Indicates whether the values stored in the stream are big endian.</param>
        /// <returns>the read value.</returns>
        /// <exception cref="System.IO.EndOfStreamException"></exception>
        [return: NotNull]
        public static __READABLEBLOCK__STREAM ReadEndian<T>([NotNull] this __READABLEBLOCK__STREAM stream, out T value, bool streamIsBigEndian)
            where T : unmanaged
        {
            if (streamIsBigEndian != System.BitConverter.IsLittleEndian) return ReadPlatform(stream, out value);

            Span<T> span = stackalloc T[1];
            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes<T>(span);

            #if !NET8_0_OR_GREATER
            ReadExactly(stream, buff);
            #else
            stream.ReadExactly(buff);
            #endif

            ReadEndian<T>(buff, out value, streamIsBigEndian);

            return stream;
        }


        /// <summary>
        /// Writes a struct value to the stream.
        /// </summary>
        /// <typeparam name="T">A simple struct or a native type.</typeparam>
        /// <param name="stream">The target stream.</param>
        /// <param name="value">The value to write.</param>        
        [return: NotNull]
        public static __WRITEABLEBLOCK__STREAM WritePlatform<T>([NotNull] this __WRITEABLEBLOCK__STREAM stream, T value)
            where T : unmanaged
        {
            Span<T> span = stackalloc T[1];
            span[0] = value;

            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes(span);            

            stream.Write(buff);

            return stream;
        }

        /// <summary>
        /// Reads a struct value from the stream.
        /// </summary>
        /// <typeparam name="T">A simple struct or a native type.</typeparam>
        /// <param name="stream">The source stream</param>
        /// <param name="value">value read</param>
        /// <returns>the read value.</returns>
        /// <exception cref="System.IO.EndOfStreamException"></exception>
        [return: NotNull]
        public static __READABLEBLOCK__STREAM ReadPlatform<T>([NotNull] this __READABLEBLOCK__STREAM stream, out T value)
            where T : unmanaged
        {
            Span<T> span = stackalloc T[1];
            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes<T>(span);

            #if !NET8_0_OR_GREATER
            ReadExactly(stream, buff);
            #else
            stream.ReadExactly(buff);
            #endif

            value = span[0];

            return stream;
        }

        [return: NotNull]
        public static __WRITEABLEBLOCK__STREAM WriteBytes([NotNull] this __WRITEABLEBLOCK__STREAM stream, __READABLEBLOCK_SPAM array)
        {
            stream.Write(array);
            return stream;
        }

        [return: NotNull]
        public static __READABLEBLOCK__STREAM ReadBytes([NotNull] this __READABLEBLOCK__STREAM stream, __WRITEABLEBLOCK_SPAN array)
        {
            #if !NET8_0_OR_GREATER
            ReadExactly(stream, array);
            #else
            stream.ReadExactly(array);
            #endif

            return stream;
        }

        #endregion

        #region core - Progress

        /// <summary>
        /// Writes a struct value to the stream.
        /// </summary>
        /// <typeparam name="T">A simple struct or a native type.</typeparam>
        /// <param name="stream">The target stream.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="streamIsBigEndian">Indicates whether the values stored in the stream are big endian.</param>
        [return: NotNull]
        public static IProgress<Byte> WriteEndian<T>([NotNull] this IProgress<Byte> stream, T value, bool streamIsBigEndian)
            where T : unmanaged
        {
            if (streamIsBigEndian != System.BitConverter.IsLittleEndian) return WritePlatform(stream, value);

            Span<T> span = stackalloc T[1];
            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes(span);

            WriteEndian(buff, value, streamIsBigEndian);

            return WriteBytes(stream, buff);
        }

        /// <summary>
        /// Writes a struct value to the stream.
        /// </summary>
        /// <typeparam name="T">A simple struct or a native type.</typeparam>
        /// <param name="stream">The target stream.</param>
        /// <param name="value">The value to write.</param>
        [return: NotNull]
        public static IProgress<Byte> WritePlatform<T>([NotNull] this IProgress<Byte> stream, T value)
            where T : unmanaged
        {
            Span<T> span = stackalloc T[1];
            span[0] = value;

            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes(span);            

            return WriteBytes(stream, buff);
        }

        [return: NotNull]
        public static IProgress<Byte> WriteBytes([NotNull] this IProgress<Byte> stream, __READABLEBLOCK_SPAM array)
        {
            foreach (var b in array) stream.Report(b);
            return stream;
        }

        #endregion

        #region core - Enumerator

        /// <summary>
        /// Reads a struct value from the stream.
        /// </summary>
        /// <typeparam name="T">A simple struct or a native type.</typeparam>
        /// <param name="stream">The source stream</param>
        /// <param name="value">value read</param>
        /// <param name="streamIsBigEndian">Indicates whether the values stored in the stream are big endian.</param>
        /// <returns>the read value.</returns>
        /// <exception cref="System.IO.EndOfStreamException"></exception>
        [return: NotNull]
        public static IEnumerator<Byte> ReadEndian<T>([NotNull] this IEnumerator<Byte> stream, out T value, bool streamIsBigEndian)
            where T : unmanaged
        {
            if (streamIsBigEndian != System.BitConverter.IsLittleEndian) return ReadPlatform(stream, out value);

            Span<T> span = stackalloc T[1];
            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes<T>(span);

            stream = ReadBytes(stream, buff);

            ReadEndian<T>(buff, out value, streamIsBigEndian);

            return stream;
        }

        /// <summary>
        /// Reads a struct value from the stream.
        /// </summary>
        /// <typeparam name="T">A simple struct or a native type.</typeparam>
        /// <param name="stream">The source stream</param>
        /// <param name="value">value read</param>        
        /// <returns>the read value.</returns>
        /// <exception cref="System.IO.EndOfStreamException"></exception>
        [return: NotNull]
        public static IEnumerator<Byte> ReadPlatform<T>([NotNull] this IEnumerator<Byte> stream, out T value)
            where T : unmanaged
        {
            Span<T> span = stackalloc T[1];
            var buff = System.Runtime.InteropServices.MemoryMarshal.AsBytes<T>(span);

            stream = ReadBytes(stream, buff);

            value = span[0];

            return stream;
        }

        [return: NotNull]
        public static IEnumerator<Byte> ReadBytes([NotNull] this IEnumerator<Byte> stream, __WRITEABLEBLOCK_SPAN array)
        {
            while(!array.IsEmpty)
            {
                if (!stream.MoveNext()) throw new System.IO.EndOfStreamException();
                array[0] = stream.Current;
                array = array.Slice(1);
            }

            return stream;
        }

        #endregion
    }
}