using System;
using System.Linq;
using GameFramework.Logging;
using UnityEngine;
using VContainer;

namespace GameFramework.UI.Window
{
    [Loggable]
    public partial class WindowViewBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private WindowAnimatorBehaviour _windowAnimator;
        [Inject]
        private WindowSettings _windowSettings;
        [Inject] 
        private CommonWindowSettings _commonWindowSettings;
        
        public event Action HideWindowClicked;
        public event Action CloseWindowClicked;

        public virtual void Show(Action onShow, params IWindowParameter[] parameters)
        {
            var animationWindowParameter = parameters.GetParameter(AnimationWindowParameter.Default);
            if (_windowAnimator && animationWindowParameter.Animate)
            {
                _windowAnimator.PlayShowAnimation(onShow);
            }
            else
            {
                onShow?.Invoke();
            }
        }

        public virtual void Hide(Action onHide, params IWindowParameter[] parameters)
        {
            var animationWindowParameter = parameters.GetParameter(AnimationWindowParameter.Default);
            if (_windowAnimator && animationWindowParameter.Animate)
            {
                _windowAnimator.PlayHideAnimation(onHide);
            }
            else
            {
                onHide?.Invoke();
            }
        }

        public void HideWindow() => HideWindowClicked?.Invoke();
        public void CloseWindow() => CloseWindowClicked?.Invoke();
    }
}
