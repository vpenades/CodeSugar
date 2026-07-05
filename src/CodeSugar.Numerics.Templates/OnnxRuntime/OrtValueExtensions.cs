using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

using __ORTVALUE = Microsoft.ML.OnnxRuntime.OrtValue;
using __ORTVALUEINFO = Microsoft.ML.OnnxRuntime.OrtTensorTypeAndShapeInfo;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    internal static partial class CodeSugarNumericsExtensions
    {
        

        private static bool _TryGetTensorInfo<T>(__ORTVALUE value, out __ORTVALUEINFO info)
        {
            info = default;
            if (value == null) return false;
            if (!value.IsTensor) return false;
            if (value.IsSparseTensor) return false;            

            info = value.GetTensorTypeAndShape();

            var dataType = info.ElementDataType.GetElementType();

            if (dataType != typeof(T)) return false;

            return true;
        }

        private static Exception _GetTensorInfoException<T>(__ORTVALUE value, string name)
        {
            
            if (value == null) return new ArgumentNullException(name);
            if (!value.IsTensor) return new ArgumentException("Not a tensor", name);
            if (value.IsSparseTensor) return new ArgumentException("Not dense tensor", name);

            var info = value.GetTensorTypeAndShape();

            var dataType = info.ElementDataType.GetElementType();

            if (dataType != typeof(T)) return new ArgumentException($"Type mismatch, expected: {info.ElementDataType} but was {typeof(T).Name}", name);

            return new ArgumentException("Error", name);
        }
    }

}