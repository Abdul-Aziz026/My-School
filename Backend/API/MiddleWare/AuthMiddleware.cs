using Application.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.MiddleWare;

// This is authorization helper middleware...
public class AuthMiddleware(RequestDelegate _next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var roles = context.User
                    .FindAll(ClaimTypes.Role)
                    .Select(r => r.Value)
                    .ToList();
                var currentUserContext = new CurrentUserContext
                {
                    UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Email = context.User.FindFirstValue(JwtRegisteredClaimNames.Email),
                    Roles = roles,
                    Jti = context.User.FindFirstValue(JwtRegisteredClaimNames.Jti)
                };
                TellMe.SetCurrentUserContext(currentUserContext);

                TellMe.IpAddress =
                    context.Connection.RemoteIpAddress?.ToString() ?? "";
            }
            await _next(context);
        }
        finally
        {
            // 🔥 CRITICAL: cleanup after request
            TellMe.ClearCurrentUserContext();
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class AuthMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthMiddleware>();
    }
}
