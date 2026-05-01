using System.Collections.Generic;
using System.Linq;
using GameFramework.Infrastructure;
using GameFramework.Logging;
using GameFramework.StaticData;
using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    [Loggable]
    public partial class TooltipService: ITooltipService
    {
        private readonly IStaticDataService _staticDataService;
        private readonly SceneFactory _sceneFactory;
        private readonly List<TooltipBehaviour> _tooltipBehaviours = new();
        private TooltipServiceBehaviour _tooltipServiceBehaviour;

        public TooltipService(IStaticDataService staticDataService, SceneFactory sceneFactory)
        {
            _staticDataService = staticDataService;
            _sceneFactory = sceneFactory;
        }

        public TTooltip Show<TTooltip>(string tooltipId, params ITooltipParameter[] parameters) where TTooltip : TooltipBehaviour
        {
            var settings = _staticDataService.Get<TooltipSettings>(tooltipId);
            if (settings == null)
            {
                return null;
            }
            
            var commonSettings = _staticDataService.Get<CommonTooltipSettings>();

            if (settings.HideOthersBeforeShow)
            {
                HideAll(parameters);
            }
            else
            {
                Hide(tooltipId, parameters);
            }

            if (_tooltipServiceBehaviour == null)
            {
                _tooltipServiceBehaviour = _sceneFactory.Instantiate(commonSettings.TooltipServicePrefab);
            }

            if (!parameters.HasParameter<PositionTooltipParameter>())
            {
                LogError("No position parameter specified!");
                return null;
            }
            
            var tooltipInstance = _sceneFactory.Instantiate(settings.Prefab, _tooltipServiceBehaviour.transform);
            
            tooltipInstance.Initialize(tooltipId);
            tooltipInstance.Show(null, parameters);
            
            _tooltipBehaviours.Add(tooltipInstance);
            return (TTooltip) tooltipInstance;
        }

        public void Hide(string tooltipId, params ITooltipParameter[] parameters)
        {
            var storedTooltip = GetTooltipBehaviour(tooltipId);
            if (storedTooltip == null)
            {
                return;
            }

            _tooltipBehaviours.Remove(storedTooltip);
            storedTooltip.Hide(() => Object.Destroy(storedTooltip.gameObject), parameters);
        }

        public void HideAll(params ITooltipParameter[] parameters)
        {
            foreach (var tooltipBehaviour in _tooltipBehaviours.ToList())
            {
                Hide(tooltipBehaviour.Id, parameters);
            }
        }

        public TooltipBehaviour GetTooltipBehaviour(string tooltipId)
        {
            return _tooltipBehaviours.FirstOrDefault(t => t.Id == tooltipId);
        }

        public IReadOnlyList<TooltipBehaviour> GetAllTooltips()
        {
            return _tooltipBehaviours;
        }
    }
}
