namespace GameFramework.UI.Window
{
    public static class WindowExtensions
    {
        public static bool TryGetParameter<TParameter>(this IWindowParameter[] parameters, out TParameter outParameter) where TParameter : class, IWindowParameter
        {
            outParameter = null;
            
            if (parameters == null || parameters.Length == 0)
            {
                return false;
            }
            
            foreach (var parameter in parameters)
            {
                if (parameter is TParameter tParameter)
                {
                    outParameter = tParameter;
                    return true;
                }
            }

            return true;
        }

        public static TParameter GetParameter<TParameter>(this IWindowParameter[] parameters, TParameter defaultParameter) where TParameter : class, IWindowParameter
        {
            if (parameters == null || parameters.Length == 0)
            {
                return defaultParameter;
            }

            foreach (var parameter in parameters)
            {
                if (parameter is TParameter tParameter)
                {
                    return tParameter;
                }
            }

            return defaultParameter;
        }
    }
}