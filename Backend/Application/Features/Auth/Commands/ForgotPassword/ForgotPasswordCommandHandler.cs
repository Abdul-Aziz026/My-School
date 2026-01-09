
using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Validator;
using Contracts.Events;
using MediatR;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IHostEnvironment _environment;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMessageBus _bus;
    public ForgotPasswordCommandHandler(IUserRepository userRepository,
                                        IJwtTokenService jwtTokenService,
                                        IHostEnvironment environment,
                                        IMessageBus bus)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _environment = environment;
        _bus = bus;
    }
    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var dto = request.ForgotPasswordDto;
        EmailValidator.Validate(dto.Email);

        try
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user is null)
                return; // idempotent, do not reveal

            var rawToken = Guid.NewGuid().ToString();
            if (_environment.EnvironmentName == "Test")
            {
                rawToken = "abcdefghijklmnopqrstuvwxyz";
            }
            var tokenHash = _jwtTokenService.ComputeTokenHash(rawToken);

            user.PasswordResetTokenHash = tokenHash;
            user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(15); // Token valid for 15 minutes

            await _userRepository.UpdateAsync(user);
            var frontendBase = $"http://localhost:5000";
            var resetLink = string.IsNullOrWhiteSpace(frontendBase) ?
                $"http://localhost:5000/api/auth/reset-password?token={rawToken}&email={Uri.EscapeDataString(user.Email)}"
                : $"{frontendBase}/api/auth/reset-password?token={rawToken}&email={Uri.EscapeDataString(user.Email)}";

            var subject = "Reset your password";
            var body = $"<p>Click the link below to reset your password. This link is valid for 15 minutes.</p>" +
                       $"<a href='{resetLink}'>Reset Password</a>";

            var emailSendCommand = new SendEmailCommand()
            {
                ToMail = user.Email,
                Name = user.UserName,
                Subject = subject,
                Body = body,
            };
            await _bus.PublishAsync(emailSendCommand);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something Error happen for password change: {ex.Message}");
        }
        return;
    }
}
