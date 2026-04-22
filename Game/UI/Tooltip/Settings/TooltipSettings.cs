using GameFramework.StaticData;
using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    [CreateAssetMenu(fileName = "TooltipSettings", menuName = "GameFramework/UI/TooltipSettings")]
    public class TooltipSettings : KeyedStaticDataAsset
    {
        public bool HideOthersBeforeShow = true;
        public TooltipBehaviour Prefab;
        public Vector2 Offset;
        public AnimationClip ShowAnimation;
        public AnimationClip HideAnimation;
    }
}
