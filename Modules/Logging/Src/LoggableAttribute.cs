using System;

namespace GameFramework.Logging
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class LoggableAttribute : Attribute
    { 
        public string Tag { get; }
        
        public LoggableAttribute(string tag = null)
        {
            Tag = tag;
        }
    }
}