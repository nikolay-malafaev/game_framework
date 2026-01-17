using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameFramework.StaticData.Tests
{
    [CreateAssetMenu(fileName = "AddressableServiceTestsConfigurations",
        menuName = "Testing/Addressables Service Tests Configurations")]
    public class AddressableServiceTestsConfigurations : StaticDataAsset
    {
        public AssetReference AssetReferenceTestScriptableObject;
        public AssetReference AssetReferenceTestPrefab;
    }
}