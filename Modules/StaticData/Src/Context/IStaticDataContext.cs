using GameFramework.DI;

namespace GameFramework.StaticData
{
    public interface IStaticDataContext : IContext
    {
        IRemoteConfigService RemoteConfigService { get; }
        StaticDataSettings StaticDataSettings { get; }
        IAddressableService AddressableService { get; }
    }
}