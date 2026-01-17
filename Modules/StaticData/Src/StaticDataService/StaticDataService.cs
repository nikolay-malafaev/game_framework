using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameFramework.Loading;

namespace GameFramework.StaticData
{
    public class StaticDataService : IStaticDataService, ILoadingOperation
    {
        private readonly Dictionary<Type, List<StaticDataAsset>> _staticDataAssets = new();
        private readonly IAddressableService _addressableService;
        private readonly StaticDataSettings _staticDataSettings;

        public StaticDataService(IAddressableService addressableService, StaticDataSettings staticDataSettings)
        {
            _addressableService = addressableService;
            _staticDataSettings = staticDataSettings;
        }
        
        public void Dispose()
        {
            _addressableService.Dispose();
        }
        
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
        
        public async UniTask<LoadingResult> Run()
        {
            StaticDataAsset[] staticDataAssets = await _addressableService.LoadByLabelAsync<StaticDataAsset>(_staticDataSettings.AddressableDefaultLabel);

            foreach (StaticDataAsset staticDataAsset in staticDataAssets)
            {
                Type assetType = staticDataAsset.GetType();
                if (!_staticDataAssets.ContainsKey(assetType))
                {
                    _staticDataAssets.Add(assetType, new List<StaticDataAsset>());
                }
                _staticDataAssets[assetType].Add(staticDataAsset);
            }
            
            return LoadingResult.Success("StaticData loaded successfully");
        }
    }
}