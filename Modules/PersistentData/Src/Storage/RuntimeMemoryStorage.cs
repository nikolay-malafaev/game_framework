using System;
using System.Collections.Generic;

namespace GameFramework.PersistentData
{
    public sealed class RuntimeMemoryStorage : IStorage
    {
        private readonly Dictionary<string, byte[]> _map = new();
        
        public byte[] Read(string key)
        {
            if (!_map.TryGetValue(key, out var bytes))
            {
                return Array.Empty<byte>();
            }

            return bytes;
        }

        public void Write(string key, ReadOnlyMemory<byte> data)
        {
            _map[key] = data.ToArray();
        }

        public bool Exists(string key)
        {
            return _map.ContainsKey(key);
        }

        public void Delete(string key)
        {   
            _map.Remove(key);
        }
    }
}