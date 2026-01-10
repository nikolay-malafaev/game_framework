using System;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameFramework.Verification.Tests
{
    public class GeneratedVerificationMethodsTests
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
        
        private MethodInfo GetMethod(Type type)
        {
            var arguments = new []
            {
                typeof(bool),                   // condition
                typeof(string),                 // userMessage
                typeof(UnityEngine.Object),     // context
                typeof(string),                 // sourceFilePath
                typeof(int),                    // sourceLineNumber
                typeof(string)                  // memberName
            };
            return type.GetMethod(
                "Verify", 
                BindingFlags.NonPublic | BindingFlags.Static,
                null,
                arguments,
                null);
        }
        
        private MethodInfo GetMethodWithOverloadDelegate(Type type)
        {
            var arguments = new []
            {
                typeof(Func<bool>),             // condition
                typeof(string),                 // userMessage
                typeof(UnityEngine.Object),     // context
                typeof(string),                 // sourceFilePath
                typeof(int),                    // sourceLineNumber
                typeof(string)                  // memberName
            };
            return type.GetMethod(
                "Verify", 
                BindingFlags.NonPublic | BindingFlags.Static,
                null,
                arguments,
                null);
        }
        
        [Test]
        public void Verify_WhenMethodExists_ShouldBeNotNull()
        {
            var method = GetMethod(typeof(StubVerifiableClass));
            Assert.NotNull(method);
        }

        [Test]
        public void VerifyWithOverloadDelegate_WhenMethodExists_ShouldBeNotNull()
        {
            var method = GetMethodWithOverloadDelegate(typeof(StubVerifiableClass));
            Assert.NotNull(method);
        }
        
        [Test]
        public void Verify_WhenMethodAbsent_ShouldBeNull()
        {
            var method = GetMethod(typeof(StubOrdinaryClass));
            Assert.Null(method);
        }
        
        [Test]
        public void VerifyWithOverloadDelegate_WhenMethodAbsent_ShouldBeNull()
        {
            var method = GetMethodWithOverloadDelegate(typeof(StubOrdinaryClass));
            Assert.Null(method);
        }
        
        [Test]
        public void Verify_ValidCondition_ShouldReturnTrue()
        {
            bool conditional = true;
            var stubClass = new StubVerifiableClass();
            var method = GetMethod(typeof(StubVerifiableClass));
            object result = method.Invoke(stubClass, new object[]
            {
                conditional,
                string.Empty,
                null,
                string.Empty,
                0,
                string.Empty
            } );
            Assert.IsTrue(result is bool);
            var resultBool = (bool) result;
            Assert.IsTrue(resultBool);
        }
        
        [Test]
        public void Verify_InvalidCondition_ShouldReturnFalse()
        {
            LogAssert.Expect(LogType.Error, new Regex(".*"));
            
            bool conditional = false;
            var stubClass = new StubVerifiableClass();
            var method = GetMethod(typeof(StubVerifiableClass));
            object result = method.Invoke(stubClass, new object[]
            {
                conditional,
                string.Empty,
                null,
                string.Empty,
                0,
                string.Empty
            } );
            Assert.IsTrue(result is bool);
            var resultBool = (bool) result;
            Assert.IsFalse(resultBool);
        }
        
        [Test]
        public void VerifyWithOverloadDelegate_ValidCondition_ShouldReturnTrue()
        {
            Func<bool> conditional = () => true;
            var stubClass = new StubVerifiableClass();
            var method = GetMethodWithOverloadDelegate(typeof(StubVerifiableClass));
            object result = method.Invoke(stubClass, new object[]
            {
                conditional,
                string.Empty,
                null,
                string.Empty,
                0,
                string.Empty
            } );
            Assert.IsTrue(result is bool);
            var resultBool = (bool) result;
            Assert.IsTrue(resultBool);
            
            LogAssert.NoUnexpectedReceived();
        }
        
        [Test]
        public void VerifyWithOverloadDelegate_InvalidCondition_ShouldReturnFalse()
        {
            LogAssert.Expect(LogType.Error, new Regex(".*"));
            
            Func<bool> conditional = () => false;
            var stubClass = new StubVerifiableClass();
            var method = GetMethodWithOverloadDelegate(typeof(StubVerifiableClass));
            object result = method.Invoke(stubClass, new object[]
            {
                conditional,
                string.Empty,
                null,
                string.Empty,
                0,
                string.Empty
            } );
            Assert.IsTrue(result is bool);
            var resultBool = (bool) result;
            Assert.IsFalse(resultBool);
            
            LogAssert.NoUnexpectedReceived();
        }
    }
}