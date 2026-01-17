using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

using Object = UnityEngine.Object;

namespace GameFramework.StaticData.Tests
{
    public class LoadAsyncTests : AddressableServiceTestsBase
    {
        [UnityTest]
        public IEnumerator ValidAssetReference_LoadsAsset() => UniTask.ToCoroutine(async () =>
        {
            Assert.IsNotNull(_validAssetReferenceSO,
                $"Setup failed: _validAssetReferenceSO is null. Check address '{ValidAddressSO}'.");
            Assert.IsTrue(_validAssetReferenceSO.RuntimeKeyIsValid(),
                "Setup failed: _validAssetReferenceSO key is invalid.");
            
            var asset = await _service.LoadAsync<ScriptableObject>(_validAssetReferenceSO);

            Assert.IsNotNull(asset);
            Assert.IsInstanceOf<ScriptableObject>(asset);

            _service.Release(_validAssetReferenceSO);
        });

        [UnityTest]
        public IEnumerator NullAssetReference_ThrowsArgumentException() => UniTask.ToCoroutine(async () =>
        {
            try
            {
                await _service.LoadAsync<ScriptableObject>(null as AssetReference);
                Assert.Fail("Expected ArgumentException was not thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("assetReference", ex.ParamName);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected ArgumentException but got {ex.GetType()}");
            }
        });

        [UnityTest]
        public IEnumerator InvalidAssetReference_ThrowsException() => UniTask.ToCoroutine(async () =>
        {
            Assert.IsNotNull(_invalidAssetReference, "Setup failed: _invalidAssetReference is null.");
            Assert.IsFalse(_invalidAssetReference.RuntimeKeyIsValid(),
                "Setup failed: _invalidAssetReference key should be invalid.");

            try
            {
                await _service.LoadAsync<ScriptableObject>(_invalidAssetReference);
                Assert.Fail("Expected an exception for invalid AssetReference key, but none was thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("assetReference", ex.ParamName);
                StringAssert.Contains("Provided AssetReference is null or invalid.", ex.Message);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Caught expected exception for invalid key: {ex.Message}");
            }
        });

        [UnityTest]
        public IEnumerator LoadSameReferenceTwice_ReturnsSameInstance() => UniTask.ToCoroutine(async () =>
        {
            Assert.IsNotNull(_validAssetReferenceSO, "Setup failed: _validAssetReferenceSO is null.");

            var asset1 = await _service.LoadAsync<ScriptableObject>(_validAssetReferenceSO);
            var asset2 = await _service.LoadAsync<ScriptableObject>(_validAssetReferenceSO);

            Assert.IsNotNull(asset1);
            Assert.AreSame(asset1, asset2);

            _service.Release(_validAssetReferenceSO);
        });
        
        [UnityTest]
        public IEnumerator MultipleValidAssetReferences_LoadsAllAssets() => UniTask.ToCoroutine(async () =>
        {
            Assert.IsNotNull(_validAssetReferenceSO, "Setup failed: _validAssetReferenceSO is null.");
            Assert.IsNotNull(_validAssetReferencePrefab, "Setup failed: _validAssetReferencePrefab is null.");

            var references = new List<AssetReference> { _validAssetReferenceSO, _validAssetReferencePrefab };
            var assets = await _service.LoadAsync<Object>(references);

            Assert.IsNotNull(assets);
            Assert.AreEqual(2, assets.Length);
            Assert.IsNotNull(assets[0]);
            Assert.IsInstanceOf<ScriptableObject>(assets[0]);
            Assert.IsNotNull(assets[1]);
            Assert.IsInstanceOf<GameObject>(assets[1]);

            _service.Release(references);
        });

        [UnityTest]
        public IEnumerator EmptyReferenceList_ReturnsEmptyArray() => UniTask.ToCoroutine(async () =>
        {
            var references = new List<AssetReference>();
            var assets = await _service.LoadAsync<Object>(references);

            Assert.IsNotNull(assets);
            Assert.AreEqual(0, assets.Length);
        });

        [UnityTest]
        public IEnumerator NullReferenceList_ReturnsEmptyArray() => UniTask.ToCoroutine(async () =>
        {
            var assets = await _service.LoadAsync<Object>(null as IEnumerable<AssetReference>);

            Assert.IsNotNull(assets);
            Assert.AreEqual(0, assets.Length);
        });

        [UnityTest]
        public IEnumerator ListWithNullAndValidReferences_LoadsValidAsset() =>
            UniTask.ToCoroutine(async () =>
            {
                Assert.IsNotNull(_validAssetReferenceSO, "Setup failed: _validAssetReferenceSO is null.");

                var references = new List<AssetReference> { null, _validAssetReferenceSO, _invalidAssetReference };

                LogAssert.Expect(LogType.Warning, new Regex("Skipping invalid AssetReference"));
                LogAssert.Expect(LogType.Warning, new Regex("Skipping invalid AssetReference"));

                var assets = await _service.LoadAsync<Object>(references);

                await UniTask.NextFrame();

                Assert.IsNotNull(assets);
                Assert.AreEqual(1, assets.Length);
                Assert.IsInstanceOf<ScriptableObject>(assets[0]);

                _service.Release(references);
            });
    }
}