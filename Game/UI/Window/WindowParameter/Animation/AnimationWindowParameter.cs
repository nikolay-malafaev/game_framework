using UnityEngine;

namespace GameFramework.UI.Window
{
    public class AnimationWindowParameter : IWindowParameter
    {
        public readonly bool Animate;
        
        public AnimationWindowParameter(bool animate)
        {
            Animate = animate;
        }
        
        public new static AnimationWindowParameter Default => new AnimationWindowParameter(true);
    }
}