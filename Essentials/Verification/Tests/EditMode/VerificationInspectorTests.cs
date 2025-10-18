using System;
using NUnit.Framework;

namespace GameFramework.Verification.Tests
{
    public class VerificationInspectorTests
    {
        [Test]
        public void Verify_ValidCondition_ShouldReturnTrue()
        {
            bool conditional = true;
            bool result = VerificationInspector.Verify(conditional);
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Verify_InvalidCondition_ShouldReturnFalseAndLogError()
        {
            bool conditional = false;
            bool result = VerificationInspector.Verify(conditional);
            Assert.IsFalse(result);
            // todo log error
        }
        
        [Test]
        public void VerifyWithOverloadDelegate_ValidCondition_ShouldReturnTrue()
        {
            Func<bool> conditional = () => true;
            bool result = VerificationInspector.Verify(conditional);
            Assert.IsTrue(result);
        }
        
        [Test]
        public void VerifyWithOverloadDelegate_InvalidCondition_ShouldReturnFalseAndLogError()
        {
            Func<bool> conditional = () => false;
            bool result = VerificationInspector.Verify(conditional);
            Assert.IsFalse(result);
            // todo log error
        }
    }
}