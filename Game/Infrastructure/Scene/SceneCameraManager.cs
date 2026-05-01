using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Infrastructure
{
    [Loggable]
    public partial class SceneCameraManager : SerializedMonoBehaviour
    {
        [SerializeField]
        private Dictionary<string, Camera> _cameras = new();

        [SerializeField]
        [ValueDropdown("GetCameraKeys")]
        private string _defaultCameraKey;

        private Camera _activeCamera;

        public event Action<Camera> OnCameraChanged;

        private void Start()
        {
            if (_cameras == null || _cameras.Count == 0) return;
            SwitchCamera(_defaultCameraKey);
        }

        public void SwitchCamera(string key)
        {
            if (!_cameras.ContainsKey(key))
            {
                LogError("Camera not found: " + key);
                return;
            }

            foreach (var (_, v) in _cameras)
            {
                v.enabled = false;
            }

            _cameras[key].enabled = true;
            _activeCamera = _cameras[key];
            OnCameraChanged?.Invoke(_activeCamera);
        }

        public Camera GetCamera(string key)
        {
            if (_cameras.ContainsKey(key))
            {
                return _cameras[key];
            }

            return null;
        }

        public Camera GetActiveCamera()
        {
            return _activeCamera;
        }
        
        private IEnumerable GetCameraKeys()
        {
            if (_cameras == null || _cameras.Count == 0)
            {
                return Enumerable.Empty<string>();
            }

            return _cameras.Keys;
        }
    }
}