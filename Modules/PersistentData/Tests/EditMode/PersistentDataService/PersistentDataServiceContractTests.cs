using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameFramework.PersistentData.Tests
{
    /// <summary>
    /// Контрактные тесты для любой реализации IPersistentDataService.
    /// Наследник обязан предоставить экземпляр сервиса.
    /// </summary>
    public abstract class PersistentDataServiceContractTests
    {
        private readonly List<string> _keysToCleanup = new();

        /// <summary>
        /// Переопредели и верни готовый к работе сервис.
        /// Если сервис требует async-инициализации — переопредели CreateServiceAsync().
        /// </summary>
        protected virtual IPersistentDataService CreateService()
        {
            throw new NotImplementedException("Override CreateService() or CreateServiceAsync().");
        }

        protected virtual UniTask<IPersistentDataService> CreateServiceAsync()
        {
            return UniTask.FromResult(CreateService());
        }

        [TearDown]
        public void TearDown()
        {
            // Чистим ключи "best effort": даже если тест упал, стараемся удалить.
            IPersistentDataService service;
            try
            {
                service = CreateService();
            }
            catch
            {
                // Если сервис невозможно создать синхронно — просто пропускаем cleanup.
                _keysToCleanup.Clear();
                return;
            }

            foreach (var key in _keysToCleanup)
            {
                try { service.Delete(key); }
                catch { /* ignore */ }
            }

            _keysToCleanup.Clear();
        }

        private string NewKey(string suffix = null)
        {
            var key = $"tests.persistent.{GetType().Name}.{Guid.NewGuid():N}";
            if (!string.IsNullOrWhiteSpace(suffix))
                key += "." + suffix;

            _keysToCleanup.Add(key);
            return key;
        }

        [UnityTest]
        public System.Collections.IEnumerator Exists_WhenMissing_ReturnsFalse()
        {
            return UniTask.ToCoroutine(async () =>
            {
                var service = await CreateServiceAsync();
                Assert.NotNull(service);

                var key = NewKey("missing");
                Assert.IsFalse(service.Exists(key));
            });
        }

        [UnityTest]
        public System.Collections.IEnumerator Load_WhenMissing_ReturnsFalseAndDefaultValue()
        {
            LogAssert.Expect(LogType.Exception, new Regex("FileNotFoundException"));
            
            return UniTask.ToCoroutine(async () =>
            {
                var service = await CreateServiceAsync();
                Assert.NotNull(service);

                var key = NewKey("load.missing");

                var (found, value) = await service.Load(key, defaultValue: 123);
                
                await UniTask.Yield();
                
                Assert.IsFalse(found);
                Assert.AreEqual(123, value);
                
                LogAssert.NoUnexpectedReceived();
            });
        }

        [UnityTest]
        public System.Collections.IEnumerator Save_Then_Exists_IsTrue()
        {
            return UniTask.ToCoroutine(async () =>
            {
                var service = await CreateServiceAsync();
                Assert.NotNull(service);

                var key = NewKey("save.exists");

                var saved = await service.Save(key, 42);
                Assert.IsTrue(saved, "Save должен вернуть true для корректного кейса.");

                Assert.IsTrue(service.Exists(key), "Exists должен стать true после Save.");
            });
        }

        [UnityTest]
        public System.Collections.IEnumerator Save_Then_Load_ReturnsSavedValue_Int()
        {
            return UniTask.ToCoroutine(async () =>
            {
                var service = await CreateServiceAsync();
                Assert.NotNull(service);

                var key = NewKey("roundtrip.int");

                var saved = await service.Save(key, 42);
                Assert.IsTrue(saved);

                var (found, value) = await service.Load(key, defaultValue: -1);
                Assert.IsTrue(found);
                Assert.AreEqual(42, value);
            });
        }

        [UnityTest]
        public System.Collections.IEnumerator Save_Then_Load_ReturnsSavedValue_String()
        {
            return UniTask.ToCoroutine(async () =>
            {
                var service = await CreateServiceAsync();
                Assert.NotNull(service);

                var key = NewKey("roundtrip.string");

                var saved = await service.Save(key, "hello");
                Assert.IsTrue(saved);

                var (found, value) = await service.Load(key, defaultValue: "default");
                Assert.IsTrue(found);
                Assert.AreEqual("hello", value);
            });
        }

        [UnityTest]
        public System.Collections.IEnumerator Save_Overwrite_SameKey_LoadReturnsLatest()
        {
            return UniTask.ToCoroutine(async () =>
            {
                var service = await CreateServiceAsync();
                Assert.NotNull(service);

                var key = NewKey("overwrite");

                Assert.IsTrue(await service.Save(key, 1));
                Assert.IsTrue(await service.Save(key, 2));

                var (found, value) = await service.Load(key, defaultValue: -1);
                Assert.IsTrue(found);
                Assert.AreEqual(2, value);
            });
        }

        [UnityTest]
        public System.Collections.IEnumerator Delete_RemovesKey_ExistsFalse_LoadReturnsDefault()
        {
            LogAssert.Expect(LogType.Exception, new Regex("FileNotFoundException"));
            
            return UniTask.ToCoroutine(async () =>
            {
                var service = await CreateServiceAsync();
                Assert.NotNull(service);

                var key = NewKey("delete");

                Assert.IsTrue(await service.Save(key, 99));
                Assert.IsTrue(service.Exists(key));

                Assert.DoesNotThrow(() => service.Delete(key));

                Assert.IsFalse(service.Exists(key));

                var (found, value) = await service.Load(key, defaultValue: 777);
                
                await UniTask.Yield();
                
                Assert.IsFalse(found);
                Assert.AreEqual(777, value);
                
                LogAssert.NoUnexpectedReceived();
            });
        }

        [UnityTest]
        public System.Collections.IEnumerator Delete_WhenMissing_DoesNotThrow()
        {
            return UniTask.ToCoroutine(async () =>
            {
                var service = await CreateServiceAsync();
                Assert.NotNull(service);

                var key = NewKey("delete.missing");
                Assert.DoesNotThrow(() => service.Delete(key));
            });
        }

        [UnityTest]
        public System.Collections.IEnumerator Save_Then_Load_Roundtrip_CustomPayload()
        {
            return UniTask.ToCoroutine(async () =>
            {
                var service = await CreateServiceAsync();
                Assert.NotNull(service);

                var key = NewKey("roundtrip.payload");

                var payload = new TestPayload(7, "abc");
                Assert.IsTrue(await service.Save(key, payload));

                var (found, loaded) = await service.Load(key, defaultValue: TestPayload.Empty);
                Assert.IsTrue(found);
                Assert.AreEqual(payload, loaded);
            });
        }

        [Serializable]
        private sealed class TestPayload : IEquatable<TestPayload>
        {
            public static readonly TestPayload Empty = new(0, string.Empty);

            public int A;
            public string B;

            public TestPayload(int a, string b)
            {
                A = a;
                B = b;
            }

            public bool Equals(TestPayload other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return A == other.A && string.Equals(B, other.B, StringComparison.Ordinal);
            }

            public override bool Equals(object obj) => Equals(obj as TestPayload);

            public override int GetHashCode()
            {
                unchecked
                {
                    return (A * 397) ^ (B != null ? StringComparer.Ordinal.GetHashCode(B) : 0);
                }
            }
        }
    }
}
