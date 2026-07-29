using UserService.Application.Interfaces.Factories;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Interfaces.Services.Security;
using UserService.Application.Model.Factories;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Persistence.Repositories;
using UserService.Infrastructure.Services.Security;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserService.Application.Validation;

namespace UserService.UnitTests.LoginServiceTests;

public abstract class LoginServiceTestsBase : IDisposable
{
    protected const string ValidPassword = "Aa1!aaa";

    protected readonly ServiceProvider ServiceProvider;
    protected readonly Mock<IJwtTokenService> JwtTokenServiceMock = new();

    protected LoginServiceTestsBase()
    {
        var services = new ServiceCollection();
        var dbName = Guid.NewGuid().ToString();

        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(dbName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserFactory, UserFactory>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddValidatorsFromAssemblyContaining<RegistrationCommandValidator>();
        services.AddSingleton(JwtTokenServiceMock.Object);

        JwtTokenServiceMock
            .Setup(x => x.GenerateJwtToken(It.IsAny<User>()))
            .Returns("test-jwt-token");

        ServiceProvider = services.BuildServiceProvider();
    }

    protected IServiceScope CreateScope() => ServiceProvider.CreateScope();

    public void Dispose() => ServiceProvider.Dispose();
}
