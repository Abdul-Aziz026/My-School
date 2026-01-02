
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Features.Auth.DTOs;
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
/// <exception cref="UnauthorizedAccessException">Invalid, expired or revoked refresh token</exception>
public class RenewUserTokensCommandHandler : IRequestHandler<RenewUserTokensCommand, RefreshTokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    public RenewUserTokensCommandHandler(IUserRepository userRepository,
                                         IRefreshTokenRepository refreshTokenRepository,
                                         IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
    }
    public async Task<RefreshTokenResponse> Handle(RenewUserTokensCommand request, CancellationToken cancellationToken)
    {
        var token = request.RefreshToken;
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

    private string ComputeTokenHash(string token)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
}
