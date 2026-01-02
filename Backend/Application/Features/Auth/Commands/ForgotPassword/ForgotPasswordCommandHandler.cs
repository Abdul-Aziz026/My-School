
using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Repositories;
using Contracts.Events;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageBus _bus;
    public ForgotPasswordCommandHandler(IUserRepository userRepository,
                                        IMessageBus bus)
    {
        _userRepository = userRepository;
        _bus = bus;
    }
    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var dto = request.ForgotPasswordDto;
        if (dto is null || string.IsNullOrWhiteSpace(dto.Email))
            return ; // idempotent, do not reveal

        try
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user is null)
                return; // idempotent, do not reveal

            var rawToken = GenerateSecureToken();
            var tokenHash = ComputeTokenHash(rawToken);

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

    private string ComputeTokenHash(string token)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
    private static string GenerateSecureToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string Base64UrlEncode(byte[] input)
    {
        // Convert to Base64, then make URL-safe
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
