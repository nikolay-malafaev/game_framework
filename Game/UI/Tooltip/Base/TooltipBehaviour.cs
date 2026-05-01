using System;
using GameFramework.Infrastructure;
using GameFramework.StaticData;
using GameFramework.Types;
using UnityEngine;
using VContainer;

namespace GameFramework.UI.Tooltip
{
    public class TooltipBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TooltipAnimatorBehaviour _animator;

        [Inject]
        private IStaticDataService _staticDataService;

        [Inject]
        private SceneCameraManager _sceneCameraManager;

        private TooltipSettings _tooltipSettings;
        private CommonTooltipSettings _commonTooltipSettings;
        private RectTransform _rectTransform;
        private Canvas _tooltipCanvas;

        public string Id { get; private set; }

        public void Initialize(string tooltipId)
        {
            Id = tooltipId;
            _rectTransform = GetComponent<RectTransform>();
            _tooltipCanvas = GetComponentInParent<Canvas>();
            if (_tooltipCanvas != null)
            {
                _tooltipCanvas = _tooltipCanvas.rootCanvas;
            }
            _tooltipSettings = _staticDataService.Get<TooltipSettings>(tooltipId);
            _commonTooltipSettings = _staticDataService.Get<CommonTooltipSettings>();
            _animator?.Setup(_tooltipSettings, _commonTooltipSettings);
        }

        public void Show(Action callback = null, params ITooltipParameter[] parameters)
        {
            SetPosition(parameters);
            var animationParameter = parameters.GetParameter(AnimationTooltipParameter.Default);
            if (_animator && animationParameter.Animate)
            {
                _animator.PlayShowAnimation(callback);
            }
            else
            {
                callback?.Invoke();
            }
            OnShow(parameters);
        }

        public void Hide(Action callback = null, params ITooltipParameter[] parameters)
        {
            OnHide(parameters);
            var animationParameter = parameters.GetParameter(AnimationTooltipParameter.Default);
            if (_animator && animationParameter.Animate)
            {
                _animator.PlayHideAnimation(callback);
            }
            else
            {
                callback?.Invoke();
            }
        }
        
        public virtual void UpdateInfo(params ITooltipParameter[] parameters) { }
        
        protected virtual void OnShow(params ITooltipParameter[] parameters) { }
        protected virtual void OnHide(params ITooltipParameter[] parameters) { }

        private void SetPosition(params ITooltipParameter[] parameters)
        {
            if (!parameters.TryGetParameter(out PositionTooltipParameter positionParameter))
            {
                return;
            }

            var parentRect = transform.parent as RectTransform;
            if (parentRect == null)
            {
                return;
            }

            if (!TryGetLocalPoint(positionParameter, parentRect, out var localPoint))
            {
                return;
            }

            var positionSettings = ResolvePositionSettings(positionParameter.OverridenPositionSettings);
            var finalPosition    = ApplyDirectionOffset(localPoint, positionSettings);
            finalPosition        = ApplyKeepOnScreen(finalPosition, positionSettings, parentRect);

            _rectTransform.anchoredPosition = finalPosition;
        }

        private bool TryGetLocalPoint(
            PositionTooltipParameter parameter,
            RectTransform parentRect,
            out Vector2 localPoint)
        {
            if (parameter is WorldPositionTooltipParameter worldParam)
            {
                return TryWorldToLocalPoint(worldParam.WorldPosition, parentRect, out localPoint);
            }

            if (parameter is UIPositionTooltipParameter uiParam)
            {
                return TryUIToLocalPoint(uiParam, parentRect, out localPoint);
            }

            localPoint = default;
            return false;
        }

        private bool TryWorldToLocalPoint(Vector3 worldPosition, RectTransform parentRect, out Vector2 localPoint)
        {
            var cam = _sceneCameraManager.GetActiveCamera();
            if (cam == null)
            {
                localPoint = default;
                return false;
            }

            var screenPoint   = cam.WorldToScreenPoint(worldPosition);
            var tooltipCamera = _tooltipCanvas != null ? _tooltipCanvas.worldCamera : null;

            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, screenPoint, tooltipCamera, out localPoint);
        }

        private bool TryUIToLocalPoint(
            UIPositionTooltipParameter parameter,
            RectTransform parentRect,
            out Vector2 localPoint)
        {
            var sourceCanvas = parameter.Target.GetComponentInParent<Canvas>();
            if (sourceCanvas != null)
            {
                sourceCanvas = sourceCanvas.rootCanvas;
            }

            var sourceCamera  = sourceCanvas != null ? sourceCanvas.worldCamera : null;
            var screenPoint   = RectTransformUtility.WorldToScreenPoint(sourceCamera, parameter.Target.position);
            var tooltipCamera = _tooltipCanvas != null ? _tooltipCanvas.worldCamera : null;

            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, screenPoint, tooltipCamera, out localPoint);
        }

        private Vector2 ApplyDirectionOffset(Vector2 localPoint, in TooltipPositionSettings settings)
        {
            var halfSize = _rectTransform.sizeDelta * 0.5f;

            var directionOffset = settings.TooltipDirection switch
            {
                TooltipDirection.Left  => new Vector2(-(halfSize.x + settings.Distance), 0f),
                TooltipDirection.Right => new Vector2(  halfSize.x + settings.Distance,  0f),
                TooltipDirection.Up    => new Vector2(0f,  halfSize.y + settings.Distance),
                TooltipDirection.Down  => new Vector2(0f, -(halfSize.y + settings.Distance)),
                _                      => Vector2.zero
            };

            return localPoint + settings.Offset + directionOffset;
        }

        private Vector2 ApplyKeepOnScreen(Vector2 position, in TooltipPositionSettings settings, RectTransform parentRect)
        {
            if (!settings.KeepOnScreen)
            {
                return position;
            }

            var halfSize   = _rectTransform.sizeDelta * 0.5f;
            var parentSize = parentRect.rect.size;

            position.x = Mathf.Clamp(position.x, -parentSize.x * 0.5f + halfSize.x, parentSize.x * 0.5f - halfSize.x);
            position.y = Mathf.Clamp(position.y, -parentSize.y * 0.5f + halfSize.y, parentSize.y * 0.5f - halfSize.y);

            return position;
        }

        private TooltipPositionSettings ResolvePositionSettings(Optional<TooltipPositionSettings> overridenSettings)
        {
            if (overridenSettings.HasValue())
            {
                return overridenSettings.Value();
            }

            return _tooltipSettings.PositionSettings;
        }
    }
}
