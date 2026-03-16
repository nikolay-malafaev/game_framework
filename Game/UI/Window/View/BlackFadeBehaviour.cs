using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.Window
{
    [RequireComponent(typeof(Button))]
    public class BlackFadeBehaviour : MonoBehaviour
    {
        public event Action OnClick;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                OnClick?.Invoke();
            });
        }
    }
}
