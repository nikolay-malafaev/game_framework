using System;

namespace GameFramework.PersistentData
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T value);
        T Deserialize<T>(ReadOnlyMemory<byte> data);
    }
}
