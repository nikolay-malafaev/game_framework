using System;

namespace GameFramework.Verification.NativeInterface
{
    public class NativeInterfaceNotSupportedException : Exception
    {
        public NativeInterfaceNotSupportedException()
            : base("GameFramework.NativeInterface: Not supported device!") { }
    }
}