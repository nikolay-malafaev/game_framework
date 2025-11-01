namespace GameFramework.Verification
{
    public static class VerificationMessenger
    {
        public static void BroadcastMessage(string userMessage, UnityEngine.Object context, string sourceFilePath, int sourceLineNumber, string memberName)
        {
            string message = VerificationUtils.GetVerificationMessage(userMessage, context, sourceFilePath, sourceLineNumber, memberName);
            UnityEngine.Debug.unityLogger.Log(UnityEngine.LogType.Error, "Verification", message);
#if VERIFICATION_WINDOW_ENABLED
            ShowWindow(message);
#endif
        }

        private static void ShowWindow(string message)
        {
            if (!VerificationUtils.NeedShowAlertDialog || UnityEngine.Application.isBatchMode)
            {
                return;
            }
            
            UnityEngine.Time.timeScale = 0;
            
            NativeInterface.AlertDialog.Show("Verification failed!", message, "Ok", "Skip All", 
                null, OnNegativeClicked, OnCloseAlert);
        }
        
        private static void OnNegativeClicked()
        {
            VerificationUtils.OnClickSkipAll();
        }

        private static void OnCloseAlert()
        {
            UnityEngine.Time.timeScale = 1;
        }
    }
}