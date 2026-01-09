using Application.Features.Auth.Commands.ResetPassword;

namespace Application.Features.Auth.DTOs;

public class ResetPasswordDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public ResetPasswordCommand ToResetPasswordCommand()
    {
        return new ResetPasswordCommand()
        {
            Token = this.Token,
            Email = this.Email,
            NewPassword = this.NewPassword
        };
    }
}
