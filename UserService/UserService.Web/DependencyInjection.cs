using System.Text.Json.Serialization;
using EventManager.Common.AspNetCore.Helpers;

namespace UserService.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddSwagger();
        services.AddAuthentication(configuration);
        services.AddOtl(configuration);

        return services;
    }
}
