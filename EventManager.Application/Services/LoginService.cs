using EventManager.Application.Commands;
using EventManager.Application.Interfaces.Factories;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Interfaces.Services.Security;
using EventManager.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace EventManager.Application.Services;

public class LoginService(
    IUserRepository userRepository,
    IPasswordHasherService passwordHasherService,
    IJwtTokenService jwtTokenService,
    IUserFactory userFactory) : ILoginService
{
    public async Task RegisterUserAsync(RegistrationCommand request, CancellationToken ct = default)
    {
        if(await userRepository.IsUserExistAsync(request.Login, ct))
        {
            throw new ValidationException(ValidationMessages.UserAlreadyExistsMsg);
        }

        var passwordHash = passwordHasherService.Hash(request.Password);
        var userEntity = userFactory.Create(request.Login, passwordHash, request.Role);
        await userRepository.AddAsync(userEntity, ct);
        await userRepository.SaveChangesAsync(ct);
    }

    public async Task<string> LoginAsync(LoginCommand request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByLoginAsync(request.Login, ct);
        var isPasswordCorrect = passwordHasherService.Verify(request.Password, user.PasswordHash);

        if (!isPasswordCorrect)
        {
            throw new ValidationException(ValidationMessages.PasswordIsNotCorrectMsg);
        }

        return jwtTokenService.GenerateJwtToken(user);
    }
}
