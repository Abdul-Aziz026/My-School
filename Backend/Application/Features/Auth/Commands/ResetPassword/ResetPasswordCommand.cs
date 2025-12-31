
using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest
{
    public ResetPasswordDto ResetPasswordDto { get; }
    public ResetPasswordCommand(ResetPasswordDto resetPasswordDto)
    {
        ResetPasswordDto = resetPasswordDto;
    }
}
