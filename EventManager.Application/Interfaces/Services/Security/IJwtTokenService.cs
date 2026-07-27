using EventManager.Domain.Entities;

namespace EventManager.Application.Interfaces.Services.Security;

public interface IJwtTokenService
{
    string GenerateJwtToken(User user);
}
