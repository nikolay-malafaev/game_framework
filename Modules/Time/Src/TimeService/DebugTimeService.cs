using System;

namespace GameFramework.Time
{
    public class DebugTimeService : BaseTimeService
    {
        private TimeSpan _offset;
        private bool _freeze;
        private DateTime _frozenLocal;
        private DateTime _frozenUtc;
        private float _frozenGameSeconds;

        public override DateTime Now => _freeze ? _frozenLocal : base.Now + _offset;
        public override DateTime UtcNow => _freeze ? _frozenUtc : base.UtcNow + _offset;
        public override float GameSeconds => _freeze ? _frozenGameSeconds : base.GameSeconds + (float) _offset.TotalSeconds;
        
        public void SetOffset(TimeSpan offset) => _offset = offset;

        public void Freeze(bool freeze)
        {
            if (_freeze == freeze) return;
            _freeze = freeze;
            if (_freeze)
            {
                _frozenLocal = base.Now + _offset;
                _frozenUtc = base.UtcNow + _offset;
                _frozenGameSeconds = base.GameSeconds + (float) _offset.TotalSeconds;
            }
        }
    }
}