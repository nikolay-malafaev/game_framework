using UnityEngine;

namespace GameFramework.UI.Window
{
    public class UnityAnimationWindowParameter : AnimationWindowParameter
    {
        public readonly AnimationClip Animation;

        public UnityAnimationWindowParameter(bool animate, AnimationClip animation = null) : base(animate)
        {
            Animation = animation;
        }

        public new static UnityAnimationWindowParameter Default => new UnityAnimationWindowParameter(true, null);
    }
}