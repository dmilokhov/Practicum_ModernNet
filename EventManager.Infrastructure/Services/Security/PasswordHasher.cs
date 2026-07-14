using EventManager.Application.Interfaces.Services.Security;
using System.Security.Cryptography;
using System.Text;

namespace EventManager.Infrastructure.Services.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        if(password == null) throw new ArgumentNullException(nameof(password));

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    public bool Verify(string password, string hash)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (hash == null) throw new ArgumentNullException(nameof(hash));

        var computedHash = Hash(password);

        return string.Equals(computedHash, hash, StringComparison.OrdinalIgnoreCase);
    }
}
