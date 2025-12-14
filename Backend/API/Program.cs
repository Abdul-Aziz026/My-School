using API.MiddleWare;
using Application.Interfaces;
using Infrastructure.Configuration;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Initialize Configuration Helper
ConfigurationHelper.Initialize(builder.Configuration);

// Register JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Use JWT Bearer tokens to authenticate requests
     // registers the JWT Bearer authentication handler
    .AddJwtBearer(options =>
    {
        // Configure token validation parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Validate the token issuer
            ValidateAudience = true, // Validate the token audience
            ValidateIssuerSigningKey = true, // Validate the signing key
            ValidateLifetime = true,

            ValidIssuer = ConfigurationHelper.GetConfigurationValue("Jwt:Issuer"), // Expected issuer
            ValidAudience = ConfigurationHelper.GetConfigurationValue("Jwt:Audience"), // Expected audience
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(ConfigurationHelper.GetConfigurationValue("Jwt:SecretKey"))) // Signing key
        };
    });

builder.Services.AddAuthorization();


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

var app = builder.Build();

// authentication & authorization middlewares
app.UseAuthentication();
app.UseAuthorization();

// Custom middlewate
app.UseWhen(
    context => context.Request.Query.ContainsKey("Token"),
    app =>
    {
        app.UseAuthMiddleware();
    });


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
// Configure routing for controllers
app.MapControllers();
app.Run();
