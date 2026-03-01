using System;
using System.Collections.Generic;
using System.Linq;

namespace GameFramework.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly Dictionary<Type, List<StaticDataAsset>> _staticDataAssets = new();
        
        public TAsset Get<TAsset>() where TAsset : StaticDataAsset
        {
            Type assetType = typeof(TAsset);

            if (!_staticDataAssets.ContainsKey(assetType))
            {
                throw new Exception($"Configuration type {assetType} is not exists");
            }

            return _staticDataAssets[assetType][0] as TAsset;
        }
        
        public IReadOnlyList<TAsset> GetAll<TAsset>() where TAsset : StaticDataAsset
        {
            Type assetType = typeof(TAsset);

            if (!_staticDataAssets.ContainsKey(assetType))
            {
                throw new Exception($"Configuration type {assetType} is not exists");
            }

            return _staticDataAssets[assetType].Select(staticDataAsset => staticDataAsset as TAsset).ToList();
        }

        public bool Contains<TAsset>() where TAsset : StaticDataAsset
        {
            Type assetType = typeof(TAsset);
            return _staticDataAssets.ContainsKey(assetType);
        }

        public void Add<TAsset>(TAsset asset) where TAsset : StaticDataAsset
        {
            Type assetType = typeof(TAsset);
            if (!_staticDataAssets.ContainsKey(assetType))
            {
                _staticDataAssets.Add(assetType, new List<StaticDataAsset>());
            }
            _staticDataAssets[typeof(TAsset)].Add(asset);
        }

        public void Add<TAsset>(IReadOnlyList<TAsset> assets) where TAsset : StaticDataAsset
        {
            Type assetType = typeof(TAsset);
            if (!_staticDataAssets.ContainsKey(assetType))
            {
                _staticDataAssets.Add(assetType, new List<StaticDataAsset>());
            }
            _staticDataAssets[typeof(TAsset)].AddRange(assets);
        }
    }
}