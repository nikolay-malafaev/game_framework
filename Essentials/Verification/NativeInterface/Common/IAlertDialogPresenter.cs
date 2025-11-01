using System;

namespace GameFramework.Verification.NativeInterface
{
    public interface IAlertDialogPresenter
    {
        void Show(string title, string message,
            string positiveButtonText, string negativeButtonText,
            Action onPositiveResult, Action onNegativeResult, Action onCloseAlert);
    }
}
