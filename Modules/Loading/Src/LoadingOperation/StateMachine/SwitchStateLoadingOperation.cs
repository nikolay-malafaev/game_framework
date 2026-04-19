using Cysharp.Threading.Tasks;
using GameFramework.StateMachine;

namespace GameFramework.Loading
{
    public class SwitchStateLoadingOperation<TState> : ILoadingOperation where TState : class, IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IStateParameter[] _parameters;

        public SwitchStateLoadingOperation(IStateMachine stateMachine, params IStateParameter[] parameters)
        {
            _stateMachine = stateMachine;
            _parameters = parameters;
        }

        public async UniTask<LoadingResult> Run()
        {
            if (_stateMachine.HasState<TState>())
            {
                _stateMachine.SwitchState<TState>(_parameters);
                return LoadingResult.Success(string.Format("State \"{0}\" was switched.", nameof(TState)));
            }

            return LoadingResult.Error(string.Format("State \"{0}\" not found.", nameof(TState)));
        }
    }

    public class SwitchStateAsyncLoadingOperation<TState> : ILoadingOperation where TState : class, IAsyncState
    {
        private readonly IAsyncStateMachine _stateMachine;
        private readonly IStateParameter[] _parameters;

        public SwitchStateAsyncLoadingOperation(IAsyncStateMachine stateMachine, params IStateParameter[] parameters)
        {
            _stateMachine = stateMachine;
            _parameters = parameters;
        }

        public async UniTask<LoadingResult> Run()
        {
            if (_stateMachine.HasState<TState>())
            {
                await _stateMachine.SwitchStateAsync<TState>(_parameters);
                return LoadingResult.Success(string.Format("State \"{0}\" was switched.", nameof(TState)));
            }

            return LoadingResult.Error(string.Format("State \"{0}\" not found.", nameof(TState)));
        }
    }

    public static partial class LoadingBundleExtensions
    {
        public static LoadingBundle SwitchState<TState>(this LoadingBundle loadingBundle, IStateMachine stateMachine, params IStateParameter[] parameters) where TState : class, IState
        {
            loadingBundle.AddOperation(new SwitchStateLoadingOperation<TState>(stateMachine, parameters));
            return loadingBundle;
        }

        public static LoadingBundle SwitchStateAsync<TState>(this LoadingBundle loadingBundle, IAsyncStateMachine stateMachine, params IStateParameter[] parameters) where TState : class, IAsyncState
        {
            loadingBundle.AddOperation(new SwitchStateAsyncLoadingOperation<TState>(stateMachine, parameters));
            return loadingBundle;
        }
    }
}