using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;
#pragma warning disable CS0184 // 'is' expression's given expression is never of the provided type

namespace GameFramework.StaticData.Tests
{
    public class LoadByLabelAsync : AddressableServiceTestsBase
    {
        [UnityTest]
        public IEnumerator LoadByLabelAsync_ValidLabel_LoadsMatchingAssets() => UniTask.ToCoroutine(async () =>
        {
            var assets = await _service.LoadByLabelAsync<Object>(ValidLabel);

            Assert.IsNotNull(assets);
            Assert.GreaterOrEqual(assets.Length, 1);
            Assert.IsTrue(assets.Any(a => a is ScriptableObject));

            await _service.ReleaseByLabel<Object>(ValidLabel);
        });

        [UnityTest]
        public IEnumerator LoadByLabelAsync_ValidLabelSpecificType_LoadsOnlyMatchingType() => UniTask.ToCoroutine(
            async () =>
            {
                var assets = await _service.LoadByLabelAsync<ScriptableObject>(ValidLabel);
        
                Assert.IsNotNull(assets);
                Assert.GreaterOrEqual(assets.Length, 1);
                // ReSharper disable once ConvertTypeCheckToNullCheck
                Assert.IsTrue(assets.All(a => a is ScriptableObject));
                Assert.IsFalse(assets.Any(a => a is GameObject));
        
                await _service.ReleaseByLabel<ScriptableObject>(ValidLabel);
            });
        
        
        [UnityTest]
        public IEnumerator LoadByLabelAsync_InvalidLabel_ReturnsEmptyArray() => UniTask.ToCoroutine(async () =>
        {
            LogAssert.Expect(LogType.Warning,
                new Regex(@"No assets found for label 'InvalidLabel' and type (UnityEngine\.)?Object\."));
            
            var assets = await _service.LoadByLabelAsync<Object>(InvalidLabel);

            await UniTask.NextFrame();
            await UniTask.NextFrame();
        
            Assert.IsNotNull(assets);
            Assert.AreEqual(0, assets.Length);
        });
        
        [UnityTest]
        public IEnumerator LoadByLabelAsync_NullLabel_ThrowsArgumentException() => UniTask.ToCoroutine(async () =>
        {
            try
            {
                await _service.LoadByLabelAsync<Object>(null);
                Assert.Fail("Expected ArgumentException was not thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("label", ex.ParamName);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected ArgumentException but got {ex.GetType()}");
            }
        });
        
        [UnityTest]
        public IEnumerator LoadByLabelAsync_EmptyLabel_ThrowsArgumentException() => UniTask.ToCoroutine(async () =>
        {
            try
            {
                await _service.LoadByLabelAsync<Object>(string.Empty);
                Assert.Fail("Expected ArgumentException was not thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("label", ex.ParamName);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected ArgumentException but got {ex.GetType()}");
            }
        });
    }
}