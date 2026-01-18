using System;

namespace GameFramework.Time
{
    public class DebugTimeService : ITimeService
    {
        private readonly ProductionTimeService _base = new ();
        private TimeSpan _offset;
        private bool _freeze;
        private DateTime _frozenLocal;
        private DateTime _frozenUtc;
        private float _frozenGameSeconds;

        public DateTime Now => _freeze ? _frozenLocal : _base.Now + _offset;
        public DateTime UtcNow => _freeze ? _frozenUtc : _base.UtcNow + _offset;
        public long MonotonicTicks => _base.MonotonicTicks;
        public float GameSeconds => _freeze ? _frozenGameSeconds : _base.GameSeconds + (float) _offset.TotalSeconds;
        
        public void SetOffset(TimeSpan offset) => _offset = offset;

        public void Freeze(bool freeze)
        {
            if (_freeze == freeze) return;
            _freeze = freeze;
            if (_freeze)
            {
                _frozenLocal = _base.Now + _offset;
                _frozenUtc = _base.UtcNow + _offset;
                _frozenGameSeconds = _base.GameSeconds + (float) _offset.TotalSeconds;
            }
        }
    }
}