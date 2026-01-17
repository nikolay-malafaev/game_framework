using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.StaticData
{
    [CreateAssetMenu(fileName = "StaticDataSettings", menuName = "GameFramework Settings/StaticData/StaticDataSettings")]
    public class StaticDataSettingsAsset : SerializedScriptableObject
    {
        [InlineProperty]
        [HideLabel]
        public StaticDataSettings StaticDataSettings;
        
#if UNITY_EDITOR
        private void Reset()
        {
            StaticDataSettings = StaticDataSettings.Default;
        }
#endif
    }
}