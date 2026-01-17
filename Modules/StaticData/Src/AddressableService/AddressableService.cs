using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace GameFramework.StaticData
{
    public sealed class AddressableService : IAddressableService
    {
        private readonly Dictionary<AssetReference, AsyncOperationHandle> _assetReferenceRequests = new();
        private readonly Dictionary<string, AsyncOperationHandle> _addressRequests = new();

        public void Dispose()
        {
            foreach (AsyncOperationHandle handler in _assetReferenceRequests.Values)
            {
                Addressables.Release(handler);
            }
            _assetReferenceRequests.Clear();

            foreach (AsyncOperationHandle handler in _addressRequests.Values)
            {
                Addressables.Release(handler);
            }
            _addressRequests.Clear();
        }

        public async UniTask<TAsset[]> LoadAsync<TAsset>(IEnumerable<AssetReference> assetReferences) where TAsset : UnityEngine.Object
        {
            if (assetReferences == null) return Array.Empty<TAsset>();

            var tasks = new List<UniTask<TAsset>>();
            foreach (var assetReference in assetReferences)
            {
                if (assetReference != null && assetReference.RuntimeKeyIsValid())
                {
                    tasks.Add(LoadAsync<TAsset>(assetReference));
                }
                else
                {
                    Debug.LogWarning($"Skipping invalid AssetReference in batch load.");
                }
            }

            return await UniTask.WhenAll(tasks);
        }


        public async UniTask<TAsset> LoadAsync<TAsset>(AssetReference assetReference) where TAsset : UnityEngine.Object
        {
            if (assetReference == null || !assetReference.RuntimeKeyIsValid())
            {
                throw new ArgumentException("Provided AssetReference is null or invalid.", nameof(assetReference));
            }

            AsyncOperationHandle handle = CreateAsyncOperationHandle<TAsset>(assetReference);

            await handle.ToUniTask();

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new Exception(
                    $"Failed to load asset from {assetReference}. Status: {handle.Status}, Error: {handle.OperationException}");
            }

            TAsset loadedAsset = handle.Result as TAsset;

            if (loadedAsset == null)
            {
                throw new Exception($"Loaded asset from {assetReference} is not of type {typeof(TAsset)} or is null.");
            }

            return loadedAsset;
        }

        public async UniTask<TAsset> LoadByAddressAsync<TAsset>(string assetAddress) where TAsset : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetAddress))
            {
                throw new ArgumentException("Provided asset address is null or empty.", nameof(assetAddress));
            }

            AsyncOperationHandle handle = CreateAsyncOperationHandle<TAsset>(assetAddress);

            try
            {
                await handle.ToUniTask();
                
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new Exception(
                        $"Failed to load asset from address {assetAddress}. See inner exception for details.",
                        handle.OperationException);
                }

                TAsset loadedAsset = handle.Result as TAsset;
                if (loadedAsset == null)
                {
                    throw new Exception($"Loaded asset from address {assetAddress} is not of type {typeof(TAsset)}.");
                }

                return loadedAsset;
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                    _addressRequests.Remove(assetAddress);
                }

                throw new Exception(
                    $"Failed to load asset from address {assetAddress}. See inner exception for details.",
                    ex);
            }
        }

        public async UniTask<TAsset[]> LoadByAddressAsync<TAsset>(IEnumerable<string> assetAddresses) where TAsset : UnityEngine.Object
        {
            List<UniTask<TAsset>> tasks = new List<UniTask<TAsset>>();
            foreach (string assetAddress in assetAddresses)
            {
                if (!string.IsNullOrEmpty(assetAddress))
                {
                    tasks.Add(LoadByAddressAsync<TAsset>(assetAddress));
                }
                else
                {
                    Debug.LogError($"Skipping null or empty address in batch load by address.");
                }
            }

            return await UniTask.WhenAll(tasks);
        }

        public async UniTask<TAsset[]> LoadByLabelAsync<TAsset>(string label) where TAsset : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentException("Provided label is null or empty.", nameof(label));
            }

            List<string> assetsAddresses = await GetAssetsAddressesByLabelAsync(label, typeof(TAsset));

            if (assetsAddresses == null || assetsAddresses.Count == 0)
            {
                Debug.LogWarning($"No assets found for label '{label}' and type {typeof(TAsset)}.");
                return Array.Empty<TAsset>();
            }


            TAsset[] assets = await LoadByAddressAsync<TAsset>(assetsAddresses);

            return assets;
        }

        public void Release(AssetReference assetReference)
        {
            if (assetReference == null) return;

            if (_assetReferenceRequests.TryGetValue(assetReference, out var handler))
            {
                Addressables.Release(handler);
                _assetReferenceRequests.Remove(assetReference);
            }
        }

        public void Release(IEnumerable<AssetReference> assetReferences)
        {
            if (assetReferences == null) return;
            foreach (AssetReference assetReference in assetReferences)
            {
                Release(assetReference);
            }
        }

        public void ReleaseByAddress(string address)
        {
            if (string.IsNullOrEmpty(address)) return;

            if (_addressRequests.TryGetValue(address, out var handler))
            {
                Addressables.Release(handler);
                _addressRequests.Remove(address);
            }
        }

        public async UniTask ReleaseByLabel<TAsset>(string label) where TAsset : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(label)) return;

            List<string> assetsAddresses = await GetAssetsAddressesByLabelAsync(label, typeof(TAsset));

            if (assetsAddresses == null) return;

            foreach (string assetAddress in assetsAddresses)
            {
                ReleaseByAddress(assetAddress);
            }
        }

        private AsyncOperationHandle CreateAsyncOperationHandle<TAsset>(AssetReference assetReference) where TAsset : UnityEngine.Object
        {
            if (_assetReferenceRequests.TryGetValue(assetReference, out var handle) == false || !handle.IsValid())
            {
                handle = Addressables.LoadAssetAsync<TAsset>(assetReference);
                _assetReferenceRequests[assetReference] = handle;
            }

            return handle;
        }

        private AsyncOperationHandle CreateAsyncOperationHandle<TAsset>(string address) where TAsset : UnityEngine.Object
        {
            if (_addressRequests.TryGetValue(address, out var handle) == false || !handle.IsValid())
            {
                handle = Addressables.LoadAssetAsync<TAsset>(address);
                _addressRequests[address] = handle;
            }

            return handle;
        }

        private async UniTask<List<string>> GetAssetsAddressesByLabelAsync(string label, Type type = null)
        {
            AsyncOperationHandle<IList<IResourceLocation>> operationHandle = Addressables.LoadResourceLocationsAsync(label, type);

            IList<IResourceLocation> resourcesLocations = await operationHandle.ToUniTask();

            List<string> assetsAddresses = new List<string>(resourcesLocations.Count);

            foreach (var resourceLocation in resourcesLocations)
            {
                assetsAddresses.Add(resourceLocation.PrimaryKey);
            }

            Addressables.Release(operationHandle);

            return assetsAddresses;
        }
    }
}