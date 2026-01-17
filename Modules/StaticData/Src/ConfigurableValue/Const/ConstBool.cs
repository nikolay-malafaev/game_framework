namespace GameFramework.StaticData
{
    public class ConstBool : ConstValue<bool>
    {
        protected override bool GetRemoteValue(IRemoteConfigService remoteConfigService)
        {
            return remoteConfigService.GetBool(RemoteKey, GetLocalValue());
        }
    }
}