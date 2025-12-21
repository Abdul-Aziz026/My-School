using API.MiddleWare;
using Application.Interfaces;
using Infrastructure.Configuration;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog 
Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // keep last 7 days 
                    .CreateLogger();

// Use Serilog instead of default logger
builder.Host.UseSerilog();

// Initialize Configuration Helper
ConfigurationHelper.Initialize(builder.Configuration);

// Register JWT Authentication
builder.Services.AddAuthentication("Bearer") // Use JWT Bearer tokens to authenticate requests
     // registers the JWT Bearer authentication handler
    .AddJwtBearer("Bearer", options =>
    {
        // 
        //options.Events = new JwtBearerEvents
        //{
        //    OnMessageReceived = context =>
        //    {
        //        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        //        Log.Information($"OnMessageReceived - Token: {token?.Substring(0, 20)}...");
        //        return Task.CompletedTask;
        //    },
        //    OnAuthenticationFailed = context =>
        //    {
        //        Log.Error($"OnAuthenticationFailed: {context.Exception.GetType().Name} - {context.Exception.Message}");
        //        return Task.CompletedTask;
        //    },
        //    OnTokenValidated = context =>
        //    {
        //        Log.Information($"OnTokenValidated - User: {context.Principal?.Identity?.Name}");
        //        return Task.CompletedTask;
        //    },
        //    OnChallenge = context =>
        //    {
        //        Log.Warning($"OnChallenge - Error: {context.Error}, ErrorDescription: {context.ErrorDescription}");
        //        return Task.CompletedTask;
        //    }
        //};

        //var secretKey = ConfigurationHelper.GetConfigurationValue("Jwt:SecretKey");
        //var issuer = ConfigurationHelper.GetConfigurationValue("Jwt:Issuer");
        //var audience = ConfigurationHelper.GetConfigurationValue("Jwt:Audience");

        //Log.Information($"JWT Config - Issuer: {issuer}, Audience: {audience}, KeyLength: {secretKey?.Length}");

        // Configure token validation parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Validate the token issuer
            ValidateAudience = true, // Validate the token audience
            ValidateIssuerSigningKey = true, // Validate the signing key
            ValidateLifetime = true,

            ValidIssuer = ConfigurationHelper.GetConfigurationValue("Jwt:Issuer"), // Expected issuer
            ValidAudience = ConfigurationHelper.GetConfigurationValue("Jwt:Audience"), // Expected audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationHelper.GetConfigurationValue("Jwt:SecretKey"))) // Signing key
        };
    });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<IDatabaseContext, DatabaseContext>();
builder.Services.AddSingleton<IAuthService, AuthService>();

// Learn more about configuring OpenAPI 
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // add Scalar.AspNetCore...
}

// Configure the HTTP request pipeline.
// app.UseHttpsRedirection();

// authentication & authorization middlewares
app.UseAuthentication();
app.UseAuthorization();

// Custom middlewate
//app.UseWhen(
//    context => context.Request.Headers.ContainsKey("Authorization"),
//    app =>
//    {
//        app.UseAuthMiddleware();
//    });
app.UseAuthMiddleware();


// Configure routing for controllers
app.MapControllers();
app.Run();
