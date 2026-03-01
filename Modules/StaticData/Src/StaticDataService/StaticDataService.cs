using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

namespace GameFramework.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly Dictionary<Type, List<StaticDataAsset>> _staticDataAssets = new();
        
        public TAsset Get<TAsset>() where TAsset : StaticDataAsset
        {
            Type assetType = typeof(TAsset);

            if (!_staticDataAssets.TryGetValue(assetType, out List<StaticDataAsset> assets) || assets.Count == 0)
            {
                throw new Exception($"Configuration type {assetType} does not exist or is empty.");
            }

            return (TAsset) assets[0];
        }
        
        public IReadOnlyList<TAsset> GetAll<TAsset>() where TAsset : StaticDataAsset
        {
            Type assetType = typeof(TAsset);

            if (!_staticDataAssets.TryGetValue(assetType, out List<StaticDataAsset> assets))
            {
                throw new Exception($"Configuration type {assetType} does not exist.");
            }

            return assets.Cast<TAsset>().ToList();
        }

        public bool Contains<TAsset>() where TAsset : StaticDataAsset
        {
            Type assetType = typeof(TAsset);
            return _staticDataAssets.ContainsKey(assetType);
        }

        public void Add<TAsset>(TAsset asset) where TAsset : StaticDataAsset
        {
            if (asset == null) return;
            
            Type assetType = asset.GetType();
            
            if (!_staticDataAssets.TryGetValue(assetType, out List<StaticDataAsset> assets))
            {
                assets = new List<StaticDataAsset>();
                _staticDataAssets[assetType] = assets;
            }
            
            assets.Add(asset);
        }

        public void Add<TAsset>(IReadOnlyList<TAsset> assets) where TAsset : StaticDataAsset
        {
            if (assets == null) return;
            
            foreach (var asset in assets)
            {
                Add(asset);
            }
        }
    }
}