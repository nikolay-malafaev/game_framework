using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    public interface ITooltipService
    {
        TTooltip Show<TTooltip>(string tooltipId, params ITooltipParameter[] parameters) where TTooltip : TooltipBehaviour;
        void Hide(string tooltipId, params ITooltipParameter[] parameters);
        void HideAll(params ITooltipParameter[] parameters);
    }

    public enum TooltipDirection
    {
        Left,
        Right,
        Up,
        Down
    }
}
