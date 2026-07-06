using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.InteropServices;
using System.Text;
using System.Buffers;
using System.Reflection;
using System.Transactions;
using System.Threading;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    /// <example>    
    /// var crc = _Crc32Checksum.InitState();
    /// crc = _Crc32Checksum.Append(crc, 6);
    /// crc = _Crc32Checksum.Append(crc, 9);
    /// var checksum = _Crc32Checksum.GetChecksum(crc);
    /// </example>
    internal static partial class CodeSugarNumericsExtensions
    {
        static class _Crc32Checksum
        {
            #region constants

            static _Crc32Checksum()
            {
                uint poly = 0xedb88320;
                _Table = new uint[256];
                uint temp = 0;
                for (uint i = 0; i < _Table.Length; ++i)
                {
                    temp = i;
                    for (int j = 8; j > 0; --j)
                    {
                        if ((temp & 1) == 1)
                        {
                            temp = (uint)((temp >> 1) ^ poly);
                        }
                        else
                        {
                            temp >>= 1;
                        }
                    }
                    _Table[i] = temp;
                }
            }

            private static readonly uint[] _Table;

            #endregion

            #region API

            public static uint InitState() => 0xffffffff;

            public static uint GetChecksum(uint state) => ~state;

            public static uint Append<T>(uint crc, ReadOnlySpan<T> values)
            {
                for (int i = 0; i < values.Length; ++i)
                {                    
                    crc = Append(crc, values[i]);
                }

                return crc;
            }

            public static uint Append<T>(uint crc, T value)
            {
                switch (value)
                {
                    case byte number: return Append(crc, number);
                    case sbyte number: return Append(crc, (byte)number);

                    case string txt:
                        var count = System.Text.Encoding.UTF8.GetByteCount(txt);
                        if (count < 65536)
                        {
                            Span<byte> buff = stackalloc byte[count];
                            System.Text.Encoding.UTF8.GetBytes(txt, buff);
                            return Append(crc, buff);
                        }
                        else
                        {
                            var buff = System.Text.Encoding.UTF8.GetBytes(txt);
                            return Append(crc, buff);
                        }                        

                    case Int16 number:
                        {
                            Span<byte> buff = stackalloc byte[2];
                            System.Buffers.Binary.BinaryPrimitives.WriteInt16LittleEndian(buff, number);
                            return Append(crc, buff);
                        }

                    case UInt16 number:
                        {
                            Span<byte> buff = stackalloc byte[2];
                            System.Buffers.Binary.BinaryPrimitives.WriteUInt16LittleEndian(buff, number);
                            return Append(crc, buff);
                        }

                    case Int32 number:
                        {
                            Span<byte> buff = stackalloc byte[4];
                            System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(buff, number);
                            return Append(crc, buff);
                        }

                    case UInt32 number:
                        {
                            Span<byte> buff = stackalloc byte[4];
                            System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(buff, number);
                            return Append(crc, buff);
                        }

                    case Int64 number:
                        {
                            Span<byte> buff = stackalloc byte[8];
                            System.Buffers.Binary.BinaryPrimitives.WriteInt64LittleEndian(buff, number);
                            return Append(crc, buff);
                        }

                    case UInt64 number:
                        {
                            Span<byte> buff = stackalloc byte[8];
                            System.Buffers.Binary.BinaryPrimitives.WriteUInt64LittleEndian(buff, number);
                            return Append(crc, buff);
                        }

                    #if NET5_0_OR_GREATER
                    case Half number:
                        {
                            Span<byte> buff = stackalloc byte[2];
                            System.Buffers.Binary.BinaryPrimitives.WriteHalfLittleEndian(buff, number);
                            return Append(crc, buff);
                        }
                    #endif                    

                    case Single number:
                        {
                            Span<byte> buff = stackalloc byte[4];
                            #if NET
                            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(buff, number);
                            #else                            
                            System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(buff, BitConverter.SingleToInt32Bits(number));
                            #endif
                            return Append(crc, buff);                         
                        }

                    case Double number:
                        {
                            Span<byte> buff = stackalloc byte[8];
                            #if NET
                            System.Buffers.Binary.BinaryPrimitives.WriteDoubleLittleEndian(buff, number);
                            #else
                            System.Buffers.Binary.BinaryPrimitives.WriteInt64LittleEndian(buff, BitConverter.DoubleToInt64Bits(number));
                            #endif
                            return Append(crc, buff);                            
                        }                    

                    default: throw new NotImplementedException(typeof(T).Name);
                }
            }

            public static uint Append(uint crc, ReadOnlySpan<byte> bytes)
            {                
                for (int i = 0; i < bytes.Length; ++i)
                {
                    crc = Append(crc, bytes[i]);
                }
                return crc;
            }

            public static uint Append(uint crc, byte value)
            {
                byte index = (byte)((crc & 0xff) ^ value);
                return (uint)((crc >> 8) ^ _Table[index]);
            }            

#endregion
        }
    }
}
