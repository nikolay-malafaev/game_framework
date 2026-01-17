using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameFramework.Logging
{
    public class LogRecorder : IDisposable
    {
        // should use because array pool return length multiple of a power of two
        struct Chunk
        {
            public byte[] Buffer;
            public int Length;

            public Chunk(byte[] buffer, int length)
            {
                Buffer = buffer;
                Length = length;
            }

            public ReadOnlySpan<Byte> AsSpan()
            {
                return Buffer.AsSpan(0, Length);
            }
        }

        private const int LogWeight = 1024; // weight in bytes
        private const int LogCapacity = 10; // max count logs
        private const int BufferWeight = LogWeight * LogCapacity;

        private readonly ArrayPool<byte> _arrayPool;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly Queue<Chunk> _queue = new();
        private readonly SemaphoreSlim _hasDataSemaphore = new(0, int.MaxValue);

        private byte[] _hotBuffer = new byte[BufferWeight];
        private int _writeBufferIndex = 0;
        private bool _disposing = false;
        private LogFileWriter _logFileWriter;
        private string _path;

        public LogRecorder(LogFileWriter logFileWriter, string folder)
        {
            _logFileWriter = logFileWriter;
            _arrayPool = ArrayPool<byte>.Create(
                maxArrayLength: BufferWeight,
                maxArraysPerBucket: 10);
            _cancellationToken = new CancellationTokenSource();
            FormatPath(folder, logFileWriter.IsCompressed);
            CheckAndCachedLogFiles(folder);
            _ = FlushLoop();
        }

        public void Dispose()
        {
            if(_disposing) return;
            
            _disposing = true;
            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
            
            if (_writeBufferIndex > 0)
            {
                EnqueueHotBufferAndReset();
            }
            FlushQueue();
            
            _hasDataSemaphore.Dispose();
        }

        public void RecordLog(string message)
        {
            if (_disposing || string.IsNullOrEmpty(message))
            {
                return;
            }

            if (_hotBuffer.Length - _writeBufferIndex < LogWeight)
            {
                EnqueueHotBufferAndReset();
            }

            int writtenBytes = WriteUtf8TruncatedWithNewline(
                message,
                _hotBuffer.AsSpan(_writeBufferIndex, LogWeight),
                LogWeight);

            if (writtenBytes == 0)
            {
                return;
            }

            _writeBufferIndex += writtenBytes;
        }

        private async UniTask FlushLoop()
        {
            var token = _cancellationToken.Token;
            
            while (true)
            {
                try
                {
                    await _hasDataSemaphore.WaitAsync(token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                await UniTask.RunOnThreadPool(FlushQueue);
            }
        }
        
        private void FlushQueue()
        {
            while (_queue.Count > 0)
            {
                Chunk chunk = _queue.Dequeue();
                try
                {
                    _logFileWriter.Append(_path, chunk.AsSpan());
                }
                finally
                {
                    _arrayPool.Return(chunk.Buffer);
                }
            }
        }

        private void EnqueueHotBufferAndReset()
        {
            if (_writeBufferIndex == 0)
            {
                return;
            }

            byte[] bufferForQueue = _arrayPool.Rent(_writeBufferIndex);
            Buffer.BlockCopy(_hotBuffer, 0, bufferForQueue, 0, _writeBufferIndex);
            _queue.Enqueue(new Chunk(bufferForQueue, _writeBufferIndex));
            Array.Clear(_hotBuffer, 0, _hotBuffer.Length);
            _writeBufferIndex = 0;
            _hasDataSemaphore.Release();
        }

        private int WriteUtf8TruncatedWithNewline(string message, Span<byte> destination, int maxBytesPerLog)
        {
            if (maxBytesPerLog <= 0 || destination.Length < maxBytesPerLog)
            {
                return 0;
            }

            int maxPayload = maxBytesPerLog - 1;

            Encoder encoder = Encoding.UTF8.GetEncoder();
            encoder.Convert(
                chars: message.AsSpan(),
                bytes: destination.Slice(0, maxPayload),
                flush: true,
                out int charsUsed,
                out int bytesUsed,
                out bool completed);

            destination[bytesUsed] = (byte)'\n';
            return bytesUsed + 1;
        }

        private void FormatPath(string folder, bool isCompressed)
        {
            var fileName = DateTime.UtcNow.ToString(
                "dd-MM-yyyy_HH-mm-ss",
                System.Globalization.CultureInfo.InvariantCulture
            );
            var extension = isCompressed ? "dat" : "log";
            _path = Path.Combine(folder, $"{fileName}.{extension}");
        }

        private void CheckAndCachedLogFiles(string folder)
        {
            static bool IsValidLogFile(string filePath)
            {
                var extension = Path.GetExtension(filePath);
                if (extension != ".log" && extension != ".dat")
                {
                    return false;
                }

                var name = Path.GetFileNameWithoutExtension(filePath);

                return DateTime.TryParseExact(
                    name,
                    "dd-MM-yyyy_HH-mm-ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _
                );
            }
            
            List<FileInfo> files = new DirectoryInfo(folder)
                .GetFiles()
                .Where(f => IsValidLogFile(f.FullName))
                .OrderBy(f => f.CreationTimeUtc)
                .ToList();

            int countCachedLogFiles = LoggingContainer.Context.LoggingSettings.CountCachedLogFiles;
            while (files.Count >= countCachedLogFiles)
            {
                File.Delete(files[0].FullName);
                files.RemoveAt(0);
            }
        }
    }
}