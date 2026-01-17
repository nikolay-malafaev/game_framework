using UnityEngine;

namespace GameFramework.PersistentData
{
    public class DefaultPersistentDataContext : IPersistentDataContext
    {
        public string PersistentDataPath => Application.persistentDataPath;
        public string TemporaryCachePath => Application.temporaryCachePath;
    }
}
