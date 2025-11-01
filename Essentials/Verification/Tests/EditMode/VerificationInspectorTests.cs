using System;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameFramework.Verification.Tests
{ 
    public class VerificationInspectorTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var type = Type.GetType(
                "GameFramework.Verification.VerificationUtils, GameFramework.Verification",
                throwOnError: true);

            var prop = type.GetProperty(
                "NeedShowAlertDialog",
                BindingFlags.Static | BindingFlags.Public);

            prop?.SetValue(null, false);
        }
        
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
            LogAssert.Expect(LogType.Error, new Regex(".*"));
            
            var result = VerificationInspector.Verify(false);
            Assert.IsFalse(result);
            
            LogAssert.NoUnexpectedReceived();
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
            LogAssert.Expect(LogType.Error, new Regex(".*"));
            
            Func<bool> conditional = () => false;
            bool result = VerificationInspector.Verify(conditional);
            Assert.IsFalse(result);
            
            LogAssert.NoUnexpectedReceived();
        }
    }
}