using System;
using Object = UnityEngine.Object;

namespace GameFramework.Verification
{
    public class ProductionVerificationContext : IVerificationContext
    {
        public bool Verify(bool condition, string message = null, Object context = null, 
            string sourceFilePath = "", 
            int sourceLineNumber = 0, 
            string memberName = "")
        {
            if (condition)
            {
                return true;
            }
            string formatMessage = VerificationUtils.FormatMessage(message, context, sourceFilePath, sourceLineNumber, memberName);
            UnityEngine.Debug.unityLogger.Log(UnityEngine.LogType.Exception, "Verification", formatMessage, context);
            VerificationAnalytics.SendEvent();
            return false;
        }

        public bool Verify(Func<bool> condition, string message = null, Object context = null, 
            string sourceFilePath = "", 
            int sourceLineNumber = 0, 
            string memberName = "")
        {
            if (condition())
            {
                return true;
            }
            string formatMessage = VerificationUtils.FormatMessage(message, context, sourceFilePath, sourceLineNumber, memberName);
            UnityEngine.Debug.unityLogger.Log(UnityEngine.LogType.Exception, "Verification", formatMessage, context);
            VerificationAnalytics.SendEvent();
            return false;
        }
    }
}