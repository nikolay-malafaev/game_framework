using System;
using NUnit.Framework;

namespace GameFramework.Time.Tests
{
    public class CountdownTimerEditModeTests
    {
        private CountdownTimer _timer;

        [SetUp]
        public void SetUp()
        {
            _timer = new CountdownTimer();
        }

        [TearDown]
        public void TearDown()
        {
            _timer.Dispose();
        }

        [Test]
        public void Start_WithNegativeDuration_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _timer.Start(-1f));
        }

        [Test]
        public void Start_WithZeroDuration_DoesNothing()
        {
            var startedCalls = 0;
            var startedValue = -1f;

            _timer.Started += v =>
            {
                startedCalls++;
                startedValue = v;
            };

            _timer.Start(0f);

            Assert.That(_timer.IsRunning, Is.False);
            Assert.That(startedCalls, Is.EqualTo(0));
            Assert.That(startedValue, Is.EqualTo(-1f));
        }

        [Test]
        public void Start_WithPositiveDuration_SetsRunningAndInvokesStarted()
        {
            var startedCalls = 0;
            var startedValue = -1f;

            _timer.Started += v =>
            {
                startedCalls++;
                startedValue = v;
            };

            _timer.Start(2.5f);

            Assert.That(_timer.IsRunning, Is.True);
            Assert.That(startedCalls, Is.EqualTo(1));
            Assert.That(startedValue, Is.EqualTo(2.5f).Within(0.0001f));

            Assert.That(_timer.RemainingSeconds.CurrentValue, Is.EqualTo(2.5f).Within(0.0001f));
        }

        [Test]
        public void Start_WithPastTargetDate_Throws()
        {
            var past = DateTime.UtcNow.AddSeconds(-1);

            Assert.Throws<ArgumentException>(() => _timer.Start(past));
        }

        [Test]
        public void Start_WithFutureTargetDate_SetsRunningAndInvokesStarted()
        {
            var startedCalls = 0;
            var startedValue = -1f;

            _timer.Started += v =>
            {
                startedCalls++;
                startedValue = v;
            };

            var future = DateTime.UtcNow.AddSeconds(10);
            _timer.Start(future);

            Assert.That(_timer.IsRunning, Is.True);
            Assert.That(startedCalls, Is.EqualTo(1));

            Assert.That(startedValue, Is.GreaterThan(0f));
            Assert.That(startedValue, Is.InRange(9.0f, 10.5f));
            Assert.That(_timer.RemainingSeconds.CurrentValue, Is.GreaterThan(0f));
        }
    }
}
