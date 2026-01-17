using System;
using System.Diagnostics;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameFramework.PersistentData
{
    internal class PersistentDataReaderWindow : OdinEditorWindow
    {
        [MenuItem("GameFramework/PersistentData/Persistent Data Reader")]
        private static void Open()
        {
            var window = GetWindow<PersistentDataReaderWindow>();
            window.titleContent = new GUIContent("Persistent Data Reader");
            window.minSize = new Vector2(520, 360);
            window.Show();
        }
        
        [Sirenix.OdinInspector.FilePath(AbsolutePath = true, RequireExistingPath = true)]
        [LabelText("Source file")]
        [OnValueChanged(nameof(RebuildOutputPath))]
        [SerializeField] 
        private string _inputFilePath;
        
        [Sirenix.OdinInspector.FilePath(AbsolutePath = true, RequireExistingPath = true)]
        [LabelText("Output file")]
        [SerializeField] 
        private string _outputFilePath;
        
        [LabelText("Postfix")]
        [SerializeField] 
        private string _outputPostfix = ".decoded";
        
        [LabelText("Keep Source Extension")]
        [SerializeField]
        private bool _keepSourceExtension = false;
        
        [SerializeReference] [ShowInInspector] private ICompressor _compressor;
        [SerializeReference] [ShowInInspector] private IEncrypter _encrypter;
        
        [SerializeField] 
        private string _password = "";
        
        [Button(ButtonSizes.Medium)]
        [ButtonGroup("UnpackGroup")]
        [EnableIf(nameof(CanUnpack))]
        private void Unpack()
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                
                var sourceDirectory = Path.GetDirectoryName(_inputFilePath);
                var extension = Path.GetExtension(_inputFilePath);
                var fileName = Path.GetFileNameWithoutExtension(_inputFilePath);
                var fileStorage = new FileStorage(sourceDirectory, extension);
                byte[] bytes;
                bytes = fileStorage.Read(fileName);
                if (_encrypter != null)
                {
                    _encrypter.SetPassword(_password);
                    bytes = _encrypter.Decrypt(bytes);
                }

                if (_compressor != null)
                {
                    bytes = _compressor.Decompress(bytes);
                }

                var newDirectory = Path.GetDirectoryName(_outputFilePath);
                var newExtension = Path.GetExtension(_outputFilePath);
                var newFileName = Path.GetFileNameWithoutExtension(_outputFilePath);
                var newFileStorage = new FileStorage(newDirectory, newExtension);
                newFileStorage.Write(newFileName, bytes);
                
                sw.Stop();
                Debug.Log($"Stopwatch: {sw.ElapsedMilliseconds} ms");
            }
            catch (Exception e)
            {
                Debug.Log(e);
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
            if(_compressor == null && _encrypter == null) return false;
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
                _outputFilePath = Path.Combine(directory, $"{fileName}{postfix}.dat");
            }
            
            _outputFilePath = Path.GetFullPath(_outputFilePath);
            _inputFilePath = Path.GetFullPath(_inputFilePath);
        }
    }
}