namespace GameFramework.Device
{
    public class DefaultDeviceContext : IDeviceContext
    {
        public IDeviceInfo DeviceInfo { get; } = new UnityDeviceInfo();
    }
}