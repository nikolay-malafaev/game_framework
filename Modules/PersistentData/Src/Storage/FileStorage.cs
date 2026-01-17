using System;
using System.IO;

namespace GameFramework.PersistentData
{
    public class FileStorage : IStorage
    {
        private string _directory;
        private string _extension;
        private bool _writeAtomic;

        public FileStorage(string directory, string extension = "", bool atomic = true)
        {
            _directory = string.IsNullOrEmpty(directory) ? throw new ArgumentException($"Directory {directory} is null or empty.") : directory;
            _extension = extension;
            _writeAtomic = atomic;
        }
        
        public byte[] Read(string fileName)
        {
            string path = GetPath(fileName);
            
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path is null/empty.");

            using var fs = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.None,
                bufferSize: 64 * 1024,
                options: FileOptions.Asynchronous | FileOptions.SequentialScan);

            if (fs.Length > int.MaxValue)
                throw new IOException($"File too large for byte[]: {fs.Length} bytes.");

            var result = new byte[fs.Length];
            int offset = 0;
            while (offset < result.Length)
            {
                int read = fs.Read(result, offset, result.Length - offset);
                if (read == 0) throw new EndOfStreamException("Unexpected end of stream.");
                offset += read;
            }
            
            return result;
        }

        public void Write(string fileName, ReadOnlyMemory<byte> data)
        {
            string path = GetPath(fileName);
            if (_writeAtomic)
            {
                WriteAtomic(path, data);
            }
            else
            {
                WriteStreamed(path, data);
            }
        }

        public bool Exists(string fileName)
        {
            string path = GetPath(fileName);
            return File.Exists(path);
        }

        public void Delete(string fileName)
        {
            string path = GetPath(fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        
        private void WriteStreamed(string path, ReadOnlyMemory<byte> data)
        {
            using var stream = new FileStream(
                path,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 64 * 1024,
                options: FileOptions.WriteThrough);
            stream.Write(data.Span);
            stream.Flush(flushToDisk: true);
        }

        private void WriteAtomic(string path, ReadOnlyMemory<byte> data)
        {
            var tempPath = $"{path}.temp";
            try
            {
                using (var stream = new FileStream(
                           tempPath,
                           FileMode.Create,
                           FileAccess.Write,
                           FileShare.None,
                           bufferSize: 64 * 1024,
                           options: FileOptions.WriteThrough))
                {
                    stream.Write(data.Span);
                    stream.Flush(flushToDisk: true);
                }
                File.Delete(path);
                File.Move(tempPath, path);
            }
            finally
            {
                File.Delete(tempPath);
            }
        }
        
        private string GetPath(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key is null/empty.", nameof(key));

            var ext = _extension?.Trim();
            if (string.IsNullOrEmpty(ext))
                return Path.Combine(_directory, key);

            ext = ext.TrimStart('.');
            return Path.Combine(_directory, $"{key}.{ext}");
        }
    }
}