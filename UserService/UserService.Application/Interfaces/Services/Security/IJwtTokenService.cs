using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Services.Security;

public interface IJwtTokenService
{
    string GenerateJwtToken(User user);
}
