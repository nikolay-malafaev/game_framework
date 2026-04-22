namespace GameFramework.UI.Tooltip
{
    public class AnimationTooltipParameter : ITooltipParameter
    {
        public readonly bool Animate;

        public AnimationTooltipParameter(bool animate)
        {
            Animate = animate;
        }

        public static AnimationTooltipParameter Default => new AnimationTooltipParameter(true);
    }
}
