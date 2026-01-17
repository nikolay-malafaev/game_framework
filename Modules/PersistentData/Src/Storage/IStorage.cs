using System;

namespace GameFramework.PersistentData
{
    public interface IStorage
    {
        byte[] Read(string key);
        void Write(string key, ReadOnlyMemory<byte> data);
        bool Exists(string key);
        void Delete(string key);
    }
}
