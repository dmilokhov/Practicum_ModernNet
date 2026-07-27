using EventManager.Application.Commands;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Interfaces.Services.Security;
using EventManager.Domain.Constants;
using EventManager.Domain.Entities;
using EventManager.Domain.Enums;
using EventManager.Domain.Exceptions;
using EventManager.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EventManager.UnitTests.LoginServiceTests;

public class LoginTests : LoginServiceTestsBase
{
    [Fact]
    public async Task Login_Positive_ReturnsToken()
    {
        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var loginService = scope.ServiceProvider.GetRequiredService<ILoginService>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

        var user = new User(Guid.NewGuid(), "login-user", passwordHasher.Hash(ValidPassword), Roles.User);
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var token = await loginService.LoginAsync(new LoginCommand
        {
            Login = "login-user",
            Password = ValidPassword
        });

        token.Should().Be("test-jwt-token");
        JwtTokenServiceMock.Verify(x => x.GenerateJwtToken(It.Is<User>(u => u.Login == "login-user")), Times.Once);
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsUnauthorizedException()
    {
        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var loginService = scope.ServiceProvider.GetRequiredService<ILoginService>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

        await dbContext.Users.AddAsync(new User(Guid.NewGuid(), "login-user", passwordHasher.Hash(ValidPassword), Roles.User));
        await dbContext.SaveChangesAsync();

        var action = async () => await loginService.LoginAsync(new LoginCommand
        {
            Login = "login-user",
            Password = "WrongPass1!"
        });

        await action.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage(ExceptionMessages.InvalidLoginOrPasswordMsg);
    }

    [Fact]
    public async Task Login_UnknownUser_ThrowsUnauthorizedException()
    {
        using var scope = CreateScope();
        var loginService = scope.ServiceProvider.GetRequiredService<ILoginService>();

        var action = async () => await loginService.LoginAsync(new LoginCommand
        {
            Login = "missing-user",
            Password = ValidPassword
        });

        await action.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage(ExceptionMessages.InvalidLoginOrPasswordMsg);
    }

    [Fact]
    public async Task Login_EmptyCredentials_ThrowsValidationException()
    {
        using var scope = CreateScope();
        var loginService = scope.ServiceProvider.GetRequiredService<ILoginService>();

        var action = async () => await loginService.LoginAsync(new LoginCommand
        {
            Login = "",
            Password = ""
        });

        await action.Should().ThrowAsync<ValidationException>();
    }
}
