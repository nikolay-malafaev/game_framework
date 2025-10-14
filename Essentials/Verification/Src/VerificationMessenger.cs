namespace GameFramework.Verification
{
    public static class VerificationMessenger
    {
        public static void BroadcastVerifyMessage(string reason, string userMessage, UnityEngine.Object context, 
            string sourceFilePath, 
            int sourceLineNumber,
            string memberName)
        {
            string message = VerificationUtils.GetVerificationMessage(reason, userMessage, context, sourceFilePath, sourceLineNumber, memberName);
            // log
#if VERIFICATION_WINDOW_ENABLED
            ShowWindow(message);
#endif
        }

        private static void ShowWindow(string message)
        {
            if (!VerificationUtils.NeedShowAlertDialog)
            {
                return;
            }
            
            UnityEngine.Time.timeScale = 0;
            
            //NativeInterface.Presenter.ShowAlertDialog("Verification failed!", message, 
            //    "Ok", "Skip All",
            //    OnPositiveClicked, OnNegativeClicked);
        }
        
        private static void OnPositiveClicked()
        {
            UnityEngine.Time.timeScale = 1;
        }
        
        private static void OnNegativeClicked()
        {
            UnityEngine.Time.timeScale = 1;
            VerificationUtils.OnClickSkipAll();
        }
    }
}