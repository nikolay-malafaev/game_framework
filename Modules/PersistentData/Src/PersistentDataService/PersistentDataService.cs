using System;
using System.Collections.Concurrent;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameFramework.PersistentData
{
    public class PersistentDataService : IPersistentDataService
    {
        private readonly bool _encrypted;
        private readonly bool _compressed;
        private readonly IStorage _storage;
        private readonly ISerializer _serializer;
        private readonly IEncrypter _encrypter;
        private readonly ICompressor _compressor;
        private readonly KeyedLocks _keyedLocks;
        
        public PersistentDataService(bool encrypted, bool compressed, IStorage storage, ISerializer serializer, ICompressor compressor, IEncrypter encrypter)
        {
            _encrypted = encrypted;
            _compressed = compressed;
            _storage = storage;
            _serializer = serializer;
            _encrypter = encrypter;
            _compressor = compressor;
            _keyedLocks = new();
        }

        public async UniTask<(bool, T)> Load<T>(string key, T defaultValue = default)
        {
            try
            {
                using KeyedLocks.Releaser releaser = await _keyedLocks.WaitAsync(key);
                return await UniTask.RunOnThreadPool(() => (true, ReadAndPrepare<T>(key)));
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return (false, defaultValue);
            }
        }

        public async UniTask<bool> Save<T>(string key, T value)
        {
            try
            {
                using KeyedLocks.Releaser releaser = await _keyedLocks.WaitAsync(key);
                return await UniTask.RunOnThreadPool(() =>
                {
                    PrepareAndWrite(key, value);
                    return true;
                });
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return false;
            }
        }
        
        public bool Exists(string key)
        {
            return _storage.Exists(key);
        }

        public void Delete(string key)
        {
            _storage.Delete(key);
        }

        private T ReadAndPrepare<T>(string key)
        {
            byte[] data = _storage.Read(key);
            
            if (_encrypted)
            {
                data = _encrypter.Decrypt(data);
            }
            
            if (_compressed)
            {
                data = _compressor.Decompress(data);
            }

            return _serializer.Deserialize<T>(data);
        }

        private void PrepareAndWrite<T>(string key, T value)
        {
            byte[] data = _serializer.Serialize(value);
            
            if (_compressed)
            {
                data = _compressor.Compress(data.AsSpan());
            }
            
            if (_encrypted)
            {
                data = _encrypter.Encrypt(data);
            }

            _storage.Write(key, data);
        }
    }
    
    internal sealed class KeyedLocks
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public async UniTask<Releaser> WaitAsync(string key)
        {
            var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            return new Releaser(semaphore);
        }

        public readonly struct Releaser : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;
            
            public Releaser(SemaphoreSlim semaphore) => _semaphore = semaphore;
            
            public readonly void Dispose() => _semaphore.Release();
        }
    }
}