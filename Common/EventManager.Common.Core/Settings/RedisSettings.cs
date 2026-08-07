namespace EventManager.Common.Core.Settings;

public sealed class RedisSettings
{
    public const string SectionName = "Redis";

    public string EndPoint { get; set; } = string.Empty;
    public int ConnectTimeout { get; set; } = 5000;
    public int SyncTimeout { get; set; } = 3000;
}
