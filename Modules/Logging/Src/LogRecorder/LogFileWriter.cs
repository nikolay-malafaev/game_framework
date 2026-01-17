using System;
using System.Buffers.Binary;
using System.IO;
using GameFramework.PersistentData;

namespace GameFramework.Logging
{
    public class LogFileWriter
    {
        // [u32 compressedSize][u32 uncompressedSize][payload...]
        private int _headerSize;
        private ICompressor _compressor;
        
        public bool IsCompressed => _compressor != null; 
        
        public LogFileWriter(int headerSize = 0, ICompressor compressor = null)
        {
            _headerSize = headerSize;
            _compressor = compressor;
        }
        
        public void Append(string path, ReadOnlySpan<byte> data)
        {
            if (_compressor != null)
            {
                AppendWithCompress(path, data);
            }
            else
            {
                AppendPlain(path, data);
            }
        }
        
        private void AppendWithCompress(string path, ReadOnlySpan<byte> uncompressed)
        {
            Span<byte> compressed = _compressor.Compress(uncompressed).AsSpan();
            Span<byte> header = stackalloc byte[_headerSize];
            
            BinaryPrimitives.WriteUInt32LittleEndian(header.Slice(0, 4), (uint)compressed.Length);
            BinaryPrimitives.WriteUInt32LittleEndian(header.Slice(4, 4), (uint)uncompressed.Length);

            using var fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            fileStream.Write(header);
            fileStream.Write(compressed);
            fileStream.Flush();
        }
        
        private void AppendPlain(string path, ReadOnlySpan<byte> data)
        {
            using var fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            fileStream.Write(data);
            fileStream.Flush();
        }
    }
}