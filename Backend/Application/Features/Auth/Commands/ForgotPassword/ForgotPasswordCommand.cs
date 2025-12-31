using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest
{
    public ForgotPasswordDto ForgotPasswordDto { get; }
    public ForgotPasswordCommand(ForgotPasswordDto forgotPasswordDto)
    {
        ForgotPasswordDto = forgotPasswordDto;
    }
}
