using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace GameFramework.StateMachine
{
    public class StateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IExitableState> _registeredStates = new();
        private IExitableState _currentState;
        private readonly Dictionary<Type, Delegate> _switchStateDelegates = new();        
        
        public Type PreviousState { get; private set; }

        public void Update()
        {
            _currentState?.Update();
        }

        public bool HasState<TState>() where TState : class, IExitableState
        {
            return _registeredStates.ContainsKey(typeof(TState));
        }

        public void SwitchState<TState>() where TState : class, IState
        {
            SwitchStateAsync<TState>().Forget(UnityEngine.Debug.LogException);
        }

        public void SwitchState<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            SwitchStateAsync<TState, TPayload>(payload).Forget(UnityEngine.Debug.LogException);
        }

        public async UniTask SwitchStateAsync<TState>() where TState : class, IState
        {
            TState nextState = await GetAndPrepareNextState<TState>();
            await nextState.Enter();
            ExecuteSwitchStateEvent(nextState);
        }

        public async UniTask SwitchStateAsync<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            TState nextState = await GetAndPrepareNextState<TState>();
            await nextState.Enter(payload);
            ExecuteSwitchStateEvent(nextState);
        }
        
        public TState GetState<TState>() where TState : class, IExitableState
        {
            Type stateType = typeof(TState);

            if (_registeredStates.ContainsKey(stateType) == false)
                throw new Exception($"The condition with type {stateType} is not registered");

            return _registeredStates[stateType] as TState;
        }
        
        public TState GetCurrentState<TState>() where TState : class, IExitableState
        {
            return _currentState as TState;
        }
        
        public void RegisterState<TState>(TState state) where TState : class, IExitableState
        {
            Type stateType = typeof(TState);

            if (_registeredStates.ContainsKey(stateType) == true)
                return;

            _registeredStates.Add(stateType, state);
        }

        // todo need Func<State> - for resolve??? mb use di
        public void LazyRegisterState<TState>() where TState : class, IExitableState, new()
        {
            
        }

        public void SubscribeToSwitchState<TState>(Action<TState> callback) where TState : class, IExitableState
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

        public void UnsubscribeFromSwitchState<TState>(Action<TState> callback) where TState : class, IExitableState
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

        private void ExecuteSwitchStateEvent<TState>(TState state) where TState : class, IExitableState
        {
            if (!_switchStateDelegates.TryGetValue(typeof(TState), out var existing))
            {
                return;
            }

            ((Action<TState>)existing)?.Invoke(state);
        }

        private async UniTask<TState> GetAndPrepareNextState<TState>() where TState : class, IExitableState
        {
            TState nextState = GetState<TState>();

            if (_currentState != null)
                await _currentState.Exit();

            PreviousState = _currentState?.GetType();
            _currentState = nextState;

            return nextState;
        }

        struct SwitchStateEvent<TState>
        {
            public Action<TState> _onSwitchState;
        }
    }
}