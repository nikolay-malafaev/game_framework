using System;
using Object = UnityEngine.Object;

namespace GameFramework.Verification
{
    public class DebugVerificationContext : IVerificationContext
    {
        private bool _isWindowSkipped = false;
        
        public bool Verify(bool condition, string message = null, Object context = null, 
            string sourceFilePath = "",
            int sourceLineNumber = 0, 
            string memberName = "")
        {
            if (condition)
            {
                return true;
            }
            OnVerificationFailed(message, context, sourceFilePath, sourceLineNumber, memberName);
            return false;
        }

        public bool Verify(Func<bool> condition, string message = null, Object context = null, string sourceFilePath = "",
            int sourceLineNumber = 0, string memberName = "")
        {
            if (condition())
            {
                return true;
            }
            OnVerificationFailed(message, context, sourceFilePath, sourceLineNumber, memberName);
            return false;
        }

        private void OnVerificationFailed(string message, Object context, 
            string sourceFilePath,
            int sourceLineNumber, 
            string memberName)
        {
            string formatMessage = VerificationUtils.FormatMessage(message, context, sourceFilePath, sourceLineNumber, memberName);
            UnityEngine.Debug.unityLogger.Log(UnityEngine.LogType.Exception, "Verification", formatMessage, context);
            if (!_isWindowSkipped)
            {
                UnityEngine.Time.timeScale = 0;
                VerificationWindow.Show(formatMessage, null, () => _isWindowSkipped = true);
            }
        }
    }
}