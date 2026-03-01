using Cysharp.Threading.Tasks;
using GameFramework.StateMachine;

namespace GameFramework.Loading
{
    public class SwitchStateLoadingOperation<TState> : ILoadingOperation where TState : class, IState
    {
        private readonly IStateMachine _stateMachine;

        public SwitchStateLoadingOperation(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public async UniTask<LoadingResult> Run()
        {
            if (_stateMachine.HasState<TState>())
            {
                await _stateMachine.SwitchStateAsync<TState>();
                return LoadingResult.Success(string.Format("State \"{0}\" was switched.", nameof(TState)));
            }

            return LoadingResult.Error(string.Format("State \"{0}\" not found.", nameof(TState)));
        }
    }

    public static partial class LoadingBundleExtensions
    {
        public static LoadingBundle SwitchState<TState>(this LoadingBundle loadingBundle, IStateMachine stateMachine) where TState : class, IState
        {
            loadingBundle.AddOperation(new SwitchStateLoadingOperation<TState>(stateMachine));
            return loadingBundle;
        }
    }
}