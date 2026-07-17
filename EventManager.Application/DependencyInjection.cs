using EventManager.Application.Interfaces;
using EventManager.Application.Interfaces.Factories;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Interfaces.Services.Validation;
using EventManager.Application.Model.Factories;
using EventManager.Application.Model.Validators;
using EventManager.Application.Services;
using EventManager.Application.Services.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IBookingFactory, BookingFactory>();
        services.AddSingleton<IUserFactory, UserFactory>();

        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<ILoginService, LoginService>();

        services.AddScoped<ISubmitBookingValidationService, SubmitBookingValidationService>();

        services.AddTransient<IEventFilterValidator, EventFilterValidator>();

        services.AddValidatorsFromAssemblyContaining<RegistrationCommandValidator>();
        return services;
    }
}
