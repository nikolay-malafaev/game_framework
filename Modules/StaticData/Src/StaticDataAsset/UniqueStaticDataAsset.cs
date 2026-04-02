using Sirenix.OdinInspector;

namespace GameFramework.StaticData
{
    public abstract class UniqueStaticDataAsset : StaticDataAsset 
    {
#if UNITY_EDITOR
        [OnInspectorGUI]
        [PropertyOrder(-10)]
        private void DrawWarning()
        {
            if (IsDuplicate())
            {
                UnityEditor.EditorGUILayout.HelpBox("Error: The asset is unique, but an instance of this type already exists in the project! Extra assets must be deleted.", UnityEditor.MessageType.Error);
            }
        }

        private bool IsDuplicate()
        {
            string typeName = GetType().Name;
            string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeName}");
            return guids.Length > 1;
        }
#endif
    }
}