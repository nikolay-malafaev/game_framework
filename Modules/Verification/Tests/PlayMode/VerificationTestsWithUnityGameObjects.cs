using NUnit.Framework;
using UnityEngine;

namespace GameFramework.Verification.Tests
{
    public class VerificationTestsWithUnityGameObjects
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            VerificationContainer.SetContext(new TestVerificationContext());
        }
        
        [OneTimeTearDown]
        public void TearDown()
        {
            VerificationContainer.ClearContext();
        }
        
        [Test]
        public void Verify_ValidGameObject_ShouldReturnTrue()
        {
            GameObject gameObject = new GameObject()
            {
                name = "test"
            };
            var result = VerificationContainer.Context.Verify(gameObject);
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Verify_InvalidGameObject_ShouldReturnFalseAndLogError()
        {
            GameObject gameObject = null;
            var result = VerificationContainer.Context.Verify(gameObject);
            Assert.IsFalse(result);
        }
    }
}