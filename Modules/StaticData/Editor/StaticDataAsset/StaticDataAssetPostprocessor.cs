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

                if (config is UniqueStaticDataAsset)
                {
                    string typeName = config.GetType().Name;
                    var typeGuids = AssetDatabase.FindAssets($"t:{typeName}");
                    if (typeGuids.Length > 1)
                    {
                        string errorMsg = $"Asset creation error. Asset of type '{typeName}' is unique, and an asset of this type already exists in the project.";
                        UnityEngine.Debug.LogError(errorMsg);
                        Verification.NativeInterface.AlertDialog.Show("Asset creation error", errorMsg, "Ok", "Cancel");
                        
                        AssetDatabase.DeleteAsset(path);
                        continue;
                    }
                }

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
            
            AddressableAssetEntry entry = settings.FindAssetEntry(guid);
            bool isNewEntry = false;

            if (entry == null)
            {
                AddressableAssetGroup group = GetOrCreateDefaultAddressableAssetGroup(settings);
                entry = settings.CreateOrMoveEntry(guid, group);
                entry.address = GetAddressableAddress(assetPath);
                isNewEntry = true;
            }

            string label = GetOrCreateConfigLabel(settings);
            entry.SetLabel(label, true);

            var modificationEvent = isNewEntry 
                ? AddressableAssetSettings.ModificationEvent.EntryAdded 
                : AddressableAssetSettings.ModificationEvent.EntryModified;
            
            settings.SetDirty(modificationEvent, entry, true);
            AssetDatabase.SaveAssets();
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
