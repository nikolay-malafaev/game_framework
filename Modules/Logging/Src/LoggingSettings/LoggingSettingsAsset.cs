using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameFramework.Logging
{
    [CreateAssetMenu(fileName = "LoggingSettings", menuName = "GameFramework Settings/Logging/LoggingSettings")]
    public class LoggingSettingsAsset : SerializedScriptableObject
    {
        [InlineProperty]
        [HideLabel]
        [OdinSerialize]
        private LoggingSettings _loggingSettings;
        
        public LoggingSettings LoggingSettings => _loggingSettings;

#if UNITY_EDITOR
        private void Reset()
        {
            _loggingSettings ??= LoggingSettings.Default;
        }
#endif
    }
}