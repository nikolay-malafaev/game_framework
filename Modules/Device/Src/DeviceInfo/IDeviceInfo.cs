namespace GameFramework.Device
{
    public interface IDeviceInfo
    {
        PlatformType PlatformType { get; }
        OperatingSystem OperatingSystem { get; }
    }
}
