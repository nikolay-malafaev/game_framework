using System;
using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public interface IStateMachine
    {
        Type PreviousState { get; }
        bool HasState<TState>() where TState : class, IExitableState;
        UniTask SwitchState<TState>() where TState : class, IState;
        UniTask SwitchState<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
        TState GetState<TState>() where TState : class, IExitableState;
        TState GetCurrentState<TState>() where TState : class, IExitableState;
        void RegisterState<TState>(TState state) where TState : class, IExitableState;
        void LazyRegisterState<TState>() where TState : class, IExitableState, new();
    }
}