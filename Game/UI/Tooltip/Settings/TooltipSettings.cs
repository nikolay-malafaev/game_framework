using GameFramework.StaticData;
using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    [CreateAssetMenu(fileName = "TooltipSettings", menuName = "GameFramework/UI/TooltipSettings")]
    public class TooltipSettings : KeyedStaticDataAsset
    {
        public TooltipPositionSettings PositionSettings;
        public bool HideOthersBeforeShow = true;
        public TooltipBehaviour Prefab;
        public AnimationClip ShowAnimation;
        public AnimationClip HideAnimation;
    }

    [System.Serializable]
    public struct TooltipPositionSettings
    {
        public TooltipDirection TooltipDirection;
        public Vector2 Offset;
        public float Distance;
        public bool KeepOnScreen;
    }
}
