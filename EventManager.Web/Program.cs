using EventManager.Application;
using EventManager.Infrastructure;
using EventManager.Infrastructure.Persistence;
using EventManager.Middleware;
using EventManager.Web;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddInfrastructure();
builder.Services.AddPresentation();

builder.Logging.AddConsole();

//Db Context
var connectionString = builder.Configuration.GetConnectionString("Default")
                       ?? throw new InvalidOperationException("Connection string 'Default' not found");
builder.Services.AddDbContext<AppDbContext>(options => options
    .UseNpgsql(connectionString)
    .LogTo(Console.WriteLine)
    .EnableDetailedErrors());

//after build
var app = builder.Build();

app.UseExceptionHandling();

if (isDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLogging();
app.UseHttpsRedirection();
app.UseRouting();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapControllers();

app.Run();
