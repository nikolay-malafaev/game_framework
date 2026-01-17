using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace GameFramework.PersistentData.Tests
{
    public sealed class FileStorageTests
    {
        private string _rootDir;
        private string _testDir;

        [SetUp]
        public void SetUp()
        {
            _rootDir = Path.Combine(Application.persistentDataPath, "TestData");
            _testDir = Path.Combine(_rootDir, SanitizeFileName(TestContext.CurrentContext.Test.Name));
            Directory.CreateDirectory(_testDir);
        }

        [TearDown]
        public void TearDown()
        {
#if !UNITY_EDITOR
            try
            {
                if (Directory.Exists(_testDir))
                    Directory.Delete(_testDir, recursive: true);

                if (Directory.Exists(_rootDir) && Directory.GetFileSystemEntries(_rootDir).Length == 0)
                    Directory.Delete(_rootDir, recursive: true);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FileStorageTests] Cleanup failed: {e}");
            }
#endif
        }

        [Test]
        public void WriteRead_RoundTrip_Atomic()
        {
            var storage = new FileStorage(_testDir, extension: "bin", atomic: true);
            var data = MakeBytes(1024, seed: 123);

            storage.Write("roundtrip_atomic", data);

            Assert.That(storage.Exists("roundtrip_atomic"), Is.True);
            var read = storage.Read("roundtrip_atomic");

            CollectionAssert.AreEqual(data, read);
        }

        [Test]
        public void WriteRead_RoundTrip_NonAtomic()
        {
            var storage = new FileStorage(_testDir, extension: "bin", atomic: false);
            var data = MakeBytes(257, seed: 77);

            storage.Write("roundtrip_streamed", data);

            var read = storage.Read("roundtrip_streamed");
            CollectionAssert.AreEqual(data, read);
        }

        [Test]
        public void Exists_ReturnsFalse_WhenFileMissing()
        {
            var storage = new FileStorage(_testDir, extension: "bin");
            Assert.That(storage.Exists("missing"), Is.False);
        }

        [Test]
        public void Read_ThrowsFileNotFoundException_WhenFileMissing()
        {
            var storage = new FileStorage(_testDir, extension: "bin");
            Assert.Throws<FileNotFoundException>(() => storage.Read("missing"));
        }

        [Test]
        public void Delete_RemovesFile()
        {
            var storage = new FileStorage(_testDir, extension: "bin");
            storage.Write("to_delete", MakeBytes(16, seed: 5));

            Assert.That(storage.Exists("to_delete"), Is.True);

            storage.Delete("to_delete");

            Assert.That(storage.Exists("to_delete"), Is.False);
        }

        [Test]
        public void Ctor_Throws_OnEmptyDirectory()
        {
            Assert.Throws<ArgumentException>(() => new FileStorage("", "bin"));
        }

        private static byte[] MakeBytes(int length, int seed)
        {
            var data = new byte[length];
            var rng = new System.Random(seed);
            rng.NextBytes(data);
            return data;
        }

        private static byte[] Concat(byte[] a, byte[] b)
        {
            var result = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, result, 0, a.Length);
            Buffer.BlockCopy(b, 0, result, a.Length, b.Length);
            return result;
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }
    }
}
