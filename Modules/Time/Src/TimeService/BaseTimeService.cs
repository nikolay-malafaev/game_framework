using System;
using System.Diagnostics;

namespace GameFramework.Time
{
    public abstract class BaseTimeService : ITimeService
    { 
        public virtual DateTime Now => DateTime.Now;
        public virtual DateTime UtcNow => DateTime.UtcNow;
        public virtual long MonotonicTicks => Stopwatch.GetTimestamp();
        public virtual float GameSeconds => UnityEngine.Time.time;
        
        public bool IsPaused => _pauseRequests > 0;
        public event Action<bool> Paused;

        private int _pauseRequests = 0;
        private float _previousTimeScale = 1f;

        public virtual void Pause()
        {
            _pauseRequests++;
            
            if (_pauseRequests == 1)
            {
                _previousTimeScale = UnityEngine.Time.timeScale;
                UnityEngine.Time.timeScale = 0f;
                Paused?.Invoke(true);
            }
        }

        public virtual void UnPause()
        {
            if (_pauseRequests == 0) return;

            _pauseRequests--;

            if (_pauseRequests == 0)
            {
                UnityEngine.Time.timeScale = _previousTimeScale;
                Paused?.Invoke(false);
            }
        }
    }
}
