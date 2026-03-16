using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerraPlaySpace.MF.Runtime.UI.RadioButton
{
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class RadioButton : MonoBehaviour, IRadioButton
    {
        [SerializeField] private string _id;
        [SerializeField] private RadioButtonGroup _group;
        [SerializeField] private List<GameObject> _controlledObjects = new();
        [SerializeField] private bool _isToggledOnStart;

        [Header("States")]
        [SerializeField] private GameObject _active;
        [SerializeField] private GameObject _disable;
        
        private bool _isActiveButton = false;
        
        public string Id => _id;
        public bool IsActiveButton => _isActiveButton;
        
        public event Action<IRadioButton> OnButtonClicked;
        
        private void Start()
        {
            if (_group)
            {
                _group.RegisterButton(this, _isToggledOnStart);
            }
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClick);
        }

        public void SetActiveButton(bool state)
        {
            _isActiveButton = state;
            _controlledObjects.ForEach(obj => obj.SetActive(state));
            if (_active)
            {
                _active.SetActive(_isActiveButton);
            }
            if (_disable)
            {
                _disable.SetActive(!_isActiveButton);
            }
        }

        private void OnClick()
        {
            OnButtonClicked?.Invoke(this);
        }
    }
}