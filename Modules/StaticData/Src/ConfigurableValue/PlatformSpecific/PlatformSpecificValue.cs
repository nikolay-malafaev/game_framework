using System;
using System.Collections.Generic;
using GameFramework.Device;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameFramework.StaticData
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class PlatformSpecificValue<T> : ConfigurableValue<T>
    {
        [OdinSerialize, ShowInInspector] private T _defaultValue;
        [OdinSerialize, ShowInInspector] private T _editorValue;
        [OdinSerialize, ShowInInspector] private Dictionary<PlatformType, T> _platformValues = new();

        private PlatformType CurrentPlatformType => DeviceContainer.Context.DeviceInfo.PlatformType;

        public override T Value
        {
            get
            {
#if UNITY_EDITOR
                return _editorValue;
#else
                if (_platformValues.ContainsKey(CurrentPlatformType))
                {
                    return _platformValues[CurrentPlatformType];
                }
                return _defaultValue;
#endif
            }
        }

        public PlatformSpecificValue(T defaultValue)
        {
            _defaultValue = defaultValue;
            _editorValue = defaultValue;
        }
        
        public PlatformSpecificValue(T defaultValue, T editorValue)
        {
            _defaultValue = defaultValue;
            _editorValue = editorValue;
        }
    }
}