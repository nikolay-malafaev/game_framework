using System;
using UnityEngine;

namespace GameFramework.Verification.NativeInterface
{
    internal sealed class AlertDialogButtonClickListener : AndroidJavaProxy
    {
        private readonly Action _onClickCallback;
        
        public AlertDialogButtonClickListener(Action onClickCallback) : base("android.content.DialogInterface$OnClickListener")
        {
            _onClickCallback = onClickCallback;
        }
        
        // ReSharper disable once InconsistentNaming
        [UnityEngine.Scripting.Preserve]
        public void onClick(AndroidJavaObject dialog, int which) {
            _onClickCallback?.Invoke();
        }
    }
}