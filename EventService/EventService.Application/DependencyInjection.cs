using EventService.Application.Interfaces;
using EventService.Application.Interfaces.Services;
using EventService.Application.Model.Validators;
using EventService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddScoped<IEventCrudService, EventCrudService>();


        services.AddTransient<IEventFilterValidator, EventFilterValidator>();
        return services;
    }
}
