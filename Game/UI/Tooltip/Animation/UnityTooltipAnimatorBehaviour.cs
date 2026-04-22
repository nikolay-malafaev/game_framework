using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    public class UnityTooltipAnimatorBehaviour : TooltipAnimatorBehaviour
    {
        [SerializeField]
        private Animation _animationBehaviour;

        private TooltipSettings _tooltipSettings;
        private CommonTooltipSettings _commonSettings;
        private Coroutine _animationCoroutine;

        public override void Setup(TooltipSettings settings, CommonTooltipSettings commonSettings)
        {
            _tooltipSettings = settings;
            _commonSettings = commonSettings;
        }

        public override void PlayShowAnimation(Action callback)
        {
            var clip = new[] { _tooltipSettings?.ShowAnimation, _commonSettings?.DefaultShowAnimation }
                .FirstOrDefault(c => c != null);
            PlayAnimationImpl(clip, callback);
        }

        public override void PlayHideAnimation(Action callback)
        {
            var clip = new[] { _tooltipSettings?.HideAnimation, _commonSettings?.DefaultHideAnimation }
                .FirstOrDefault(c => c != null);
            PlayAnimationImpl(clip, callback);
        }

        private void PlayAnimationImpl(AnimationClip clip, Action callback)
        {
            if (!_animationBehaviour || !clip)
            {
                callback?.Invoke();
                return;
            }

            _animationBehaviour.Stop();

            if (_animationBehaviour.GetClip(clip.name) == null)
                _animationBehaviour.AddClip(clip, clip.name);

            _animationBehaviour.Play(clip.name);

            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);

            _animationCoroutine = StartCoroutine(WaitAnimationEnd(clip.length, callback));
        }

        private IEnumerator WaitAnimationEnd(float duration, Action callback)
        {
            yield return new WaitForSeconds(duration);
            callback?.Invoke();
        }
    }
}
