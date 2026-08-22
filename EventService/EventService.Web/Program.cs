using EventManager.Common.AspNetCore.Middleware;
using EventService.Application;
using EventService.Infrastructure;
using EventService.Infrastructure.Persistence;
using EventService.Web;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.IsDevelopment();

//before Build
if(isDevelopment)
{
    builder.Host.UseDefaultServiceProvider(options =>
    {
        options.ValidateScopes = true;
        options.ValidateOnBuild = true;
    });
}

//services
builder.Services.AddApplication();
await builder.Services.AddInfrastructureAsync(builder.Configuration);
builder.Services.AddPresentation(builder.Configuration);

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration)
        .WriteTo.Console(new CompactJsonFormatter()));

//after build
var app = builder.Build();

app.UseExceptionHandling();
app.UseAuthResponse();

if (isDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLogging();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Services.ApplyMigrations();

app.MapControllers();
app.MapPrometheusScrapingEndpoint();

app.Run();
