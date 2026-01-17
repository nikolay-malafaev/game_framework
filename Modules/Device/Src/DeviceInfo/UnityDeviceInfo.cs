using UnityEngine;

namespace GameFramework.Device
{
    public sealed class UnityDeviceInfo : IDeviceInfo
    {
        private readonly PlatformType _platformType;
        private readonly OperatingSystem _operatingSystem;

        public PlatformType PlatformType => _platformType;
        public OperatingSystem OperatingSystem => _operatingSystem;

        public UnityDeviceInfo()
        {
            (_platformType, _operatingSystem) = Resolve();
        }

        private static (PlatformType platform, OperatingSystem os) Resolve()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return (PlatformType.Web, OperatingSystem.Unknown);
#endif

#if UNITY_PS5 || UNITY_PS4 || UNITY_SWITCH || UNITY_GAMECORE || UNITY_XBOXONE
            return (PlatformType.Console, OperatingSystem.Unknown);
#endif

            return Application.platform switch
            {
                // Desktop (Player)
                RuntimePlatform.WindowsPlayer => (PlatformType.Desktop, OperatingSystem.Windows),
                RuntimePlatform.OSXPlayer     => (PlatformType.Desktop, OperatingSystem.MacOS),
                RuntimePlatform.LinuxPlayer   => (PlatformType.Desktop, OperatingSystem.Linux),

                // Desktop (Editor)
                RuntimePlatform.WindowsEditor => (PlatformType.Desktop, OperatingSystem.Windows),
                RuntimePlatform.OSXEditor     => (PlatformType.Desktop, OperatingSystem.MacOS),
                RuntimePlatform.LinuxEditor   => (PlatformType.Desktop, OperatingSystem.Linux),

                // Mobile
                RuntimePlatform.Android      => (PlatformType.Mobile, OperatingSystem.Android),
                RuntimePlatform.IPhonePlayer => (PlatformType.Mobile, OperatingSystem.iOS),
                RuntimePlatform.tvOS         => (PlatformType.Mobile, OperatingSystem.tvOS),

                // XR
                RuntimePlatform.VisionOS     => (PlatformType.XR, OperatingSystem.VisionOS),

                // WebGL
                RuntimePlatform.WebGLPlayer  => (PlatformType.Web, OperatingSystem.Unknown),

                _ => (PlatformType.Unknown, OperatingSystem.Unknown)
            };
        }
    }
}
