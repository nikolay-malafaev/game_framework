using System.Collections.Generic;
using UnityEngine;

namespace TerraPlaySpace.MF.Runtime.UI.RadioButton
{
    public class RadioButtonGroup : MonoBehaviour
    {
        [SerializeField] private Mode _mode = Mode.Radio;
        private HashSet<IRadioButton> _registeredButtons = new();
        private IRadioButton _lastActiveButton;
        
        private void OnDestroy()
        {
            foreach (var button in _registeredButtons)
            {
                button.OnButtonClicked -= HandleButtonClick;
            }
            _registeredButtons.Clear();
        }
        
        public void RegisterButton(IRadioButton button, bool isToggledOnStart)
        {
            button.OnButtonClicked += HandleButtonClick;
            button.SetActiveButton(false);
            if (isToggledOnStart)
            {
                HandleButtonClick(button);
            }
            _registeredButtons.Add(button);
        }
        
        private void HandleButtonClick(IRadioButton clickedButton)
        {
            var newState = true;

            if (_mode == Mode.Toggle)
            {
                newState = !clickedButton.IsActiveButton;
            }
        
            if (_mode == Mode.Radio && _lastActiveButton != null)
            {
                _lastActiveButton.SetActiveButton(false);
            }

            clickedButton.SetActiveButton(newState);
            _lastActiveButton = clickedButton;
        }
        
        private enum Mode
        {
            Radio,
            Toggle
        }
    }
}