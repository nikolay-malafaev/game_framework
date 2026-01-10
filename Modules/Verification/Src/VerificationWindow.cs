using System;

namespace GameFramework.Verification
{
    internal static class VerificationWindow
    {
        public static void Show(string message, Action onOkClick = null, Action onSkipClick = null)
        {
            OnShowWindow();
            NativeInterface.AlertDialog.Show("Verification failed", message, 
                "Ok", "Skip All", 
                onOkClick, onSkipClick, OnCloseWindow);
        }
        
        private static void OnShowWindow()
        {
            UnityEngine.Time.timeScale = 0;
        }

        private static void OnCloseWindow()
        {
            UnityEngine.Time.timeScale = 1;
        }
    }
}