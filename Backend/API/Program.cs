using API.MiddleWare;
using Application.Authorization;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Infrastructure.Helper;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure rate limiting
builder.Services.AddRateLimiter(options =>
{
    // Global rate limiter: 1000 requests per minute with a queue of 100
    options.AddFixedWindowLimiter("GlobalLimiter", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);    // Time window of 1 minute
        opt.PermitLimit = 1000;                   // Allow 100 requests per minute
        opt.QueueLimit = 100;                      // Queue limit of 5
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // Policy 2: Login endpoint - 5 attempts per 10 minutes
    options.AddFixedWindowLimiter("login", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(10);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2; // No queuing
    });

    // Policy 3: Registration endpoint - 3 attempts per hour
    options.AddFixedWindowLimiter("register", opt =>
    {
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromHours(1);
        opt.QueueLimit = 0;
    });

    // Policy 4: API endpoint - 100 requests per minute
    options.AddSlidingWindowLimiter("api", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.SegmentsPerWindow = 6; // 10-second segments
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

});

// Configure Serilog 
Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // keep last 7 days 
                    .CreateLogger();

// Use Serilog instead of default logger
builder.Host.UseSerilog();

// Initialize Configuration Helper
ConfigurationHelper.Initialize(builder.Configuration);

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
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

        options.MapInboundClaims = false;
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanViewUsers", policy =>
        //policy.Requirements.Add(new PermissionRequirement(Permissions.ViewUsers)));
        policy.Requirements.Add(new PermissionRequirement("CanViewUsersPermission")));
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();

// Learn more about configuring OpenAPI 
builder.Services.AddOpenApi();

var app = builder.Build();

// Use "GlobalLimiter" as rate limiting middleware globally 1000 requests per minute
app.UseRateLimiter(new RateLimiterOptions
{
    OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests (GlobalLimiter)", cancellationToken);
    },
});

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
app.UseAuthMiddleware();
app.UseAuthorization();

// Custom middlewate
//app.UseWhen(
//    context => context.Request.Headers.ContainsKey("Authorization"),
//    app =>
//    {
//        app.UseAuthMiddleware();
//    });


// Configure routing for controllers
app.MapControllers();
app.Run();
