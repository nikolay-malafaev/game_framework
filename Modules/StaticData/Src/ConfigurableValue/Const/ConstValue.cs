using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameFramework.StaticData
{
    public abstract class ConstValue<T> : ConfigurableValue<T>
    {
        [OdinSerialize] 
        private T _localValue;
        
        [OdinSerialize]
        [LabelText("Can be remote")]
        private bool _canBeRemote;
        
        [OdinSerialize]
        [LabelText("Remote key")]
        [ShowIf("_canBeRemote")]
        private string _remoteKey;
        
        public override T Value
        {
            get
            {
                IRemoteConfigService remoteConfigService = StaticDataContainer.Context.RemoteConfigService;
                if (_canBeRemote && remoteConfigService != null && !_remoteKey.Empty())
                {
                    return GetRemoteValue(remoteConfigService);
                }
                return GetLocalValue();
            }
        }

        public string RemoteKey => _remoteKey;
        
        protected abstract T GetRemoteValue(IRemoteConfigService remoteConfigService);

        protected T GetLocalValue()
        {
            return _localValue;
        }
    }
}