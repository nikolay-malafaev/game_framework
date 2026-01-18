using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace GameFramework.Time.Tests
{
    public class SchedulerTests
    {
        private Scheduler _scheduler;

        [SetUp]
        public void SetUp()
        {
            _scheduler = new Scheduler();
        }

        [TearDown]
        public void TearDown()
        {
            _scheduler?.Dispose();
            _scheduler = null;
        }

        [Test]
        public void Schedule_Duration_RegistersCallback_AndInvokesAction_OnFinish()
        {
            int calls = 0;
            _scheduler.Schedule(() => calls++, 1f);

            var callbacks = GetInternalCallbacks(_scheduler);
            Assert.That(callbacks.Count, Is.EqualTo(1), "Должен зарегистрироваться 1 внутренний callback.");

            var timer = callbacks.Keys.Single();

            SimulateFinish(_scheduler, timer);

            Assert.That(calls, Is.EqualTo(1), "Action должен быть вызван ровно 1 раз.");
            Assert.That(GetInternalCallbacks(_scheduler).Count, Is.EqualTo(0), "После завершения callback должен быть удалён из словаря.");
        }

        [Test]
        public void Schedule_DateTime_RegistersCallback_AndInvokesAction_OnFinish()
        {
            int calls = 0;
            _scheduler.Schedule(() => calls++, DateTime.UtcNow.AddSeconds(10));

            var callbacks = GetInternalCallbacks(_scheduler);
            Assert.That(callbacks.Count, Is.EqualTo(1));

            var timer = callbacks.Keys.Single();

            SimulateFinish(_scheduler, timer);

            Assert.That(calls, Is.EqualTo(1));
            Assert.That(GetInternalCallbacks(_scheduler).Count, Is.EqualTo(0));
        }

        [Test]
        public void Schedule_ReusesTimer_AfterFinish()
        {
            _scheduler.Schedule(() => { }, 1f);

            var pool = GetTimersPool(_scheduler);
            var callbacks = GetInternalCallbacks(_scheduler);
            Assert.That(pool.Count, Is.EqualTo(1));
            Assert.That(callbacks.Count, Is.EqualTo(1));

            var timer = callbacks.Keys.Single();

            SimulateFinish(_scheduler, timer);

            Assert.That(GetInternalCallbacks(_scheduler).Count, Is.EqualTo(0));
            Assert.That(GetTimersPool(_scheduler).Count, Is.EqualTo(1), "Пул не должен очищаться — таймер остаётся для повторного использования.");

            _scheduler.Schedule(() => { }, 1f);

            Assert.That(GetTimersPool(_scheduler).Count, Is.EqualTo(1), "После повторного Schedule пул не должен увеличиться.");
            Assert.That(GetInternalCallbacks(_scheduler).Count, Is.EqualTo(1));
        }

        [Test]
        public void Schedule_AllocatesSecondTimer_WhenFirstNotFinished()
        {
            _scheduler.Schedule(() => { }, 1f); // таймер занят
            Assert.That(GetTimersPool(_scheduler).Count, Is.EqualTo(1));
            Assert.That(GetInternalCallbacks(_scheduler).Count, Is.EqualTo(1));

            _scheduler.Schedule(() => { }, 1f);

            Assert.That(GetTimersPool(_scheduler).Count, Is.EqualTo(2), "Должен быть создан второй таймер, если первый ещё занят.");
            Assert.That(GetInternalCallbacks(_scheduler).Count, Is.EqualTo(2), "Должно быть 2 активных callback (по одному на таймер).");
        }

        [Test]
        public void Finish_IsIdempotent_ActionInvokedOnlyOnce_AndCallbackRemoved()
        {
            int calls = 0;
            _scheduler.Schedule(() => calls++, 1f);

            var callbacks = GetInternalCallbacks(_scheduler);
            var timer = callbacks.Keys.Single();

            SimulateFinish(_scheduler, timer);

            var callbacksAfter = GetInternalCallbacks(_scheduler);

            Assert.That(calls, Is.EqualTo(1), "Action должен быть вызван только один раз.");
            Assert.That(callbacksAfter.ContainsKey(timer), Is.False, "Callback должен быть удалён после первого завершения.");
        }

        [Test]
        public void Schedule_AllowsNullAction_FinishDoesNotThrow()
        {
            _scheduler.Schedule(null, 1f);

            var callbacks = GetInternalCallbacks(_scheduler);
            Assert.That(callbacks.Count, Is.EqualTo(1));
            var timer = callbacks.Keys.Single();

            Assert.DoesNotThrow(() => SimulateFinish(_scheduler, timer));
            Assert.That(GetInternalCallbacks(_scheduler).Count, Is.EqualTo(0));
        }

        // ----------------- Helpers (Reflection) -----------------

        private static List<CountdownTimer> GetTimersPool(Scheduler scheduler)
        {
            var field = typeof(Scheduler).GetField("_timersPool", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field, "Не найдено поле _timersPool (возможны изменения реализации).");

            var value = field.GetValue(scheduler);
            Assert.NotNull(value);

            return (List<CountdownTimer>)value;
        }

        private static Dictionary<CountdownTimer, Action> GetInternalCallbacks(Scheduler scheduler)
        {
            var field = typeof(Scheduler).GetField("_internalCallbacks", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field, "Не найдено поле _internalCallbacks (возможны изменения реализации).");

            var value = field.GetValue(scheduler);
            Assert.NotNull(value);

            return (Dictionary<CountdownTimer, Action>)value;
        }

        private static void SimulateFinish(Scheduler scheduler, CountdownTimer timer)
        {
            var callbacks = GetInternalCallbacks(scheduler);

            Assert.That(callbacks.ContainsKey(timer), Is.True, "Для таймера должен существовать внутренний callback.");
            var internalCallback = callbacks[timer];

            internalCallback.Invoke();
        }
    }
}
