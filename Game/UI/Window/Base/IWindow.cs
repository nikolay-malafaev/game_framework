using System;

namespace GameFramework.UI.Window
{
    public interface IWindow
    {
        string Id { get; }
        WindowState State { get; }
        void Open();
        void Open(Action onOpen, params IWindowParameter[] parameters);
        void Close();
        void Close(Action onClose, params IWindowParameter[] parameters);
        void Show();
        void Show(Action onShow, params IWindowParameter[] parameters);
        void Hide();
        void Hide(Action onHide, params IWindowParameter[] parameters);
    }
}
