using Application.Common.Interfaces.Publisher;
using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Logout;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.Queries.GetCurrentUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly IMessageBus _messageBus;
    public AuthController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    [HttpPost("register")]
    [EnableRateLimiting("register")] // Apply rate limiting to register endpoint 3 attempts per 1 hour
    public async Task<ActionResult<AuthResponse>> Register(RegisterDto registerUserRequest)
    {
        var command = registerUserRequest.ToRegisterUserCommand();
        var response = await _messageBus.SendAsync<RegisterUserCommand, AuthResponse>(command);

        if (response.Status != ResultStatus.Succeeded)
        {
            return BadRequest(new { Error = response.ErrorMessage });
        }

        await SetRefreshTokenInCookie(response.RefreshToken, response.RefreshTokenExpiry);
        return Ok(response);
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")] // Apply rate limiting to login endpoint 5 attempts per 1 minute
    public async Task<ActionResult<AuthResponse>> Login(LoginDto loginDto)
    {
        var command = loginDto.ToLoginUserCommand();
        var response = await _messageBus.SendAsync<LoginUserCommand, AuthResponse>(command);

        if (response.Status != ResultStatus.Succeeded)
        {
            throw new UnauthorizedAccessException(response.ErrorMessage);
        }

        await SetRefreshTokenInCookie(response.RefreshToken, response.RefreshTokenExpiry);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(object? obj)
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedAccessException("Refresh token not found");
        }
        var command = new RenewUserTokensCommand(refreshToken);
        var response = await _messageBus.SendAsync<RenewUserTokensCommand, AuthResponse>(command);

        if (response.Status != ResultStatus.Succeeded)
        {
            // Clear invalid refresh token cookie
            Response.Cookies.Delete("refreshToken");
            throw new UnauthorizedAccessException("Refresh token not found");
        }

        await SetRefreshTokenInCookie(response.RefreshToken, response.RefreshTokenExpiry);
        return Ok(response);
    }


    [HttpPost("logout")]
    //[Authorize]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var command = new LogoutUserCommand(refreshToken!);
        await _messageBus.SendAsync<LogoutUserCommand>(command);
        // Secure cookie deletion
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,     // must be true in production
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });

        return Ok(new { message = "Logged out successfully" });
    }

    [EnableRateLimiting("api")]
    //[Authorize]// Ensure this endpoint requires authentication
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserDtoResponse>> Me()
    {
        var query = new GetCurrentUserQuery();
        var response = await _messageBus.SendAsync<GetCurrentUserQuery, CurrentUserDtoResponse>(query);

        if (!response.IsSuccess)
        {
            return NotFound(new { Error = response.ErrorMessage });
        }

        return Ok(response);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var command = new ForgotPasswordCommand(dto);
        await _messageBus.SendAsync<ForgotPasswordCommand>(command);
        return Ok(new { Message = "If the account exists, a password reset email has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        var command = request.ToResetPasswordCommand();
        await _messageBus.SendAsync<ResetPasswordCommand>(command);
        return Ok(new { Message = "Password has been reset successfully." });
    }

    private async Task SetRefreshTokenInCookie(string refreshToken, DateTime refreshTokenExpiry)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,          // localhost
            SameSite = SameSiteMode.Lax,
            Expires = refreshTokenExpiry,
            Path = "/"
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    private string GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Subject;
    }

    private static string? GetUserIdFromClaims(ClaimsPrincipal user)
    {
        // prefer 'sub', fallback to NameIdentifier
        return user?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
