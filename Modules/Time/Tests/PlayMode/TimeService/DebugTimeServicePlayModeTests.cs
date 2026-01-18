using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameFramework.Time.Tests
{
    public class DebugTimeServicePlayModeTests
    {
        [UnityTest]
        public System.Collections.IEnumerator ProductionTimeService_GameSeconds_EqualsUnityTime()
        {
            var sut = new ProductionTimeService();

            var expected = UnityEngine.Time.time;
            var actual = sut.GameSeconds;

            Assert.That(actual, Is.EqualTo(expected).Within(0.0001f));
            yield return null;
        }

        [UnityTest]
        public System.Collections.IEnumerator DebugTimeService_OffsetAppliedToGameSeconds()
        {
            var sut = new DebugTimeService();
            var offset = TimeSpan.FromSeconds(10);

            sut.SetOffset(offset);

            var expected = UnityEngine.Time.time + (float)offset.TotalSeconds;
            var actual = sut.GameSeconds;

            Assert.That(actual, Is.EqualTo(expected).Within(0.02f));
            yield return null;
        }

        [UnityTest]
        public System.Collections.IEnumerator DebugTimeService_Freeze_HoldsNowUtcNowAndGameSecondsConstant()
        {
            var sut = new DebugTimeService();
            sut.SetOffset(TimeSpan.FromSeconds(3));

            sut.Freeze(true);

            var now1 = sut.Now;
            var utc1 = sut.UtcNow;
            var gs1 = sut.GameSeconds;

            yield return new WaitForSeconds(0.2f);

            var now2 = sut.Now;
            var utc2 = sut.UtcNow;
            var gs2 = sut.GameSeconds;

            Assert.AreEqual(now1, now2, "Now должен быть заморожен и не меняться.");
            Assert.AreEqual(utc1, utc2, "UtcNow должен быть заморожен и не меняться.");
            Assert.AreEqual(gs1, gs2, "GameSeconds должен быть заморожен и не меняться.");
        }

        [UnityTest]
        public System.Collections.IEnumerator DebugTimeService_Unfreeze_ResumesTimeFlow()
        {
            var sut = new DebugTimeService();

            sut.Freeze(true);
            var frozenNow = sut.Now;
            var frozenGameSeconds = sut.GameSeconds;

            yield return new WaitForSeconds(0.1f);

            sut.Freeze(false);

            yield return new WaitForSeconds(0.1f);

            Assert.That(sut.Now, Is.GreaterThan(frozenNow), "После разморозки Now должен снова идти вперёд.");
            Assert.That(sut.GameSeconds, Is.GreaterThan(frozenGameSeconds), "После разморозки GameSeconds должен снова расти.");
        }

        [UnityTest]
        public System.Collections.IEnumerator DebugTimeService_ChangingOffsetWhileFrozen_DoesNotAffectFrozenValues()
        {
            var sut = new DebugTimeService();
            sut.SetOffset(TimeSpan.FromSeconds(1));

            sut.Freeze(true);

            var now1 = sut.Now;
            var utc1 = sut.UtcNow;
            var gs1 = sut.GameSeconds;

            sut.SetOffset(TimeSpan.FromSeconds(999));

            yield return new WaitForSeconds(0.1f);

            Assert.AreEqual(now1, sut.Now);
            Assert.AreEqual(utc1, sut.UtcNow);
            Assert.AreEqual(gs1, sut.GameSeconds);
        }
    }
}