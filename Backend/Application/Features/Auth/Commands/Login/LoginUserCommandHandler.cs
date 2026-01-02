using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Features.Auth.DTOs;
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
public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, RefreshTokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly AuthLockoutSettings _authLockoutSettings;
    public LoginUserCommandHandler(IUserRepository userRepository, 
                                   IJwtTokenService jwtTokenService,
                                   IOptions<AuthLockoutSettings> authLockoutSettings)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _authLockoutSettings = authLockoutSettings.Value;
    }
    public async Task<RefreshTokenResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var response = new RefreshTokenResponse();
        var loginUser = request.LoginDto;
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
            return null!;
        }
        user.FailedLoginAttempts++;
        if (user.FailedLoginAttempts >= _authLockoutSettings.MaxFailedLoginAttempts)
        {
            user.LockoutEnd = DateTime.Now.AddMinutes(_authLockoutSettings.LockoutDuration);
            user.LockoutEnabled = false;
            await _userRepository.UpdateAsync<User>(user);

            response.ErrorMessage = $"Account locked due to many failed attempts. Try again after {_authLockoutSettings.LockoutDuration} minutes.";
            return response;
        }
        return await _jwtTokenService.GenerateTokenResponseAsync(user);
    }
}
