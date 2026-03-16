using GameFramework.StaticData;
using UnityEngine;

namespace GameFramework.UI.Window
{
    [CreateAssetMenu(fileName = "CommonWindowSettings", menuName = "GameFramework/UI/CommonWindowSettings")]
    public class CommonWindowSettings : UniqueStaticDataAsset
    {
        public AnimationClip DefaultShowAnimation;
        public AnimationClip DefaultHideAnimation;
    }
}