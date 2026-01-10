using System;

namespace GameFramework.Verification.NativeInterface
{
    public static class AlertDialog
    {
        private static readonly IAlertDialogPresenter Presenter = GetPresenter();
        
        public static void Show(string title, string message, 
            string positiveButtonText, string negativeButtonText,
            Action onPositiveResult = null, Action onNegativeResult = null, Action onCloseAlert = null)
        {
            if (Presenter == null)
            {
                throw new NativeInterfaceNotSupportedException();
            }
            Presenter.Show(title, message, 
                positiveButtonText, negativeButtonText, 
                onPositiveResult, onNegativeResult, onCloseAlert);
        }
        
        private static IAlertDialogPresenter GetPresenter()
        {
#if UNITY_EDITOR
            return new EditorAlertDialogPresenter();
#elif UNITY_ANDROID
            return new AndroidAlertDialogPresenter();
#elif UNITY_IOS
            return null; // todo add for ios
#elif UNITY_WEBGL
            return new WebGLAlertDialogPresenter();
#elif UNITY_STANDALONE_WIN
            return new WindowsAlertDialogPresenter();
#elif UNITY_STANDALONE_OSX
            return null; // todo add for mac os
#else
            return null;
#endif
        }
    }
}