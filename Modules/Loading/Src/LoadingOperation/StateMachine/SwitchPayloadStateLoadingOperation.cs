using Cysharp.Threading.Tasks;
using GameFramework.StateMachine;

namespace GameFramework.Loading
{
    public class SwitchPayloadStateLoadingOperation<TState, TPayload>: ILoadingOperation where TState : class, IPayloadedState<TPayload>
    {
        private readonly IStateMachine _stateMachine;
        private readonly TPayload _payload;
    
        public SwitchPayloadStateLoadingOperation(IStateMachine stateMachine, TPayload payload)
        {
            _stateMachine = stateMachine;
            _payload = payload;
        }
        
        public async UniTask<LoadingResult> Run()
        {
            if (_stateMachine.HasState<TState>())
            {
                await _stateMachine.SwitchState<TState, TPayload>(_payload);
                return LoadingResult.Success(string.Format("State \"{0}\" was switched.", nameof(TState)));
            }
            return LoadingResult.Error(string.Format("State \"{0}\" not found.", nameof(TState)));
        }
    }

    public static partial class LoadingBundleExtensions
    {
        public static LoadingBundle SwitchState<TState, TPayload>(this LoadingBundle loadingBundle, IStateMachine stateMachine, TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            loadingBundle.AddOperation(new SwitchPayloadStateLoadingOperation<TState, TPayload>(stateMachine, payload));
            return loadingBundle;
        }
    }
}