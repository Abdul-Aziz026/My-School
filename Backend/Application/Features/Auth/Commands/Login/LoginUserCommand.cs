using Application.Features.Auth.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Auth.Commands.Login;

public class LoginUserCommand : IRequest<AuthResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
