using System;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine;
using Random = System.Random;

namespace Unity.Netcode.EditorTests
{
    public class FastBufferWriterTests : BaseFastBufferReaderWriterTest
    {

        #region Common Checks
        private void WriteCheckBytes(FastBufferWriter writer, int writeSize, string failMessage = "")
        {
            Assert.IsTrue(writer.TryBeginWrite(2), "Writer denied write permission");
            writer.WriteValue((byte)0x80);
            Assert.AreEqual(writeSize + 1, writer.Position, failMessage);
            Assert.AreEqual(writeSize + 1, writer.Length, failMessage);
            writer.WriteValue((byte)0xFF);
            Assert.AreEqual(writeSize + 2, writer.Position, failMessage);
            Assert.AreEqual(writeSize + 2, writer.Length, failMessage);
        }

        private void VerifyCheckBytes(byte[] underlyingArray, int writeSize, string failMessage = "")
        {
            Assert.AreEqual(0x80, underlyingArray[writeSize], failMessage);
            Assert.AreEqual(0xFF, underlyingArray[writeSize + 1], failMessage);
        }

        private unsafe void VerifyBytewiseEquality<T>(T value, byte[] underlyingArray, int valueOffset, int bufferOffset, int size, string failMessage = "") where T : unmanaged
        {
            byte* asBytePointer = (byte*)&value;
            for (var i = 0; i < size; ++i)
            {
                Assert.AreEqual(asBytePointer[i + valueOffset], underlyingArray[i + bufferOffset], failMessage);
            }
        }

        private unsafe void VerifyTypedEquality<T>(T value, byte* unsafePtr) where T : unmanaged
        {
            var checkValue = (T*)unsafePtr;
            Assert.AreEqual(value, *checkValue);
        }

        private void VerifyPositionAndLength(FastBufferWriter writer, int position, string failMessage = "")
        {
            Assert.AreEqual(position, writer.Position, failMessage);
            Assert.AreEqual(position, writer.Length, failMessage);
        }

        private unsafe void CommonChecks<T>(FastBufferWriter writer, T valueToTest, int writeSize, string failMessage = "") where T : unmanaged
        {

            VerifyPositionAndLength(writer, writeSize, failMessage);

            WriteCheckBytes(writer, writeSize, failMessage);

            var underlyingArray = writer.ToArray();

            VerifyBytewiseEquality(valueToTest, underlyingArray, 0, 0, writeSize, failMessage);

            VerifyCheckBytes(underlyingArray, writeSize, failMessage);

            VerifyTypedEquality(valueToTest, writer.GetUnsafePtr());
        }
        #endregion

        #region Generic Checks
        protected override unsafe void RunTypeTest<T>(T valueToTest)
        {
            var writeSize = FastBufferWriter.GetWriteSize(valueToTest);
            var alternateWriteSize = FastBufferWriter.GetWriteSize<T>();
            Assert.AreEqual(sizeof(T), writeSize);
            Assert.AreEqual(sizeof(T), alternateWriteSize);

            var writer = new FastBufferWriter(writeSize + 2, Allocator.Temp);

            using (writer)
            {

                Assert.IsTrue(writer.TryBeginWrite(writeSize + 2), "Writer denied write permission");

                var failMessage = $"RunTypeTest failed with type {typeof(T)} and value {valueToTest}";

                writer.WriteValue(valueToTest);

                CommonChecks(writer, valueToTest, writeSize, failMessage);
            }
        }
        protected override unsafe void RunTypeTestSafe<T>(T valueToTest)
        {
            var writeSize = FastBufferWriter.GetWriteSize(valueToTest);
            var writer = new FastBufferWriter(writeSize + 2, Allocator.Temp);

            using (writer)
            {
                Assert.AreEqual(sizeof(T), writeSize);

                var failMessage = $"RunTypeTest failed with type {typeof(T)} and value {valueToTest}";

                writer.WriteValueSafe(valueToTest);

                CommonChecks(writer, valueToTest, writeSize, failMessage);
            }
        }

        private unsafe void VerifyArrayEquality<T>(T[] value, byte* unsafePtr, int offset) where T : unmanaged
        {
            int* sizeValue = (int*)(unsafePtr + offset);
            Assert.AreEqual(value.Length, *sizeValue);

            fixed (T* asTPointer = value)
            {
                var underlyingTArray = (T*)(unsafePtr + sizeof(int) + offset);
                for (var i = 0; i < value.Length; ++i)
                {
                    Assert.AreEqual(asTPointer[i], underlyingTArray[i]);
                }
            }
        }

        protected override unsafe void RunTypeArrayTest<T>(T[] valueToTest)
        {
            var writeSize = FastBufferWriter.GetWriteSize(valueToTest);
            var writer = new FastBufferWriter(writeSize + 2, Allocator.Temp);
            using (writer)
            {

                Assert.AreEqual(sizeof(int) + sizeof(T) * valueToTest.Length, writeSize);
                Assert.IsTrue(writer.TryBeginWrite(writeSize + 2), "Writer denied write permission");

                writer.WriteValue(valueToTest);
                VerifyPositionAndLength(writer, writeSize);

                WriteCheckBytes(writer, writeSize);

                VerifyArrayEquality(valueToTest, writer.GetUnsafePtr(), 0);

                var underlyingArray = writer.ToArray();
                VerifyCheckBytes(underlyingArray, writeSize);
            }
        }

        protected override unsafe void RunTypeArrayTestSafe<T>(T[] valueToTest)
        {
            var writeSize = FastBufferWriter.GetWriteSize(valueToTest);
            var writer = new FastBufferWriter(writeSize + 2, Allocator.Temp);
            using (writer)
            {

                Assert.AreEqual(sizeof(int) + sizeof(T) * valueToTest.Length, writeSize);

                writer.WriteValueSafe(valueToTest);
                VerifyPositionAndLength(writer, writeSize);

                WriteCheckBytes(writer, writeSize);

                VerifyArrayEquality(valueToTest, writer.GetUnsafePtr(), 0);

                var underlyingArray = writer.ToArray();
                VerifyCheckBytes(underlyingArray, writeSize);
            }
        }
        #endregion


        #region Tests
        [Test, Description("Tests ")]
        public void WhenWritingUnmanagedType_ValueIsWrittenCorrectly(
            [Values(typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
                typeof(long), typeof(ulong), typeof(bool), typeof(char), typeof(float), typeof(double),
                typeof(ByteEnum), typeof(SByteEnum), typeof(ShortEnum), typeof(UShortEnum), typeof(IntEnum),
                typeof(UIntEnum), typeof(LongEnum), typeof(ULongEnum), typeof(Vector2), typeof(Vector3), typeof(Vector4),
                typeof(Quaternion), typeof(Color), typeof(Color32), typeof(Ray), typeof(Ray2D), typeof(TestStruct))]
            Type testType,
            [Values] WriteType writeType)
        {
            BaseTypeTest(testType, writeType);
        }

        [Test]
        public void WhenWritingArrayOfUnmanagedElementType_ArrayIsWrittenCorrectly(
            [Values(typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
                typeof(long), typeof(ulong), typeof(bool), typeof(char), typeof(float), typeof(double),
                typeof(ByteEnum), typeof(SByteEnum), typeof(ShortEnum), typeof(UShortEnum), typeof(IntEnum),
                typeof(UIntEnum), typeof(LongEnum), typeof(ULongEnum), typeof(Vector2), typeof(Vector3), typeof(Vector4),
                typeof(Quaternion), typeof(Color), typeof(Color32), typeof(Ray), typeof(Ray2D), typeof(TestStruct))]
            Type testType,
            [Values] WriteType writeType)
        {
            BaseArrayTypeTest(testType, writeType);
        }

        [TestCase(false, WriteType.WriteDirect)]
        [TestCase(false, WriteType.WriteSafe)]
        [TestCase(true, WriteType.WriteDirect)]
        [TestCase(true, WriteType.WriteSafe)]
        public unsafe void WhenWritingString_ValueIsWrittenCorrectly(bool oneByteChars, WriteType writeType)
        {
            string valueToTest = "Hello, I am a test string!";

            var serializedValueSize = FastBufferWriter.GetWriteSize(valueToTest, oneByteChars);

            var writer = new FastBufferWriter(serializedValueSize + 3, Allocator.Temp);
            using (writer)
            {
                var offset = 0;
                switch (writeType)
                {
                    case WriteType.WriteDirect:
                        Assert.IsTrue(writer.TryBeginWrite(serializedValueSize + 2), "Writer denied write permission");
                        writer.WriteValue(valueToTest, oneByteChars);
                        break;
                    case WriteType.WriteSafe:
                        writer.WriteValueSafe(valueToTest, oneByteChars);
                        break;

                }

                VerifyPositionAndLength(writer, serializedValueSize + offset);
                WriteCheckBytes(writer, serializedValueSize + offset);

                int* sizeValue = (int*)(writer.GetUnsafePtr() + offset);
                Assert.AreEqual(valueToTest.Length, *sizeValue);

                fixed (char* asCharPointer = valueToTest)
                {
                    if (oneByteChars)
                    {
                        byte* underlyingByteArray = writer.GetUnsafePtr() + sizeof(int) + offset;
                        for (var i = 0; i < valueToTest.Length; ++i)
                        {
                            Assert.AreEqual((byte)asCharPointer[i], underlyingByteArray[i]);
                        }

                    }
                    else
                    {
                        char* underlyingCharArray = (char*)(writer.GetUnsafePtr() + sizeof(int) + offset);
                        for (var i = 0; i < valueToTest.Length; ++i)
                        {
                            Assert.AreEqual(asCharPointer[i], underlyingCharArray[i]);
                        }
                    }
                }

                var underlyingArray = writer.ToArray();
                VerifyCheckBytes(underlyingArray, serializedValueSize + offset);
            }
        }

        [TestCase(1, 0)]
        [TestCase(2, 0)]
        [TestCase(3, 0)]
        [TestCase(4, 0)]
        [TestCase(5, 0)]
        [TestCase(6, 0)]
        [TestCase(7, 0)]
        [TestCase(8, 0)]

        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 1)]
        [TestCase(5, 1)]
        [TestCase(6, 1)]
        [TestCase(7, 1)]

        [TestCase(1, 2)]
        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(4, 2)]
        [TestCase(5, 2)]
        [TestCase(6, 2)]

        [TestCase(1, 3)]
        [TestCase(2, 3)]
        [TestCase(3, 3)]
        [TestCase(4, 3)]
        [TestCase(5, 3)]

        [TestCase(1, 4)]
        [TestCase(2, 4)]
        [TestCase(3, 4)]
        [TestCase(4, 4)]

        [TestCase(1, 5)]
        [TestCase(2, 5)]
        [TestCase(3, 5)]

        [TestCase(1, 6)]
        [TestCase(2, 6)]

        [TestCase(1, 7)]
        public unsafe void WhenWritingPartialValueWithCountAndOffset_ValueIsWrittenCorrectly(int count, int offset)
        {
            var random = new Random();
            var valueToTest = ((ulong)random.Next() << 32) + (ulong)random.Next();
            var writer = new FastBufferWriter(sizeof(ulong) + 2, Allocator.Temp);
            using (writer)
            {

                Assert.IsTrue(writer.TryBeginWrite(count + 2), "Writer denied write permission");
                writer.WritePartialValue(valueToTest, count, offset);

                var failMessage = $"TestWritingPartialValues failed with value {valueToTest}";
                VerifyPositionAndLength(writer, count, failMessage);
                WriteCheckBytes(writer, count, failMessage);
                var underlyingArray = writer.ToArray();
                VerifyBytewiseEquality(valueToTest, underlyingArray, offset, 0, count, failMessage);
                VerifyCheckBytes(underlyingArray, count, failMessage);

                ulong mask = 0;
                for (var i = 0; i < count; ++i)
                {
                    mask = (mask << 8) | 0b11111111;
                }

                ulong* checkValue = (ulong*)writer.GetUnsafePtr();
                Assert.AreEqual((valueToTest >> (offset * 8)) & mask, *checkValue & mask);
            }
        }

        [Test]
        public void WhenCallingToArray_ReturnedArrayContainsCorrectData()
        {
            var testStruct = GetTestStruct();
            var requiredSize = FastBufferWriter.GetWriteSize(testStruct);
            var writer = new FastBufferWriter(requiredSize, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(requiredSize);
                writer.WriteValue(testStruct);
                var array = writer.ToArray();
                var underlyingArray = writer.ToArray();
                for (var i = 0; i < array.Length; ++i)
                {
                    Assert.AreEqual(array[i], underlyingArray[i]);
                }
            }
        }

        [Test]
        public void WhenCallingWriteByteWithoutCallingTryBeingWriteFirst_OverflowExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                Assert.Throws<OverflowException>(() => { writer.WriteByte(1); });
            }
        }

        [Test]
        public void WhenCallingWriteBytesWithoutCallingTryBeingWriteFirst_OverflowExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                var bytes = new byte[] { 0, 1, 2 };
                Assert.Throws<OverflowException>(() => { writer.WriteBytes(bytes, bytes.Length); });
            }
        }

        [Test]
        public void WhenCallingWriteValueWithUnmanagedTypeWithoutCallingTryBeingWriteFirst_OverflowExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                int i = 1;
                Assert.Throws<OverflowException>(() => { writer.WriteValue(i); });
            }
        }

        [Test]
        public void WhenCallingWriteValueWithByteArrayWithoutCallingTryBeingWriteFirst_OverflowExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                var bytes = new byte[] { 0, 1, 2 };
                Assert.Throws<OverflowException>(() => { writer.WriteValue(bytes); });
            }
        }

        [Test]
        public void WhenCallingWriteValueWithStringWithoutCallingTryBeingWriteFirst_OverflowExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                Assert.Throws<OverflowException>(() => { writer.WriteValue(""); });
            }
        }

        [Test]
        public void WhenCallingWriteValueAfterCallingTryBeginWriteWithTooFewBytes_OverflowExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                int i = 0;
                writer.TryBeginWrite(sizeof(int) - 1);
                Assert.Throws<OverflowException>(() => { writer.WriteValue(i); });
            }
        }

        [Test]
        public void WhenCallingWriteBytePastBoundaryMarkedByTryBeginWrite_OverflowExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(sizeof(int) - 1);
                writer.WriteByte(1);
                writer.WriteByte(2);
                writer.WriteByte(3);
                Assert.Throws<OverflowException>(() => { writer.WriteByte(4); });
            }
        }

        [Test]
        public void WhenCallingWriteByteDuringBitwiseContext_InvalidOperationExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                using var context = writer.EnterBitwiseContext();
                Assert.Throws<InvalidOperationException>(() => { writer.WriteByte(1); });
            }
        }

        [Test]
        public void WhenCallingWriteBytesDuringBitwiseContext_InvalidOperationExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                var bytes = new byte[] { 0, 1, 2 };
                using var context = writer.EnterBitwiseContext();
                Assert.Throws<InvalidOperationException>(() => { writer.WriteBytes(bytes, bytes.Length); });
            }
        }

        [Test]
        public void WhenCallingWriteValueWithUnmanagedTypeDuringBitwiseContext_InvalidOperationExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                int i = 1;
                using var context = writer.EnterBitwiseContext();
                Assert.Throws<InvalidOperationException>(() => { writer.WriteValue(i); });
            }
        }

        [Test]
        public void WhenCallingWriteValueWithByteArrayDuringBitwiseContext_InvalidOperationExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                var bytes = new byte[] { 0, 1, 2 };
                using var context = writer.EnterBitwiseContext();
                Assert.Throws<InvalidOperationException>(() => { writer.WriteBytes(bytes, bytes.Length); });
            }
        }

        [Test]
        public void WhenCallingWriteValueWithStringDuringBitwiseContext_InvalidOperationExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                var bytes = new byte[] { 0, 1, 2 };
                using var context = writer.EnterBitwiseContext();
                Assert.Throws<InvalidOperationException>(() => { writer.WriteValue(""); });
            }
        }

        [Test]
        public void WhenCallingWriteByteSafeDuringBitwiseContext_InvalidOperationExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                using var context = writer.EnterBitwiseContext();
                Assert.Throws<InvalidOperationException>(() => { writer.WriteByteSafe(1); });
            }
        }

        [Test]
        public void WhenCallingWriteBytesSafeDuringBitwiseContext_InvalidOperationExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                var bytes = new byte[] { 0, 1, 2 };
                using var context = writer.EnterBitwiseContext();
                Assert.Throws<InvalidOperationException>(() => { writer.WriteBytesSafe(bytes, bytes.Length); });
            }
        }

        [Test]
        public void WhenCallingWriteValueSafeWithUnmanagedTypeDuringBitwiseContext_InvalidOperationExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                int i = 1;
                using var context = writer.EnterBitwiseContext();
                Assert.Throws<InvalidOperationException>(() => { writer.WriteValueSafe(i); });
            }
        }

        [Test]
        public void WhenCallingWriteValueSafeWithByteArrayDuringBitwiseContext_InvalidOperationExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                var bytes = new byte[] { 0, 1, 2 };
                using var context = writer.EnterBitwiseContext();
                Assert.Throws<InvalidOperationException>(() => { writer.WriteBytesSafe(bytes, bytes.Length); });
            }
        }

        [Test]
        public void WhenCallingWriteValueSafeWithStringDuringBitwiseContext_InvalidOperationExceptionIsThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                using var context = writer.EnterBitwiseContext();
                Assert.Throws<InvalidOperationException>(() => { writer.WriteValueSafe(""); });
            }
        }

        [Test]
        public void WhenCallingWriteByteAfterExitingBitwiseContext_InvalidOperationExceptionIsNotThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                using (var context = writer.EnterBitwiseContext())
                {
                    context.WriteBit(true);
                }
                writer.WriteByte(1);
            }
        }

        [Test]
        public void WhenCallingWriteBytesAfterExitingBitwiseContext_InvalidOperationExceptionIsNotThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                var bytes = new byte[] { 0, 1, 2 };
                using (var context = writer.EnterBitwiseContext())
                {
                    context.WriteBit(true);
                }
                writer.WriteBytes(bytes, bytes.Length);
            }
        }

        [Test]
        public void WhenCallingWriteValueWithUnmanagedTypeAfterExitingBitwiseContext_InvalidOperationExceptionIsNotThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                var bytes = new byte[] { 0, 1, 2 };
                int i = 1;
                using (var context = writer.EnterBitwiseContext())
                {
                    context.WriteBit(true);
                }
                writer.WriteValue(i);
            }
        }

        [Test]
        public void WhenCallingWriteValueWithByteArrayAfterExitingBitwiseContext_InvalidOperationExceptionIsNotThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                var bytes = new byte[] { 0, 1, 2 };
                using (var context = writer.EnterBitwiseContext())
                {
                    context.WriteBit(true);
                }
                writer.WriteValue(bytes);
            }
        }

        [Test]
        public void WhenCallingWriteValueWithStringAfterExitingBitwiseContext_InvalidOperationExceptionIsNotThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                using (var context = writer.EnterBitwiseContext())
                {
                    context.WriteBit(true);
                }
                writer.WriteValue("");
            }
        }

        [Test]
        public void WhenCallingWriteByteSafeAfterExitingBitwiseContext_InvalidOperationExceptionIsNotThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                using (var context = writer.EnterBitwiseContext())
                {
                    context.WriteBit(true);
                }
                writer.WriteByteSafe(1);
            }
        }

        [Test]
        public void WhenCallingWriteBytesSafeAfterExitingBitwiseContext_InvalidOperationExceptionIsNotThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                var bytes = new byte[] { 0, 1, 2 };
                using (var context = writer.EnterBitwiseContext())
                {
                    context.WriteBit(true);
                }
                writer.WriteBytesSafe(bytes, bytes.Length);
            }
        }

        [Test]
        public void WhenCallingWriteValueSafeWithUnmanagedTypeAfterExitingBitwiseContext_InvalidOperationExceptionIsNotThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                var bytes = new byte[] { 0, 1, 2 };
                int i = 1;
                using (var context = writer.EnterBitwiseContext())
                {
                    context.WriteBit(true);
                }
                writer.WriteValueSafe(i);
            }
        }

        [Test]
        public void WhenCallingWriteValueSafeWithByteArrayAfterExitingBitwiseContext_InvalidOperationExceptionIsNotThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                var bytes = new byte[] { 0, 1, 2 };
                using (var context = writer.EnterBitwiseContext())
                {
                    context.WriteBit(true);
                }
                writer.WriteValueSafe(bytes);
            }
        }

        [Test]
        public void WhenCallingWriteValueSafeWithStringAfterExitingBitwiseContext_InvalidOperationExceptionIsNotThrown()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);

            using (writer)
            {
                writer.TryBeginWrite(100);
                using (var context = writer.EnterBitwiseContext())
                {
                    context.WriteBit(true);
                }
                writer.WriteValueSafe("");
            }
        }

        [Test]
        public void WhenCallingTryBeginWrite_TheAllowedWritePositionIsMarkedRelativeToCurrentPosition()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);
            using (writer)
            {
                writer.TryBeginWrite(100);
                writer.WriteByte(1);
                writer.TryBeginWrite(1);
                writer.WriteByte(1);
                Assert.Throws<OverflowException>(() => { writer.WriteByte(1); });
            }
        }

        [Test]
        public void WhenWritingAfterSeeking_TheNewWriteGoesToTheCorrectPosition()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);
            using (writer)
            {
                writer.Seek(5);
                writer.WriteByteSafe(0);
                Assert.AreEqual(writer.Position, 6);

                writer.Seek(0);
                writer.WriteByteSafe(1);
                Assert.AreEqual(writer.Position, 1);

                writer.Seek(10);
                Assert.AreEqual(writer.Position, 10);

                writer.Seek(2);
                writer.WriteByteSafe(2);

                writer.Seek(1);
                writer.WriteByteSafe(3);

                writer.Seek(4);
                writer.WriteByteSafe(4);

                writer.Seek(3);
                writer.WriteByteSafe(5);

                Assert.AreEqual(writer.Position, 4);

                var expected = new byte[] { 1, 3, 2, 5, 4, 0 };
                var underlyingArray = writer.ToArray();
                for (var i = 0; i < expected.Length; ++i)
                {
                    Assert.AreEqual(expected[i], underlyingArray[i]);
                }
            }
        }

        [Test]
        public void WhenSeekingForward_LengthUpdatesToNewPosition()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);
            using (writer)
            {
                Assert.AreEqual(writer.Length, 0);
                writer.Seek(5);
                Assert.AreEqual(writer.Length, 5);
                writer.Seek(10);
                Assert.AreEqual(writer.Length, 10);
            }
        }

        [Test]
        public void WhenSeekingBackward_LengthDoesNotChange()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);
            using (writer)
            {
                Assert.AreEqual(writer.Length, 0);
                writer.Seek(5);
                Assert.AreEqual(writer.Length, 5);
                writer.Seek(0);
                Assert.AreEqual(writer.Length, 5);
            }
        }

        [Test]
        public void WhenTruncatingToSpecificPositionAheadOfWritePosition_LengthIsUpdatedAndPositionIsNot()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);
            using (writer)
            {
                writer.Seek(10);
                Assert.AreEqual(writer.Position, 10);
                Assert.AreEqual(writer.Length, 10);

                writer.Seek(5);
                Assert.AreEqual(writer.Position, 5);
                Assert.AreEqual(writer.Length, 10);

                writer.Truncate(8);
                Assert.AreEqual(writer.Position, 5);
                Assert.AreEqual(writer.Length, 8);
            }
        }

        [Test]
        public void WhenTruncatingToSpecificPositionBehindWritePosition_BothLengthAndPositionAreUpdated()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);
            using (writer)
            {
                writer.Seek(10);
                Assert.AreEqual(writer.Position, 10);
                Assert.AreEqual(writer.Length, 10);

                writer.Truncate(8);
                Assert.AreEqual(writer.Position, 8);
                Assert.AreEqual(writer.Length, 8);
            }
        }

        [Test]
        public void WhenTruncatingToCurrentPosition_LengthIsUpdated()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);
            using (writer)
            {
                writer.Seek(10);
                Assert.AreEqual(writer.Position, 10);
                Assert.AreEqual(writer.Length, 10);

                writer.Seek(5);
                writer.Truncate();
                Assert.AreEqual(writer.Position, 5);
                Assert.AreEqual(writer.Length, 5);
            }
        }

        [Test]
        public void WhenCreatingNewFastBufferWriter_CapacityIsCorrect()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);
            Assert.AreEqual(100, writer.Capacity);
            writer.Dispose();

            writer = new FastBufferWriter(200, Allocator.Temp);
            Assert.AreEqual(200, writer.Capacity);
            writer.Dispose();
        }

        [Test]
        public void WhenCreatingNewFastBufferWriter_MaxCapacityIsCorrect()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);
            Assert.AreEqual(100, writer.MaxCapacity);
            writer.Dispose();

            writer = new FastBufferWriter(100, Allocator.Temp, 200);
            Assert.AreEqual(200, writer.MaxCapacity);
            writer.Dispose();
        }

        [Test]
        public void WhenRequestingWritePastBoundsForNonGrowingWriter_TryBeginWriteReturnsFalse()
        {
            var writer = new FastBufferWriter(150, Allocator.Temp);
            using (writer)
            {
                var testStruct = GetTestStruct();
                writer.TryBeginWriteValue(testStruct);
                writer.WriteValue(testStruct);

                // Seek to exactly where the write would cross the buffer boundary
                writer.Seek(150 - FastBufferWriter.GetWriteSize(testStruct) + 1);

                // Writer isn't allowed to grow because it didn't specify a maxSize
                Assert.IsFalse(writer.TryBeginWriteValue(testStruct));
            }
        }

        [Test]
        public void WhenTryBeginWriteReturnsFalse_WritingThrowsOverflowException()
        {
            var writer = new FastBufferWriter(150, Allocator.Temp);
            using (writer)
            {
                var testStruct = GetTestStruct();
                writer.TryBeginWriteValue(testStruct);
                writer.WriteValue(testStruct);

                // Seek to exactly where the write would cross the buffer boundary
                writer.Seek(150 - FastBufferWriter.GetWriteSize(testStruct) + 1);

                // Writer isn't allowed to grow because it didn't specify a maxSize
                Assert.IsFalse(writer.TryBeginWriteValue(testStruct));
                Assert.Throws<OverflowException>(() => writer.WriteValue(testStruct));
            }
        }

        [Test]
        public void WhenTryBeginWriteReturnsFalseAndOverflowExceptionIsThrown_DataIsNotAffected()
        {
            var writer = new FastBufferWriter(150, Allocator.Temp);
            using (writer)
            {
                var testStruct = GetTestStruct();
                writer.TryBeginWriteValue(testStruct);
                writer.WriteValue(testStruct);

                // Seek to exactly where the write would cross the buffer boundary
                writer.Seek(150 - FastBufferWriter.GetWriteSize(testStruct) + 1);

                // Writer isn't allowed to grow because it didn't specify a maxSize
                Assert.IsFalse(writer.TryBeginWriteValue(testStruct));
                Assert.Throws<OverflowException>(() => writer.WriteValue(testStruct));
                VerifyBytewiseEquality(testStruct, writer.ToArray(), 0, 0, FastBufferWriter.GetWriteSize(testStruct));
            }
        }

        [Test]
        public void WhenRequestingWritePastBoundsForGrowingWriter_BufferGrowsWithoutLosingData()
        {
            var growingWriter = new FastBufferWriter(150, Allocator.Temp, 500);
            using (growingWriter)
            {
                var testStruct = GetTestStruct();
                growingWriter.TryBeginWriteValue(testStruct);
                growingWriter.WriteValue(testStruct);

                // Seek to exactly where the write would cross the buffer boundary
                growingWriter.Seek(150 - FastBufferWriter.GetWriteSize(testStruct) + 1);

                Assert.IsTrue(growingWriter.TryBeginWriteValue(testStruct));

                // Growth doubles the size
                Assert.AreEqual(300, growingWriter.Capacity);
                Assert.AreEqual(growingWriter.Position, 150 - FastBufferWriter.GetWriteSize(testStruct) + 1);
                growingWriter.WriteValue(testStruct);

                // Verify the growth properly copied the existing data
                VerifyBytewiseEquality(testStruct, growingWriter.ToArray(), 0, 0, FastBufferWriter.GetWriteSize(testStruct));
                VerifyBytewiseEquality(testStruct, growingWriter.ToArray(), 0, 150 - FastBufferWriter.GetWriteSize(testStruct) + 1, FastBufferWriter.GetWriteSize(testStruct));
            }
        }

        [Test]
        public void WhenRequestingWriteExactlyAtBoundsForGrowingWriter_BufferDoesntGrow()
        {
            var growingWriter = new FastBufferWriter(300, Allocator.Temp, 500);
            using (growingWriter)
            {
                var testStruct = GetTestStruct();
                growingWriter.TryBeginWriteValue(testStruct);
                growingWriter.WriteValue(testStruct);

                growingWriter.Seek(300 - FastBufferWriter.GetWriteSize(testStruct));
                Assert.IsTrue(growingWriter.TryBeginWriteValue(testStruct));
                Assert.AreEqual(300, growingWriter.Capacity);
                Assert.AreEqual(growingWriter.Position, growingWriter.ToArray().Length);
                growingWriter.WriteValue(testStruct);
                Assert.AreEqual(300, growingWriter.Position);

                VerifyBytewiseEquality(testStruct, growingWriter.ToArray(), 0, 0, FastBufferWriter.GetWriteSize(testStruct));
                VerifyBytewiseEquality(testStruct, growingWriter.ToArray(), 0, 300 - FastBufferWriter.GetWriteSize(testStruct), FastBufferWriter.GetWriteSize(testStruct));
            }
        }

        [Test]
        public void WhenBufferGrows_MaxCapacityIsNotExceeded()
        {
            var growingWriter = new FastBufferWriter(300, Allocator.Temp, 500);
            using (growingWriter)
            {
                var testStruct = GetTestStruct();
                growingWriter.TryBeginWriteValue(testStruct);
                growingWriter.WriteValue(testStruct);

                growingWriter.Seek(300);
                Assert.IsTrue(growingWriter.TryBeginWriteValue(testStruct));

                Assert.AreEqual(500, growingWriter.Capacity);
                Assert.AreEqual(growingWriter.Position, 300);

                growingWriter.WriteValue(testStruct);

                VerifyBytewiseEquality(testStruct, growingWriter.ToArray(), 0, 0, FastBufferWriter.GetWriteSize(testStruct));
                VerifyBytewiseEquality(testStruct, growingWriter.ToArray(), 0, 300, FastBufferWriter.GetWriteSize(testStruct));
            }
        }

        [Test]
        public void WhenBufferGrowthRequiredIsMoreThanDouble_BufferGrowsEnoughToContainRequestedValue()
        {
            var growingWriter = new FastBufferWriter(1, Allocator.Temp, 500);
            using (growingWriter)
            {
                var testStruct = GetTestStruct();
                Assert.IsTrue(growingWriter.TryBeginWriteValue(testStruct));

                // Buffer size doubles with each growth, so since we're starting with a size of 1, that means
                // the resulting size should be the next power of 2 above the size of testStruct.
                Assert.AreEqual(Math.Pow(2, Math.Ceiling(Mathf.Log(FastBufferWriter.GetWriteSize(testStruct), 2))),
                    growingWriter.Capacity);
                Assert.AreEqual(growingWriter.Position, 0);

                growingWriter.WriteValue(testStruct);

                VerifyBytewiseEquality(testStruct, growingWriter.ToArray(), 0, 0, FastBufferWriter.GetWriteSize(testStruct));
            }
        }

        [Test]
        public void WhenTryingToWritePastMaxCapacity_GrowthDoesNotOccurAndTryBeginWriteReturnsFalse()
        {
            var growingWriter = new FastBufferWriter(300, Allocator.Temp, 500);
            using (growingWriter)
            {
                Assert.IsFalse(growingWriter.TryBeginWrite(501));

                Assert.AreEqual(300, growingWriter.Capacity);
                Assert.AreEqual(growingWriter.Position, 0);
            }
        }

        [Test]
        public unsafe void WhenCallingTryBeginWriteInternal_AllowedWritePositionDoesNotMoveBackward()
        {
            var writer = new FastBufferWriter(100, Allocator.Temp);
            using (writer)
            {
                writer.TryBeginWrite(25);
                writer.TryBeginWriteInternal(5);
                Assert.AreEqual(writer.Handle->AllowedWriteMark, 25);
            }
        }
        #endregion
    }
}
