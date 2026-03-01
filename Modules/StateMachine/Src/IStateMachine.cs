using System;
using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public interface IStateMachine
    {
        Type PreviousState { get; }
        void Update();
        bool HasState<TState>() where TState : class, IExitableState;
        void SwitchState<TState>() where TState : class, IState;
        void SwitchState<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
        UniTask SwitchStateAsync<TState>() where TState : class, IState;
        UniTask SwitchStateAsync<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
        TState GetState<TState>() where TState : class, IExitableState;
        TState GetCurrentState<TState>() where TState : class, IExitableState;
        void RegisterState<TState>(TState state) where TState : class, IExitableState;
        void LazyRegisterState<TState>() where TState : class, IExitableState, new();
        void SubscribeToSwitchState<TState>(Action<TState> callback) where TState : class, IExitableState;
        void UnsubscribeFromSwitchState<TState>(Action<TState> callback) where TState : class, IExitableState;
    }
}