using System;
using Object = UnityEngine.Object;

namespace GameFramework.Verification
{
    public class TestVerificationContext : IVerificationContext
    {
        public bool Verify(bool condition, string message = null, Object context = null, string sourceFilePath = "",
            int sourceLineNumber = 0, string memberName = "")
        {
            return condition;
        }

        public bool Verify(Func<bool> condition, string message = null, Object context = null, string sourceFilePath = "",
            int sourceLineNumber = 0, string memberName = "")
        {
            return condition();
        }
    }
}