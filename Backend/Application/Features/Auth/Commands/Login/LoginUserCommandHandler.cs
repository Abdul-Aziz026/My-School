using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Features.Auth.DTOs;
using Application.Features.Users.DTOs;
using Application.Settings;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.Auth.Commands.Login;


/// <summary>
/// User login method
/// </summary>
/// <param name="loginUser"></param>
/// <returns>A JWT token as a string if login is successful; otherwise, null</returns>
public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _irefreshTokenRepository;
    private readonly AuthLockoutSettings _authLockoutSettings;
    public LoginUserCommandHandler(IUserRepository userRepository, 
                                   IJwtTokenService jwtTokenService,
                                   IOptions<AuthLockoutSettings> authLockoutSettings,
                                   IRefreshTokenRepository irefreshTokenRepository  )
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _authLockoutSettings = authLockoutSettings.Value;
        _irefreshTokenRepository = irefreshTokenRepository;
    }
    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var response = new AuthResponse();
        var loginUser = request;
        var user = await _userRepository.GetByEmailAsync(loginUser.Email.ToLower());
        if (user is null)
        {
            response.Status = ResultStatus.Failed;
            response.ErrorMessage = "Invalid email or password.";
            return response;
        }

        if (!user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            var remaining = user.LockoutEnd.Value - DateTime.Now;
            response.ErrorMessage = $"Account locked. Try again in {remaining.Minutes} minutes and {remaining.Seconds} seconds.";
            response.Status = ResultStatus.Failed;
            return response;
        }

        var passwordMatches = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.PasswordHash);
        if (!passwordMatches)
        {
            user.FailedLoginAttempts++;
            // Check if we need to lock the account
            if (user.FailedLoginAttempts >= _authLockoutSettings.MaxFailedLoginAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(_authLockoutSettings.LockoutDuration);
                user.LockoutEnabled = false;
                await _userRepository.UpdateAsync(user);

                response.Status = ResultStatus.Failed;
                response.ErrorMessage = $"Account locked due to too many failed attempts. Try again after {_authLockoutSettings.LockoutDuration} minutes.";
                return response;
            }

            await _userRepository.UpdateAsync(user);
            response.Status = ResultStatus.Failed;
            response.ErrorMessage = "Invalid email or password.";
            return response;
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LockoutEnabled = true;
        
        await _userRepository.UpdateAsync<User>(user);

        var tokenResult = await _jwtTokenService.GenerateTokenResponseAsync(user);
        
        // save refresh token
        await _irefreshTokenRepository.AddAsync(tokenResult?.userRefreshTokenEntity!);

        response.Status = ResultStatus.Succeeded;
        response.AccessToken = tokenResult?.AccessToken!;
        response.AccessTokenExpiry = tokenResult!.AccessTokenExpiry;
        response.RefreshToken = tokenResult.RefreshToken;
        response.RefreshTokenExpiry = tokenResult.RefreshTokenExpiry;
        response.User = user.ToUserDtoResponse();

        return response;
    }
}
