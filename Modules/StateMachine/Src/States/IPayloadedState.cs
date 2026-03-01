using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public interface IPayloadedState<TPayload> : IExitableState
    {
        UniTask Enter(TPayload payload);
    }
}