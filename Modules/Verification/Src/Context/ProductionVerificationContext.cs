using System;
using GameFramework.Logging;
using Object = UnityEngine.Object;

namespace GameFramework.Verification
{
    [Loggable("Verification")]
    public partial class ProductionVerificationContext : IVerificationContext
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
            string logMessage = VerificationUtils.FormatLogMessage(message, sourceFilePath, sourceLineNumber, memberName);
            LogError(logMessage, context);
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
            string logMessage = VerificationUtils.FormatLogMessage(message, sourceFilePath, sourceLineNumber, memberName);
            LogError(logMessage, context);
            VerificationAnalytics.SendEvent();
            return false;
        }
    }
}