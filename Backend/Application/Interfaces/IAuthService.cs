
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<string>? LoginAsync(LoginDto user);
    Task<string> RegisterAsync(RegisterDto user);
    Task<string> CreateJwtToken(User user);
}
