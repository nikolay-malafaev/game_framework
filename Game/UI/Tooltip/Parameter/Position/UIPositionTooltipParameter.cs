using GameFramework.Types;
using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    public class UIPositionTooltipParameter : PositionTooltipParameter
    {
        public readonly RectTransform Target;

        public UIPositionTooltipParameter(
            RectTransform target,
            Optional<TooltipPositionSettings> overridenPositionSettings = default)
            : base(overridenPositionSettings)
        {
            Target = target;
        }
    }
}