using Application.Features.Auth.DTOs;
using Domain.Entities;

namespace Application.Common.Interfaces.Services;

public interface IJwtTokenService
{
    Task<RefreshTokenResponse> GenerateTokenResponseAsync(User newUser);
}
