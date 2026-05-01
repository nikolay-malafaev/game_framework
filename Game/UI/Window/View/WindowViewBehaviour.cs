using System;
using System.Linq;
using GameFramework.Logging;
using GameFramework.StaticData;
using UnityEngine;
using VContainer;

namespace GameFramework.UI.Window
{
    [Loggable]
    public partial class WindowViewBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private WindowAnimatorBehaviour _windowAnimator;
        
        protected CommonWindowSettings _commonWindowSettings;
        protected WindowSettings _windowSettings;
        
        public string WindowId { get; private set; }
        
        public event Action HideWindowClicked;
        public event Action CloseWindowClicked;

        public void Initialize(IObjectResolver objectResolver, string windowId)
        {
            WindowId = windowId;
            _commonWindowSettings = objectResolver.Resolve<IStaticDataService>().Get<CommonWindowSettings>();
            _windowSettings = objectResolver.Resolve<IStaticDataService>().Get<WindowSettings>(WindowId);
        }

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
