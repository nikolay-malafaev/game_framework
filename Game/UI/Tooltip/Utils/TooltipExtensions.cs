namespace GameFramework.UI.Tooltip
{
    public static class TooltipExtensions
    {
        public static TParameter GetParameter<TParameter>(
            this ITooltipParameter[] parameters,
            TParameter defaultParameter) where TParameter : class, ITooltipParameter
        {
            if (parameters == null || parameters.Length == 0)
                return defaultParameter;

            foreach (var parameter in parameters)
            {
                if (parameter is TParameter tParameter)
                    return tParameter;
            }

            return defaultParameter;
        }

        public static bool HasParameter<TParameter>(
            this ITooltipParameter[] parameters) where TParameter : class, ITooltipParameter
        {
            if (parameters == null || parameters.Length == 0)
                return false;

            foreach (var parameter in parameters)
            {
                if (parameter is TParameter)
                    return true;
            }

            return false;
        }

        public static bool TryGetParameter<TParameter>(
            this ITooltipParameter[] parameters,
            out TParameter outParameter) where TParameter : class, ITooltipParameter
        {
            outParameter = null;

            if (parameters == null || parameters.Length == 0)
                return false;

            foreach (var parameter in parameters)
            {
                if (parameter is TParameter tParameter)
                {
                    outParameter = tParameter;
                    return true;
                }
            }

            return false;
        }
    }
}
