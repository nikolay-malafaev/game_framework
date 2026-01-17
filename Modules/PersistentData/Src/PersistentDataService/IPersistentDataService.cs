using Cysharp.Threading.Tasks;

namespace GameFramework.PersistentData
{ 
    public interface IPersistentDataService
    {
        UniTask<(bool, T)> Load<T>(string key, T defaultValue = default);
        UniTask<bool> Save<T>(string key, T value);
        bool Exists(string key);
        void Delete(string key);
    }
}
