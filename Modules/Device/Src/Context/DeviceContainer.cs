using GameFramework.DI;

namespace GameFramework.Device
{
    public class DeviceContainer : ContainerBase<DeviceContainer, IDeviceContext>
    {
        protected override IDeviceContext CreateContext()
        {
            return new DefaultDeviceContext();
        }
    }
}