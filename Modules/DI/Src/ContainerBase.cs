using System;

namespace GameFramework.DI
{
    public abstract class ContainerBase<TContainer, TContext>
        where TContainer : ContainerBase<TContainer, TContext>, new() where TContext : class, IContext
    {
        private static readonly Lazy<TContainer> Instance = new(() => new TContainer(), isThreadSafe: true);

        protected abstract TContext DefaultContext { get; }

        private TContext _storedContext = null;

        public static TContext Context
        {
            get
            {
                var instance = Instance.Value;
                
                if (instance._storedContext != null)
                    return instance._storedContext;

                return instance.DefaultContext;
            }
        }

        public static void SetContext(TContext context)
        {
            Instance.Value._storedContext = context;
        }

        public static void ClearContext()
        {
            Instance.Value._storedContext = null;
        } 
    }
}