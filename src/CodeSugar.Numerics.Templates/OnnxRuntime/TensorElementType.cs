using System;
using System.Linq;
using System.Numerics;

#nullable disable

using __TYPECODE = System.TypeCode;
using __TENSORTYPECODE = Microsoft.ML.OnnxRuntime.Tensors.TensorElementType;
using __TENSORBASE = Microsoft.ML.OnnxRuntime.Tensors.TensorBase;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarNumericsExtensions
    {
        public static Type GetElementType(this __TENSORBASE tensor)
        {
            return tensor.GetTypeInfo().ElementType.GetElementType();
        }

        public static __TYPECODE ToTypeCode(this __TENSORTYPECODE etype)
        {
            switch (etype)
            {
                case __TENSORTYPECODE.Bool: return __TYPECODE.Boolean;

                case __TENSORTYPECODE.Int8: return __TYPECODE.SByte;
                case __TENSORTYPECODE.UInt8: return __TYPECODE.Byte;

                case __TENSORTYPECODE.Int16: return __TYPECODE.Int16;
                case __TENSORTYPECODE.UInt16: return __TYPECODE.UInt16;

                case __TENSORTYPECODE.Int32: return __TYPECODE.Int32;
                case __TENSORTYPECODE.UInt32: return __TYPECODE.UInt32;

                case __TENSORTYPECODE.Int64: return __TYPECODE.Int64;
                case __TENSORTYPECODE.UInt64: return __TYPECODE.UInt64;

                case __TENSORTYPECODE.Float16: break;
                case __TENSORTYPECODE.Float: return __TYPECODE.Single;
                case __TENSORTYPECODE.Double: return __TYPECODE.Double;

                case __TENSORTYPECODE.String: return __TYPECODE.String;

                case __TENSORTYPECODE.Complex64: break;
            }

            throw new NotSupportedException(etype.ToString());
        }

        public static __TENSORTYPECODE ToTensorElementType(this __TYPECODE etype)
        {
            switch (etype)
            {
                case __TYPECODE.Boolean: return __TENSORTYPECODE.Bool;

                case __TYPECODE.SByte: return __TENSORTYPECODE.Int8;
                case __TYPECODE.Byte: return __TENSORTYPECODE.UInt8;

                case __TYPECODE.Int16: return __TENSORTYPECODE.Int16;
                case __TYPECODE.UInt16: return __TENSORTYPECODE.UInt16;

                case __TYPECODE.Int32: return __TENSORTYPECODE.Int32;
                case __TYPECODE.UInt32: return __TENSORTYPECODE.UInt32;

                case __TYPECODE.Int64: return __TENSORTYPECODE.Int64;
                case __TYPECODE.UInt64: return __TENSORTYPECODE.UInt64;

                // case __TYPECODE.Float16: break;
                case __TYPECODE.Single: return __TENSORTYPECODE.Float;
                case __TYPECODE.Double: return __TENSORTYPECODE.Double;

                case __TYPECODE.String: return __TENSORTYPECODE.String;

                // case __TYPECODE.Complex64: break;
            }

            throw new NotSupportedException(etype.ToString());
        }        

        public static Type GetElementType(this __TENSORTYPECODE etype)
        {
            switch (etype)
            {
                case __TENSORTYPECODE.Bool: return typeof(Boolean);

                case __TENSORTYPECODE.Int8: return typeof(SByte);
                case __TENSORTYPECODE.UInt8: return typeof(Byte);

                case __TENSORTYPECODE.Int16: return typeof(Int16);
                case __TENSORTYPECODE.UInt16: return typeof(UInt16);

                case __TENSORTYPECODE.Int32: return typeof(Int32);
                case __TENSORTYPECODE.UInt32: return typeof(UInt32);

                case __TENSORTYPECODE.Int64: return typeof(Int64);
                case __TENSORTYPECODE.UInt64: return typeof(UInt64);

                case __TENSORTYPECODE.Float16: return typeof(Half);
                case __TENSORTYPECODE.Float: return typeof(Single);
                case __TENSORTYPECODE.Double: return typeof(Double);

                case __TENSORTYPECODE.String: return typeof(String);

                case __TENSORTYPECODE.Complex64: return typeof(Complex); //  needs checking

                default: throw new NotImplementedException(etype.ToString());
            }
        }
    }
}
