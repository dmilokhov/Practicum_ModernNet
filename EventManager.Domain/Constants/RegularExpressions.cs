namespace EventManager.Domain.Constants;

public static class RegularExpressions
{
    public const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{7,}$";
}
