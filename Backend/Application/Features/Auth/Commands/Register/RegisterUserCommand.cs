using Application.Features.Auth.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Auth.Commands.Register;

public class RegisterUserCommand : IRequest<AuthResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
