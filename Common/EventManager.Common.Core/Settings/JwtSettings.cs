namespace EventManager.Common.Core.Settings;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int JwtTokenStoreMinutes { get; set; }
    public string Secret { get; set; } = string.Empty;
}
