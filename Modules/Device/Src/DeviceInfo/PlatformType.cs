    namespace GameFramework.Device
    {
        public enum PlatformType
        {
            Unknown,
            Desktop,
            Mobile,
            Console,
            Web,
            XR
        }
        
        [System.Flags]
        public enum PlatformTypeFlags
        {
            None    = 0,
            Desktop = 1 << 0,
            Mobile  = 1 << 1,
            Console = 1 << 2,
            Web     = 1 << 3,
            XR      = 1 << 4,
            All     = Desktop | Mobile | Console | Web | XR
        }

        public static class PlatformExtensions
        {
            public static bool Contains(this PlatformTypeFlags mask, PlatformType single)
            {
                if (single == PlatformType.Unknown) return false;

                PlatformTypeFlags flag = single switch
                {
                    PlatformType.Desktop => PlatformTypeFlags.Desktop,
                    PlatformType.Mobile  => PlatformTypeFlags.Mobile,
                    PlatformType.Console => PlatformTypeFlags.Console,
                    PlatformType.Web     => PlatformTypeFlags.Web,
                    PlatformType.XR      => PlatformTypeFlags.XR,
                    _ => PlatformTypeFlags.None
                };

                return flag != PlatformTypeFlags.None && (mask & flag) == flag;
            }
            
            public static PlatformTypeFlags ToFlag(this PlatformType single)
            {
                return single switch
                {
                    PlatformType.Desktop => PlatformTypeFlags.Desktop,
                    PlatformType.Mobile  => PlatformTypeFlags.Mobile,
                    PlatformType.Console => PlatformTypeFlags.Console,
                    PlatformType.Web     => PlatformTypeFlags.Web,
                    PlatformType.XR      => PlatformTypeFlags.XR,
                    _ => PlatformTypeFlags.None
                };
            }
        }
    }