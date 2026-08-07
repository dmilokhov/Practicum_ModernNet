namespace UserService.Domain.Constants;

public static class ValidationMessages
{
    public const string LoginIsRequiredMsg = "Login is required";
    public const string PasswordIsRequiredMsg = "Password is required";
    public const string PasswordTooWeakMsg = 
        "Password must be at least 7 characters long and include an uppercase letter, a lowercase letter, a digit, and a special character.";
    public const string UserAlreadyExistsMsg = "User with the same login already exist";
}
