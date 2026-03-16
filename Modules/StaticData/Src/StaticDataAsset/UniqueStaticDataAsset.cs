using Sirenix.OdinInspector;

namespace GameFramework.StaticData
{
    [InfoBox("Error: The asset is unique, but an instance of this type already exists in the project! Extra assets must be deleted.", InfoMessageType.Error, "IsDuplicate")]
    public abstract class UniqueStaticDataAsset : StaticDataAsset 
    {
#if UNITY_EDITOR
        private bool IsDuplicate()
        {
            string typeName = GetType().Name;
            string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeName}");
            return guids.Length > 1;
        }
#endif
    }
}