using EventManager.Application.Interfaces.Services.Security;
using EventManager.Domain.Entities;
using EventManager.Domain.Enums;
using EventManager.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EventManager.Infrastructure.Services.Security;

public class JwtTokenService(IOptions<JwtSettings> jwtSettings) : IJwtTokenService
{
    public string GenerateJwtToken(User user)
    {
        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = user.Id,
            [JwtRegisteredClaimNames.UniqueName] = user.Login,
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
            ["role"] = user.Role.ToString()
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Secret));
        var signCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtSettings.Value.Issuer,
            Audience = jwtSettings.Value.Audience,
            Claims = claims,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.Value.JwtTokenStoreMinutes),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = signCred,
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
