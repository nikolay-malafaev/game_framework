using System;
using System.Buffers.Binary;
using System.IO;
using GameFramework.PersistentData;

namespace GameFramework.Logging
{
    public class LogFileReader
    {
        private readonly int _headerSize;
        private readonly ICompressor _compressor;

        public LogFileReader(int headerSize, ICompressor compressor)
        {
            _headerSize = headerSize;
            _compressor = compressor;
        }
        
        public byte[] Read(string path, Action<ReadOnlyMemory<byte>> onEntry = null)
        {
            if (!File.Exists(path)) return Array.Empty<byte>();
            if (_compressor == null) throw new NullReferenceException(nameof(_compressor));

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Span<byte> header = stackalloc byte[_headerSize];

            using var ms = new MemoryStream();

            while (true)
            {
                int read = ReadExactly(fs, header);
                if (read == 0) break; // EOF
                if (read < _headerSize) break; // обрезанный хвост (crash во время записи)

                uint compressedSize = BinaryPrimitives.ReadUInt32LittleEndian(header.Slice(0, 4));
                uint uncompressedSize = BinaryPrimitives.ReadUInt32LittleEndian(header.Slice(4, 4));

                if (compressedSize == 0 || compressedSize > int.MaxValue) break;

                byte[] compressed = new byte[compressedSize];
                if (ReadExactly(fs, compressed) < compressed.Length) break; // обрезанный хвост

                byte[] data = _compressor.Decompress(compressed);

                if (uncompressedSize != 0 && data.Length != (int)uncompressedSize)
                    break;

                onEntry?.Invoke(data);

                ms.Write(data, 0, data.Length);
            }

            return ms.ToArray();
        }

        private static int ReadExactly(Stream stream, Span<byte> buffer)
        {
            int total = 0;
            while (total < buffer.Length)
            {
                int n = stream.Read(buffer.Slice(total));
                if (n == 0) break;
                total += n;
            }

            return total;
        }

        private static int ReadExactly(Stream stream, byte[] buffer)
        {
            int total = 0;
            while (total < buffer.Length)
            {
                int n = stream.Read(buffer, total, buffer.Length - total);
                if (n == 0) break;
                total += n;
            }

            return total;
        }
    }
}
