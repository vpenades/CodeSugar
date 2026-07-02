using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Numerics.Tensors;
using System.Runtime.Intrinsics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;





using TUnit;

using XYZ = System.Numerics.Vector3;
using XYZW = System.Numerics.Vector4;

namespace CodeSugar
{
    internal class SystemTensorsTests
    {
        [Before(HookType.Class)]
        public static void Initialize()
        {
            #if NET8_0_OR_GREATER
            Console.Out.WriteLine($"Vector512 HwdSupport:{Vector512.IsHardwareAccelerated}");
            Console.Out.WriteLine($"Vector256 HwdSupport:{Vector256.IsHardwareAccelerated}");
            Console.Out.WriteLine($"Vector128 HwdSupport:{Vector128.IsHardwareAccelerated}");
            #endif
        }



        #if NET8_0_OR_GREATER

        [Test]
        public async Task TestTensorSpanCasting()
        {
            var tensorVector3 = System.Numerics.Tensors.Tensor.Create<XYZ>(new XYZ[256]).Reshape([16,16]).AsTensorSpan();
            var tensorVector3Info = TensorInfo.From(tensorVector3);
            await Assert.That(tensorVector3Info.FlattenedLength).IsEqualTo(256);
            await Assert.That(tensorVector3Info.Rank).IsEqualTo(2);
            await Assert.That(tensorVector3Info.Lengths[0]).IsEqualTo(16);
            await Assert.That(tensorVector3Info.Lengths[1]).IsEqualTo(16);

            tensorVector3 = System.Numerics.Tensors.Tensor.Create<XYZ>(new XYZ[256]).Reshape([16, 16]).AsTensorSpan();
            tensorVector3.CastTo(out System.Numerics.Tensors.TensorSpan<float> tensorFloats);
            tensorVector3Info = TensorInfo.From(tensorFloats);
            await Assert.That(tensorVector3Info.FlattenedLength).IsEqualTo((nint)256 * 3);
            await Assert.That(tensorVector3Info.Rank).IsEqualTo(3);
            await Assert.That(tensorVector3Info.Lengths[0]).IsEqualTo(16);
            await Assert.That(tensorVector3Info.Lengths[1]).IsEqualTo(16);
            await Assert.That(tensorVector3Info.Lengths[2]).IsEqualTo(3);

            /*
            tensorFloats[new nint[] { 0, 0, 1 }] = 5;            

            tensorFloats.CastTo(out tensorVector3);
            await Assert.That(tensorVector3.FlattenedLength).IsEqualTo((nint)256);
            await Assert.That(tensorVector3.Rank).IsEqualTo(2);
            await Assert.That(tensorVector3.Lengths[0]).IsEqualTo((nint)16);
            await Assert.That(tensorVector3.Lengths[1]).IsEqualTo((nint)16);
            await Assert.That(tensorVector3[new nint[] { 0, 0, }]).IsEqualTo(new XYZ(0,5,0));
            */
        }

        

        #if NET10_0_OR_GREATER

        [Test]
        public async Task TestSubTensorSpan()
        {
            // we need to create it each time to cross the boundary
            static TensorSpan<int> _create()
            {
                return System.Numerics.Tensors.Tensor.Create<int>(Enumerable.Range(0, 16 * 16).ToArray()).Reshape([16, 16]).AsTensorSpan();
            }

            // rank 2 tests
            var tensor2 = _create();
            var tensor2Info = TensorInfo.From(tensor2);
            await Assert.That(tensor2Info.IsStrided).IsFalse();

            tensor2 = _create();
            var row5 = tensor2.SliceSubTensor(5);
            var row5Info = TensorInfo.From(row5);
            var row5span = row5.GetFullSpan().ToArray();            
            await Assert.That(row5Info.Rank).IsEqualTo(tensor2.Rank-1);            
            await Assert.That(row5span.Length).IsEqualTo(16);
            await Assert.That(row5span[0]).IsEqualTo(80);

            tensor2 = _create();
            var xrow5 = tensor2.AsReadOnlyTensorSpan().SliceSubTensor(5);
            var xrow5span = xrow5.GetFullSpan().ToArray();
            
            await Assert.That(xrow5.Rank).IsEqualTo(tensor2.Rank - 1);            
            await Assert.That(xrow5span.Length).IsEqualTo(16);
            await Assert.That(xrow5span[0]).IsEqualTo(80);

            // rank 3 tests
            var tensor3 = System.Numerics.Tensors.Tensor.Create<int>(Enumerable.Range(0, 16 * 16 * 16).ToArray()).Reshape([16, 16, 16]).AsTensorSpan();
            var tensor3info = TensorInfo.From(tensor3);
            var bmp5 = tensor3.SliceSubTensor(5);
            var bmp5info = TensorInfo.From(bmp5);
            await Assert.That(tensor3.IsStrided()).IsFalse();            
            await Assert.That(bmp5info.Rank).IsEqualTo(tensor3info.Rank - 1);
        }

        #endif

        #endif

        /// <summary>
        /// this is used to query tensor's info without crossing the async await boundary
        /// </summary>
        struct TensorInfo
        {
            public static TensorInfo From<T>(TensorSpan<T> tensor)
            {
                return new TensorInfo(tensor.Rank, tensor.Lengths, tensor.Strides, tensor.IsStrided());
            }

            public static TensorInfo From<T>(ReadOnlyTensorSpan<T> tensor)
            {
                return new TensorInfo(tensor.Rank, tensor.Lengths, tensor.Strides, tensor.IsStrided());
            }

            public static TensorInfo From(ITensor tensor)
            {
                return new TensorInfo(tensor.Rank, tensor.Lengths, tensor.Strides, tensor.IsStrided());
            }

            

            private TensorInfo(int rank, ReadOnlySpan<nint> lengths, ReadOnlySpan<nint> strides, bool isStrided)
            {
                Rank = rank;
                Lengths = lengths.ToArray();
                Strides = strides.ToArray();
            }

            public int Rank { get; }
            public nint[] Lengths { get; }
            public nint[] Strides { get; }

            public bool IsStrided { get; }

            public nint FlattenedLength => Lengths.Aggregate((a, b) => a * b);
        }


    }
}
