using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GameFramework.Verification.NativeInterface
{
    public class WindowsAlertDialogPresenter : IAlertDialogPresenter
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
        
        public void Show(string title, string message, string positiveButtonText, string negativeButtonText, 
            Action onPositiveResult, Action onNegativeResult, Action onCloseAlert)
        {
            var result = MessageBox(IntPtr.Zero, title, message, (uint) MessageBoxType.OKCancel);

            switch (result)
            {
                case 1:
                    onPositiveResult?.Invoke();
                    break;
                case 2:
                    onNegativeResult?.Invoke();
                    break;
            }
            onCloseAlert?.Invoke();
        }
        
        private enum MessageBoxType : uint
        {
            OK = 0x00000000,
            OKCancel = 0x00000001,
            YesNo = 0x00000004,
            Information = 0x00000040,
            Warning = 0x00000030,
            Error = 0x00000010
        }
    }
}