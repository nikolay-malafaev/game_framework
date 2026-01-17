using System;
using UnityEngine;

namespace GameFramework.PersistentData
{
    public sealed class PlayerPrefsStorage : IStorage
    {
        private readonly string _prefix;
        
        public PlayerPrefsStorage(string prefix = "storage:")
        {
            _prefix = prefix ?? string.Empty;
        }
        
        public byte[] Read(string key)
        {
            var prefsKey = _prefix + key;
            if (!PlayerPrefs.HasKey(prefsKey))
            {
                return Array.Empty<byte>();
            }

            var base64 = PlayerPrefs.GetString(prefsKey, string.Empty);
            if (string.IsNullOrEmpty(base64))
            {
                return Array.Empty<byte>();
            }

            try
            {
                var bytes = Convert.FromBase64String(base64);
                return bytes;
            }
            catch (FormatException exception)
            {
                Debug.LogException(exception);
                return Array.Empty<byte>();
            }
        }
        
        public void Write(string key, ReadOnlyMemory<byte> data)
        {
            var prefsKey = _prefix + key;
            var base64 = Convert.ToBase64String(data.ToArray());

            PlayerPrefs.SetString(prefsKey, base64);
            PlayerPrefs.Save();
        }

        public bool Exists(string key)
        {
            var prefsKey = _prefix + key;
            return PlayerPrefs.HasKey(prefsKey);
        }

        public void Delete(string key)
        {
            var prefsKey = _prefix + key;
            PlayerPrefs.DeleteKey(prefsKey);
            PlayerPrefs.Save();
        }
    }
}