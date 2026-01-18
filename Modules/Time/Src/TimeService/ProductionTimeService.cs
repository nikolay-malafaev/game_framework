using System;
using System.Diagnostics;

namespace GameFramework.Time
{
    public class ProductionTimeService : ITimeService
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public long MonotonicTicks => Stopwatch.GetTimestamp();
        public float GameSeconds => UnityEngine.Time.time;
    }
}