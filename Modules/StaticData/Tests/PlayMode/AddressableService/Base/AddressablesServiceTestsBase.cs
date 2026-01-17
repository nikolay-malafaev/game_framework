using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace GameFramework.StaticData.Tests
{
    [TestFixture]
    public abstract class AddressableServiceTestsBase
    {
        protected AddressableService _service;

        protected const string TestsConfigurationsAddress = "AddressableServiceTestsConfigurations";

        protected const string ValidAddressSO = "TestScriptableObject";
        protected const string ValidAddressPrefab = "TestPrefab";

        protected const string InvalidAddress = "InvalidAddress";
        protected const string ValidLabel = "TestLabel";
        protected const string InvalidLabel = "InvalidLabel";

        protected AddressableServiceTestsConfigurations _testsConfigurations;

        protected AssetReference _validAssetReferenceSO;
        protected AssetReference _validAssetReferencePrefab;
        protected AssetReference _invalidAssetReference;
        
        [UnitySetUp]
        public IEnumerator UnitySetUp() => UniTask.ToCoroutine(async () =>
        {
            await Addressables.InitializeAsync().ToUniTask();

            _service = new AddressableService();

            try
            {
                _testsConfigurations =
                    await Addressables.LoadAssetAsync<AddressableServiceTestsConfigurations>(
                        TestsConfigurationsAddress);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Setup Error: Could not find asset location for address: {ValidAddressSO}. Exception: {e}");
                throw;
            }

            _validAssetReferenceSO = _testsConfigurations.AssetReferenceTestScriptableObject;
            _validAssetReferencePrefab = _testsConfigurations.AssetReferenceTestPrefab;
            _invalidAssetReference = new AssetReference("invalid-guid-string");

            await UniTask.DelayFrame(1);
        });

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            _service?.Dispose();
            if (_validAssetReferenceSO != null && _validAssetReferenceSO.IsValid() &&
                _validAssetReferenceSO.OperationHandle.IsValid())
                _validAssetReferenceSO.ReleaseAsset();
            if (_validAssetReferencePrefab != null && _validAssetReferencePrefab.IsValid() &&
                _validAssetReferencePrefab.OperationHandle.IsValid())
                _validAssetReferencePrefab.ReleaseAsset();

            yield return UniTask.DelayFrame(1).ToCoroutine();
        }
    }
}