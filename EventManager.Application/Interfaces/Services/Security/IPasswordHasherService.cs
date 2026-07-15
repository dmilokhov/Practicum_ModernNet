namespace EventManager.Application.Interfaces.Services.Security;

public interface IPasswordHasherService
{
    /// <summary>
    ///  Hash a password using SHA-256
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// Check password to its hash.
    /// </summary>
    /// <returns>result of the check</returns>
    bool Verify(string password, string hash);
}
