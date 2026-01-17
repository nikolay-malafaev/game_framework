using System;

namespace GameFramework.PersistentData
{
    public interface IEncrypter
    {
        byte[] Encrypt(ReadOnlyMemory<byte> data);
        byte[] Decrypt(ReadOnlyMemory<byte> data);
        void SetPassword(string password);
    }
}
