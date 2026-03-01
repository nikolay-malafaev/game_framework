using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public interface IState : IExitableState
    {
        UniTask Enter();
    }
}