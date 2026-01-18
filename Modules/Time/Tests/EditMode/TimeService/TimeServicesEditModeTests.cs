using System;
using System.Threading;
using NUnit.Framework;

namespace GameFramework.Time.Tests
{
    public class TimeServicesEditModeTests
    {
        private static readonly TimeSpan AllowedNowSkew = TimeSpan.FromSeconds(1);

        [Test]
        public void ProductionTimeService_Now_CloseToSystemNow()
        {
            var sut = new ProductionTimeService();

            var systemNow = DateTime.Now;
            var serviceNow = sut.Now;

            Assert.That((serviceNow - systemNow).Duration(), Is.LessThan(AllowedNowSkew));
        }

        [Test]
        public void ProductionTimeService_UtcNow_CloseToSystemUtcNow()
        {
            var sut = new ProductionTimeService();

            var systemUtc = DateTime.UtcNow;
            var serviceUtc = sut.UtcNow;

            Assert.That((serviceUtc - systemUtc).Duration(), Is.LessThan(AllowedNowSkew));
        }

        [Test]
        public void ProductionTimeService_MonotonicTicks_Increases()
        {
            var sut = new ProductionTimeService();

            var t1 = sut.MonotonicTicks;
            Thread.Sleep(2);
            var t2 = sut.MonotonicTicks;

            Assert.That(t2, Is.GreaterThan(t1));
        }

        [Test]
        public void DebugTimeService_MonotonicTicks_Increases()
        {
            var sut = new DebugTimeService();

            var t1 = sut.MonotonicTicks;
            Thread.Sleep(2);
            var t2 = sut.MonotonicTicks;

            Assert.That(t2, Is.GreaterThan(t1));
        }

        [Test]
        public void DebugTimeService_OffsetAppliedToNowAndUtcNow()
        {
            var sut = new DebugTimeService();
            var offset = TimeSpan.FromMinutes(30);

            sut.SetOffset(offset);

            var systemNow = DateTime.Now;
            var systemUtc = DateTime.UtcNow;

            var nowDeltaSeconds = (sut.Now - systemNow).TotalSeconds;
            var utcDeltaSeconds = (sut.UtcNow - systemUtc).TotalSeconds;

            // Допускаем небольшой джиттер на выполнение кода
            const double toleranceSeconds = 0.25;

            Assert.That(nowDeltaSeconds, Is.InRange(offset.TotalSeconds - toleranceSeconds, offset.TotalSeconds + toleranceSeconds));
            Assert.That(utcDeltaSeconds, Is.InRange(offset.TotalSeconds - toleranceSeconds, offset.TotalSeconds + toleranceSeconds));
        }
    }
}
