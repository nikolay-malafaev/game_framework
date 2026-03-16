using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using VContainer;

namespace GameFramework.UI.Window
{
    public class UnityWindowAnimatorBehaviour : WindowAnimatorBehaviour
    {
        [SerializeField]
        private Animation _animationBehaviour;
        [Inject]
        private WindowSettings _windowSettings;
        [Inject] 
        private CommonWindowSettings _commonWindowSettings;
        
        private Coroutine _animationCoroutine = null;
        
        public override void PlayShowAnimation(Action callback, params IWindowParameter[] parameters)
        {
            var animationWindowParameter = parameters.GetParameter(UnityAnimationWindowParameter.Default);
            var animations = new []
            {
                animationWindowParameter.Animation, 
                _windowSettings.ShowAnimation,
                _commonWindowSettings.DefaultShowAnimation
            };
            
            AnimationClip animationClip = animations.FirstOrDefault(a => a != null);
            PlayAnimationImpl(animationClip, callback);
        }

        public override void PlayHideAnimation(Action callback, params IWindowParameter[] parameters)
        {
            var animationWindowParameter = parameters.GetParameter(UnityAnimationWindowParameter.Default);
            var animations = new []
            {
                animationWindowParameter.Animation, 
                _windowSettings.HideAnimation,
                _commonWindowSettings.DefaultHideAnimation
            };
            
            AnimationClip animationClip = animations.FirstOrDefault(a => a != null);
            PlayAnimationImpl(animationClip, callback);
        }

        private void PlayAnimationImpl(AnimationClip animationClip, Action callback)
        {
            if (!_animationBehaviour || !animationClip)
            {
                callback?.Invoke();
                return;
            }
            
            _animationBehaviour.Stop();
            
            if (_animationBehaviour.GetClip(animationClip.name) == null)
            {
                _animationBehaviour.AddClip(animationClip, animationClip.name);
            }
            
            _animationBehaviour.Play(animationClip.name);

            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(WaitAnimationEnd(animationClip.length, callback));
        }

        private IEnumerator WaitAnimationEnd(float duration, Action callback)
        {
            yield return new WaitForSeconds(duration);
            callback?.Invoke();
        }
    }
}