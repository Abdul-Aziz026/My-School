using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Repositories;
using Contracts.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageBus _messageBus;
    public ResetPasswordCommandHandler(IUserRepository userRepository,
                                       IMessageBus messageBus)
    {
        _userRepository = userRepository;
        _messageBus = messageBus;
    }
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var dto = request;
        if (dto is null ||
            string.IsNullOrWhiteSpace(dto.Token) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            throw new ArgumentNullException("Invalid password reset request.");
        }

        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if ( user is null || 
            string.IsNullOrWhiteSpace(dto.NewPassword) ||
            dto.NewPassword.Length < 6 ||
            dto.NewPassword.Length > 50 )
        {
            Console.WriteLine("ResetPassword attempted failed");
            throw new ArgumentException("Invalid token or email."); // generic
        }

        // Verify token: compare stored hash and check expiry
        if (string.IsNullOrWhiteSpace(user.PasswordResetTokenHash) || !user.PasswordResetExpiry.HasValue || user.PasswordResetExpiry.Value < DateTime.UtcNow)
        {
            Console.WriteLine("ResetPassword token missing or expired for user {UserId}", user.Id);
            throw new InvalidOperationException("Invalid or expired token.");
        }
        if (user.PasswordResetExpiry < DateTime.UtcNow)
        {
            Console.WriteLine("Token expired");
            throw new InvalidOperationException("Token has expired.");
        }

        // Update password
        var newHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.PasswordHash = newHash;

        // clear and reset all the fields
        user.PasswordResetTokenHash = null;
        user.PasswordResetExpiry = null;
        user.FailedLoginAttempts = 0;
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;

        var updated = await _userRepository.UpdateAsync(user);
        var mailSendCommand = new SendEmailCommand()
        {
            ToMail = user.Email,
            Name = user.UserName,
            Subject = "Your password has been reset",
            Body = $"Hello {user.UserName},\n\nYour password has been successfully reset. If you did not initiate this change, please contact our support team immediately.\n\nBest regards,\nSupport Team"
        };
        await _messageBus.PublishAsync(mailSendCommand);
        return;
    }
}
