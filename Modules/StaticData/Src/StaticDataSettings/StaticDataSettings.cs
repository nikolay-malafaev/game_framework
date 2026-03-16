using System;

namespace GameFramework.StaticData
{
    [Serializable]
    public class StaticDataSettings
    {
        public string AddressableDefaultLabel;
        public string AddressableDefaultGroupName;
        
        public static StaticDataSettings Default
        {
            get
            {
                return new StaticDataSettings()
                {
                    AddressableDefaultLabel = "StaticData",
                    AddressableDefaultGroupName = "Common"
                };
            }
        }
    }
}