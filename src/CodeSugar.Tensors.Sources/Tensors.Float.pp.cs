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

using __XYZ = System.Numerics.Vector3;
using __TENSOR = System.Numerics.Tensors.Tensor<float>;
using __TENSORSPAN = System.Numerics.Tensors.TensorSpan<float>;
using __READONLYTENSORSPAN = System.Numerics.Tensors.ReadOnlyTensorSpan<float>;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.Numerics.Tensors
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForTensors
    {
        public static void ApplyMultiplyAddToPixelElements(this __TENSOR tensor, __XYZ mul, __XYZ add)
        {
            tensor.AsTensorSpan().ApplyMultiplyAddToPixelElements(mul, add);
        }

        public static void ApplyMultiplyAddToPixelElements(in this __TENSORSPAN tensor, __XYZ mul, __XYZ add)
        {
            // tensor = tensor.Squeeze();

            if (!_TryInferImageSize(tensor.Lengths, tensor.Strides, out _, out _, out int channels, out bool isCHW)) throw new ArgumentException("can't infer image size", nameof(tensor));

            if (isCHW)
            {
                channels = Math.Min(3, channels);                

                for (nint i = 0; i < channels; ++i)
                {
                    var emul = i == 0 ? mul.X : (i == 1 ? mul.Y : mul.Z);
                    var eadd = i == 0 ? add.X : (i == 1 ? add.Y : add.Z);                    
                    
                    var span = tensor.GetSubSpan(i);

                    System.Numerics.Tensors.TensorPrimitives.Multiply(span, emul, span);
                    System.Numerics.Tensors.TensorPrimitives.Add(span, eadd, span);
                }
                return;
            }

            if (channels == 3)
            {
                var span = tensor.GetFullSpan();
                var vvv = System.Runtime.InteropServices.MemoryMarshal.Cast<float, System.Numerics.Vector3>(span);

                for (int i = 0; i < vvv.Length; ++i)
                {
                    vvv[i] = vvv[i] * mul + add;
                }

                return;
            }

            throw new NotImplementedException();
        }        
    }
}
