using System;
using Cysharp.Threading.Tasks;

namespace GameFramework.StateMachine
{
    public interface IAsyncStateMachine
    {
        Type PreviousState { get; }
        void Update();
        bool HasState<TState>() where TState : class, IAsyncState;
        UniTask SwitchStateAsync<TState>(params IStateParameter[] parameters) where TState : class, IAsyncState;
        void SwitchState<TState>(params IStateParameter[] parameters) where TState : class, IAsyncState;
        TState GetState<TState>() where TState : class, IAsyncState;
        TState GetCurrentState<TState>() where TState : class, IAsyncState;
        bool IsCurrentState<TState>() where TState : class, IAsyncState;
        void RegisterState<TState>(TState state) where TState : class, IAsyncState;
        void LazyRegisterState<TState>() where TState : class, IAsyncState, new();
        void SubscribeToSwitchState<TState>(Action<TState> callback) where TState : class, IAsyncState;
        void UnsubscribeFromSwitchState<TState>(Action<TState> callback) where TState : class, IAsyncState;
    }
}
