using GameFramework.DI;

namespace GameFramework.PersistentData
{
    public interface IPersistentDataContext : IContext
    {
        string PersistentDataPath { get; }
        string TemporaryCachePath { get; }
    }
}
