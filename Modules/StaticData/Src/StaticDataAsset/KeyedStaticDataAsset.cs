using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.StaticData
{
    public abstract class KeyedStaticDataAsset : StaticDataAsset
    {
        [SerializeField]
        [InfoBox("Error: Key cannot be null or empty!", InfoMessageType.Error, "IsKeyEmpty")]
        [InfoBox("Error: An asset with the same key already exists in the project!", InfoMessageType.Error, "IsDuplicateKey")]
        private string _key = null;
        
        public string Key => _key;
        
#if UNITY_EDITOR
        private void Reset()
        {
            if (string.IsNullOrEmpty(_key))
            {
                _key = Guid.NewGuid().ToString();
            }
        }

        private bool IsKeyEmpty()
        {
            return string.IsNullOrEmpty(_key);
        }

        private bool IsDuplicateKey()
        {
            if (string.IsNullOrEmpty(_key)) return false;

            string typeName = GetType().Name;
            string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeName}");
            
            int count = 0;
            foreach (var guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<KeyedStaticDataAsset>(path);
                if (asset != null && asset.Key == _key)
                {
                    count++;
                    if (count > 1) return true;
                }
            }

            return false;
        }
#endif 
    }
}