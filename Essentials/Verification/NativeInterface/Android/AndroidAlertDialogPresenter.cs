using System;
using GameFramework.Verification.NativeInterface;
using UnityEngine;

namespace GameFramework.Verification.NativeInterface
{
    public class AndroidAlertDialogPresenter : IAlertDialogPresenter
    {
        // ReSharper disable once StringLiteralTypo
        private static readonly AndroidJavaClass alertDialogWrapperClass; //=
        //    new("com.terraplayspace.alertdialog.AlertDialogUnityWrapper");

        public void Show(string title, string message, string positiveButtonText, string negativeButtonText,
            Action onPositiveResult, Action onNegativeResult, Action onCloseAlert)
        {
            var unityActivity = AndroidUtilities.GetUnityActivity();

            var positiveListener = new AlertDialogButtonClickListener(onPositiveResult);
            var negativeListener = new AlertDialogButtonClickListener(onNegativeResult);

            alertDialogWrapperClass.CallStatic(
                "showDialog",
                unityActivity,
                title,
                message,
                positiveButtonText,
                negativeButtonText,
                positiveListener,
                negativeListener
            );
        }
        
        private class ClickListener : AndroidJavaProxy
        {
            private readonly Action<bool> _onResult; // true=OK, false=Cancel
            public ClickListener(Action<bool> onResult)
                : base("android.content.DialogInterface$OnClickListener") => _onResult = onResult;

            // void onClick(DialogInterface dialog, int which)
            void onClick(AndroidJavaObject dialog, int which)
            {
                // -1 = BUTTON_POSITIVE, -2 = BUTTON_NEGATIVE
                _onResult?.Invoke(which == -1);
            }
        }
        
        public static void Show(string title, string message, Action onOk = null, Action onCancel = null, bool cancelable = true)
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                using (var builder = new AndroidJavaObject("android.app.AlertDialog$Builder", activity))
                {
                    var listener = new ClickListener(isOk =>
                    {
                        if (isOk) onOk?.Invoke();
                        else onCancel?.Invoke();
                    });

                    builder.Call<AndroidJavaObject>("setTitle", title);
                    builder.Call<AndroidJavaObject>("setMessage", message);
                    builder.Call<AndroidJavaObject>("setCancelable", cancelable);
                    builder.Call<AndroidJavaObject>("setPositiveButton", "OK", listener);
                    builder.Call<AndroidJavaObject>("setNegativeButton", "Cancel", listener);

                    using (var dialog = builder.Call<AndroidJavaObject>("create"))
                    {
                        dialog.Call("show");
                    }
                }
            }));
        }
    }
}