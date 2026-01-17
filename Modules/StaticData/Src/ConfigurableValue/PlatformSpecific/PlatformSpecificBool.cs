using System;

namespace GameFramework.StaticData
{
    [Serializable]
    public class PlatformSpecificBool : PlatformSpecificValue<bool>
    {
        public bool Enabled => Value;

        public PlatformSpecificBool(bool defaultValue) : base(defaultValue) { }
        public PlatformSpecificBool(bool defaultValue, bool editorValue) : base(defaultValue) { }
    }
}