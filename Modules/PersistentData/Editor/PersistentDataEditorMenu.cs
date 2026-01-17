using UnityEditor;

namespace GameFramework.PersistentData
{
    internal static class PersistentDataEditorMenu
    {
        [MenuItem("GameFramework/PersistentData/Open Temporary Cache Path", false)]
        public static void OpenTemporaryCachePath()
        {
            var path = PersistentDataContainer.Context.TemporaryCachePath;
            if (!path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                path += System.IO.Path.DirectorySeparatorChar;
            }
            EditorUtility.RevealInFinder(path);
        }
        
        [MenuItem("GameFramework/PersistentData/Open Persistent Data Path", false)]
        public static void OpenPersistentDataPath()
        {
            var path = PersistentDataContainer.Context.PersistentDataPath;
            if (!path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                path += System.IO.Path.DirectorySeparatorChar;
            }
            EditorUtility.RevealInFinder(path);
        }
    }
}