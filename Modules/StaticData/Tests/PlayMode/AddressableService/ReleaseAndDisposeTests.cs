using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace GameFramework.StaticData.Tests
{
    public class ReleaseAndDisposeTests : AddressableServiceTestsBase
    {
        [UnityTest]
        public IEnumerator Release_ValidAssetReference_ReleasesAsset() => UniTask.ToCoroutine(async () =>
        {
            Assert.IsNotNull(_validAssetReferenceSO, "Setup failed: _validAssetReferenceSO is null.");
        
            var asset = await _service.LoadAsync<ScriptableObject>(_validAssetReferenceSO);
            Assert.IsNotNull(asset);
        
            _service.Release(_validAssetReferenceSO);
        
            var assetAfterRelease = await _service.LoadAsync<ScriptableObject>(_validAssetReferenceSO);
            Assert.IsNotNull(assetAfterRelease);
        
            _service.Release(_validAssetReferenceSO);
        });
        
        [Test]
        public void Release_NullAssetReference_DoesNothing()
        {
            Assert.DoesNotThrow(() => _service.Release(null as AssetReference));
        }
        
        [UnityTest]
        public IEnumerator Release_MultipleReferences_ReleasesAll() => UniTask.ToCoroutine(async () =>
        {
            Assert.IsNotNull(_validAssetReferenceSO, "Setup failed: _validAssetReferenceSO is null.");
            Assert.IsNotNull(_validAssetReferencePrefab, "Setup failed: _validAssetReferencePrefab is null.");
        
            var references = new List<AssetReference> { _validAssetReferenceSO, _validAssetReferencePrefab };
            await _service.LoadAsync<Object>(references);
        
            Assert.DoesNotThrow(() => _service.Release(references));
            
            var assetsAfterRelease = await _service.LoadAsync<Object>(references);
            Assert.AreEqual(2, assetsAfterRelease.Length);
        
            _service.Release(references);
        });
        
        [UnityTest]
        public IEnumerator ReleaseByAddress_ValidAddress_ReleasesAsset() => UniTask.ToCoroutine(async () =>
        {
            var asset = await _service.LoadByAddressAsync<ScriptableObject>(ValidAddressSO);
            Assert.IsNotNull(asset);
        
            Assert.DoesNotThrow(() => _service.ReleaseByAddress(ValidAddressSO));
        
            var assetAfterRelease = await _service.LoadByAddressAsync<ScriptableObject>(ValidAddressSO);
            Assert.IsNotNull(assetAfterRelease);
        
            _service.ReleaseByAddress(ValidAddressSO);
        });

        [Test]
        public void ReleaseByAddress_NullOrEmptyAddress_DoesNothing()
        {
            Assert.DoesNotThrow(() => _service.ReleaseByAddress(null));
            Assert.DoesNotThrow(() => _service.ReleaseByAddress(string.Empty));
        }

        [UnityTest]
        public IEnumerator ReleaseByLabel_ValidLabel_ReleasesAssets() => UniTask.ToCoroutine(async () =>
        {
            var assets = await _service.LoadByLabelAsync<Object>(ValidLabel);
            Assert.GreaterOrEqual(assets.Length, 2);
        
            await _service.ReleaseByLabel<Object>(ValidLabel);
        
            var assetsAfterRelease = await _service.LoadByLabelAsync<Object>(ValidLabel);
            Assert.GreaterOrEqual(assetsAfterRelease.Length, 2);
        
            await _service.ReleaseByLabel<Object>(ValidLabel);
        });
        
        [UnityTest]
        public IEnumerator ReleaseByLabel_InvalidLabel_DoesNothing() => UniTask.ToCoroutine(async () =>
        {
            await _service.ReleaseByLabel<Object>(InvalidLabel);
            Assert.Pass();
        });

        [UnityTest]
        public IEnumerator Dispose_ReleasesAllLoadedAssets() => UniTask.ToCoroutine(async () =>
        {
            Assert.IsNotNull(_validAssetReferenceSO, "Setup failed: _validAssetReferenceSO is null.");

            var assetRef = await _service.LoadAsync<ScriptableObject>(_validAssetReferenceSO);
            var assetAddr = await _service.LoadByAddressAsync<GameObject>(ValidAddressPrefab);

            Assert.IsNotNull(assetRef);
            Assert.IsNotNull(assetAddr);

            _service.Dispose();

            _service = new AddressableService();

            var assetRefAfterDispose = await _service.LoadAsync<ScriptableObject>(_validAssetReferenceSO);
            var assetAddrAfterDispose = await _service.LoadByAddressAsync<GameObject>(ValidAddressPrefab);

            Assert.IsNotNull(assetRefAfterDispose);
            Assert.IsNotNull(assetAddrAfterDispose);

            _service.Dispose();
        });
    }
}