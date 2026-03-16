using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Types;

namespace GameFramework.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly Dictionary<Type, UniqueStaticDataAsset> _uniqueAssets = new();
        private readonly Dictionary<Type, List<KeyedStaticDataAsset>> _keyedAssets = new();

        public Optional<TAsset> Get<TAsset>() where TAsset : UniqueStaticDataAsset
        {
            Type assetType = typeof(TAsset);

            if (_uniqueAssets.TryGetValue(assetType, out UniqueStaticDataAsset asset))
            {
                return (TAsset)asset;
            }

            return Optional<TAsset>.None;
        }

        public Optional<TAsset> Get<TAsset>(string id) where TAsset : KeyedStaticDataAsset
        {
            Type assetType = typeof(TAsset);

            if (_keyedAssets.TryGetValue(assetType, out List<KeyedStaticDataAsset> assets))
            {
                foreach (var asset in assets.Where(asset => asset.Key == id))
                {
                    return (TAsset)asset;
                }
            }

            return Optional<TAsset>.None;
        }

        public IReadOnlyList<TAsset> GetAll<TAsset>() where TAsset : KeyedStaticDataAsset
        {
            Type assetType = typeof(TAsset);

            if (_keyedAssets.TryGetValue(assetType, out List<KeyedStaticDataAsset> assets))
            {
                return assets.Cast<TAsset>().ToList();
            }

            return Array.Empty<TAsset>();
        }

        public bool Contains<TAsset>() where TAsset : UniqueStaticDataAsset
        {
            return _uniqueAssets.ContainsKey(typeof(TAsset));
        }

        public bool Contains<TAsset>(string id) where TAsset : KeyedStaticDataAsset
        {
            Type assetType = typeof(TAsset);
            return _keyedAssets.TryGetValue(assetType, out List<KeyedStaticDataAsset> assets) &&
                   assets.Any(a => a.Key == id);
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