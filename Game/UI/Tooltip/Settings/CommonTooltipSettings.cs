using GameFramework.StaticData;
using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    [CreateAssetMenu(fileName = "CommonTooltipSettings", menuName = "GameFramework/UI/CommonTooltipSettings")]
    public class CommonTooltipSettings : UniqueStaticDataAsset
    {
        public AnimationClip DefaultShowAnimation;
        public AnimationClip DefaultHideAnimation;
    }
}
