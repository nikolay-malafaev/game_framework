namespace GameFramework.StateMachine
{
    public interface IState
    {
        void Enter(params IStateParameter[] parameters);
        void Exit();
        void Update();
    }
}