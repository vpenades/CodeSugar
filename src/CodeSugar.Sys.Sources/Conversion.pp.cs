// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarForSystem
    {
        public static string ConvertToHexString(this string plainText, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;            
            var bytes = encoding.GetBytes(plainText);
            return ConvertToHexString(bytes);
        }        
        public static string ConvertToHexString(this IReadOnlyList<Byte> bytes)
        {
            #if NET6_0_OR_GREATER
            switch(bytes)
            {
                case Byte[] array: return Convert.ToHexString(array);
                case ArraySegment<Byte> segment: return Convert.ToHexString(segment.Array, segment.Offset, segment.Count);
            }
            #endif

            var hex = new StringBuilder(bytes.Count * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        public static string ConvertToBase64String(this string plainText, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;

            var bytes = encoding.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }        
        

        public static bool TryParseHexString(this string hexString, out string plainText, Encoding encoding = null)
        {
            plainText = null;
            if (!TryParseHexString(hexString, out byte[] bytes)) return false;
            encoding ??= Encoding.UTF8;
            plainText = encoding.GetString(bytes);
            return true;
        }

        public static bool TryParseHexString(this string hexString, out byte[] bytes)
        {            
            bytes = new byte[hexString.Length / 2];

            for (int i = 0; i < bytes.Length; ++i)
            {
                var slice = hexString.Substring(i * 2, 2);

                if (!TryParseHexString(slice, out byte value)) return false;
                
                bytes[i] = value;
            }

            return true;
        }

        public static bool TryParseHexString(this string hexString, out byte value)
        {
            value = 0;
            if (hexString == null || hexString.Length > 2) return false;

            foreach(var c in hexString)
            {
                if (!TryParseHexNibble(c, out var nibble)) return false;
                value |= (byte)nibble;
                value <<= 4;
            }

            return true;
        }

        public static bool TryParseHexNibble(this char c, out int nibble)
        {
            switch (c)
            {
                case '0': nibble = 0; return true;
                case '1': nibble = 1; return true;
                case '2': nibble = 2; return true;
                case '3': nibble = 3; return true;
                case '4': nibble = 4; return true;
                case '5': nibble = 5; return true;
                case '6': nibble = 6; return true;
                case '7': nibble = 7; return true;
                case '8': nibble = 8; return true;
                case '9': nibble = 9; return true;
                case 'A': nibble = 10; return true;
                case 'B': nibble = 11; return true;
                case 'C': nibble = 12; return true;
                case 'D': nibble = 13; return true;
                case 'E': nibble = 14; return true;
                case 'F': nibble = 15; return true;
                case 'a': nibble = 10; return true;
                case 'b': nibble = 11; return true;
                case 'c': nibble = 12; return true;
                case 'd': nibble = 13; return true;
                case 'e': nibble = 14; return true;
                case 'f': nibble = 15; return true;
                default: nibble = 0; return false;
            }
        }


        public static bool TryParseBase64String(this string base64string, out string plainText, Encoding encoding = null)
        {
            var bytes = Convert.FromBase64String(base64string);
            try
            {
                encoding ??= Encoding.UTF8;
                plainText = encoding.GetString(bytes);
            }
            catch (FormatException) { }

            plainText = null;
            return false;
        }
    }
}
