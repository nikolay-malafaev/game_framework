using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace GameFramework.StaticData
{
    public interface IAddressableService
    {
        UniTask<TAsset[]> LoadAsync<TAsset>(IEnumerable<AssetReference> assetReferences) where TAsset : UnityEngine.Object;

        UniTask<TAsset> LoadAsync<TAsset>(AssetReference assetReference) where TAsset : UnityEngine.Object;

        UniTask<TAsset> LoadByAddressAsync<TAsset>(string assetAddress) where TAsset : UnityEngine.Object;

        UniTask<TAsset[]> LoadByAddressAsync<TAsset>(IEnumerable<string> assetAddresses) where TAsset : UnityEngine.Object;
        
        UniTask<TAsset[]> LoadByLabelAsync<TAsset>(string label) where TAsset : UnityEngine.Object;
        
        void Release(IEnumerable<AssetReference> assetReferences);
        
        void Release(AssetReference assetReference);

        UniTask ReleaseByLabel<TAsset>(string label) where TAsset : UnityEngine.Object;

        void ReleaseByAddress(string address);
    }
}