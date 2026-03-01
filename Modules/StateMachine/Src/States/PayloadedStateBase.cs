using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public abstract class PayloadedStateBase<TPayload> : IPayloadedState<TPayload>
    {
        private readonly IStateMachine _stateMachine;

        public PayloadedStateBase(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public virtual UniTask Enter(TPayload payload) => UniTask.CompletedTask;

        public virtual void Update() { }
        
        public virtual UniTask Exit() => UniTask.CompletedTask;
    }
}