using API.Extensions;
using API.MiddleWare;
using Infrastructure.Helper;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog 
Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // keep last 7 days 
                    .CreateLogger();

// Use Serilog instead of default logger
builder.Host.UseSerilog();

builder.Services.AddConfigurationSettings(builder.Configuration);
// Initialize Configuration Helper
ConfigurationHelper.Initialize(builder.Configuration);
builder.Services.AddAuditLogging();
builder.Services.AddRateLimitingServices();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
// swagger UI...
builder.Services.AddSwaggerGen();

builder.Services.AddJwtAuthentication();
builder.Services.AddApplicationServices();

var app = builder.Build();

// Use "GlobalLimiter" as rate limiting middleware globally 1000 requests per minute...
app.UseRateLimiter();

// Configure the HTTP request pipeline...
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // add Scalar.AspNetCore...
    // Swagger UI...
    app.UseSwagger();
    app.UseSwaggerUI();
}

// authentication & authorization middlewares...
app.UseAuthentication();
app.UseGlobalExceptionMiddleware();
app.UseAuthMiddleware();
app.UseAuditMiddleware();
app.UseAuthorization();

app.MapControllers();
app.Run();
