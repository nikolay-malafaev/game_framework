using System;
using System.IO;
using GameFramework.PersistentData;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Logging.Editor
{
    internal class LogUnpackerWindow : OdinEditorWindow
    {
        [MenuItem("GameFramework/Logging/Log Unpacker")]
        private static void Open()
        {
            var window = GetWindow<LogUnpackerWindow>();
            window.titleContent = new GUIContent("Log Unpacker");
            window.minSize = new Vector2(520, 360);
            window.Show();
        }
        
        [Sirenix.OdinInspector.FilePath(AbsolutePath = true, RequireExistingPath = true)]
        [LabelText("Source log file")]
        [OnValueChanged(nameof(RebuildOutputPath))]
        [SerializeField] 
        private string _inputFilePath;
        
        [Sirenix.OdinInspector.FilePath(AbsolutePath = true, RequireExistingPath = true)]
        [LabelText("Output log file")]
        [SerializeField] 
        private string _outputFilePath;
        
        [LabelText("Postfix")]
        [SerializeField] 
        private string _outputPostfix = ".decoded";
        
        [LabelText("Keep Source Extension")]
        [SerializeField]
        private bool _keepSourceExtension = false;
        
        [Button(ButtonSizes.Medium)]
        [ButtonGroup("UnpackGroup")]
        [EnableIf(nameof(CanUnpack))]
        private void Unpack()
        {
            try
            {
                int headerSize = LoggingContainer.Context.LoggingSettings.CompressionHeaderSize;
                ICompressor compressor = LoggingContainer.Context.LogFileCompressor;
                LogFileReader reader = LoggingContainer.Context.GetLogFileReader(headerSize, compressor);
                byte[] data = reader.Read(_inputFilePath);
                using var fileStream = new FileStream(_outputFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                fileStream.Write(data);
                fileStream.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [Button(ButtonSizes.Medium)]
        [ButtonGroup("UnpackGroup")]
        [EnableIf(nameof(CanUnpack))]
        private void UnpackAndOpen()
        {
            Unpack();
            EditorUtility.OpenWithDefaultApp(_outputFilePath);
        }

        private bool CanUnpack()
        {
            if (string.IsNullOrWhiteSpace(_inputFilePath)) return false;
            return true;
        }

        private void RebuildOutputPath()
        {
            if (string.IsNullOrWhiteSpace(_inputFilePath))
            {
                _outputFilePath = string.Empty;
                return;
            }

            var directory = Path.GetDirectoryName(_inputFilePath);
            var fileName = Path.GetFileNameWithoutExtension(_inputFilePath);

            if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(fileName))
            {
                _outputFilePath = string.Empty;
                return;
            }

            var postfix = _outputPostfix ?? string.Empty;
            if (_keepSourceExtension)
            {
                var extension = Path.GetExtension(_inputFilePath);
                _outputFilePath = Path.Combine(directory, $"{fileName}{postfix}.{extension}");
            }
            else
            {
                _outputFilePath = Path.Combine(directory, $"{fileName}{postfix}.log");
            }
            
            _outputFilePath = Path.GetFullPath(_outputFilePath);
            _inputFilePath = Path.GetFullPath(_inputFilePath);
        }
    }
}