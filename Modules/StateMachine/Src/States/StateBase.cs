using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public abstract class StateBase : IState
    {
        private readonly IStateMachine _stateMachine;
        
        public StateBase(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public virtual UniTask Enter() => UniTask.CompletedTask;
        public virtual UniTask Exit() => UniTask.CompletedTask;
    }
}
