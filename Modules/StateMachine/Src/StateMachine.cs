using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace GameFramework.StateMachine
{
    public class StateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IExitableState> _registeredStates = new();
        private IExitableState _currentState;
        public Type PreviousState { get; private set; }

        public bool HasState<TState>() where TState : class, IExitableState
        {
            return _registeredStates.ContainsKey(typeof(TState));
        }

        public async UniTask SwitchState<TState>() where TState : class, IState
        {
            TState nextState = await GetAndPrepareNextState<TState>();
            await nextState.Enter();
        }

        public async UniTask SwitchState<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            TState nextState = await GetAndPrepareNextState<TState>();
            await nextState.Enter(payload);
        }
        
        public void RegisterState<TState>(TState state) where TState : class, IExitableState
        {
            Type stateType = typeof(TState);

            if (_registeredStates.ContainsKey(stateType) == true)
                return;

            _registeredStates.Add(stateType, state);
        }

        public void LazyRegisterState<TState>() where TState : class, IExitableState, new()
        {
            
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
        
        private async UniTask<TState> GetAndPrepareNextState<TState>() where TState : class, IExitableState
        {
            TState nextState = GetState<TState>();

            if (_currentState != null)
                await _currentState.Exit();

            PreviousState = _currentState?.GetType();
            _currentState = nextState;

            return nextState;
        }
    }
}