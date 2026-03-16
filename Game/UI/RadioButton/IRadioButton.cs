using System;

namespace TerraPlaySpace.MF.Runtime.UI.RadioButton
{
    public interface IRadioButton
    {
        string Id { get; }
        bool IsActiveButton { get; }
        void SetActiveButton(bool state);
        event Action<IRadioButton> OnButtonClicked;
    }
}