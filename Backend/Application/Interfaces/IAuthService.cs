
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse>? LoginAsync(LoginDto user);
    Task<string> RegisterAsync(RegisterDto user);
    Task<AuthResponse> CreateJwtToken(User user);
}
