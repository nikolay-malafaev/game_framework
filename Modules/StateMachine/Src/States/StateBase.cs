using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public abstract class StateBase<TStateMachine> : IState where TStateMachine : class, IStateMachine
    {
        protected readonly TStateMachine _stateMachine;
        
        public StateBase(TStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public virtual UniTask Enter() => UniTask.CompletedTask;
        public virtual void Update() { }
        public virtual UniTask Exit() => UniTask.CompletedTask;
    }
}
