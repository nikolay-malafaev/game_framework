using System;
using System.Collections.Generic;
using System.Linq;

namespace GameFramework.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly Dictionary<Type, UniqueStaticDataAsset> _uniqueAssets = new();
        private readonly Dictionary<Type, List<KeyedStaticDataAsset>> _keyedAssets = new();

        public TAsset Get<TAsset>() where TAsset : UniqueStaticDataAsset
        {
            Type assetType = typeof(TAsset);

            if (_uniqueAssets.TryGetValue(assetType, out UniqueStaticDataAsset exactAsset))
            {
                return (TAsset) exactAsset;
            }

            foreach (var pair in _uniqueAssets)
            {
                if (assetType.IsAssignableFrom(pair.Key))
                {
                    return (TAsset) pair.Value;
                }
            }

            throw new InvalidOperationException($"StaticDataService: unique asset of type '{assetType.Name}' is not registered.");
        }

        public TAsset Get<TAsset>(string id) where TAsset : KeyedStaticDataAsset
        {
            Type assetType = typeof(TAsset);

            if (_keyedAssets.TryGetValue(assetType, out List<KeyedStaticDataAsset> exactAssets))
            {
                foreach (var asset in exactAssets)
                {
                    if (asset.Key == id)
                    {
                        return (TAsset)asset;
                    }
                }
            }

            foreach (var pair in _keyedAssets)
            {
                if (pair.Key == assetType) continue;
                
                if (assetType.IsAssignableFrom(pair.Key))
                {
                    foreach (var asset in pair.Value)
                    {
                        if (asset.Key == id)
                        {
                            return (TAsset)asset;
                        }
                    }
                }
            }

            throw new InvalidOperationException($"StaticDataService: keyed asset of type '{assetType.Name}' with id '{id}' is not registered.");
        }

        public IReadOnlyList<TAsset> GetAll<TAsset>() where TAsset : KeyedStaticDataAsset
        {
            Type assetType = typeof(TAsset);
            List<TAsset> result = new List<TAsset>();
            
            foreach (var pair in _keyedAssets)
            {
                if (assetType.IsAssignableFrom(pair.Key))
                {
                
                    result.AddRange(pair.Value.Cast<TAsset>());
                }
            }
            
            return result;
        }

        public bool Contains<TAsset>() where TAsset : UniqueStaticDataAsset
        {
            return Get<TAsset>() != null;
        }

        public bool Contains<TAsset>(string id) where TAsset : KeyedStaticDataAsset
        {
            return Get<TAsset>(id) != null;
        }

        public void Add(UniqueStaticDataAsset asset)
        {
            if (asset == null) return;
            Type assetType = asset.GetType();
            _uniqueAssets[assetType] = asset;
        }

        public void Add(IEnumerable<UniqueStaticDataAsset> assets)
        {
            if (assets == null) return;
            foreach (var asset in assets)
            {
                Add(asset);
            }
        }

        public void Add(KeyedStaticDataAsset asset)
        {
            if (asset == null) return;
            Type assetType = asset.GetType();

            if (!_keyedAssets.TryGetValue(assetType, out List<KeyedStaticDataAsset> assets))
            {
                assets = new List<KeyedStaticDataAsset>();
                _keyedAssets[assetType] = assets;
            }

            if (!assets.Contains(asset))
            {
                assets.Add(asset);
            }
        }

        public void Add(IEnumerable<KeyedStaticDataAsset> assets)
        {
            if (assets == null) return;
            foreach (var asset in assets)
            {
                Add(asset);
            }
        }
    }
}