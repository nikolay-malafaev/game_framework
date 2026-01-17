using System;
using System.Security.Cryptography;
using System.Text;

namespace GameFramework.PersistentData
{
    public class AesCbcHmacEncrypter : IEncrypter
    {
        // Формат: [1][salt16][iv16][cipher...][hmac32]
        private const byte FormatVersion = 1;

        private const int SaltSize = 16; // 128-bit
        private const int IvSize = 16; // AES block size
        private const int MacSize = 32; // HMAC-SHA256
        private const int KeySize = 32; // 256-bit AES key
        private const int Pbkdf2Iterations = 100_000;

        private string _password;
        private byte[] _passwordBytes;

        public AesCbcHmacEncrypter() : this("") { }

        public AesCbcHmacEncrypter(string password)
        {
            _password = password;
            _passwordBytes = Encoding.UTF8.GetBytes(_password);
        }

        public void SetPassword(string password)
        {
            _password = password;
            _passwordBytes = Encoding.UTF8.GetBytes(_password);
        }

        public byte[] Encrypt(ReadOnlyMemory<byte> data)
        {
            if (data.IsEmpty)
                return Array.Empty<byte>();

            byte[] salt = new byte[SaltSize];
            byte[] iv = new byte[IvSize];
            RandomNumberGenerator.Fill(salt);
            RandomNumberGenerator.Fill(iv);

            DeriveKeys(_passwordBytes, salt, out byte[] encKey, out byte[] macKey);

            byte[] plain = data.ToArray();
            byte[] cipher;

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = encKey;
                aes.IV = iv;

                using ICryptoTransform encryptor = aes.CreateEncryptor();
                cipher = encryptor.TransformFinalBlock(plain, 0, plain.Length);
            }

            int headerSize = 1 + SaltSize + IvSize;
            int macOffset = headerSize + cipher.Length;

            byte[] output = new byte[macOffset + MacSize];
            output[0] = FormatVersion;
            Buffer.BlockCopy(salt, 0, output, 1, SaltSize);
            Buffer.BlockCopy(iv, 0, output, 1 + SaltSize, IvSize);
            Buffer.BlockCopy(cipher, 0, output, headerSize, cipher.Length);

            byte[] mac;
            using (var hmac = new HMACSHA256(macKey))
            {
                mac = hmac.ComputeHash(output, 0, macOffset);
            }

            Buffer.BlockCopy(mac, 0, output, macOffset, MacSize);

            ClearBytes(encKey);
            ClearBytes(macKey);
            ClearBytes(plain);

            return output;
        }

        public byte[] Decrypt(ReadOnlyMemory<byte> data)
        {
            if (data.IsEmpty)
                return Array.Empty<byte>();

            byte[] buffer = data.ToArray();

            int minSize = 1 + SaltSize + IvSize + MacSize;
            if (buffer.Length < minSize)
                throw new CryptographicException();

            byte version = buffer[0];
            if (version != FormatVersion)
                throw new NotSupportedException($"Unsupported format version: {version}.");

            int headerSize = 1 + SaltSize + IvSize;
            int macOffset = buffer.Length - MacSize;
            int cipherLen = macOffset - headerSize;

            if (cipherLen <= 0)
                throw new CryptographicException("Invalid ciphertext.");

            byte[] salt = new byte[SaltSize];
            byte[] iv = new byte[IvSize];
            Buffer.BlockCopy(buffer, 1, salt, 0, SaltSize);
            Buffer.BlockCopy(buffer, 1 + SaltSize, iv, 0, IvSize);

            DeriveKeys(_passwordBytes, salt, out byte[] encKey, out byte[] macKey);

            // Проверяем MAC ДО расшифровки (Encrypt-then-MAC)
            byte[] expectedMac;
            using (var hmac = new HMACSHA256(macKey))
            {
                expectedMac = hmac.ComputeHash(buffer, 0, macOffset);
            }

            bool macOk = FixedTimeEquals(expectedMac, 0, buffer, macOffset, MacSize);
            ClearBytes(expectedMac);

            if (!macOk)
            {
                ClearBytes(encKey);
                ClearBytes(macKey);
                throw new CryptographicException("Invalid ciphertext.");
            }

            byte[] plain;
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = encKey;
                aes.IV = iv;

                using ICryptoTransform decryptor = aes.CreateDecryptor();
                // Важно: дешифруем прямо из buffer по offset/len
                plain = decryptor.TransformFinalBlock(buffer, headerSize, cipherLen);
            }

            ClearBytes(encKey);
            ClearBytes(macKey);

            return plain;
        }

        private static void DeriveKeys(byte[] passwordBytes, byte[] salt, out byte[] encKey, out byte[] macKey)
        {
            byte[] keyMaterial = new byte[KeySize * 2];

#if NETSTANDARD2_1_OR_GREATER || UNITY_2021_2_OR_NEWER
            using (var kdf = new Rfc2898DeriveBytes(passwordBytes, salt, Pbkdf2Iterations, HashAlgorithmName.SHA256))
#else
            // Фоллбек для очень старых профилей (будет SHA1 по умолчанию)
            using (var kdf = new Rfc2898DeriveBytes(passwordBytes, salt, Pbkdf2Iterations))
#endif
            {
                byte[] derived = kdf.GetBytes(keyMaterial.Length);
                Buffer.BlockCopy(derived, 0, keyMaterial, 0, keyMaterial.Length);
                ClearBytes(derived);
            }

            encKey = new byte[KeySize];
            macKey = new byte[KeySize];
            Buffer.BlockCopy(keyMaterial, 0, encKey, 0, KeySize);
            Buffer.BlockCopy(keyMaterial, KeySize, macKey, 0, KeySize);

            ClearBytes(keyMaterial);
        }

        private static bool FixedTimeEquals(byte[] a, int aOffset, byte[] b, int bOffset, int length)
        {
#if NETSTANDARD2_1_OR_GREATER || UNITY_2021_2_OR_NEWER
            // Есть CryptographicOperations.FixedTimeEquals в .NET Standard 2.1+
            return CryptographicOperations.FixedTimeEquals(
                new ReadOnlySpan<byte>(a, aOffset, length),
                new ReadOnlySpan<byte>(b, bOffset, length));
#else
            // Константное время вручную
            int diff = 0;
            for (int i = 0; i < length; i++)
                diff |= a[aOffset + i] ^ b[bOffset + i];
            return diff == 0;
#endif
        }

        private static void ClearBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return;
            Array.Clear(bytes, 0, bytes.Length);
        }
    }
}