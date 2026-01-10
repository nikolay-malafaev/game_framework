using System;
using GameFramework.Logging;
using Object = UnityEngine.Object;

namespace GameFramework.Verification
{
    [Loggable("Verification")]
    public partial class DebugVerificationContext : IVerificationContext
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
            string logMessage = VerificationUtils.FormatLogMessage(message, sourceFilePath, sourceLineNumber, memberName);
            LogError(logMessage, context);
            
            if (!_isWindowSkipped)
            {
                UnityEngine.Time.timeScale = 0;
                string windowMessage = VerificationUtils.FormatWindowMessage(message, sourceFilePath, sourceLineNumber, memberName);
                VerificationWindow.Show(windowMessage, null, () => _isWindowSkipped = true);
            }
        }
    }
}