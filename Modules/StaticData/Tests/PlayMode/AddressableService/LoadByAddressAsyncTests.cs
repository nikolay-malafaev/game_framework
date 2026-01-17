using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace GameFramework.StaticData.Tests
{
    public class LoadByAddressAsyncTests : AddressableServiceTestsBase
    {
        [UnityTest]
        public IEnumerator ValidAddress_LoadsAsset() => UniTask.ToCoroutine(async () =>
        {
            var asset = await _service.LoadByAddressAsync<ScriptableObject>(ValidAddressSO);

            Assert.IsNotNull(asset);
            Assert.IsInstanceOf<ScriptableObject>(asset);

            _service.ReleaseByAddress(ValidAddressSO);
        });

        [UnityTest]
        public IEnumerator NullAddress_ThrowsArgumentException() => UniTask.ToCoroutine(async () =>
        {
            IEnumerable<string> assetAddresses = null;

            try
            {
                await _service.LoadByAddressAsync<ScriptableObject>(assetAddresses);
                Assert.Fail("Expected NullReferenceException was not thrown");
            }
            catch (NullReferenceException ex)
            {
                Assert.Pass($"Expected NullReferenceException was caught. Error: {ex}");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected NullReferenceException but got {ex.GetType().Name}: {ex.Message}");
            }
        });

        [UnityTest]
        public IEnumerator EmptyAddress_ThrowsArgumentException() => UniTask.ToCoroutine(async () =>
        {
            try
            {
                await _service.LoadByAddressAsync<ScriptableObject>(string.Empty);
                Assert.Fail("Expected ArgumentException was not thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("assetAddress", ex.ParamName);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected ArgumentException but got {ex.GetType()}");
            }
        });

        [UnityTest]
        public IEnumerator InvalidAddress_ThrowsException() => UniTask.ToCoroutine(async () =>
        {
            var previousIgnore = LogAssert.ignoreFailingMessages;
            LogAssert.ignoreFailingMessages = true;

            try
            {
                Exception caughtException = null;

                try
                {
                    await _service.LoadByAddressAsync<ScriptableObject>(InvalidAddress);
                    Assert.Fail($"LoadByAddressAsync должен был выбросить исключение для адреса '{InvalidAddress}'");
                }
                catch (Exception ex)
                {
                    caughtException = ex;
                }

                Assert.IsNotNull(caughtException, "Исключение не было поймано");

                StringAssert.Contains($"Failed to load asset from address {InvalidAddress}", caughtException.Message);

                var invalidKey = FindInnerException<InvalidKeyException>(caughtException);
                Assert.IsNotNull(invalidKey, "Ожидали InvalidKeyException где-то внутри цепочки InnerException");
            }
            finally
            {
                LogAssert.ignoreFailingMessages = previousIgnore;
            }

            await UniTask.NextFrame();
        });

        [UnityTest]
        public IEnumerator LoadSameAddressTwice_ReturnsSameInstance() => UniTask.ToCoroutine(async () =>
        {
            var asset1 = await _service.LoadByAddressAsync<ScriptableObject>(ValidAddressSO);
            var asset2 = await _service.LoadByAddressAsync<ScriptableObject>(ValidAddressSO);

            Assert.IsNotNull(asset1);
            Assert.AreSame(asset1, asset2);

            _service.ReleaseByAddress(ValidAddressSO);
        });

        [UnityTest]
        public IEnumerator LoadWrongType_ThrowsException() => UniTask.ToCoroutine(async () =>
        {
            LogAssert.Expect(
                LogType.Error,
                new Regex(@"InvalidKeyException:.*not assignable.*requested Type=UnityEngine\.GameObject",
                    RegexOptions.Singleline)
            );

            try
            {
                await _service.LoadByAddressAsync<GameObject>(ValidAddressSO);
                Assert.Fail("Expected an exception for wrong asset type, but none was thrown.");
            }
            catch (Exception ex)
            {
                Exception root = ex;
                while (root.InnerException != null)
                    root = root.InnerException;

                Assert.IsInstanceOf<InvalidKeyException>(root);
                StringAssert.Contains("not assignable", root.Message);
            }
            finally
            {
                _service.ReleaseByAddress(ValidAddressSO);
            }
        });

        [UnityTest]
        public IEnumerator MultipleValidAddresses_LoadsAllAssets() => UniTask.ToCoroutine(async () =>
        {
            var addresses = new List<string> { ValidAddressSO, ValidAddressPrefab };
            var assets = await _service.LoadByAddressAsync<UnityEngine.Object>(addresses);

            Assert.IsNotNull(assets);
            Assert.AreEqual(2, assets.Length);
            Assert.IsInstanceOf<ScriptableObject>(assets.First(a => a is ScriptableObject));
            Assert.IsInstanceOf<GameObject>(assets.First(a => a is GameObject));

            _service.ReleaseByAddress(ValidAddressSO);
            _service.ReleaseByAddress(ValidAddressPrefab);
        });

        [UnityTest]
        public IEnumerator EmptyAddressList_ReturnsEmptyArray() => UniTask.ToCoroutine(async () =>
        {
            var addresses = new List<string>();
            var assets = await _service.LoadByAddressAsync<UnityEngine.Object>(addresses);

            Assert.IsNotNull(assets);
            Assert.AreEqual(0, assets.Length);
        });

        [UnityTest]
        public IEnumerator ListWithValidNullEmptyInvalid_LoadsValidAssets() => UniTask.ToCoroutine(async () =>
        {
            var addresses = new List<string> { ValidAddressSO, null, "", InvalidAddress, ValidAddressPrefab };

            LogAssert.Expect(LogType.Error, new Regex(@"Skipping null or empty address in batch load by address\."));
            LogAssert.Expect(LogType.Error, new Regex(@"Skipping null or empty address in batch load by address\."));

            LogAssert.Expect(LogType.Error,
                new Regex(@"InvalidKeyException.*No Location found for Key=.*InvalidAddress"));

            try
            {
                var assets = await _service.LoadByAddressAsync<UnityEngine.Object>(addresses);
                Assert.Fail("Expected an exception due to InvalidAddress in the list.");
            }
            catch (Exception ex)
            {
                StringAssert.Contains($"Failed to load asset from address {InvalidAddress}", ex.Message);
            }
            finally
            {
                _service.ReleaseByAddress(ValidAddressSO);
                _service.ReleaseByAddress(ValidAddressPrefab);
                _service.ReleaseByAddress(null);
                _service.ReleaseByAddress("");
                _service.ReleaseByAddress(InvalidAddress);
            }
        });

        private static TException FindInnerException<TException>(Exception exception)
            where TException : Exception
        {
            var current = exception;
            while (current != null)
            {
                if (current is TException matched)
                    return matched;

                current = current.InnerException;
            }

            return null;
        }
    }
}