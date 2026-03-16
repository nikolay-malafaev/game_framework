using GameFramework.StaticData;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameFramework.UI.Window
{
    [CreateAssetMenu(fileName = "WindowSettings", menuName = "GameFramework/UI/WindowSettings")]
    public class WindowSettings : KeyedStaticDataAsset
    {
        public AssetReferenceGameObject ViewPrefab;
        public AnimationClip ShowAnimation;
        public AnimationClip HideAnimation;
    }
}