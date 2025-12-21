using Application.DTOs;
using Application.Helper;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterDto user)
    {
        try
        {
            var tokenResponse = await _authService.RegisterAsync(user);
            var userInfo = await _authService.GetUserInfoAsync(GetUserIdFromToken(tokenResponse.AccessToken));

            var response = new AuthResponse
            {
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
    public async Task<ActionResult<AuthResponse>> Login(LoginDto user)
    {
        var tokenResponse = await _authService.LoginAsync(user);

        if (tokenResponse is null)
        {
            return Unauthorized(new { Error = "Invalid email or password" });
        }

        var userInfo = await _authService.GetUserInfoAsync(GetUserIdFromToken(tokenResponse.AccessToken));

        var response = new AuthResponse
        {
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
            var tokenResponse = await _authService.RefreshTokenAsync(refreshToken);
            var userInfo = await _authService.GetUserInfoAsync(GetUserIdFromToken(tokenResponse.AccessToken));

            var response = new AuthResponse
            {
                Token = tokenResponse.AccessToken,
                TokenExpiry = tokenResponse.AccessTokenExpiry,
                User = userInfo
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
    [HttpPost("logout")]
    //[Authorize] // Require authentication
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.RevokeRefreshTokenAsync(refreshToken);
        }

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Use true in production
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });

        return Ok(new { Message = "Logged out successfully" });
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
        // jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value // ✅ Also reads "sub"
    }
}
