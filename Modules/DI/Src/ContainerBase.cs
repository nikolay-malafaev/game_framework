using System;

namespace GameFramework.DI
{
    public abstract class ContainerBase<TContainer, TContext>
        where TContainer : ContainerBase<TContainer, TContext>, new() where TContext : class, IContext
    {
        private static readonly Lazy<TContainer> Instance = new(() => new TContainer(), isThreadSafe: true);

        private TContext _context;
        
        public static TContext Context
        {
            get
            {
                var instance = Instance.Value;
                instance._context ??= instance.CreateContext();
                return instance._context;
            }
        } 

        public static void SetContext(TContext context)
        {
            Instance.Value._context = context;
        }

        public static void ClearContext()
        {
            Instance.Value._context = null;
        }
        
        protected abstract TContext CreateContext();
    }
}