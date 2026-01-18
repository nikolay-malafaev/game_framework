using System;

namespace GameFramework.Time
{
    public interface ITimeService
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        long MonotonicTicks { get; }
        float GameSeconds { get; }
    }
}