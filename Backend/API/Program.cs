using API.MiddleWare;
using Application.Interfaces;
using Infrastructure.Configuration;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Configure Serilog 
Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // keep last 7 days 
                    .CreateLogger(); 

// Use Serilog instead of default logger
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<IDatabaseContext, DatabaseContext>();
builder.Services.AddSingleton<IAuthService, AuthService>();

ConfigurationHelper.Initialize(builder.Configuration);
var app = builder.Build();

// Custom middlewate
app.UseWhen(
    context => context.Request.Query.ContainsKey("Token"),
    app =>
    {
        app.UseAuthMiddleware();
    });


app.UseHttpsRedirection();
app.MapControllers();
app.Run();
