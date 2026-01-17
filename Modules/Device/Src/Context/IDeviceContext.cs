using GameFramework.DI;

namespace GameFramework.Device
{
    public interface IDeviceContext : IContext
    {
        IDeviceInfo DeviceInfo { get; }
    }
}
