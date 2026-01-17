namespace GameFramework.StaticData
{
    public class ConstString : ConstValue<string>
    {
        protected override string GetRemoteValue(IRemoteConfigService remoteConfigService)
        {
            return remoteConfigService.GetString(RemoteKey, GetLocalValue());
        }
    }
}