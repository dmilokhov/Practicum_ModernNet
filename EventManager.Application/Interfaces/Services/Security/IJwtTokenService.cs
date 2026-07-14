using EventManager.Domain.Enums;

namespace EventManager.Application.Interfaces.Services.Security;

public interface IJwtTokenService
{
    string GenerateJwtToken(Guid userId, string userLogin, Roles userRole);
}
