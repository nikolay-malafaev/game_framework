using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public abstract class AsyncStateBase<TStateMachine> : IAsyncState where TStateMachine : class, IAsyncStateMachine
    {
        protected readonly TStateMachine _stateMachine;
        
        public AsyncStateBase(TStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public virtual UniTask EnterAsync(params IStateParameter[] parameters) => UniTask.CompletedTask;
        public virtual void Update() { }
        public virtual UniTask ExitAsync() => UniTask.CompletedTask;
    }
}
