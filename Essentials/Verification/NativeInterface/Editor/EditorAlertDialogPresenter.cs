using System;

namespace GameFramework.Verification.NativeInterface
{
    public class EditorAlertDialogPresenter : IAlertDialogPresenter
    {
        public void Show(string title, string message, string positiveButtonText, string negativeButtonText, 
            Action onPositiveResult, Action onNegativeResult, Action onCloseAlert)
        {
            bool continueExecution = UnityEditor.EditorUtility.DisplayDialog(
                title,
                message,
                positiveButtonText,
                negativeButtonText
            );
            if (continueExecution)
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