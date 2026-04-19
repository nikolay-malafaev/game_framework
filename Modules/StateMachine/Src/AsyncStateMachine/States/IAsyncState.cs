using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public interface IAsyncState
    {
        UniTask EnterAsync(params IStateParameter[] parameters);
        UniTask ExitAsync();
        void Update();
    }
}
