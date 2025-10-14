using NUnit.Framework;
using UnityEngine;

namespace GameFramework.Verification.Tests
{
    public class VerificationInspectorTests
    {
        [Test]
        public void VerifyCondition_ValidCondition_ShouldReturnTrue()
        {
            bool conditional = true;
            bool result = VerificationInspector.Verify(conditional);
            Assert.IsTrue(result);
        }
        
        [Test]
        public void VerifyCondition_InvalidCondition_ShouldReturnFalseAndLogError()
        {
            bool conditional = false;
            bool result = VerificationInspector.Verify(conditional);
            Assert.IsFalse(result);
            
            GameObject gameObject = new GameObject();
            VerificationInspector.Verify(gameObject);
            
        }
    }
}