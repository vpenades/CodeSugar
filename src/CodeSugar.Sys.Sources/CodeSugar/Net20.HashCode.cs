#if NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

#nullable disable

namespace System
{    
    internal struct HashCode
    {
        #region static

        public static int Combine<T1>(T1 value1)
        {
            return value1.GetHashCode();
        }
        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            return value1.GetHashCode() ^ value2.GetHashCode();
        }
        public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            return Combine(value1, value2) ^ value3.GetHashCode();
        }
        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            return Combine(value1, value2) ^ Combine(value3, value4);
        }
        public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            return Combine(value1, value2, value3) ^ Combine(value4,value5);
        }
        public static int Combine<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        {
            return Combine(value1, value2, value3) ^ Combine(value4, value5, value6);
        }
        public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
        {
            return Combine(value1, value2, value3, value4) ^ Combine(value5, value6, value7);
        }
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        {
            return Combine(value1, value2, value3, value4) ^ Combine(value5, value6, value7, value8);
        }

        #endregion

        #region API

        private int _Hash;
         
        public void Add<T>(T value)
        {
            if (value is null) return;
            _Hash ^= value.GetHashCode();
        }
        
        public void Add<T>(T value, IEqualityComparer<T> comparer)
        {            
            _Hash ^= comparer.GetHashCode(value);
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", true)]
        public readonly override bool Equals(object obj)
        {
            if (obj is HashCode other) return this._Hash == other._Hash;
            return false;
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.", true)]
        public readonly override int GetHashCode() { return _Hash; }
        
        public readonly int ToHashCode() { return _Hash; }

        #endregion
    }
}

#endif
