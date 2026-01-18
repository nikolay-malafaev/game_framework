using System;
using R3;
using UnityEngine;

namespace GameFramework.Time
{
    public class CountdownTimer : ICountdownTimer
    {
        private CompositeDisposable _disposables = new();
        private ReactiveProperty<float> _remainingSeconds = new();
        private DateTime _endTimeUtc = DateTime.MinValue;
        private bool _isRunning = false;
        
        public event Action<float> Started;
        public event Action Finished;
        public ReadOnlyReactiveProperty<float> RemainingSeconds => _remainingSeconds;
        public bool IsRunning => _isRunning;

        public void Dispose()
        {
            Stop();
            _disposables.Dispose();
            _remainingSeconds.Dispose();
        }
        
        public void Start(float durationSeconds)
        {
            if (durationSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(durationSeconds), "Duration cannot be negative.");
            
            if (durationSeconds == 0)
            {
                 return;
            }

            Stop();
            
            _endTimeUtc = DateTime.UtcNow.AddSeconds(durationSeconds);
            _remainingSeconds.Value = durationSeconds; 
            _isRunning = true;

            Started?.Invoke(_remainingSeconds.Value);

            Observable.EveryUpdate()
                .Subscribe((_) => Tick())
                .AddTo(_disposables);
        }
        
        public void Start(DateTime targetDateTime)
        {
            DateTime targetUtc = targetDateTime.ToUniversalTime();
            DateTime nowUtc = DateTime.UtcNow;

            if (targetUtc <= nowUtc)
                throw new ArgumentException("Target DateTime must be in the future.", nameof(targetDateTime));

            Stop();

            _endTimeUtc = targetUtc;
            float initialRemainingSeconds = (float)(_endTimeUtc - nowUtc).TotalSeconds;
            _remainingSeconds.Value = initialRemainingSeconds;
            _isRunning = true;

            Started?.Invoke(_remainingSeconds.Value);
            
            Observable.EveryUpdate()
                .Subscribe((_) => Tick())
                .AddTo(_disposables);
        }
        
        public void Stop()
        {
            if (!_isRunning) return;

            _disposables.Clear();
            _remainingSeconds.Value = 0;
            _endTimeUtc = DateTime.MinValue;
            _isRunning = false;
        }
        
        private void Tick()
        {
            if (!_isRunning)
                return;
            
            TimeSpan remainingTimeSpan = _endTimeUtc - DateTime.UtcNow;
            float currentRemainingSeconds = (float)remainingTimeSpan.TotalSeconds;
            
            _remainingSeconds.Value = Mathf.Max(currentRemainingSeconds, 0f);
            
            if (_remainingSeconds.Value <= 0)
            {
                _isRunning = false;
                _disposables.Clear();
                _endTimeUtc = DateTime.MinValue;

                Finished?.Invoke(); 
            }
        }
    }
}