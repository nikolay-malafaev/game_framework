using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Logging
{
    internal static class LoggingEditorMenu
    {
        [MenuItem("GameFramework/Logging/Open Last Log")]
        public static void OpenLastLog()
        {
            var folder = Application.persistentDataPath;
            FileInfo lastLogFile = new DirectoryInfo(folder)
                .GetFiles()
                .Where(f => f.Extension == ".log")
                .OrderBy(f => f.CreationTimeUtc)
                .LastOrDefault();

            if (lastLogFile != null)
            {
                EditorUtility.OpenWithDefaultApp(lastLogFile.FullName);
            }
            else
            {
                Debug.Log("No last log file found");
            }
        }
    }
}