using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace CodeSugar
{
    internal class SerializationTests
    {
        private static readonly DateTimeOffset _NOW = DateTimeOffset.Now;

        [Test]
        public void TestStateful()
        {
            var list = new List<byte>();

            list.AsStatefulProgressWriter().WriteLeS32(100).WriteBeS64(200).WriteString("hello world").WritePackedS64(-10);

            Assert.That(list, Has.Count.EqualTo(25));

            using(var reader = list.GetEnumerator())
            {
                reader.ReadLeS32(out var val1).ReadBeS64(out var val2).ReadString(out var val3).ReadPackedS64(out var val4);

                Assert.That(val1, Is.EqualTo(100));
                Assert.That(val2, Is.EqualTo(200));
                Assert.That(val3, Is.EqualTo("hello world"));
                Assert.That(val4, Is.EqualTo(-10));
            }
        }


        [Test]
        public void TestEndianness()
        {
            var list = new List<byte>();

            

            list.AsStatefulProgressWriter().WriteLeS32(100).WriteBeS32(100);

            Assert.That(list, Is.EqualTo(new byte[] { 100, 0, 0, 0, 0, 0, 0, 100 }));
        }

        [Test]
        public void TestBinaryWriterCompat()
        {
            using(var m = new System.IO.MemoryStream())
            {
                using (var w = new System.IO.BinaryWriter(m, System.Text.Encoding.UTF8, true))
                {
                    w.Write((Int32)100);
                    w.Write((short)100);
                }

                m.Position = 0;

                m.ReadLeS32(out var v1); Assert.That(v1, Is.EqualTo(100));
                m.ReadLeS16(out var v2); Assert.That(v2, Is.EqualTo(100));
            }
        }

        [Test]
        public void TestSerializationStream()
        {
            using System.IO.Stream m = new System.IO.MemoryStream();

            var o = new _SerializableObject();
            o.Init();

            o.WriteTo(m);

            m.Position = 0;

            o = new _SerializableObject();
            o.ReadFrom(m);

            o.Check();
        }

        [Test]
        public void TestSerializationArray()
        {
            ArraySegment<Byte> array = new Byte[1000];            

            var o = new _SerializableObject();
            o.Init();

            o.WriteTo(array);            

            o = new _SerializableObject();
            o.ReadFrom(array);

            o.Check();
        }


        class _SerializableObject
        {
            #region data

            public bool ValueB;
            public sbyte ValueS8;
            public byte ValueU8;
            public Int16 ValueS16;
            public UInt16 ValueU16;
            public Int32 ValueS32;
            public UInt32 ValueU32;
            public Int64 ValueS64;
            public UInt64 ValueU64;
            public Half ValueF16;
            public Single ValueF32;
            public Double ValueF64;

            public long Value1;
            public long Value2;
            public long Value3;
            public ulong Value4;
            public ulong Value5;
            public ulong Value6;

            public string Value7;
            public bool Value8;
            public string Value9;

            public int Value10;
            public bool Value11;

            public TimeSpan Value12;
            public DateOnly Value13;
            public DateTime Value14;
            public DateTimeOffset Value15;
            public ulong Value16;

            public readonly List<System.Numerics.Vector3> Points1 = new List<System.Numerics.Vector3>();

            public readonly Dictionary<string, System.Numerics.Vector3> Points2 = new Dictionary<string, System.Numerics.Vector3>();

            #endregion

            #region API

            public IEnumerable<Object> Decompose()
            {
                yield return ValueB;
                yield return ValueS8;
                yield return ValueU8;
                yield return ValueS16;
                yield return ValueU16;
                yield return ValueS32;
                yield return ValueU32;
                yield return ValueS64;
                yield return ValueU64;
                yield return ValueF16;
                yield return ValueF32;
                yield return ValueF64;

                yield return Value1;
                yield return Value2;
                yield return Value3;
                yield return Value4;
                yield return Value5;
                yield return Value6;
                yield return Value7;
                yield return Value8;
                yield return Value9;
                yield return Value10;
                yield return Value11;
                yield return Value12;
                yield return Value13;
                yield return Value14;
                yield return Value15;
                yield return Value16;

                foreach (var p in Points1) yield return p;

                foreach (var (pkey, pval) in Points2.OrderBy(item => item.Key))
                {
                    yield return pkey;
                    yield return pval;
                }
            }

            public void Init()
            {
                ValueB = true;
                ValueS8 = sbyte.MinValue;
                ValueU8 = byte.MaxValue;
                ValueS16 = short.MinValue;
                ValueU16 = ushort.MaxValue;
                ValueS32 = int.MinValue;
                ValueU32 = uint.MaxValue;
                ValueS64 = long.MinValue;
                ValueU64 = ulong.MaxValue;                
                ValueF16 = default;
                ValueF32 = -1;
                ValueF64 = -1;

                Value1 = -385;
                Value2 = long.MinValue;
                Value3 = long.MaxValue;

                Value4 = 385;
                Value5 = ulong.MinValue;
                Value6 = ulong.MaxValue;

                Value7 = "hello world! 😊";
                Value8 = true;
                Value9 = "good bye! 😊";

                Value10 = 100;
                Value11 = true;

                Value12 = TimeSpan.FromSeconds(17);
                Value13 = DateOnly.FromDateTime(_NOW.DateTime);
                Value14 = _NOW.DateTime;
                Value15 = _NOW;

                Points1.Add(new System.Numerics.Vector3(1, 2, 3));
                Points1.Add(new System.Numerics.Vector3(0, float.MinValue , float.MaxValue));
                Points1.Add(new System.Numerics.Vector3(-1, -2, -3));

                Points2.Add("prop1", new System.Numerics.Vector3(1, 2, 3));
                Points2.Add("prop2", new System.Numerics.Vector3(0, float.MinValue, float.MaxValue));
                Points2.Add("prop3", new System.Numerics.Vector3(-1, -2, -3));
            }


            private static System.IO.Stream WriteToStream(System.IO.Stream stream, System.Numerics.Vector3 v)
            {
                return stream.WriteLeF32(v.X).WriteLeF32(v.Y).WriteLeF32(v.Z);
            }

            private static System.IO.Stream ReadFromStream(System.IO.Stream stream, out System.Numerics.Vector3 v)
            {
                v = default;
                return stream.ReadLeF32(out v.X).ReadLeF32(out v.Y).ReadLeF32(out v.Z);
            }

            private static ArraySegment<Byte> WriteToStream(ArraySegment<Byte> stream, System.Numerics.Vector3 v)
            {
                return stream.WriteLeF32(v.X).WriteLeF32(v.Y).WriteLeF32(v.Z);
            }

            private static ArraySegment<Byte> ReadFromStream(ArraySegment<Byte> stream, out System.Numerics.Vector3 v)
            {
                v = default;
                return stream.ReadLeF32(out v.X).ReadLeF32(out v.Y).ReadLeF32(out v.Z);
            }

            public void Check()
            {
                var other = new _SerializableObject();
                other.Init();

                var pairs = this.Decompose().Zip(other.Decompose());
                
                foreach(var (left,right) in pairs)
                {
                    Assert.That(left, Is.EqualTo(right));
                }
            }

            public System.IO.Stream WriteTo(System.IO.Stream stream)
            {
                return stream
                    .WriteBool(ValueB)
                    .WriteS8(ValueS8)
                    .WriteU8(ValueU8)
                    .WriteLeS16(ValueS16)
                    .WriteLeU16(ValueU16)
                    .WriteLeS32(ValueS32)
                    .WriteLeU32(ValueU32)
                    .WriteLeS64(ValueS64)
                    .WriteLeU64(ValueU64)
                    .WriteLeF16(ValueF16)
                    .WriteLeF32(ValueF32)
                    .WriteLeF64(ValueF64)

                    .WritePackedS64(Value1)
                    .WritePackedS64(Value2)
                    .WritePackedS64(Value3)
                    .WritePackedU64(Value4)
                    .WritePackedU64(Value5)
                    .WritePackedU64(Value6)
                    .WriteString(Value7)
                    .WriteBool(Value8)
                    .WriteString(Value9, System.Text.Encoding.UTF32)
                    .WriteBeS32(Value10)
                    .WriteBool(Value11)
                    .WriteLeTimeSpan(Value12)
                    .WriteEndian(Value13, false)
                    .WriteLeDateTime(Value14)
                    .WriteDateTimeOffset(Value15, false)
                    .WriteLeConvertible(Value16)

                    .WriteList(Points1, WriteToStream)
                    .WriteDictionary(Points2, WriteToStream);
                    
            }

            public System.IO.Stream ReadFrom(System.IO.Stream stream)
            {
                return stream
                    .ReadBool(out ValueB)
                    .ReadS8(out ValueS8)
                    .ReadU8(out ValueU8)
                    .ReadLeS16(out ValueS16)
                    .ReadLeU16(out ValueU16)
                    .ReadLeS32(out ValueS32)
                    .ReadLeU32(out ValueU32)
                    .ReadLeS64(out ValueS64)
                    .ReadLeU64(out ValueU64)
                    .ReadLeF16(out ValueF16)
                    .ReadLeF32(out ValueF32)
                    .ReadLeF64(out ValueF64)

                    .ReadPackedS64(out Value1)
                    .ReadPackedS64(out Value2)
                    .ReadPackedS64(out Value3)
                    .ReadPackedU64(out Value4)
                    .ReadPackedU64(out Value5)
                    .ReadPackedU64(out Value6)
                    .ReadString(out Value7)
                    .ReadBool(out Value8)
                    .ReadString(out Value9, System.Text.Encoding.UTF32)
                    .ReadBeS32(out Value10)
                    .ReadBool(out Value11)
                    .ReadLeTimeSpan(out Value12)
                    .ReadEndian(out Value13, false)
                    .ReadLeDateTime(out Value14)
                    .ReadDateTimeOffset(out Value15, false)
                    .ReadLeConvertible(out Value16)

                    .ReadList(Points1, ReadFromStream)
                    .ReadDictionary(Points2, ReadFromStream);
            }

            public ArraySegment<Byte> WriteTo(ArraySegment<Byte> stream)
            {
                stream = stream
                    .WriteBool(ValueB)
                    .WriteS8(ValueS8)
                    .WriteU8(ValueU8)
                    .WriteLeS16(ValueS16)
                    .WriteLeU16(ValueU16)
                    .WriteLeS32(ValueS32)
                    .WriteLeU32(ValueU32)
                    .WriteLeS64(ValueS64)
                    .WriteLeU64(ValueU64)
                    .WriteLeF16(ValueF16)
                    .WriteLeF32(ValueF32)
                    .WriteLeF64(ValueF64);

                stream = stream
                    .WritePackedS64(Value1)
                    .WritePackedS64(Value2)
                    .WritePackedS64(Value3)
                    .WritePackedU64(Value4)
                    .WritePackedU64(Value5)
                    .WritePackedU64(Value6)
                    .WriteString(Value7)
                    .WriteBool(Value8)
                    .WriteString(Value9, System.Text.Encoding.UTF32)
                    .WriteBeS32(Value10)
                    .WriteBool(Value11)
                    .WriteLeTimeSpan(Value12)
                    .WriteEndian(Value13, false)
                    .WriteLeDateTime(Value14)
                    .WriteDateTimeOffset(Value15, false)
                    .WriteLeConvertible(Value16);


                stream = stream
                    .WriteList(Points1, WriteToStream)
                    .WriteDictionary(Points2, WriteToStream);                

                return stream;
            }

            public ArraySegment<Byte> ReadFrom(ArraySegment<Byte> stream)
            {
                stream = stream
                    .ReadBool(out ValueB)
                    .ReadS8(out ValueS8)
                    .ReadU8(out ValueU8)
                    .ReadLeS16(out ValueS16)
                    .ReadLeU16(out ValueU16)
                    .ReadLeS32(out ValueS32)
                    .ReadLeU32(out ValueU32)
                    .ReadLeS64(out ValueS64)
                    .ReadLeU64(out ValueU64)
                    .ReadLeF16(out ValueF16)
                    .ReadLeF32(out ValueF32)
                    .ReadLeF64(out ValueF64);

                stream = stream
                    .ReadPackedS64(out Value1)
                    .ReadPackedS64(out Value2)
                    .ReadPackedS64(out Value3)
                    .ReadPackedU64(out Value4)
                    .ReadPackedU64(out Value5)
                    .ReadPackedU64(out Value6)
                    .ReadString(out Value7)
                    .ReadBool(out Value8)
                    .ReadString(out Value9, System.Text.Encoding.UTF32)
                    .ReadBeS32(out Value10)
                    .ReadBool(out Value11)
                    .ReadLeTimeSpan(out Value12)
                    .ReadEndian(out Value13, false)
                    .ReadLeDateTime(out Value14)
                    .ReadDateTimeOffset(out Value15, false)
                    .ReadLeConvertible(out Value16);

                stream = stream
                    .ReadList(Points1, ReadFromStream)
                    .ReadDictionary(Points2, ReadFromStream);

                return stream;
            }

            #endregion
        }
    }
}
