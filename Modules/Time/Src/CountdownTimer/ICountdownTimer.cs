using System;
using R3;

namespace GameFramework.Time
{
    public interface ICountdownTimer : IDisposable
    {
        event Action<float> Started;
        event Action Finished;
        ReadOnlyReactiveProperty<float> RemainingSeconds { get; }
        bool IsRunning { get; }
        void Start(float durationSeconds);
        void Start(DateTime targetDateTime);
        void Stop();
    }
}