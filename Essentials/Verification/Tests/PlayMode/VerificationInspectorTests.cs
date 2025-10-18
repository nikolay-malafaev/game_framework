using NUnit.Framework;
using UnityEngine;

namespace GameFramework.Verification.Tests
{
    public class VerificationInspectorTests
    {
        [Test]
        public void Verify_ValidGameObject_ShouldReturnTrue()
        {
            GameObject gameObject = new GameObject()
            {
                name = "test"
            };
            var result = VerificationInspector.Verify(gameObject);
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Verify_InvalidGameObject_ShouldReturnFalseAndLogError()
        {
            GameObject gameObject = null;
            var result = VerificationInspector.Verify(gameObject);
            Assert.IsFalse(result);
            // todo log error
        }
    }
}