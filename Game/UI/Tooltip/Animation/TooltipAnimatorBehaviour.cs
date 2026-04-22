using System;
using UnityEngine;

namespace GameFramework.UI.Tooltip
{
    public abstract class TooltipAnimatorBehaviour : MonoBehaviour
    {
        public abstract void PlayShowAnimation(Action callback);
        public abstract void PlayHideAnimation(Action callback);
        public virtual void Setup(TooltipSettings settings, CommonTooltipSettings commonSettings) { }
    }
}
