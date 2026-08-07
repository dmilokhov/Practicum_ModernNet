using UserService.Application.Commands;

namespace UserService.Application.Interfaces.Services;

public interface ILoginService
{
    public Task RegisterUserAsync(RegistrationCommand request, CancellationToken ct = default);
    public Task<string> LoginAsync(LoginCommand request, CancellationToken ct = default);
}
