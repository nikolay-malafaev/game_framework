using GameFramework.StaticData;
using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    [CreateAssetMenu(fileName = "CommonTooltipSettings", menuName = "GameFramework/UI/CommonTooltipSettings")]
    public class CommonTooltipSettings : UniqueStaticDataAsset
    {
        public TooltipServiceBehaviour TooltipServicePrefab;
        public AnimationClip DefaultShowAnimation;
        public AnimationClip DefaultHideAnimation;
    }
}
