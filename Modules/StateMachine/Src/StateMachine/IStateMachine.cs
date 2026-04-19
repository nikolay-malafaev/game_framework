using System;

namespace GameFramework.StateMachine
{
    public interface IStateMachine
    {
        Type PreviousState { get; }
        void Update();
        bool HasState<TState>() where TState : class, IState;
        void SwitchState<TState>(params IStateParameter[] parameters) where TState : class, IState;
        TState GetState<TState>() where TState : class, IState;
        TState GetCurrentState<TState>() where TState : class, IState;
        bool IsCurrentState<TState>() where TState : class, IState;
        void RegisterState<TState>(TState state) where TState : class, IState;
        void LazyRegisterState<TState>() where TState : class, IState, new();
        void SubscribeToSwitchState<TState>(Action<TState> callback) where TState : class, IState;
        void UnsubscribeFromSwitchState<TState>(Action<TState> callback) where TState : class, IState;
    }
}