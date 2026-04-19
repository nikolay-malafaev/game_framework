using System;
using System.Collections.Generic;

namespace GameFramework.StateMachine
{
    public class StateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IState> _registeredStates = new();
        private IState _currentState;
        private readonly Dictionary<Type, Delegate> _switchStateDelegates = new();        
        
        public Type PreviousState { get; private set; }

        public void Update()
        {
            _currentState?.Update();
        }

        public bool HasState<TState>() where TState : class, IState
        {
            return _registeredStates.ContainsKey(typeof(TState));
        }

        public void SwitchState<TState>(params IStateParameter[] parameters) where TState : class, IState
        {
            TState nextState = GetState<TState>();

            if (_currentState != null)
                _currentState.Exit();

            PreviousState = _currentState?.GetType();
            _currentState = nextState;

            nextState.Enter(parameters);
            ExecuteSwitchStateEvent(nextState);
        }

        public TState GetState<TState>() where TState : class, IState
        {
            Type stateType = typeof(TState);

            if (_registeredStates.ContainsKey(stateType) == false)
                throw new Exception($"The condition with type {stateType} is not registered");

            return _registeredStates[stateType] as TState;
        }
        
        public TState GetCurrentState<TState>() where TState : class, IState
        {
            return _currentState as TState;
        }

        public bool IsCurrentState<TState>() where TState : class, IState
        {
            return GetCurrentState<TState>() != null;
        }

        public void RegisterState<TState>(TState state) where TState : class, IState
        {
            Type stateType = typeof(TState);

            if (_registeredStates.ContainsKey(stateType))
                return;

            _registeredStates.Add(stateType, state);
        }

        // todo need Func<State> - for resolve??? mb use di
        public void LazyRegisterState<TState>() where TState : class, IState, new()
        {
            
        }

        public void SubscribeToSwitchState<TState>(Action<TState> callback) where TState : class, IState
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

        public void UnsubscribeFromSwitchState<TState>(Action<TState> callback) where TState : class, IState
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

        private void ExecuteSwitchStateEvent<TState>(TState state) where TState : class, IState
        {
            if (!_switchStateDelegates.TryGetValue(typeof(TState), out var existing))
            {
                return;
            }

            ((Action<TState>)existing)?.Invoke(state);
        }

        struct SwitchStateEvent<TState>
        {
            public Action<TState> _onSwitchState;
        }
    }
}