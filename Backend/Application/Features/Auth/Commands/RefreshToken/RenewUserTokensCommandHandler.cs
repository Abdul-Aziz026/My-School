
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Features.Auth.DTOs;
using Application.Features.Users.DTOs;
using Domain.Entities;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace Application.Features.Auth.Commands.RefreshToken;


/// <summary>
/// Using old refresh token for new access token and refresh token
/// </summary>
/// <param name="token">old refresh token</param>
/// <returns>new Access Token and new Refresh Token</returns>
public class RenewUserTokensCommandHandler : IRequestHandler<RenewUserTokensCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _irefreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    public RenewUserTokensCommandHandler(IUserRepository userRepository,
                                         IRefreshTokenRepository irefreshTokenRepository,
                                         IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _irefreshTokenRepository = irefreshTokenRepository;
        _jwtTokenService = jwtTokenService;
    }
    public async Task<AuthResponse> Handle(RenewUserTokensCommand request, CancellationToken cancellationToken)
    {
        var token = request.RefreshToken;
        var response = new AuthResponse();
        var tokenHash = _jwtTokenService.ComputeTokenHash(token);
        var refreshToken = await _irefreshTokenRepository.GetByTokenHashAsync(tokenHash);
        if (refreshToken is null)
        {
            response.Status = ResultStatus.InvalidToken;
            response.ErrorMessage = "Invalid or Expired access token...";
            return response;
        }
        if (refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            response.Status = ResultStatus.Revoked;
            response.ErrorMessage = "Invalid or Expired access token...";
            return response;
        }

        var user = await _userRepository.GetByIdAsync<User>(refreshToken.UserId);
        if (user is null)
        {
            response.Status = ResultStatus.Failed;
            response.ErrorMessage = "User not found...";
            return response;
        }

        // Check if user account is locked
        if (!user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            return new AuthResponse
            {
                Status = ResultStatus.Failed,
                ErrorMessage = "Account is locked"
            };
        }

        // revoke the old refresh token...
        refreshToken.IsRevoked = true;
        await _irefreshTokenRepository.UpdateAsync(refreshToken);

        // Generate new tokens
        var tokenResponse = await _jwtTokenService.GenerateTokenResponseAsync(user);

        // save refresh token
        await _irefreshTokenRepository.AddAsync(tokenResponse?.userRefreshTokenEntity!);

        response.Status = ResultStatus.Succeeded;
        response.AccessToken = tokenResponse?.AccessToken!;
        response.AccessTokenExpiry = tokenResponse!.AccessTokenExpiry;
        response.RefreshToken = tokenResponse.RefreshToken;
        response.RefreshTokenExpiry = tokenResponse.RefreshTokenExpiry;
        response.User = user.ToUserDtoResponse();
        return response;
    }
}
