namespace GameFramework.StaticData
{
    public class ConstFloat : ConstValue<float>
    {
        protected override float GetRemoteValue(IRemoteConfigService remoteConfigService)
        {
            return remoteConfigService.GetFloat(RemoteKey, GetLocalValue());
        }
    }
}