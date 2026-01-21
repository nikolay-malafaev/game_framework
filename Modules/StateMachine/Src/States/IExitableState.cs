using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public interface IExitableState
    {
        public UniTask Exit();
    }
}