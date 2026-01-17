namespace GameFramework.StaticData
{
    public interface IRemoteConfigService
    {
        bool GetBool(string key, bool defaultValue);
        int GetInt(string key, int defaultValue);
        float GetFloat(string key, float defaultValue);
        string GetString(string key, string defaultValue);
    }
}
