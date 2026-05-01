using GameFramework.Types;

namespace GameFramework.UI.Tooltip
{
    public abstract class PositionTooltipParameter : ITooltipParameter
    {
        public readonly Optional<TooltipPositionSettings> OverridenPositionSettings;

        protected PositionTooltipParameter(Optional<TooltipPositionSettings> overridenPositionSettings = default)
        {
            OverridenPositionSettings = overridenPositionSettings;
        }
    }
}