using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public interface IExitableState
    {
        UniTask Exit();
        void Update();
    }
}