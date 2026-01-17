using System;
using GameFramework.PersistentData;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Logging
{
    internal static class GameFrameworkLogHandlerInstaller
    {
        private static GameFrameworkLogHandler _gameFrameworkLogHandler = null;
        private static ILogHandler _defaultLogHandler = null;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Install()
        {
            var loggingSettings = LoggingContainer.Context.LoggingSettings;
            if (!loggingSettings.GameFrameworkLogHandler.Enabled || _gameFrameworkLogHandler != null)
            {
                return;
            }

            var persistentDataPath = PersistentDataContainer.Context.PersistentDataPath;
            _defaultLogHandler = Debug.unityLogger.logHandler;
            var logRecorder = LoggingContainer.Context.GetLogRecorder(persistentDataPath, loggingSettings.Compression.Enabled);
            _gameFrameworkLogHandler = new GameFrameworkLogHandler(_defaultLogHandler, logRecorder, loggingSettings);
            Debug.unityLogger.logHandler = _gameFrameworkLogHandler;
            Application.quitting += OnApplicationQuit;
        }
        
        private static void OnApplicationQuit()
        {
            Dispose();
        }

        private static void Dispose()
        {
            Application.quitting -= OnApplicationQuit;
            _gameFrameworkLogHandler.Dispose();
            Debug.unityLogger.logHandler = _defaultLogHandler;
            _gameFrameworkLogHandler = null;
            _defaultLogHandler = null;
        }
    }
    
    internal sealed class GameFrameworkLogHandler : ILogHandler, IDisposable
    {
        private readonly ILogHandler _debugLogHandler;
        private readonly LogRecorder _logRecorder;
        private readonly LoggingSettings _loggingSettings;
        private readonly string _productName;

        public GameFrameworkLogHandler(
            ILogHandler debugLogHandler,
            LogRecorder logRecorder,
            LoggingSettings loggingSettings)
        {
            _debugLogHandler = debugLogHandler ?? throw new ArgumentNullException(nameof(debugLogHandler));
            _logRecorder     = logRecorder     ?? throw new ArgumentNullException(nameof(logRecorder));
            _loggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            _productName = ResolveProductName();
        }
        
        public void Dispose()
        {
            _logRecorder.Dispose();
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            if (!IsLogTypeEnabled(logType))
            {
                return;
            }

            string fullFormat = CreateFullFormat(format, logType);
#if !BUILD_PRODUCTION
            _debugLogHandler?.LogFormat(logType, context, fullFormat, args);
#endif
            if (_loggingSettings.Recording.Enabled)
            {
                _logRecorder?.RecordLog(string.Format(fullFormat, args));
            }
        }

        public void LogException(System.Exception exception, Object context)
        {
#if !BUILD_PRODUCTION
            _debugLogHandler?.LogException(exception, context);
#endif
            if (_loggingSettings.Recording.Enabled)
            {
                string fullFormat = CreateFullFormat(exception?.ToString() ?? "Unknown exception", LogType.Exception);
                _logRecorder?.RecordLog(fullFormat);
            }
        }

        private bool IsLogTypeEnabled(LogType logType)
        {
            return logType switch
            {
                LogType.Log     => _loggingSettings.LogInfo.Enabled,
                LogType.Warning => _loggingSettings.LogWarning.Enabled,
                _               => true
            };
        }

        private string CreateFullFormat(string format, LogType logType)
        {
            string projectTag = GetProjectTag();
            string dateTimeTag = GetDateTimeTag();
            string logTypeTag = GetLogTypeTag(logType);
            
            return string.Concat(projectTag, dateTimeTag, logTypeTag, format ?? string.Empty);
        }

        private string GetProjectTag()
        {
            if (!_loggingSettings.ProjectTag.Enabled)
            {
                return string.Empty;
            }
            return string.Concat("[", _productName, "] ");
        }

        private string GetDateTimeTag()
        {
            if (!_loggingSettings.DateTimeTag.Enabled)
            {
                return string.Empty;
            }
            string tag = System.DateTime.UtcNow.ToString("dd.MM HH:mm:ss.fff");
            return string.Concat("[", tag, "] ");
        }

        private static string GetLogTypeTag(LogType logType)
        {
            switch (logType)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Warning:
                case LogType.Exception:
                default:
                    return string.Concat("[", logType.ToString(), "] ");
                case LogType.Log:
                    return "[Info] ";
            }
        }

        private string ResolveProductName()
        {
            return string.IsNullOrWhiteSpace(Application.productName) ? "Unity Project" : Application.productName;
        }
    }
}
