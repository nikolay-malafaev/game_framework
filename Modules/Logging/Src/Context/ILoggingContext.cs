using GameFramework.DI;
using GameFramework.PersistentData;

namespace GameFramework.Logging
{
    public interface ILoggingContext : IContext
    {
        LoggingSettings LoggingSettings { get; }
        ICompressor LogFileCompressor { get; }
        LoggingHelper LoggingHelper { get; }
        LogRecorder GetLogRecorder(string directory, bool compression = false);
        LogFileWriter GetLogFileWriter(int headerSize = 0, ICompressor compressor = null);
        LogFileReader GetLogFileReader(int headerSize = 0, ICompressor compressor = null);
    }
}
