using System;
using UnityEngine;

namespace GameFramework.UI.Window
{
    public abstract class WindowAnimatorBehaviour : MonoBehaviour
    {
        public abstract void PlayShowAnimation(Action callback, params IWindowParameter[] parameters);
        public abstract void PlayHideAnimation(Action callback, params IWindowParameter[] parameters);
    }
}