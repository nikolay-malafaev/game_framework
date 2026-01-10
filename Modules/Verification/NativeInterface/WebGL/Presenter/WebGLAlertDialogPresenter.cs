using System;
using System.Runtime.InteropServices;

namespace GameFramework.Verification.NativeInterface
{
    public class WebGLAlertDialogPresenter : IAlertDialogPresenter
    {
        [DllImport("__Internal")] private static extern int ShowConfirm(string msg);
        
        public void Show(string title, string message, string positiveButtonText, string negativeButtonText, 
            Action onPositiveResult, Action onNegativeResult, Action onCloseAlert)
        {
            int result = ShowConfirm($"{title}\n\n{message}");
            if (result == 1)
            {
                onPositiveResult?.Invoke();
            }
            else
            {
                onNegativeResult?.Invoke();
            }
            onCloseAlert?.Invoke();
        }
    }
}