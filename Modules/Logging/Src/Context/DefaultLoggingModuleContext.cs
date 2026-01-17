using GameFramework.PersistentData;
using UnityEngine;

namespace GameFramework.Logging
{
    public class DefaultLoggingContext : ILoggingContext
    {
        private LoggingSettings _loggingSettings;
        public LoggingSettings LoggingSettings
        {
            get
            {
                if (_loggingSettings == null)
                {
                    var loggingSettingsAsset = Resources.Load<LoggingSettingsAsset>("GameFrameworkSettings/LoggingSettings");
                    if (loggingSettingsAsset != null)
                    {
                        _loggingSettings = loggingSettingsAsset.LoggingSettings;
                    }
                    else
                    {
                        _loggingSettings = LoggingSettings.Default;
                    }
                }
                
                return _loggingSettings;
            }
        }
        
        public ICompressor LogFileCompressor
        {
            get
            {
                return new GZipCompressor();
            }
        }

        public LoggingHelper LoggingHelper { get; } = new LoggingHelper();

        public LogRecorder GetLogRecorder(string directory, bool compression = false)
        {
            int headerSize = LoggingSettings.CompressionHeaderSize;
            if (compression)
            {
                return new LogRecorder(GetLogFileWriter(headerSize, LogFileCompressor), directory);
            }

            return new LogRecorder(GetLogFileWriter(), directory);
        }

        public LogFileWriter GetLogFileWriter(int headerSize = 0, ICompressor compressor = null)
        {
            return new LogFileWriter(headerSize, compressor);
        }

        public LogFileReader GetLogFileReader(int headerSize = 0, ICompressor compressor = null)
        {
            return new LogFileReader(headerSize, compressor);
        }
    }
}