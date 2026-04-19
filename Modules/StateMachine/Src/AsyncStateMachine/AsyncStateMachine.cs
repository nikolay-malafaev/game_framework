using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace GameFramework.StateMachine
{
    public class AsyncStateMachine : IAsyncStateMachine
    {
        private readonly Dictionary<Type, IAsyncState> _registeredStates = new();
        private IAsyncState _currentState;
        private readonly Dictionary<Type, Delegate> _switchStateDelegates = new();        
        
        public Type PreviousState { get; private set; }

        public void Update()
        {
            _currentState?.Update();
        }

        public bool HasState<TState>() where TState : class, IAsyncState
        {
            return _registeredStates.ContainsKey(typeof(TState));
        }

        public void SwitchState<TState>(params IStateParameter[] parameters) where TState : class, IAsyncState
        {
            SwitchStateAsync<TState>(parameters).Forget(UnityEngine.Debug.LogException);
        }

        public async UniTask SwitchStateAsync<TState>(params IStateParameter[] parameters) where TState : class, IAsyncState
        {
            TState nextState = await GetAndPrepareNextState<TState>();
            await nextState.EnterAsync(parameters);
            ExecuteSwitchStateEvent(nextState);
        }
        
        public TState GetState<TState>() where TState : class, IAsyncState
        {
            Type stateType = typeof(TState);

            if (_registeredStates.ContainsKey(stateType) == false)
                throw new Exception($"The condition with type {stateType} is not registered");

            return _registeredStates[stateType] as TState;
        }
        
        public TState GetCurrentState<TState>() where TState : class, IAsyncState
        {
            return _currentState as TState;
        }

        public bool IsCurrentState<TState>() where TState : class, IAsyncState
        {
            return GetCurrentState<TState>() != null;
        }

        public void RegisterState<TState>(TState state) where TState : class, IAsyncState
        {
            Type stateType = typeof(TState);

            if (_registeredStates.ContainsKey(stateType))
                return;

            _registeredStates.Add(stateType, state);
        }

        public void LazyRegisterState<TState>() where TState : class, IAsyncState, new()
        {
            
        }

        public void SubscribeToSwitchState<TState>(Action<TState> callback) where TState : class, IAsyncState
        {
            if (callback == null)
            {
                return;
            }

            var type = typeof(TState);
            if (_switchStateDelegates.TryGetValue(type, out var existing))
            {
                _switchStateDelegates[type] = (Action<TState>)existing + callback;
            }
            else
            {
                _switchStateDelegates[type] = callback;
            }
        }

        public void UnsubscribeFromSwitchState<TState>(Action<TState> callback) where TState : class, IAsyncState
        {
            if (callback == null)
            {
                return;
            }
            var type = typeof(TState);
            if (!_switchStateDelegates.TryGetValue(type, out var existing))
            {
                return;
            }

            var updated = (Action<TState>) existing - callback;
            if (updated == null)
            {
                _switchStateDelegates.Remove(type);
            }
            else
            {
                _switchStateDelegates[type] = updated;
            }
        }

        private void ExecuteSwitchStateEvent<TState>(TState state) where TState : class, IAsyncState
        {
            if (!_switchStateDelegates.TryGetValue(typeof(TState), out var existing))
            {
                return;
            }

            ((Action<TState>)existing)?.Invoke(state);
        }

        private async UniTask<TState> GetAndPrepareNextState<TState>() where TState : class, IAsyncState
        {
            TState nextState = GetState<TState>();

            if (_currentState != null)
                await _currentState.ExitAsync();

            PreviousState = _currentState?.GetType();
            _currentState = nextState;

            return nextState;
        }
    }
}
