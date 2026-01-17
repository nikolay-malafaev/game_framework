using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

namespace GameFramework.StaticData
{
    internal sealed class StaticDataAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets)
            {
                if (string.IsNullOrEmpty(path) || !path.EndsWith(".asset"))
                    continue;

                var config = AssetDatabase.LoadAssetAtPath<StaticDataAsset>(path);
                if (config == null)
                    continue;

                var guid = AssetDatabase.AssetPathToGUID(path);
                if (string.IsNullOrEmpty(guid))
                    continue;
                
                ApplyAddressablesEditor(path, guid);
            }
        }
        
        private static void ApplyAddressablesEditor(string assetPath, string guid)
        {
            if (string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(guid))
            {
                return;
            }
            var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                UnityEngine.Debug.LogError("AddressableAssetSettingsDefaultObject.Settings is null. Skip applying Addressables.");
                return;
            }
            AddressableAssetGroup group = GetAddressableAssetGroup(settings);
            AddressableAssetEntry entry = GetAddressableAssetEntry(settings, guid, group, assetPath);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
            AssetDatabase.SaveAssets();
        }

        private static AddressableAssetGroup GetAddressableAssetGroup(AddressableAssetSettings settings)
        {
            return GetOrCreateDefaultAddressableAssetGroup(settings);
        }

        private static AddressableAssetEntry GetAddressableAssetEntry(AddressableAssetSettings settings, string guid, AddressableAssetGroup group, string assetPath)
        {
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = GetAddressableAddress(assetPath);
            string label = GetOrCreateConfigLabel(settings);
            entry.SetLabel(label, true);
            return entry;
        }

        private static string GetAddressableAddress(string assetPath)
        {
            return System.IO.Path.GetFileNameWithoutExtension(assetPath);
        }

        private static string GetOrCreateConfigLabel(AddressableAssetSettings settings)
        {
            string label = StaticDataContainer.Context.StaticDataSettings.AddressableDefaultLabel;
            if (!settings.GetLabels().Contains(label))
            {
                settings.AddLabel(label);
            }
            return label;
        }
        
        private static AddressableAssetGroup GetOrCreateDefaultAddressableAssetGroup(AddressableAssetSettings settings)
        {
            string defaultGroupName = StaticDataContainer.Context.StaticDataSettings.AddressableDefaultGroupName;
            AddressableAssetGroup defaultGroup = settings.FindGroup(defaultGroupName);
            if (defaultGroup == null)
            {
                settings.CreateGroup(defaultGroupName, true, false, true, null);
            }
            return defaultGroup;
        }
    }
}
