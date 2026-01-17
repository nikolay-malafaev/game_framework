// File: Assets/Tests/EditMode/AesCbcHmacEncrypterTests.cs
using System;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using GameFramework.PersistentData;

namespace GameFramework.PersistentData.Tests
{
    public sealed class AesCbcHmacEncrypterTests
    {
        // Дублируем константы формата, т.к. в классе они private.
        // Формат: [1][salt16][iv16][cipher...][hmac32]
        private const int SaltSize = 16;
        private const int IvSize = 16;
        private const int MacSize = 32;
        private const int HeaderSize = 1 + SaltSize + IvSize;
        private const int MinSize = 1 + SaltSize + IvSize + MacSize;

        [Test]
        public void Encrypt_Empty_ReturnsEmpty()
        {
            var e = new AesCbcHmacEncrypter("pw");
            var encrypted = e.Encrypt(ReadOnlyMemory<byte>.Empty);

            Assert.NotNull(encrypted);
            Assert.AreEqual(0, encrypted.Length);
        }

        [Test]
        public void Decrypt_Empty_ReturnsEmpty()
        {
            var e = new AesCbcHmacEncrypter("pw");
            var plain = e.Decrypt(ReadOnlyMemory<byte>.Empty);

            Assert.NotNull(plain);
            Assert.AreEqual(0, plain.Length);
        }

        [TestCase(1)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        [TestCase(31)]
        [TestCase(32)]
        [TestCase(33)]
        [TestCase(1024)]
        public void RoundTrip_SamePassword_ReturnsOriginal(int size)
        {
            var e = new AesCbcHmacEncrypter("pw");
            var input = RandomBytes(size);

            var encrypted = e.Encrypt(input);
            var decrypted = e.Decrypt(encrypted);

            Assert.That(decrypted, Is.EqualTo(input));
        }

        [Test]
        public void RoundTrip_DifferentInstancesSamePassword_ReturnsOriginal()
        {
            var enc = new AesCbcHmacEncrypter("pw");
            var dec = new AesCbcHmacEncrypter("pw");

            var input = RandomBytes(256);
            var encrypted = enc.Encrypt(input);
            var decrypted = dec.Decrypt(encrypted);

            Assert.That(decrypted, Is.EqualTo(input));
        }

        [Test]
        public void Encrypt_SamePlaintextTwice_ProducesDifferentCiphertext()
        {
            var e = new AesCbcHmacEncrypter("pw");
            var input = Enumerable.Repeat((byte)0x42, 64).ToArray();

            var a = e.Encrypt(input);
            var b = e.Encrypt(input);

            // Из-за случайных salt/iv совпадение практически невозможно.
            Assert.False(a.SequenceEqual(b));
        }

        [Test]
        public void Decrypt_WrongPassword_ThrowsCryptographicException()
        {
            var enc = new AesCbcHmacEncrypter("pw-A");
            var dec = new AesCbcHmacEncrypter("pw-B");

            var input = RandomBytes(128);
            var encrypted = enc.Encrypt(input);

            Assert.Throws<CryptographicException>(() => dec.Decrypt(encrypted));
        }

        [Test]
        public void Decrypt_TooShort_ThrowsCryptographicException()
        {
            var e = new AesCbcHmacEncrypter("pw");
            var tooShort = new byte[MinSize - 1];
            tooShort[0] = 1;

            Assert.Throws<CryptographicException>(() => e.Decrypt(tooShort));
        }

        [Test]
        public void Decrypt_CipherLenZero_ThrowsCryptographicException()
        {
            // Ровно MinSize => macOffset == headerSize => cipherLen == 0
            var e = new AesCbcHmacEncrypter("pw");
            var buffer = new byte[MinSize];
            buffer[0] = 1;

            Assert.Throws<CryptographicException>(() => e.Decrypt(buffer));
        }

        [Test]
        public void Decrypt_UnsupportedVersion_ThrowsNotSupportedException()
        {
            var e = new AesCbcHmacEncrypter("pw");
            var input = RandomBytes(64);
            var encrypted = e.Encrypt(input);

            var tampered = (byte[])encrypted.Clone();
            tampered[0] = 2; // другая версия

            Assert.Throws<NotSupportedException>(() => e.Decrypt(tampered));
        }

        [Test]
        public void Decrypt_TamperSalt_ThrowsCryptographicException()
        {
            var e = new AesCbcHmacEncrypter("pw");
            var encrypted = e.Encrypt(RandomBytes(64));

            // salt начинается с offset 1
            var tampered = FlipBit(encrypted, 1);

            Assert.Throws<CryptographicException>(() => e.Decrypt(tampered));
        }

        [Test]
        public void Decrypt_TamperIv_ThrowsCryptographicException()
        {
            var e = new AesCbcHmacEncrypter("pw");
            var encrypted = e.Encrypt(RandomBytes(64));

            // iv начинается с offset 1 + SaltSize
            var tampered = FlipBit(encrypted, 1 + SaltSize);

            Assert.Throws<CryptographicException>(() => e.Decrypt(tampered));
        }

        [Test]
        public void Decrypt_TamperCiphertext_ThrowsCryptographicException()
        {
            var e = new AesCbcHmacEncrypter("pw");
            var encrypted = e.Encrypt(RandomBytes(64));

            // первый байт ciphertext — сразу после заголовка
            var tampered = FlipBit(encrypted, HeaderSize);

            Assert.Throws<CryptographicException>(() => e.Decrypt(tampered));
        }

        [Test]
        public void Decrypt_TamperMac_ThrowsCryptographicException()
        {
            var e = new AesCbcHmacEncrypter("pw");
            var encrypted = e.Encrypt(RandomBytes(64));

            // MAC — последние 32 байта
            var tampered = FlipBit(encrypted, encrypted.Length - 1);

            Assert.Throws<CryptographicException>(() => e.Decrypt(tampered));
        }

        private static byte[] RandomBytes(int length)
        {
            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            return bytes;
        }

        private static byte[] FlipBit(byte[] src, int index)
        {
            if (index < 0 || index >= src.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            var copy = (byte[])src.Clone();
            copy[index] ^= 0x01;
            return copy;
        }
    }
}
