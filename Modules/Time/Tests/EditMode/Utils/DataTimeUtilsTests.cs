using System;
using GameFramework.Types;
using GameFramework.Utils;
using NUnit.Framework;

namespace GameFramework.Time.Tests
{
    public class DataTimeUtilsTests
    {
        [Test]
        public void GetPercentageBetweenDates_WhenTotalDurationNonPositive_ReturnsMinusOne()
        {
            var t0 = DateTime.UtcNow;
            var t1 = t0;
            var t2 = t0.AddSeconds(-1);

            Assert.AreEqual(-1, DataTimeUtils.GetPercentageBetweenDates(t0, t1));
            Assert.AreEqual(-1, DataTimeUtils.GetPercentageBetweenDates(t0, t2));
        }

        [Test]
        public void GetPercentageBetweenDates_WhenNotStartedYet_ReturnsZero()
        {
            var start = DateTime.UtcNow.AddSeconds(10);
            var end = start.AddSeconds(10);

            var pct = DataTimeUtils.GetPercentageBetweenDates(start, end);

            Assert.AreEqual(0, pct);
        }

        [Test]
        public void GetPercentageBetweenDates_WhenAlreadyFinished_ReturnsHundred()
        {
            // end в прошлом => elapsed > total => clamp к 100
            var end = DateTime.UtcNow.AddSeconds(-10);
            var start = end.AddSeconds(-10);

            var pct = DataTimeUtils.GetPercentageBetweenDates(start, end);

            Assert.AreEqual(100, pct);
        }

        [Test]
        public void GetPercentageBetweenDates_WhenHalfElapsed_IsAboutFifty()
        {
            var start = DateTime.UtcNow.AddSeconds(-5);
            var end = DateTime.UtcNow.AddSeconds(5);

            var pct = DataTimeUtils.GetPercentageBetweenDates(start, end);

            Assert.That(pct, Is.InRange(45, 55));
        }

        [Test]
        public void GetDateTimeByPercentage_PctInt_ProducesExpectedDate()
        {
            var start = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddSeconds(100);

            var dt0 = DataTimeUtils.GetDateTimeByPercentage(start, end, (pct_int)0);
            var dt50 = DataTimeUtils.GetDateTimeByPercentage(start, end, (pct_int)50);
            var dt100 = DataTimeUtils.GetDateTimeByPercentage(start, end, (pct_int)100);

            Assert.AreEqual(start, dt0);
            Assert.AreEqual(start.AddSeconds(50), dt50);
            Assert.AreEqual(end, dt100);
        }

        [Test]
        public void GetDateTimeByPercentage_NFloat_ProducesExpectedDate()
        {
            var start = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddSeconds(200);

            var dt0 = DataTimeUtils.GetDateTimeByPercentage(start, end, (n_float)0.0f);
            var dt50 = DataTimeUtils.GetDateTimeByPercentage(start, end, (n_float)0.5f);
            var dt100 = DataTimeUtils.GetDateTimeByPercentage(start, end, (n_float)1.0f);

            Assert.AreEqual(start, dt0);
            Assert.AreEqual(start.AddSeconds(100), dt50);
            Assert.AreEqual(end, dt100);
        }
    }
}