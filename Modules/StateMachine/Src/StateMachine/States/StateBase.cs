namespace GameFramework.StateMachine
{
    public abstract class StateBase<TStateMachine> : IState where TStateMachine : class, IStateMachine
    {
        protected readonly TStateMachine _stateMachine;
        
        public StateBase(TStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public virtual void Enter(params IStateParameter[] parameters) { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }
}
