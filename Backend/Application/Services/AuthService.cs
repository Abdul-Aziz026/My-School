using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Settings;
using Domain.Entities;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEmailService _emailService;

    private readonly AuthLockoutSettings _authLockoutSettings;

    public AuthService(IUserRepository userRepository,
                       IRefreshTokenRepository refreshTokenRepository,
                       IJwtTokenService jwtTokenService,
                       IEmailService emailService,
                       IOptions<AuthLockoutSettings> authLockoutSettings)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;

        _authLockoutSettings = authLockoutSettings.Value;
        _emailService = emailService;
    }


    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registerUser">including user name, email, and password.</param>
    /// <returns>A JWT token as a string if registration is successful; otherwise, null</returns>
    public async Task<RefreshTokenResponse> RegisterAsync(RegisterDto registerUser)
    {
        var response = new RefreshTokenResponse();
        var user = await _userRepository.GetByEmailAsync(registerUser.Email);
        if (user is not null)
        {
            response.Result = ActionEvent.Failed;
            return response;
        }
        // Hash the password with BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUser.Password);
        var newUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = registerUser.UserName,
            Email = registerUser.Email,
            PasswordHash = passwordHash,
            Roles = new List<string> { "User" }, // Default role
            Permissions = new List<string> { "ViewProduct" }, // Default permission
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync<User>(newUser);
        await _emailService.SendEmailAsync(newUser.Email,
                                               newUser.UserName,
                                               "Register Successfully in My School",
                                               "<h1>Welcome to Our Service!</h1><p>Thank you for registering, " + newUser.UserName + ".</p>"
                                            );
        return await _jwtTokenService.GenerateTokenResponseAsync(newUser);
    }

    /// <summary>
    /// User login method
    /// </summary>
    /// <param name="loginUser"></param>
    /// <returns>A JWT token as a string if login is successful; otherwise, null</returns>
    public async Task<RefreshTokenResponse?> LoginAsync(LoginDto loginUser)
    {
        var response = new RefreshTokenResponse();
        //Expression<Func<User, bool>> filter = o => true;
        //filter = filter.And(u => u.Email == loginUser.Email);
        var user = await _userRepository.GetByEmailAsync(loginUser.Email);
        if (user is null)
        {
            response.Result = ActionEvent.Failed;
            response.ErrorMessage = "Invalid email or password.";
            return response;
        }

        if (!user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            var remaining = user.LockoutEnd.Value - DateTime.Now;
            response.ErrorMessage = $"Account locked. Try again in {remaining.Minutes} minutes and {remaining.Seconds} seconds.";
            response.Result = ActionEvent.Failed;
            return response;
        }

        var passwordMatches = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.PasswordHash);
        if (!passwordMatches)
        {
            return null;
        }
        user.FailedLoginAttempts++;
        if (user.FailedLoginAttempts >= _authLockoutSettings.MaxFailedLoginAttempts)
        {
            user.LockoutEnd = DateTime.Now.AddMinutes(_authLockoutSettings.LockoutDuration);
            user.LockoutEnabled = false;
            await _userRepository.UpdateAsync<User>(user);

            response.ErrorMessage =  $"Account locked due to many failed attempts. Try again after {_authLockoutSettings.LockoutDuration} minutes.";
            return response;
        }
        return await _jwtTokenService.GenerateTokenResponseAsync(user);
    }

    /// <summary>
    /// Using old refresh token for new access token and refresh token
    /// </summary>
    /// <param name="token">old refresh token</param>
    /// <returns>new Access Token and new Refresh Token</returns>
    /// <exception cref="UnauthorizedAccessException">Invalid, expired or revoked refresh token</exception>
    public async Task<RefreshTokenResponse> RefreshTokenAsync(string token)
    {
        var response = new RefreshTokenResponse();
        var tokenHash = ComputeTokenHash(token);
        var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);
        if (refreshToken is null)
        {
            response.Result = ActionEvent.InvalidToken;
            response.ErrorMessage = "Invalid or Expired access token...";
            return response;
        }
        if (refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            response.Result = ActionEvent.Revoked;
            response.ErrorMessage = "Invalid or Expired access token...";
            return response;
        }
        var user = await _userRepository.GetByIdAsync<User>(refreshToken.UserId);
        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found");
        }
        // revoke the old refresh token...
        refreshToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(refreshToken);
        // Generate new tokens
        return await _jwtTokenService.GenerateTokenResponseAsync(user);
    }

    public async Task<UserInfo?> GetUserInfoAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync<User>(userId);
        if (user is null)
        {
            return null;
        }
        return new UserInfo
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            ProfilePicture = user.ProfilePicture,
            Roles = user.Roles,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var tokenHash = ComputeTokenHash(refreshToken);
        var refreshTokenResponse = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

        if (refreshTokenResponse is not null && !refreshTokenResponse.IsRevoked)
        {
            refreshTokenResponse.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(refreshTokenResponse);
        }
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        if (dto is null || string.IsNullOrWhiteSpace(dto.Email))
            return; // idempotent, do not reveal

        try
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user is null)
                return; // idempotent, do not reveal

            var rawToken = GenerateSecureToken();
            var tokenHash = ComputeTokenHash(rawToken);

            user.PasswordResetTokenHash = tokenHash;
            user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(15); // Token valid for 15 minutes

            await _userRepository.UpdateAsync(user);
            var frontendBase = $"http://localhost:5000";
            var resetLink = string.IsNullOrWhiteSpace(frontendBase) ?
                $"http://localhost:5000/api/auth/reset-password?token={rawToken}&email={Uri.EscapeDataString(user.Email)}"
                : $"{frontendBase}/api/auth/reset-password?token={rawToken}&email={Uri.EscapeDataString(user.Email)}";

            var subject = "Reset your password";
            var body = $"<p>Click the link below to reset your password. This link is valid for 15 minutes.</p>" +
                       $"<a href='{resetLink}'>Reset Password</a>";
            var sent = await _emailService.SendEmailAsync(user.Email, user.UserName, subject, body);
            if (sent)
            {
                Console.WriteLine("Password reset email sent for user {UserId}", user.Id);
            }
            else
            {
                Console.WriteLine("Password reset email failed to send for user {UserId}", user.Id);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex}Error in ForgotPasswordAsync for {dto.Email}");
            // Do not throw to caller to avoid leaking internal state
        }
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        if (dto is null || string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.NewPassword))
            throw new InvalidOperationException("Invalid password reset request.");
        try
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if(user is null || user.PasswordResetTokenHash != dto.Token)
            {
                Console.WriteLine("ResetPassword attempted for non-existent email {Email}", dto.Email);
                throw new InvalidOperationException("Invalid token or email."); // generic
            }

            // Verify token: compare stored hash and check expiry
            if (string.IsNullOrWhiteSpace(user.PasswordResetTokenHash) || !user.PasswordResetExpiry.HasValue || user.PasswordResetExpiry.Value < DateTime.UtcNow)
            {
                Console.WriteLine("ResetPassword token missing or expired for user {UserId}", user.Id);
                throw new InvalidOperationException("Invalid or expired token.");
            }
            if (user.PasswordResetExpiry < DateTime.UtcNow)
            {
                Console.WriteLine("Token expired");
                throw new InvalidOperationException("Token has expired.");
            }

            // Update password
            var newHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.PasswordHash = newHash;

            // clear and reset all the fields
            user.PasswordResetTokenHash = null;
            user.PasswordResetExpiry = null;
            user.FailedLoginAttempts = 0;
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            var updated = await _userRepository.UpdateAsync(user);
            if (updated)
            {
                Console.WriteLine("Password reset successful for user {UserId}", user.Id);
            }
            else
            {
                Console.WriteLine($"Password reset failed during update for user {user.Id}");
                throw new InvalidOperationException("Failed to reset password.");
            }

        }
        catch { }
    }
    private string ComputeTokenHash(string token)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
    private static string GenerateSecureToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
