using Application.Common.Helper;
using Application.Common.Interfaces.Publisher;
using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Logout;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.Queries.GetUser;
using Application.Features.Users.Commands.UpdateUser;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<ActionResult<AuthResponse>> Register(RegisterDto user)
    {
        try
        {
            var command = new RegisterUserCommand(user);
            var tokenResponse = await _messageBus.SendAsync<RegisterUserCommand, RefreshTokenResponse>(command);
            var getUserCommand = new GetUserByEmailQuery(TellMe.Email!);
            var userInfo = await _messageBus.SendAsync<GetUserByEmailQuery, UserInfo>(getUserCommand);

            var response = new AuthResponse()
            {
                Status = ResultStatus.Succeeded,
                Token = tokenResponse.AccessToken,
                TokenExpiry = tokenResponse.AccessTokenExpiry,
                User = userInfo!
            };

            await SetRefreshTokenInCookie(tokenResponse.RefreshToken, tokenResponse.RefreshTokenExpiry);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "An error occurred during registration" });
        }
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")] // Apply rate limiting to login endpoint 5 attempts per 1 minute
    public async Task<ActionResult<AuthResponse>> Login(LoginDto user)
    {
        var command = new LoginUserCommand(user);
        var tokenResponse = await _messageBus.SendAsync<LoginUserCommand, RefreshTokenResponse>(command);

        if (tokenResponse is null)
        {
            return Unauthorized(new { Error = "Invalid email or password" });
        }
        var getUserCommand = new GetUserByEmailQuery(TellMe.Email!);
        var userInfo = await _messageBus.SendAsync<GetUserByEmailQuery, UserInfo>(getUserCommand);
        var response = new AuthResponse
        {
            Status = ResultStatus.Succeeded,
            Token = tokenResponse.AccessToken,
            TokenExpiry = tokenResponse.AccessTokenExpiry,
            User = userInfo!
        };

        await SetRefreshTokenInCookie(tokenResponse.RefreshToken, tokenResponse.RefreshTokenExpiry);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { Error = "Refresh token not found" });
        }
        try
        {
            var command = new RenewUserTokensCommand(refreshToken);
            var tokenResponse = await _messageBus.SendAsync<RenewUserTokensCommand, RefreshTokenResponse>(command);
            var getUserCommand = new GetUserByEmailQuery(TellMe.Email!);
            var userInfo = await _messageBus.SendAsync<GetUserByEmailQuery, UserInfo>(getUserCommand);

            var response = new AuthResponse
            {
                Status = ResultStatus.Succeeded,
                Token = tokenResponse.AccessToken,
                TokenExpiry = tokenResponse.AccessTokenExpiry,
                User = userInfo!
            };

            await SetRefreshTokenInCookie(tokenResponse.RefreshToken, tokenResponse.RefreshTokenExpiry);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            Response.Cookies.Delete("refreshToken");
            return Unauthorized(new { Error = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("updateProfile")]
    public async Task<IActionResult> UpdateProfile(UpdateUserProfileDto user)
    {
        var command = new UpdateUserCommand(user);
        command.UserId = TellMe.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        await _messageBus.SendAsync<UpdateUserCommand>(command);
        return Ok("Update profile Successfully.");
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
    //[Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserInfo>> Me()
    {
        var userId = GetUserIdFromClaims(User);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new { Error = "Invalid token or user id not found" });
        }

        var getUserCommand = new GetUserByEmailQuery(TellMe.Email!);
        var userInfo = await _messageBus.SendAsync<GetUserByEmailQuery, UserInfo>(getUserCommand);
        if (userInfo is null)
        {
            return NotFound(new { Error = "User not found" });
        }

        return Ok(userInfo);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var command = new ForgotPasswordCommand(dto);
        await _messageBus.SendAsync<ForgotPasswordCommand>(command);
        return Ok(new { Message = "If the account exists, a password reset email has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var command = new ResetPasswordCommand(dto);
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
