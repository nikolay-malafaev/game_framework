using System;
using System.IO;
using NUnit.Framework;
using GameFramework.PersistentData;

namespace GameFramework.PersistentData.Tests
{
    public class GZipCompressorTests
    {
        private GZipCompressor _compressor;

        [SetUp]
        public void SetUp()
        {
            _compressor = new GZipCompressor();
        }

        [Test]
        public void Decompress_WhenInputIsEmpty_ReturnsEmptyArray()
        {
            // Arrange
            ReadOnlyMemory<byte> input = ReadOnlyMemory<byte>.Empty;

            // Act
            byte[] result = _compressor.Decompress(input.Span);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Compress_EmptyInput_DecompressBack_ReturnsEmptyArray()
        {
            // Arrange
            byte[] original = Array.Empty<byte>();

            // Act
            byte[] compressed = _compressor.Compress(original);
            byte[] decompressed = _compressor.Decompress(compressed);

            // Assert
            Assert.NotNull(compressed);
            Assert.AreEqual(0, decompressed.Length);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(15)]
        [TestCase(128)]
        [TestCase(1024)]
        public void RoundTrip_RandomData_SameBytes(int size)
        {
            // Arrange
            byte[] original = CreateRandomBytes(size, seed: 12345);

            // Act
            byte[] compressed = _compressor.Compress(original);
            byte[] decompressed = _compressor.Decompress(compressed);

            // Assert
            CollectionAssert.AreEqual(original, decompressed);
        }

        [Test]
        public void RoundTrip_LargeRandomData_SameBytes()
        {
            // Arrange
            byte[] original = CreateRandomBytes(1_000_000, seed: 777); // ~1MB

            // Act
            byte[] compressed = _compressor.Compress(original);
            byte[] decompressed = _compressor.Decompress(compressed);

            // Assert
            CollectionAssert.AreEqual(original, decompressed);
        }

        [Test]
        public void Compress_RepetitiveData_ResultIsSmallerThanOriginal()
        {
            // Arrange
            byte[] original = new byte[100_000];
            Array.Fill(original, (byte)0x2A); // повторяющиеся байты хорошо сжимаются

            // Act
            byte[] compressed = _compressor.Compress(original);

            // Assert
            Assert.Less(compressed.Length, original.Length,
                $"Ожидали, что повторяющиеся данные сожмутся. Было: original={original.Length}, compressed={compressed.Length}");
        }

        [Test]
        public void Decompress_InvalidData_Throws()
        {
            // Arrange
            byte[] notGzip = CreateRandomBytes(256, seed: 999);

            // Act + Assert
            // GZipStream при попытке распаковки мусора обычно кидает InvalidDataException,
            // но на некоторых .NET/Mono могут вылетать и другие IOException.
            Assert.Throws<IOException>(() => _compressor.Decompress(notGzip));
        }

        private static byte[] CreateRandomBytes(int size, int seed)
        {
            var rnd = new Random(seed);
            var data = new byte[size];
            rnd.NextBytes(data);
            return data;
        }
    }
}
