using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    public interface ITooltipService
    {
        void Show(string tooltipId, Vector2 position, params ITooltipParameter[] parameters);
        void Hide(string tooltipId, params ITooltipParameter[] parameters);
        void HideAll(params ITooltipParameter[] parameters);
    }
}
