using System;
using System.IO;
using System.IO.Compression;

namespace GameFramework.PersistentData
{
    public class GZipCompressor : ICompressor
    {
        public byte[] Compress(ReadOnlySpan<byte> data)
        {
            using var outputStream = new MemoryStream();
            using (var gzip = new GZipStream(outputStream, CompressionMode.Compress, leaveOpen: true))
            {
                gzip.Write(data);
            }
            return outputStream.ToArray();
        }

        public byte[] Decompress(ReadOnlySpan<byte> data)
        {
            if (data.IsEmpty)
                return Array.Empty<byte>();

            using MemoryStream inputStream = new MemoryStream(data.ToArray(), writable: false);
            using GZipStream decompressionStream = new GZipStream(inputStream, CompressionMode.Decompress);
            using MemoryStream outputStream = new MemoryStream();

            decompressionStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }
    }
}