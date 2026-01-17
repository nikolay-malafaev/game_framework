using System;
using GameFramework.StaticData;
using Sirenix.OdinInspector;

namespace GameFramework.Logging
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class LoggingSettings
    {
        public PlatformSpecificBool GameFrameworkLogHandler;
        public PlatformSpecificBool LogInfo;
        public PlatformSpecificBool LogWarning;
        public PlatformSpecificBool ProjectTag;
        public PlatformSpecificBool DateTimeTag;
        public PlatformSpecificBool Recording;
        public PlatformSpecificBool Compression;
        [MinValue(4)]
        public int CompressionHeaderSize;
        [MinValue(1)]
        public int CountCachedLogFiles;
        
        public static LoggingSettings Default
        {
            get
            {
                return new LoggingSettings()
                {
                    GameFrameworkLogHandler = new PlatformSpecificBool(true),
                    LogInfo = new PlatformSpecificBool(true),
                    LogWarning = new PlatformSpecificBool(true),
                    ProjectTag = new PlatformSpecificBool(false),
                    DateTimeTag = new PlatformSpecificBool(false),
                    Recording = new PlatformSpecificBool(false),
                    Compression = new PlatformSpecificBool(false),
                    CompressionHeaderSize = 8,
                    CountCachedLogFiles = 5
                };
            }
        }
    }
}