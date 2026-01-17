using System;

namespace GameFramework.PersistentData
{
    public interface ICompressor
    {
        public byte[] Compress(ReadOnlySpan<byte> data);
        byte[] Decompress(ReadOnlySpan<byte> data);
    }
}