using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public interface IPayloadedState<TPayload> : IExitableState
    {
        public UniTask Enter(TPayload payload);
    }
}