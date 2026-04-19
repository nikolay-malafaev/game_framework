namespace GameFramework.StateMachine
{
    public static class StateParameterExtensions
    {
        public static bool TryGetParameter<TParameter>(this IStateParameter[] parameters, out TParameter outParameter) where TParameter : class, IStateParameter
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

            return false;
        }

        public static TParameter GetParameter<TParameter>(this IStateParameter[] parameters, TParameter defaultParameter = null) where TParameter : class, IStateParameter
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
