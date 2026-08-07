namespace EventManager.Common.Core.Constants;

public static class CommonExceptionMessages
{
    public const string InvalidUserIdMsg = "User ID missing or invalid in JWT token.";
    public const string InvalidUserRoleMsg = "User Role missing or invalid in JWT token.";

    public static string SettingAreNotConfiguredMsg(string sectionName) =>
        $"{sectionName} settings are not configured properly in appsettings.json";

    public static string RedisActionFailed(string actionName, string cacheKey) =>
        $"Redis {actionName} failed for key {cacheKey}";
}
