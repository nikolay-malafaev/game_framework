using System;
using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    public class TooltipBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TooltipAnimatorBehaviour _animator;

        public string Id { get; private set; }

        public void Initialize(string tooltipId, TooltipSettings settings, CommonTooltipSettings commonSettings)
        {
            Id = tooltipId;
            _animator?.Setup(settings, commonSettings);
        }

        public void Show(Action callback = null, params ITooltipParameter[] parameters)
        {
            var animationParameter = parameters.GetParameter(AnimationTooltipParameter.Default);
            if (_animator && animationParameter.Animate)
                _animator.PlayShowAnimation(callback);
            else
                callback?.Invoke();
        }

        public void Hide(Action callback = null, params ITooltipParameter[] parameters)
        {
            var animationParameter = parameters.GetParameter(AnimationTooltipParameter.Default);
            if (_animator && animationParameter.Animate)
                _animator.PlayHideAnimation(callback);
            else
                callback?.Invoke();
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
    }
}
