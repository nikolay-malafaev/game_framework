using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameFramework.Time.Tests
{
    public class CountdownTimerPlayModeTests
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

        [UnityTest]
        public IEnumerator Start_Duration_CountsDown_And_Finishes()
        {
            var finishedCalls = 0;
            _timer.Finished += () => finishedCalls++;

            _timer.Start(0.25f);

            yield return null;
            yield return null;

            yield return WaitUntilOrTimeout(() => finishedCalls == 1, 2f);

            Assert.That(finishedCalls, Is.EqualTo(1));
            Assert.That(_timer.IsRunning, Is.False);
            Assert.That(_timer.RemainingSeconds.CurrentValue, Is.EqualTo(0f).Within(0.02f));
        }

        [UnityTest]
        public IEnumerator Stop_WhileRunning_ResetsAndDoesNotInvokeFinished()
        {
            var finishedCalls = 0;
            _timer.Finished += () => finishedCalls++;

            _timer.Start(1.0f);

            yield return null;
            yield return null;

            _timer.Stop();

            Assert.That(_timer.IsRunning, Is.False);
            Assert.That(_timer.RemainingSeconds.CurrentValue, Is.EqualTo(0f).Within(0.0001f));

            yield return new WaitForSecondsRealtime(1.1f);

            Assert.That(finishedCalls, Is.EqualTo(0));
        }

        [UnityTest]
        public IEnumerator Start_TargetDate_Works_And_Finishes()
        {
            var finishedCalls = 0;
            _timer.Finished += () => finishedCalls++;

            var target = DateTime.UtcNow.AddSeconds(0.25);
            _timer.Start(target);

            yield return null;
            yield return null;

            yield return WaitUntilOrTimeout(() => finishedCalls == 1, 2f);

            Assert.That(finishedCalls, Is.EqualTo(1));
            Assert.That(_timer.IsRunning, Is.False);
            Assert.That(_timer.RemainingSeconds.CurrentValue, Is.EqualTo(0f).Within(0.02f));
        }

        [UnityTest]
        public IEnumerator Restart_StopsPreviousSubscription_And_FinishesOnce()
        {
            var startedCalls = 0;
            var finishedCalls = 0;

            _timer.Started += _ => startedCalls++;
            _timer.Finished += () => finishedCalls++;

            _timer.Start(1.0f);
            yield return null;
            yield return null;

            _timer.Start(0.2f);

            yield return WaitUntilOrTimeout(() => finishedCalls == 1, 2f);

            Assert.That(startedCalls, Is.EqualTo(2));
            Assert.That(finishedCalls, Is.EqualTo(1));
            Assert.That(_timer.IsRunning, Is.False);
            Assert.That(_timer.RemainingSeconds.CurrentValue, Is.EqualTo(0f).Within(0.02f));
        }

        [UnityTest]
        public IEnumerator Dispose_StopsTimer_And_PreventsFinishedLater()
        {
            var finishedCalls = 0;
            _timer.Finished += () => finishedCalls++;

            _timer.Start(0.3f);

            _timer.Dispose();

            yield return new WaitForSecondsRealtime(0.5f);

            Assert.That(finishedCalls, Is.EqualTo(0));
        }

        private static IEnumerator WaitUntilOrTimeout(Func<bool> predicate, float timeoutSeconds)
        {
            var start = UnityEngine.Time.realtimeSinceStartup;
            while (!predicate())
            {
                if (UnityEngine.Time.realtimeSinceStartup - start > timeoutSeconds)
                    Assert.Fail($"Timeout after {timeoutSeconds} seconds.");
                yield return null;
            }
        }
    }
}