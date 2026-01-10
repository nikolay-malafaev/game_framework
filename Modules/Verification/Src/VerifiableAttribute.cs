using System;

namespace GameFramework.Verification
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class VerifiableAttribute : Attribute { }
}