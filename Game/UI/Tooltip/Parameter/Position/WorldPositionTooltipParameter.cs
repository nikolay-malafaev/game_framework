using GameFramework.Types;
using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    public class WorldPositionTooltipParameter : PositionTooltipParameter
    {
        public readonly Vector3 WorldPosition;

        public WorldPositionTooltipParameter(
            Vector3 worldPosition,
            Optional<TooltipPositionSettings> overridenPositionSettings = default)
            : base(overridenPositionSettings)
        {
            WorldPosition = worldPosition;
        }
    }
}