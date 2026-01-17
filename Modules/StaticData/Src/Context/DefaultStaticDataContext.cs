using UnityEngine;

namespace GameFramework.StaticData
{
    public class DefaultStaticDataContext : IStaticDataContext
    {
        private StaticDataSettings _staticDataSettings;
        
        public IRemoteConfigService RemoteConfigService { get; }

        public StaticDataSettings StaticDataSettings
        {
            get
            {
                if (_staticDataSettings == null)
                {
                    var staticDataSettingsAsset = Resources.Load<StaticDataSettingsAsset>("GameFrameworkSettings/StaticDataSettings");
                    if (staticDataSettingsAsset != null)
                    {
                        _staticDataSettings = staticDataSettingsAsset.StaticDataSettings;
                    }
                    else
                    {
                        _staticDataSettings = StaticDataSettings.Default;
                    }
                }
                
                return _staticDataSettings;
            }
        }

        public IAddressableService AddressableService { get; } = new AddressableService();
    }
}