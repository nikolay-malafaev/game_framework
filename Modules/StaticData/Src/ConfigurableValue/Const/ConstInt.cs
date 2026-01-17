namespace GameFramework.StaticData
{
    public class ConstInt : ConstValue<int>
    {
        protected override int GetRemoteValue(IRemoteConfigService remoteConfigService)
        {
            return remoteConfigService.GetInt(RemoteKey, GetLocalValue());
        }
    }
}