using System.Collections.Generic;
using System.Linq;
using GameFramework.Infrastructure;
using GameFramework.StaticData;
using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    public class TooltipService : ITooltipService
    {
        private readonly IStaticDataService _staticDataService;
        private readonly SceneFactory _sceneFactory;
        private readonly List<TooltipBehaviour> _tooltipBehaviours = new();

        public TooltipService(IStaticDataService staticDataService, SceneFactory sceneFactory)
        {
            _staticDataService = staticDataService;
            _sceneFactory = sceneFactory;
        }

        public void Show(string tooltipId, Vector2 position, params ITooltipParameter[] parameters)
        {
            var tooltipSettings = _staticDataService.Get<TooltipSettings>(tooltipId);
            if (!tooltipSettings.HasValue() || tooltipSettings.Value().Prefab == null)
                return;

            var settings = tooltipSettings.Value();

            if (settings.HideOthersBeforeShow)
                HideAll(parameters);
            else
                HideExisting(tooltipId, parameters);

            var commonSettings = _staticDataService.Get<CommonTooltipSettings>();

            var instance = _sceneFactory.Instantiate(settings.Prefab);
            instance.Initialize(tooltipId, settings, commonSettings.HasValue() ? commonSettings.Value() : null);
            instance.SetPosition(position + settings.Offset);
            instance.Show(null, parameters);
            _tooltipBehaviours.Add(instance);
        }

        public void Hide(string tooltipId, params ITooltipParameter[] parameters)
        {
            var storedTooltip = GetTooltipBehaviour(tooltipId);
            if (storedTooltip == null)
                return;

            _tooltipBehaviours.Remove(storedTooltip);
            storedTooltip.Hide(() => Object.Destroy(storedTooltip.gameObject), parameters);
        }

        public void HideAll(params ITooltipParameter[] parameters)
        {
            foreach (var tooltipBehaviour in _tooltipBehaviours.ToList())
                Hide(tooltipBehaviour.Id, parameters);
        }

        private void HideExisting(string tooltipId, ITooltipParameter[] parameters)
        {
            if (GetTooltipBehaviour(tooltipId) != null)
                Hide(tooltipId, parameters);
        }

        private TooltipBehaviour GetTooltipBehaviour(string tooltipId)
        {
            return _tooltipBehaviours.FirstOrDefault(t => t.Id == tooltipId);
        }
    }
}
