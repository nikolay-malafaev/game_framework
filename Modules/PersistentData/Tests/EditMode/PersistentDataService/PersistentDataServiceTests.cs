using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace GameFramework.PersistentData.Tests
{
    public class PersistentDataServiceTests : PersistentDataServiceContractTests
    {
        protected override IPersistentDataService CreateService()
        {
            var rootDir = Path.Combine(Application.persistentDataPath, "TestData");
            var testDir = Path.Combine(rootDir, SanitizeFileName(TestContext.CurrentContext.Test.Name));
            Directory.CreateDirectory(testDir);
            IStorage storage = new FileStorage(testDir);
            
            
            ISerializer serializer = new JsonSerializer();
            ICompressor compressor = new GZipCompressor();
            IEncrypter encrypter = new AesCbcHmacEncrypter("pw");
            return new PersistentDataService(false, false, storage, serializer, compressor, encrypter);
        }
        
        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }
    }
}